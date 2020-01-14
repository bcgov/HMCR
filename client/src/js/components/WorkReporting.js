import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col } from 'reactstrap';
import _ from 'lodash';
import queryString from 'query-string';

import SingleDropdown from './ui/SingleDropdown';
import MaterialCard from './ui/MaterialCard';
import WorkReportingUpload from './WorkReportingUpload';
import WorkReportingSubmissions from './WorkReportingSubmissions';
import Authorize from './fragments/Authorize';

import * as Constants from '../Constants';

const WorkReporting = ({ currentUser, history }) => {
  const [serviceArea, setServiceArea] = useState(null);
  const [triggerRefresh, setTriggerRefresh] = useState(null);

  useEffect(() => {
    const queryParams = queryString.parse(history.location.search);

    if (queryParams.serviceArea) setServiceArea(queryParams.serviceArea);
  }, [setServiceArea, history.location.search]);

  const handleFileSubmitted = () => {
    setTriggerRefresh(Math.random());
  };

  return (
    <React.Fragment>
      <MaterialCard>
        <Row>
          <Col>
            <Row>
              <Col sm={3}>Service Area</Col>
              <Col sm={9}>
                <SingleDropdown
                  items={_.orderBy(currentUser.serviceAreas, ['id'])}
                  defaultTitle="Select Servcie Area"
                  value={serviceArea}
                  handleOnChange={serviceArea => {
                    setServiceArea(serviceArea);
                    history.push('?' + queryString.stringify({ serviceArea }));
                  }}
                />
              </Col>
            </Row>
          </Col>
          <Col></Col>
        </Row>
      </MaterialCard>
      {serviceArea && (
        <React.Fragment>
          <Authorize requires={Constants.PERMISSIONS.FILE_W}>
            <MaterialCard>
              <Row>
                <Col>
                  <WorkReportingUpload serviceArea={serviceArea} handleFileSubmitted={handleFileSubmitted} />
                </Col>
                <Col></Col>
              </Row>
            </MaterialCard>
          </Authorize>
          <MaterialCard>
            <WorkReportingSubmissions serviceArea={serviceArea} triggerRefresh={triggerRefresh} history={history} />
          </MaterialCard>
        </React.Fragment>
      )}
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    currentUser: state.user.current,
  };
};

export default connect(mapStateToProps, null)(WorkReporting);
