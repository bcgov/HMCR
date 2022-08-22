import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Alert, Button, FormGroup, Label, Row, Col, Input, InputGroup, InputGroupAddon } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import Clipboard from 'react-clipboard.js';
import { toast } from 'react-toastify';

import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';
import PageSpinner from './ui/PageSpinner';

import { hideErrorDialog } from '../actions';

import * as api from '../Api';
import { keycloak } from '../Keycloak';

const ApiAccess = ({ hideErrorDialog }) => {
  const [loading, setLoading] = useState(true);
  const [apiClient, setApiClient] = useState(null);
  const [showSecret, setShowSecret] = useState(false);
  const [createClicked, setCreateClicked] = useState(false);
  const [version, setVersion] = useState('');

  useEffect(() => {
    api
      .getVersion()
      .then((response) => {
        setVersion(response.data);

        return api.getApiClient();
      })
      .then((response) => setApiClient(response.data))
      .catch((error) => {
        if (error.response.status === 404) hideErrorDialog();
      })
      .finally(() => setLoading(false));
  }, [hideErrorDialog]);

  const onCreateClientClick = () => {
    setCreateClicked(true);
    api
      .createApiClient()
      .then((response) => setApiClient(response.data))
      .finally(() => setCreateClicked(false));
  };

  const onResetClientSecretClicked = () => {
    setCreateClicked(true);
    api
      .resetApiClientSecret()
      .then((response) => setApiClient(response.data))
      .finally(() => setCreateClicked(false));
  };

  const renderCreateButton = () => {
    return (
      !loading &&
      !apiClient && (
        <React.Fragment>
          <Button size="sm" color="primary" onClick={onCreateClientClick} disabled={createClicked}>
            {createClicked && <FontAwesomeIcon icon="sync" spin />} Create Client
          </Button>
        </React.Fragment>
      )
    );
  };

  const renderUsage = () => {
    const tokenUrl = `${keycloak.authServerUrl}/realms/${keycloak.realm}/protocol/openid-connect/token`;
    const accessTokencURL =
      `curl --location --request POST '${tokenUrl}' \\\n` +
      `--header 'Content-Type: application/x-www-form-urlencoded' \\\n` +
      `--data-urlencode 'grant_type=client_credentials' \\\n` +
      `--data-urlencode 'client_id=${apiClient ? apiClient.clientId : '<client_id>'}' \\\n` +
      `--data-urlencode 'client_secret=<client_secret>'`;

    const accessTokenPowershell =
      `$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"\n` +
      `$headers.Add("Content-Type", "application/x-www-form-urlencoded")\n\n` +
      `$body = "grant_type=client_credentials&client_id=${
        apiClient ? apiClient.clientId : '<client_id>'
      }&client_secret=<client_secret>"\n\n` +
      `$response = Invoke-RestMethod '${tokenUrl}' -Method 'POST' -Headers $headers -Body $body\n` +
      `$response | ConvertTo-Json\n`;

    const tokenResponse =
      '{\n' +
      '    "access_token": "aaaabbbbccccdddd...",\n' +
      '    "expires_in": 3600,\n' +
      '    "refresh_expires_in": 1800,\n' +
      '    "refresh_token": "eeeeffffgggghhhh",\n' +
      '    "token_type": "bearer",\n' +
      '    "not-before-policy": 1576687694,\n' +
      '    "session_state": "...",\n' +
      '    "scope": "profile email"\n' +
      '}';

    const location = window.location;
    const sampleUrl = `${location.protocol}//${location.host}/api/version`;
    const sampleRequestcUrl = `curl --location --request GET '${sampleUrl}' \\\n--header 'Authorization: Bearer aaaabbbbccccdddd...'`;
    const sampleRequestPowershell =
      `$headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"\n` +
      `$headers.Add("Authorization", "Bearer aaaabbbbccccdddd...")\n\n` +
      `$response = Invoke-RestMethod '${sampleUrl}' -Method 'GET' -Headers $headers\n` +
      `$response | ConvertTo-Json\n`;

    return (
      <React.Fragment>
        <h2 className="mt-2">Usage</h2>
        <p>
          This section will describe to how to obtain an access token (
          <a href="https://oauth.net/2/bearer-tokens/" target="blank">
            Bearer Token
          </a>
          ) using your API Access Client and how to use the access token to make authenticated HMCR REST API requests.
        </p>
        <h3>Obtaining Access Token</h3>
        <small>cURL</small>
        <div className="code-block">
          <pre>{accessTokencURL}</pre>
        </div>
        <small>Powershell</small>
        <div className="code-block">
          <pre>{accessTokenPowershell}</pre>
        </div>
        <small>Sample Response</small>
        <div className="code-block">
          <pre>{tokenResponse}</pre>
        </div>
        <h3>Using HMCR API</h3>
        <p>
          Once you have crated a valid <strong>access_token</strong> from the previous step then you are ready to use
          HMCR API. To use the access token you simply need to set your <strong>Authorization</strong> header in your
          HTTP requests. Please see the following examples.
        </p>
        <small>cURL</small>
        <div className="code-block">
          <pre>{sampleRequestcUrl}</pre>
        </div>
        <small>Powershell</small>
        <div className="code-block">
          <pre>{sampleRequestPowershell}</pre>
        </div>
        <small>Sample Response</small>
        <div className="code-block">
          <pre>{JSON.stringify(version, null, 4)}</pre>
        </div>
      </React.Fragment>
    );
  };

  const renderClient = () => {
    return (
      !loading &&
      apiClient && (
        <React.Fragment>
          <Row>
            <Col md={6}>
              <FormGroup row>
                <Label for="clientId" sm={3}>
                  Client Id
                </Label>
                <Col>
                  <InputGroup size="sm">
                    <Input
                      type="text"
                      name="clientId"
                      id="clientId"
                      placeholder="Client Id"
                      value={apiClient.clientId}
                      readOnly
                    />
                    <InputGroupAddon addonType="append">
                      <Clipboard
                        className="btn btn-primary"
                        option-text={() => apiClient.clientId}
                        onSuccess={() => {
                          toast.info(<div className="text-center">Copied to clipboard.</div>);
                        }}
                        title="Copy Client Id to Clipboard"
                      >
                        <FontAwesomeIcon icon="copy" />
                      </Clipboard>
                    </InputGroupAddon>
                  </InputGroup>
                </Col>
              </FormGroup>
              <FormGroup row>
                <Label for="clientSecret" sm={3}>
                  Client Secret
                </Label>
                <Col>
                  <InputGroup size="sm">
                    <Input
                      type={showSecret ? 'text' : 'password'}
                      name="clientSecret"
                      id="clientSecret"
                      placeholder="Client Secret"
                      value={apiClient.clientSecret}
                      readOnly
                    />
                    <InputGroupAddon addonType="append">
                      <Button color="primary" onClick={() => setShowSecret(!showSecret)}>
                        <FontAwesomeIcon icon={showSecret ? 'eye-slash' : 'eye'} />
                      </Button>
                      <Clipboard
                        className="btn btn-primary"
                        option-text={() => apiClient.clientSecret}
                        onSuccess={() => {
                          toast.info(<div className="text-center">Copied to clipboard.</div>);
                          setShowSecret(false);
                        }}
                        title="Copy Client Secret to Clipboard"
                      >
                        <FontAwesomeIcon icon="copy" />
                      </Clipboard>
                    </InputGroupAddon>
                  </InputGroup>
                </Col>
              </FormGroup>
              <Row>
                <Col sm={3}></Col>
                <Col>
                  <Button size="sm" color="primary" onClick={onResetClientSecretClicked} disabled={createClicked}>
                    {createClicked && <FontAwesomeIcon icon="sync" spin />} Reset Client Secret
                  </Button>
                </Col>
              </Row>
            </Col>
          </Row>
        </React.Fragment>
      )
    );
  };

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
      <h2>API Access Client</h2>
      <p>
        An API Access Client is needed to obtain access to the HMCR REST API.{' '}
        {!apiClient && <React.Fragment>You can create one below.</React.Fragment>}
      </p>
      <Alert color="warning">
        <strong>Warning!</strong> The API Access Client should be kept confidential. It will have the same access level
        as your regular login and should be treated as such.
      </Alert>
      {loading && <PageSpinner />}
      {renderCreateButton()}
      {renderClient()}
      {renderUsage()}
    </MaterialCard>
  );
};

export default connect(null, { hideErrorDialog })(ApiAccess);
