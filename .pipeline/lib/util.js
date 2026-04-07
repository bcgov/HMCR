"use strict";

function getSecret(oc, namespace, secretId) {
  let secret = null;

  try {
    const raw = oc.raw("get", [
      "-n",
      namespace,
      "secret",
      secretId,
      "-o",
      "json",
    ]);

    secret = JSON.parse(raw.stdout).data;
  } catch (error) {
    console.log(
      `Error: Unable to retrieve secret ${secretId} from ${namespace}`
    );
  }

  return secret;
}

function getPersistentVolumeClaimSize(oc, namespace, claimName) {
  try {
    const raw = oc.raw("get", [
      "-n",
      namespace,
      "pvc",
      claimName,
      "-o",
      "json",
    ]);
    const claim = JSON.parse(raw.stdout);

    return claim?.spec?.resources?.requests?.storage || null;
  } catch (error) {
    return null;
  }
}

module.exports = {
  getSecret,
  getPersistentVolumeClaimSize,
};
