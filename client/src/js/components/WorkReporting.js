import React, { useState, useEffect, useRef, useCallback } from 'react';
import { useHistory } from 'react-router-dom';
import { connect } from 'react-redux';
import { Alert, Button, Row, Col } from 'reactstrap';
import _ from 'lodash';
import queryString from 'query-string';

import SingleDropdown from './ui/SingleDropdown';
import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';
import WorkReportingUpload from './WorkReportingUpload';
import WorkReportingSubmissions from './WorkReportingSubmissions';

import * as Constants from '../Constants';
import * as api from '../Api';
import { getUploadableSubmissionStreams } from '../utils';

const getNoticeColor = (severity) => {
  switch (String(severity || '').toUpperCase()) {
    case 'DANGER':
    case 'ERROR':
      return 'danger';
    case 'WARNING':
      return 'warning';
    case 'SUCCESS':
      return 'success';
    default:
      return 'info';
  }
};

const WorkReporting = ({ currentUser, submissionStreams }) => {
  const history = useHistory();
  const [serviceArea, setServiceArea] = useState(null);
  const [notices, setNotices] = useState([]);
  const [noticesLoading, setNoticesLoading] = useState(false);
  const [noticesLoadError, setNoticesLoadError] = useState(false);

  const submissionsRef = useRef();
  const noticeRequestSequence = useRef(0);
  const workReportStreamId = submissionStreams[Constants.REPORT_TYPES.HMR_WORK_REPORT.name]?.id;
  const hasUploadPermission =
    getUploadableSubmissionStreams(submissionStreams, currentUser.permissions).length > 0;

  useEffect(() => {
    const queryParams = queryString.parse(history.location.search);

    if (queryParams.serviceArea) setServiceArea(queryParams.serviceArea);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [setServiceArea]);

  const loadNotices = useCallback(
    (serviceAreaNumber) => {
      if (!workReportStreamId) return Promise.resolve();

      const requestSequence = ++noticeRequestSequence.current;
      setNoticesLoading(true);
      setNoticesLoadError(false);

      return api
        .getSubmissionConfigurationNotices(workReportStreamId, serviceAreaNumber)
        .then((response) => {
          if (noticeRequestSequence.current === requestSequence) setNotices(response.data || []);
        })
        .catch(() => {
          if (noticeRequestSequence.current === requestSequence) {
            setNotices([]);
            setNoticesLoadError(true);
          }
        })
        .finally(() => {
          if (noticeRequestSequence.current === requestSequence) setNoticesLoading(false);
        });
    },
    [workReportStreamId]
  );

  useEffect(() => {
    loadNotices(serviceArea);
  }, [loadNotices, serviceArea]);

  const handleFileSubmitted = () => {
    submissionsRef.current.refresh();
  };

  return (
    <React.Fragment>
      {notices.map((notice) => (
        <Alert
          color={getNoticeColor(notice.severity)}
          key={`${notice.submissionConfigurationId}-${notice.phase}`}
          role="status"
        >
          {notice.message}
        </Alert>
      ))}
      {noticesLoading && (
        <span className="visually-hidden" role="status">
          Loading submission notices
        </span>
      )}
      {noticesLoadError && (
        <Alert color="warning" role="alert">
          Submission notices could not be loaded.{' '}
          <Button color="link" className="p-0 align-baseline" onClick={() => loadNotices(serviceArea)}>
            Try again
          </Button>
        </Alert>
      )}
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
                  handleOnChange={(serviceArea) => {
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
          {hasUploadPermission && (
            <MaterialCard>
              <Row>
                <Col lg="8">
                  <WorkReportingUpload serviceArea={serviceArea} handleFileSubmitted={handleFileSubmitted} />
                </Col>
                <Col lg="4" />
              </Row>
            </MaterialCard>
          )}
          <MaterialCard>
            <WorkReportingSubmissions serviceArea={serviceArea} ref={submissionsRef} />
          </MaterialCard>
        </React.Fragment>
      )}
    </React.Fragment>
  );
};

const mapStateToProps = (state) => {
  return {
    currentUser: state.user.current,
    submissionStreams: state.submissions.streams,
  };
};

export default connect(mapStateToProps, null)(WorkReporting);
