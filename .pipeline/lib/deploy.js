"use strict";
const { OpenShiftClientX } = require("@bcgov/pipeline-cli");
const path = require("path");

const util = require("./util");

module.exports = (settings) => {
  const phases = settings.phases;
  const options = settings.options;
  const phase = options.env;
  const changeId = phases[phase].changeId;
  const version = options.version || `${phases[phase].phase}-1.0.0`;

  const oc = new OpenShiftClientX(
    Object.assign({ namespace: phases[phase].namespace }, options)
  );

  const templatesLocalBaseUrl = oc.toFileUrl(
    path.resolve(__dirname, "../../openshift")
  );
  var objects = [];
  const logDbSecret = util.getSecret(
    oc,
    phases[phase].namespace,
    `${phases[phase].name}-logdb${phases[phase].suffix}`
  );

  objects.push(
    ...oc.processDeploymentTemplate(
      `${templatesLocalBaseUrl}/client-deploy-config.yaml`,
      {
        param: {
          NAME: `${phases[phase].name}-client`,
          SUFFIX: phases[phase].suffix,
          VERSION: version,
          ENV: phases[phase].phase,
          HOST: phases[phase].host,
          CPU: phases[phase].client_cpu,
          MEMORY: phases[phase].client_memory,
          NAMESPACE: phases[phase].namespace,
        },
      }
    )
  );

  if (!logDbSecret) {
    console.log("Adding logDb postgresql secret");

    objects.push(
      ...oc.processDeploymentTemplate(
        `${templatesLocalBaseUrl}/secrets/logdb-postgresql-secrets.yaml`,
        {
          param: {
            NAME: `${phases[phase].name}-logdb`,
            SUFFIX: phases[phase].suffix,
          },
        }
      )
    );
  }

  objects.push(
    ...oc.processDeploymentTemplate(
      `${templatesLocalBaseUrl}/postgresql-deploy-config.yaml`,
      {
        param: {
          NAME: `${phases[phase].name}-logdb`,
          SUFFIX: phases[phase].suffix,
          VERSION: version,
          ENV: phases[phase].phase,
        },
      }
    )
  );

  objects.push(
    ...oc.processDeploymentTemplate(
      `${templatesLocalBaseUrl}/configmaps/api-appsettings.yaml`,
      {
        param: {
          ENV: phases[phase].phase,
          SUBMISSION_URL: `https://${phases[phase].url_prefix}hmcr.th.gov.bc.ca/workreporting?serviceArea={0}&showResult={1}`,
          BCEID_SERVICE: `https://gws1${phases[phase].bceid_service}.bceid.ca/webservices/client/v10/bceidservice.asmx`,
          EXPORT_URL: `https://${phases[phase].export_server}.apps.th.gov.bc.ca`,
          OAS_URL: `https://${phases[phase].oas_server}.apps.th.gov.bc.ca`,
          GEOSERVER_TIMEOUT: 120,
        },
      }
    )
  );

  objects.push(
    ...oc.processDeploymentTemplate(
      `${templatesLocalBaseUrl}/api-deploy-config.yaml`,
      {
        param: {
          NAME: `${phases[phase].name}-api`,
          LOGDB_NAME: `${phases[phase].name}-logdb`,
          SUFFIX: phases[phase].suffix,
          VERSION: version,
          HOST: phases[phase].host,
          ENV: phases[phase].phase,
          ASPNETCORE_ENVIRONMENT: phases[phase].dotnet_env,
          CPU: phases[phase].api_cpu,
          MEMORY: phases[phase].api_memory,
          NAMESPACE: phases[phase].namespace,
        },
      }
    )
  );

  objects.push(
    ...oc.processDeploymentTemplate(
      `${templatesLocalBaseUrl}/hangfire-deploy-config.yaml`,
      {
        param: {
          NAME: `${phases[phase].name}-hangfire`,
          LOGDB_NAME: `${phases[phase].name}-logdb`,
          SUFFIX: phases[phase].suffix,
          VERSION: version,
          ENV: phases[phase].phase,
          ASPNETCORE_ENVIRONMENT: phases[phase].dotnet_env,
          CPU: phases[phase].hangfire_cpu,
          MEMORY: phases[phase].hangfire_memory,
          NAMESPACE: phases[phase].namespace,
        },
      }
    )
  );

  oc.applyRecommendedLabels(
    objects,
    phases[phase].name,
    phase,
    `${changeId}`,
    phases[phase].instance
  );
  oc.importImageStreams(
    objects,
    phases[phase].tag,
    phases.build.namespace,
    phases.build.tag
  );

  let imageExists = false;
  
  oc.applyAndDeploy(objects, phases[phase].instance)
    .then(() => {
      const imageNames = ["hmcr-api", "hmcr-client", "hmcr-hangfire"];

      imageExists = imageNames.every((imageName) => {
        try {
          const imageSha = oc.raw("get", [
            "istag",
            `${imageName}:${version}`,
            "-n",
            "d3d940-tools",
            "-o",
            "json",
          ]);
          if (!imageSha) {
            console.error(
              `❌ Error: No built image found for ${imageName}:${version}`
            );
            return false;
          }

          console.log(`🔄 Tagging built image as 'latest' for ${imageName}`);

          oc.raw("tag", [
            `d3d940-tools/${imageName}:${version}`,
            `d3d940-tools/${imageName}:latest`,
          ]);

          console.log(
            `✅ Successfully tagged ${imageName}:${version} as latest.`
          );
          return true;
        } catch (error) {
          console.error(error.message);
          return false;
        }
      });

      console.log("✅ All images are now tagged as latest.");
    })
    .finally(() => {
      if (!imageExists) {
        console.log("❌ Skipping final tagging because image does not exist.");
        return;
      }
      const imageNames = ["hmcr-api", "hmcr-client", "hmcr-hangfire"];

      imageNames.forEach((imageName) => {
        const sourceImage = `d3d940-tools/${imageName}:latest`;
        const targetImage = `${phases[phase].namespace}/${imageName}`;

        console.log(
          `🔄 Tagging Image for Deployment: ${sourceImage} -> ${targetImage}`
        );

        // Tag the image for the specific deployment environment
        oc.raw("tag", [`${sourceImage}`, `${targetImage}:${version}`]);

        console.log(
          `✅ Image successfully tagged for ${phase}: ${targetImage}`
        );
      });
    });
};