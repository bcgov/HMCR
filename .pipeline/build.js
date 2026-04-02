'use strict';
const task = require('./lib/build.js')
const settings = require('./lib/config.js')

Promise.resolve(task(Object.assign(settings, { phase: 'build' }))).catch((error) => {
  console.error(error);
  process.exit(1);
})
