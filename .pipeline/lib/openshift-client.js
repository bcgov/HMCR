"use strict";

const { OpenShiftClientX } = require("@bcgov/pipeline-cli");

function normalizeImageStream(resource) {
  if (
    resource &&
    resource.kind === "ImageStream" &&
    resource.status &&
    Array.isArray(resource.status.tags)
  ) {
    resource.status.tags.forEach((tag) => {
      if (!Array.isArray(tag.items)) {
        tag.items = [];
      }
    });
  }

  return resource;
}

class HmcrOpenShiftClientX extends OpenShiftClientX {
  object(name, args) {
    return normalizeImageStream(super.object(name, args));
  }

  objects(names, args) {
    return super.objects(names, args).map(normalizeImageStream);
  }
}

module.exports = { HmcrOpenShiftClientX };
