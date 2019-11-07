import React from 'react';
import { Row, Col } from 'reactstrap';
// import { Link } from 'react-router-dom';

// import * as Constants from '../Constants';

const WorkReporting = () => {
  return (
    <Row>
      <Col style={{ border: '1px solid black' }} sm={{ size: 12, order: 2 }} lg={{ size: 8, order: 1 }}>
        Upload Form
      </Col>
      <Col style={{ border: '1px solid black' }} sm={{ size: 12, order: 1 }} lg={{ size: 4, order: 2 }}>
        Quick Links
      </Col>
    </Row>
  );
};

export default WorkReporting;
