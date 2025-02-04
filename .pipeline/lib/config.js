"use strict";
const options = require("@bcgov/pipeline-cli").Util.parseArguments();
const changeId = options.pr; //aka pull-request
const version = "1.0.0";
const name = "hmcr";
const registryBase = "image-registry.openshift-image-registry.svc:5000";

Object.assign(options.git, { owner: "ychung-mot", repository: "HMCR" });
const phases = {
  build: {
    namespace: "d3d940-tools",
    name: `${name}`,
    phase: "build",
    changeId: changeId,
    suffix: `-build-${changeId}`,
    instance: `${name}-build-${changeId}`,
    version: `${version}-${changeId}`,
    tag: `build-${version}-${changeId}`,
    transient: true,
  },
  dev: {
    namespace: "d3d940-dev",
    name: `${name}`,
    phase: "dev",
    changeId: changeId,
    suffix: `-dev-${changeId}`,
    instance: `${name}-dev-${changeId}`,
    version: `${version}-${changeId}`,
    tag: `dev-${version}-${changeId}`,
    host: `hmcr-d3d940-dev.apps.silver.devops.gov.bc.ca`,
    url_prefix: "dev-",
    bceid_service: ".test",
    oas_server: "prdoas5",
    export_server: "tstoas5",
    dotnet_env: "Development",
    transient: true,
    hangfire_cpu: "300m",
    hangfire_memory: "300Mi",
    api_cpu: "200m",
    api_memory: "512Mi",
    client_cpu: "100m",
    client_memory: "100Mi",
    imageRegistry: {
      api: `${registryBase}/d3d940-dev/hmcr-api:dev-${version}-${changeId}`,
      client: `${registryBase}/d3d940-dev/hmcr-client:dev-${version}-${changeId}`,
      hangfire: `${registryBase}/d3d940-dev/hmcr-hangfire:dev-${version}-${changeId}`,
    },
  },
  test: {
    namespace: "d3d940-test",
    name: `${name}`,
    phase: "test",
    changeId: changeId,
    suffix: `-test`,
    instance: `${name}-test`,
    version: `${version}`,
    tag: `test-${version}`,
    host: `hmcr-d3d940-test.apps.silver.devops.gov.bc.ca`,
    url_prefix: "tst-",
    bceid_service: ".test",
    oas_server: "prdoas5",
    export_server: "tstoas5",
    dotnet_env: "Staging",
    hangfire_cpu: "300m",
    hangfire_memory: "300Mi",
    api_cpu: "200m",
    api_memory: "512Mi",
    client_cpu: "100m",
    client_memory: "100Mi",
    imageRegistry: {
      api: `${registryBase}/d3d940-test/hmcr-api:${version}`,
      client: `${registryBase}/d3d940-test/hmcr-client:${version}`,
      hangfire: `${registryBase}/d3d940-test/hmcr-hangfire:${version}`,
    },
  },
  uat: {
    namespace: "d3d940-test",
    name: `${name}`,
    phase: "uat",
    changeId: changeId,
    suffix: `-uat`,
    instance: `${name}-uat`,
    version: `${version}`,
    tag: `uat-${version}`,
    host: `hmcr-d3d940-uat.apps.silver.devops.gov.bc.ca`,
    url_prefix: "uat-",
    bceid_service: ".test",
    oas_server: "prdoas5",
    export_server: "tstoas5",
    dotnet_env: "UAT",
    hangfire_cpu: "300m",
    hangfire_memory: "300Mi",
    api_cpu: "200m",
    api_memory: "512Mi",
    client_cpu: "100m",
    client_memory: "100Mi",
    imageRegistry: {
      api: `${registryBase}/d3d940-uat/hmcr-api:${version}`,
      client: `${registryBase}/d3d940-uat/hmcr-client:${version}`,
      hangfire: `${registryBase}/d3d940-uat/hmcr-hangfire:${version}`,
    },
  },
  prod: {
    namespace: "d3d940-prod",
    name: `${name}`,
    phase: "prod",
    changeId: changeId,
    suffix: `-prod`,
    instance: `${name}-prod`,
    version: `${version}`,
    tag: `prod-${version}`,
    host: `hmcr-d3d940-prod.apps.silver.devops.gov.bc.ca`,
    url_prefix: "",
    bceid_service: "",
    oas_server: "prdoas5",
    export_server: "prdoas5",
    dotnet_env: "Production",
    hangfire_cpu: "1",
    hangfire_memory: "1Gi",
    api_cpu: "200m",
    api_memory: "600Mi",
    client_cpu: "100m",
    client_memory: "300Mi",
    imageRegistry: {
      api: `${registryBase}/d3d940-prod/hmcr-api:${version}`,
      client: `${registryBase}/d3d940-prod/hmcr-client:${version}`,
      hangfire: `${registryBase}/d3d940-prod/hmcr-hangfire:${version}`,
    },
  },
};

// This callback forces the node process to exit as failure.
process.on("unhandledRejection", (reason) => {
  console.log(reason);
  process.exit(1);
});

module.exports = exports = { phases, options };
