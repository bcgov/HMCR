"use strict";
const { OpenShiftClientX } = require("@bcgov/pipeline-cli");
const path = require("path");

module.exports = (settings) => {
  const phases = settings.phases;
  const options = settings.options;
  const oc = new OpenShiftClientX(
    Object.assign({ namespace: phases.build.namespace }, options)
  );
  const phase = "build";
  let objects = [];
  const templatesLocalBaseUrl = oc.toFileUrl(
    path.resolve(__dirname, "../../openshift")
  );
  const githubRunNumber = process.env.GITHUB_RUN_NUMBER || "manual";
  const version = options.version || `v1.0.${githubRunNumber}`;

  console.log(`🚀 VERSION USED is ${version}`)

  objects.push(
    ...oc.processDeploymentTemplate(
      `${templatesLocalBaseUrl}/client-build-config.yaml`,
      {
        param: {
          NAME: `${settings.phases[phase].name}-client`,
          SUFFIX: settings.phases[phase].suffix,
          VERSION: version,
          SOURCE_REPOSITORY_URL: `${oc.git.uri}`,
          SOURCE_REPOSITORY_REF: `${oc.git.branch_ref}`,
        },
      }
    )
  );

  objects.push(
    ...oc.processDeploymentTemplate(
      `${templatesLocalBaseUrl}/api-build-config.yaml`,
      {
        param: {
          NAME: `${settings.phases[phase].name}-api`,
          SUFFIX: settings.phases[phase].suffix,
          VERSION: version,
          SOURCE_REPOSITORY_URL: `${oc.git.uri}`,
          SOURCE_REPOSITORY_REF: `${oc.git.branch_ref}`,
        },
      }
    )
  );

  objects.push(
    ...oc.processDeploymentTemplate(
      `${templatesLocalBaseUrl}/hangfire-build-config.yaml`,
      {
        param: {
          NAME: `${settings.phases[phase].name}-hangfire`,
          SUFFIX: settings.phases[phase].suffix,
          VERSION: version,
          SOURCE_REPOSITORY_URL: `${oc.git.uri}`,
          SOURCE_REPOSITORY_REF: `${oc.git.branch_ref}`,
        },
      }
    )
  );

  oc.applyRecommendedLabels(
    objects,
    phases[phase].name,
    phase,
    phases[phase].changeId,
    phases[phase].instance
  );
  oc.applyAndBuild(objects);
};
