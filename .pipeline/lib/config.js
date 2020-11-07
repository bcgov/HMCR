'use strict';
const options= require('pipeline-cli').Util.parseArguments()
const changeId = options.pr //aka pull-request
const version = '1.0.0'
const name = 'hmcr'

const phases = {
  build:  {namespace:'txkggj-tools' , name: `${name}`, phase: 'build' , changeId:changeId, suffix: `-build-${changeId}` , instance: `${name}-build-${changeId}` , version:`${version}-${changeId}`  , tag:`build-${version}-${changeId}`, transient: true},
  dev:    {namespace:'txkggj-dev'   , name: `${name}`, phase: 'dev'   , changeId:changeId, suffix: `-dev-${changeId}`   , instance: `${name}-dev-${changeId}`   , version:`${version}-${changeId}`  , tag:`dev-${version}-${changeId}`  , host: `hmcr-${changeId}-txkggj-dev.pathfinder.gov.bc.ca` , url_prefix: 'dev-', bceid_service: '.test' ,  oas_server: 'devoas1', dotnet_env: 'Development', transient: true},
  test:   {namespace:'txkggj-test'  , name: `${name}`, phase: 'test'  , changeId:changeId, suffix: `-test`              , instance: `${name}-test`              , version:`${version}`              , tag:`test-${version}`             , host: `hmcr-txkggj-test.pathfinder.gov.bc.ca`            , url_prefix: 'tst-', bceid_service: '.test' ,  oas_server: 'tstoas2', dotnet_env: 'Staging'},
  uat:    {namespace:'txkggj-test'  , name: `${name}`, phase: 'uat'  ,  changeId:changeId, suffix: `-uat`              ,  instance: `${name}-uat`              ,  version:`${version}`              , tag:`uat-${version}`              , host: `hmcr-txkggj-uat.pathfinder.gov.bc.ca`             , url_prefix: 'uat-', bceid_service: '.test' ,  oas_server: 'tstoas2', dotnet_env: 'UAT'},
  prod:   {namespace:'txkggj-prod'  , name: `${name}`, phase: 'prod'  , changeId:changeId, suffix: `-prod`              , instance: `${name}-prod`              , version:`${version}`              , tag:`prod-${version}`             , host: `hmcr-txkggj-prod.pathfinder.gov.bc.ca`                 , url_prefix: ''    , bceid_service: ''      ,  oas_server: 'prdoas2', dotnet_env: 'Production'},
};

// This callback forces the node process to exit as failure.
process.on('unhandledRejection', (reason) => {
  console.log(reason);
  process.exit(1);
});

module.exports = exports = {phases, options};