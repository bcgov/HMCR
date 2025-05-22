#!/bin/sh

echo "---> Setting window.RUNTIME_REACT_APP values ..."
JS_PATH=~/js/env.config.js

echo "window.RUNTIME_REACT_APP_SSO_HOST='${VITE_SSO_HOST}';" > $JS_PATH
echo "window.RUNTIME_REACT_APP_SSO_REALM='${VITE_SSO_REALM}';" >> $JS_PATH
echo "window.RUNTIME_REACT_APP_SSO_CLIENT='${VITE_SSO_CLIENT}';" >> $JS_PATH
echo "window.RUNTIME_REACT_APP_API_HOST='${VITE_API_HOST}';" >> $JS_PATH

echo "---> Creating nginx.conf ..."
envsubst '${HMCR_DEPLOY_SUFFIX}' < /tmp/src/nginx.conf.tmpl > /etc/nginx/nginx.conf