'use strict';
const settings = require('./lib/config.js')
const task = require('./lib/deploy.js')

Promise.resolve(task(Object.assign(settings, { phase: settings.options.env }))).catch((error) => {
  console.error(error);
  process.exit(1);
})
