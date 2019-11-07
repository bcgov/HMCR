'use strict';
const {OpenShiftClientX} = require('pipeline-cli')
const path = require('path');

module.exports = (settings)=>{
  const phases = settings.phases
  const options= settings.options
  const phase=options.env
  const changeId = phases[phase].changeId
  const oc=new OpenShiftClientX(Object.assign({'namespace':phases[phase].namespace}, options));
  const templatesLocalBaseUrl =oc.toFileUrl(path.resolve(__dirname, '../../openshift'))
  var objects = []

  // The deployment of your cool app goes here ▼▼▼
  objects.push(
    ...oc.processDeploymentTemplate(
      `${templatesLocalBaseUrl}/client-deploy-config.yaml`,
      {
        param: {
          NAME: `${phases[phase].name}-client`,
          SUFFIX: phases[phase].suffix,
          VERSION: phases[phase].tag,
          HOST: phases[phase].host
        }
      }
    )
  );

  objects.push(
    ...oc.processDeploymentTemplate(
      `${templatesLocalBaseUrl}/api-deploy-config.yaml`,
      {
        param: {
          NAME: `${phases[phase].name}-api`,
          SUFFIX: phases[phase].suffix,
          VERSION: phases[phase].tag,
          HOST: phases[phase].host,
          ASPNETCORE_ENVIRONMENT: phases[phase].dotnet_env
        }
      }
    )
  );

  oc.applyRecommendedLabels(objects, phases[phase].name, phase, `${changeId}`, phases[phase].instance)
  oc.importImageStreams(objects, phases[phase].tag, phases.build.namespace, phases.build.tag)
  oc.applyAndDeploy(objects, phases[phase].instance)
}
