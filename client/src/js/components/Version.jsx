import React, { useState, useEffect } from 'react';
import { Row, Col } from 'reactstrap';

import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';

import * as api from '../Api';

const Version = () => {
  const [versionInfo, setVersionInfo] = useState({});

  useEffect(() => {
    api.getVersion().then((response) => setVersionInfo(response.data));
  }, []);

  return (
    <MaterialCard>
      <UIHeader>Application Version</UIHeader>
      <Row>
        <Col xs="3">
          <strong>Name</strong>
        </Col>
        <Col>{versionInfo.name}</Col>
      </Row>
      <Row>
        <Col xs="3">
          <strong>Description</strong>
        </Col>
        <Col>{versionInfo.description}</Col>
      </Row>
      <Row>
        <Col xs="3">
          <strong>Version</strong>
        </Col>
        <Col>{versionInfo.version}</Col>
      </Row>
      <Row>
        <Col xs="3">
          <strong>Framework</strong>
        </Col>
        <Col>{versionInfo.targetFramework}</Col>
      </Row>
      <Row>
        <Col xs="3">
          <strong>Build Time</strong>
        </Col>
        <Col>{versionInfo.fileCreationTime}</Col>
      </Row>
      <Row>
        <Col xs="3">
          <strong>Runtime Version</strong>
        </Col>
        <Col>{versionInfo.imageRuntimeVersion}</Col>
      </Row>
      <Row>
        <Col xs="3">
          <strong>Git Commit</strong>
        </Col>
        <Col>{versionInfo.commit}</Col>
      </Row>
      <Row>
        <Col xs="3">
          <strong>Environment</strong>
        </Col>
        <Col>{versionInfo.environment}</Col>
      </Row>
    </MaterialCard>
  );
};

export default Version;
