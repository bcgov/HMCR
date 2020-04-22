import React, { useState, useEffect } from 'react';
import { Alert } from 'reactstrap';

import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';
import { keycloak } from '../Keycloak';

// import * as api from '../Api';

const ApiAccess = () => {
  const [keycloakInfo, setKeycloakInfo] = useState({});

  useEffect(() => {
    setKeycloakInfo(keycloak.idTokenParsed);
  }, []);

  console.log(keycloakInfo);

  return (
    <MaterialCard>
      <UIHeader>REST API Access</UIHeader>
      <p>
        HMCR provides a complete{' '}
        <a href="https://en.wikipedia.org/wiki/Representational_state_transfer" target="blank">
          REST API
        </a>
        . It can be used to interact with the HMCR application directly without the UI.
      </p>
      <p>
        Please refer to the{' '}
        <a href="/swagger/index.html" target="blank">
          Swagger documentation
        </a>{' '}
        for a list of usable APIs.
      </p>
      <Alert color="warning">
        <strong>Warning!</strong> The following sections contain advanced HMCR Application usage.
      </Alert>
      <h2>Obtaining Access</h2>
      <p>
        The HMCR REST API is protected by the use of JWT Token. To access the API you must obtain the necessary access
        token from the BC Government{' '}
        <a href="https://www.keycloak.org/" target="blank">
          Keycloak
        </a>{' '}
        Server first.
      </p>
      <p>
        <strong>Access Token</strong>
      </p>
    </MaterialCard>
  );
};

export default ApiAccess;
