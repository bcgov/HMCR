#!/bin/sh
set -eu

echo "---> Setting window.RUNTIME_REACT_APP values ..."
JS_PATH=~/js/env.config.js
mkdir -p "$(dirname "$JS_PATH")"

js_escape() {
  printf '%s' "$1" | sed -e ':a' -e 'N' -e '$!ba' -e 's/\\/\\\\/g' -e "s/'/\\\\'/g" -e 's/\r/\\r/g' -e ':b' -e 's/\n/\\n/g' -e 'tb'
}

{
  echo "window.RUNTIME_REACT_APP_SSO_HOST='$(js_escape "${REACT_APP_SSO_HOST:-}")';"
  echo "window.RUNTIME_REACT_APP_SSO_REALM='$(js_escape "${REACT_APP_SSO_REALM:-}")';"
  echo "window.RUNTIME_REACT_APP_SSO_CLIENT='$(js_escape "${REACT_APP_SSO_CLIENT:-}")';"
  echo "window.RUNTIME_REACT_APP_API_HOST='$(js_escape "${REACT_APP_API_HOST:-}")';"
  echo "window.RUNTIME_REACT_APP_VERSION='$(js_escape "${REACT_APP_VERSION:-}")';"
} > "$JS_PATH"

echo "---> Creating nginx.conf ..."
envsubst '${HMCR_DEPLOY_SUFFIX}' < /tmp/src/nginx.conf.tmpl > /etc/nginx/nginx.conf
