"use strict";
const { OpenShiftClientX } = require("@bcgov/pipeline-cli");
const path = require("path");

const buildTask = require("./build");
const util = require("./util");

const imageNames = ["hmcr-api", "hmcr-client", "hmcr-hangfire"];

function missingBuildImages(oc, buildNamespace, version) {
  return imageNames.filter((imageName) => {
    try {
      oc.raw(
        "get",
        ["istag", `${imageName}:${version}`, "-o", "name"],
        { namespace: buildNamespace }
      );
      return false;
    } catch (error) {
      return true;
    }
  });
}

async function ensureBuildImages(settings, oc, version) {
  const buildNamespace = settings.phases.build.namespace;
  const missingImages = missingBuildImages(oc, buildNamespace, version);

  if (missingImages.length === 0) {
    return;
  }

  console.log(
    `⚠️ Missing build image tags for ${version}: ${missingImages.join(", ")}. Rebuilding before deploy.`
  );
  await buildTask(settings);

  const remainingMissingImages = missingBuildImages(oc, buildNamespace, version);
  if (remainingMissingImages.length > 0) {
    throw new Error(
      `Build images are still missing after rebuild for ${version}: ${remainingMissingImages.join(", ")}`
    );
  }
}

module.exports = async (settings) => {
  const phases = settings.phases;
  const options = settings.options;
  const phase = options.env;
  const changeId = phases[phase].changeId;
  const githubRunNumber = process.env.GITHUB_RUN_NUMBER;
  const version = options.version || `v1.0.${githubRunNumber}`;
  console.log(`🚀 Using version: ${version}`);

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
  await ensureBuildImages(settings, oc, version);
  oc.importImageStreams(
    objects,
    phases[phase].tag,
    phases.build.namespace,
    version
  );

  // Ensure image streams are imported before proceeding
  imageNames.forEach((imageName) => {
    try {
      console.log(`🔄 Importing image stream for ${imageName}`);
      oc.raw("import-image", [
        `${imageName}:${phases.build.tag}`,
        `--from=d3d940-tools/${imageName}:${phases.build.tag}`,
        "--confirm",
        "-n",
        phases.build.namespace,
      ]);
      console.log(`✅ Successfully imported image stream for ${imageName}`);
    } catch (error) {
      console.error(`❌ Failed to import image stream for ${imageName}: ${error.message}`);
    }
  });

  let imageExists = false;
  
  return oc.applyAndDeploy(objects, phases[phase].instance)
    .then(() => {
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
