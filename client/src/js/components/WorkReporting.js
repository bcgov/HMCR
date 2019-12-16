import React from 'react';
import { Row, Col } from 'reactstrap';
// import { Link } from 'react-router-dom';

import MaterialCard from './ui/MaterialCard';
import WorkReportingUpload from './WorkReportingUpload';
// import * as Constants from '../Constants';

const WorkReporting = () => {
  return (
    <Row>
      <Col sm={{ size: 12, order: 2 }} lg={{ size: 8, order: 1 }}>
        <WorkReportingUpload />
      </Col>
      <Col sm={{ size: 12, order: 1 }} lg={{ size: 4, order: 2 }}>
        <MaterialCard>Quick Links...to be updated</MaterialCard>
      </Col>
    </Row>
  );
};

export default WorkReporting;
