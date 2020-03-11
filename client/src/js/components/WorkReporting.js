import React, { useState, useEffect, useRef } from 'react';
import { useHistory } from 'react-router-dom';
import { connect } from 'react-redux';
import { Row, Col } from 'reactstrap';
import _ from 'lodash';
import queryString from 'query-string';

import SingleDropdown from './ui/SingleDropdown';
import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';
import WorkReportingUpload from './WorkReportingUpload';
import WorkReportingSubmissions from './WorkReportingSubmissions';
import Authorize from './fragments/Authorize';

import * as Constants from '../Constants';

const WorkReporting = ({ currentUser }) => {
  const history = useHistory();
  const [serviceArea, setServiceArea] = useState(null);

  const submissionsRef = useRef();

  useEffect(() => {
    const queryParams = queryString.parse(history.location.search);

    if (queryParams.serviceArea) setServiceArea(queryParams.serviceArea);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [setServiceArea]);

  const handleFileSubmitted = () => {
    submissionsRef.current.refresh();
  };

  return (
    <React.Fragment>
      <MaterialCard>
        <UIHeader>Report Upload</UIHeader>
        <Row>
          <Col lg="8">
            <Row>
              <Col sm={3}>Service Area</Col>
              <Col sm={9}>
                <SingleDropdown
                  items={_.orderBy(currentUser.serviceAreas, ['id'])}
                  defaultTitle="Select Service Area"
                  value={serviceArea}
                  handleOnChange={serviceArea => {
                    setServiceArea(serviceArea);
                    history.push('?' + queryString.stringify({ serviceArea }));
                  }}
                />
              </Col>
            </Row>
          </Col>
          <Col lg="4" />
        </Row>
      </MaterialCard>
      {serviceArea && (
        <React.Fragment>
          <Authorize requires={Constants.PERMISSIONS.FILE_W}>
            <MaterialCard>
              <Row>
                <Col lg="8">
                  <WorkReportingUpload serviceArea={serviceArea} handleFileSubmitted={handleFileSubmitted} />
                </Col>
                <Col lg="4" />
              </Row>
            </MaterialCard>
          </Authorize>
          <MaterialCard>
            <WorkReportingSubmissions serviceArea={serviceArea} ref={submissionsRef} />
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
