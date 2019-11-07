import React from 'react';
import { Row, Col, Button } from 'reactstrap';
import { Link } from 'react-router-dom';

import * as Constants from '../Constants';

const Home = () => {
  return (
    <React.Fragment>
      <Row>
        <Col>
          <h4 className="text-center">Welcome to Template</h4>
          <p>Template for BC Gov Web Site</p>

          <div>
            <ul>
              <li>Backend: .NET Core</li>
              <li>Frotend: React</li>
            </ul>
          </div>
          <div className="text-center mt-5">
            <Link to={Constants.PATHS.ABOUT}>
              <Button color="primary">About</Button>
            </Link>
          </div>
        </Col>
      </Row>
      <Row>
        <Col>
          <img className="w-30" src={`${process.env.PUBLIC_URL}/images/bc.png`} alt="British Columbia" />
        </Col>
      </Row>
    </React.Fragment>
  );
};

export default Home;
