import Keycloak from 'keycloak-js';

import * as api from './Api';

const keycloakConfig = {
  url: window.RUNTIME_REACT_APP_SSO_HOST ? window.RUNTIME_REACT_APP_SSO_HOST : import.meta.env.VITE_SSO_HOST,
  realm: window.RUNTIME_REACT_APP_SSO_REALM ? window.RUNTIME_REACT_APP_SSO_REALM : import.meta.env.VITE_SSO_REALM,
  clientId: window.RUNTIME_REACT_APP_SSO_CLIENT ? window.RUNTIME_REACT_APP_SSO_CLIENT : import.meta.env.VITE_SSO_CLIENT,
};

export const keycloak = new Keycloak(keycloakConfig);

export const init = (onSuccess) => {
  keycloak
    .init({
      onLoad: 'login-required',
      checkLoginIframe: false,
      promiseType: 'native',
      pkceMethod: 'S256',
    })
    .then((authenticated) => {
      if (authenticated && onSuccess) {
        onSuccess();
      }
    });

  keycloak.onAuthLogout = () => {
    window.location.reload();
  };

  api.instance.interceptors.request.use(
    (config) =>
      new Promise((resolve) =>
        keycloak
          .updateToken(5)
          .then(() => {
            config.headers.Authorization = `Bearer ${keycloak.token}`;
            resolve(config);
          })
          .catch(() => {
            keycloak.login();
          }),
      ),
  );
};

export const logout = () => {
  keycloak.logout();
};
