import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Col, FormGroup, FormFeedback, Label, CustomInput, Spinner, Alert, Button } from 'reactstrap';
import { Formik, Form } from 'formik';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import SingleDropdownField from './ui/SingleDropdownField';
import { FormRow } from './forms/FormInputs';
import SubmitButton from './ui/SubmitButton';
import SimpleModalWrapper from './ui/SimpleModalWrapper';

import { showValidationErrorDialog } from '../actions';

import * as Constants from '../Constants';
import * as api from '../Api';

const defaultFormValues = { reportTypeId: null, reportFile: null };

const isFileCsvType = file => file && file.name.endsWith('.csv');

const updateUploadStatusIcon = status => {
  switch (status) {
    case Constants.UPLOAD_STATE_STATUS.COMPLETE:
      return <FontAwesomeIcon icon="check-circle" className="fa-color-success" />;
    case Constants.UPLOAD_STATE_STATUS.ERROR:
      return <FontAwesomeIcon icon="times-circle" className="fa-color-danger" />;
    case Constants.UPLOAD_STATE_STATUS.WARNING:
      return <FontAwesomeIcon icon="exclamation-circle" className="fa-color-warning" />;
    default:
      return <Spinner size="sm" color="primary" />;
  }
};

const updateUploadStatusMessage = (state, status) => {
  switch (state) {
    case Constants.UPLOAD_STATE.RESUB_CHECK:
      return <div>Checking for re-submitted rows... {updateUploadStatusIcon(status)}</div>;
    case Constants.UPLOAD_STATE.SAVING:
      return <div>Saving report data... {updateUploadStatusIcon(status)}</div>;
    default:
      throw new Error('updateUploadStatusMessage state not set');
  }
};

const WorkReportingUpload = ({
  currentUser,
  showValidationErrorDialog,
  serviceArea,
  handleFileSubmitted,
  submissionStreams,
  ...props
}) => {
  const [fileInputKey, setFileInputKey] = useState(Math.random());
  const [submitting, setSubmitting] = useState(false);
  const [showStatusModal, setShowStatusModal] = useState(false);
  const [resubCheckStatus, setResubCheckStatus] = useState(null);
  const [savingStatus, setSavingStatus] = useState(null);
  const [errorMessages, setErrorMessages] = useState(null);
  const [completeMessage, setCompleteMessage] = useState(null);
  const [reportTypes, setReportTypes] = useState([]);

  useEffect(() => {
    setReportTypes(Object.values(submissionStreams).filter(o => o.isActive));
  }, [submissionStreams]);

  const resetUploadStatus = () => {
    setFileInputKey(Math.random());
    setSubmitting(false);
  };

  const resetMessages = () => {
    setResubCheckStatus(null);
    setSavingStatus(null);
    setErrorMessages(null);
    setCompleteMessage(null);
  };

  const handleSubmit = (values, setFieldValue) => {
    const stagingTableName = reportTypes.find(type => values.reportTypeId === type.id).stagingTableName;
    const apiPath = Constants.REPORT_TYPES[stagingTableName].api;
    if (!apiPath) return;

    const reset = () => {
      resetUploadStatus();
      setFieldValue('reportFile', null);
    };

    const formData = new FormData();
    formData.append('reportFile', values.reportFile);
    formData.append('serviceAreaNumber', serviceArea);

    setSubmitting(true);
    setShowStatusModal(true);

    handleCheckResubmissions(apiPath, formData, reset);
  };

  const handleCheckResubmissions = (apiPath, data, resetCallback) => {
    setResubCheckStatus(
      updateUploadStatusMessage(Constants.UPLOAD_STATE.RESUB_CHECK, Constants.UPLOAD_STATE_STATUS.START)
    );
    api.instance
      .post(`${apiPath}/resubmissions`, data)
      .then(response => {
        if (response.data && response.data.length > 0) {
          setResubCheckStatus(
            <React.Fragment>
              {updateUploadStatusMessage(Constants.UPLOAD_STATE.RESUB_CHECK, Constants.UPLOAD_STATE_STATUS.WARNING)}
              <Alert color="warning">
                <p>The following rows are different from the existing data in the database:</p>
                <ul>
                  {response.data.map((duplicate, index) => (
                    <li key={index}>{duplicate}</li>
                  ))}
                </ul>
                <p>Would you like to overwrite the existing database values?</p>
                <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
                  <Button
                    color="danger"
                    size="sm"
                    className="mr-2"
                    onClick={() => {
                      setResubCheckStatus(
                        updateUploadStatusMessage(
                          Constants.UPLOAD_STATE.RESUB_CHECK,
                          Constants.UPLOAD_STATE_STATUS.COMPLETE
                        )
                      );
                      handleUploadFile(apiPath, data, resetCallback);
                    }}
                  >
                    Confirm
                  </Button>
                  <Button
                    size="sm"
                    onClick={() => {
                      resetCallback();
                      setShowStatusModal(false);
                    }}
                  >
                    Cancel
                  </Button>
                </div>
              </Alert>
            </React.Fragment>
          );
        } else {
          setResubCheckStatus(
            updateUploadStatusMessage(Constants.UPLOAD_STATE.RESUB_CHECK, Constants.UPLOAD_STATE_STATUS.COMPLETE)
          );
          handleUploadFile(apiPath, data, resetCallback);
        }
      })
      .catch(error => {
        setResubCheckStatus(
          updateUploadStatusMessage(Constants.UPLOAD_STATE.RESUB_CHECK, Constants.UPLOAD_STATE_STATUS.ERROR)
        );
        setErrorMessages(error.response.data.errors);
        resetCallback();
        handleFileSubmitted();
      });
  };

  const handleUploadFile = (apiPath, data, resetCallback) => {
    setSavingStatus(updateUploadStatusMessage(Constants.UPLOAD_STATE.SAVING, Constants.UPLOAD_STATE_STATUS.START));
    api.instance
      .post(apiPath, data)
      .then(response => {
        setSavingStatus(
          updateUploadStatusMessage(Constants.UPLOAD_STATE.SAVING, Constants.UPLOAD_STATE_STATUS.COMPLETE)
        );
        setCompleteMessage(response.data);
        resetCallback();
      })
      .catch(error => {
        setSavingStatus(updateUploadStatusMessage(Constants.UPLOAD_STATE.SAVING, Constants.UPLOAD_STATE_STATUS.ERROR));
        setErrorMessages(error.response.data.errors);
      })
      .finally(() => {
        resetCallback();
        handleFileSubmitted();
      });
  };

  const validateFile = (e, setFieldValue, setFieldError, fieldName, sizeLimit) => {
    const file = e.currentTarget.files[0];

    if (file.name.length > 100) {
      setFieldError(fieldName, 'The file name exceeds the maximum allowed length of 100 characters.');
      return;
    }

    if (!isFileCsvType(file)) {
      setFieldError(fieldName, 'The selected file is not a CSV file');
      return;
    }

    if (file.size > sizeLimit * 1024 * 1024) {
      setFieldError(fieldName, `The selected file exceeds the ${sizeLimit}MB size limit`);
      return;
    }

    setFieldValue(fieldName, e.currentTarget.files[0]);
  };

  return (
    <React.Fragment>
      <Formik enableReinitialize={true} initialValues={defaultFormValues}>
        {({ values, errors, setFieldValue, setFieldError }) => (
          <Form>
            <React.Fragment>
              <FormRow name="reportTypeId" label="Report Type">
                <SingleDropdownField defaultTitle="Select Report Type" items={reportTypes} name="reportTypeId" />
              </FormRow>
              {values.reportTypeId && (
                <React.Fragment>
                  <FormGroup row>
                    <Label for="reportFileBrowser" sm={3}>
                      Report File
                    </Label>
                    <Col sm={9}>
                      <Alert color="info">
                        File restrictions:{' '}
                        <ul>
                          <li>.csv files only</li>
                          <li>
                            Up to {reportTypes.find(o => o.id === values.reportTypeId).fileSizeLimitMb}MB per file
                          </li>
                        </ul>
                      </Alert>
                      <CustomInput
                        type="file"
                        id="reportFileBrowser"
                        name="reportFile"
                        label="Select Report File"
                        accept=".csv"
                        onChange={e =>
                          validateFile(
                            e,
                            setFieldValue,
                            setFieldError,
                            'reportFile',
                            reportTypes.find(o => o.id === values.reportTypeId).fileSizeLimitMb
                          )
                        }
                        key={fileInputKey}
                        invalid={errors.reportFile && errors.reportFile.length > 0}
                      />
                      {errors.reportFile && (
                        <FormFeedback style={{ display: 'unset' }}>{errors.reportFile}</FormFeedback>
                      )}
                    </Col>
                  </FormGroup>
                  <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
                    <SubmitButton
                      size="sm"
                      type="Button"
                      disabled={!values.reportFile || submitting || (errors.reportFile && errors.reportFile.length > 0)}
                      submitting={submitting}
                      onClick={() => handleSubmit(values, setFieldValue)}
                    >
                      Submit
                    </SubmitButton>
                  </div>
                </React.Fragment>
              )}
            </React.Fragment>
          </Form>
        )}
      </Formik>
      <SimpleModalWrapper
        isOpen={showStatusModal}
        toggle={() => {
          if (!submitting) setShowStatusModal(false);
        }}
        backdrop="static"
        title="Upload Status"
        disableClose={submitting}
        onComplete={resetMessages}
      >
        {resubCheckStatus}
        {savingStatus}
        {errorMessages && Object.keys(errorMessages).length > 0 && (
          <Alert color="danger">
            <p>Upload unsuccessful. The following errors were found:</p>
            <ul style={{ marginLeft: '-40px' }}>
              {Object.keys(errorMessages).map(key => {
                return (
                  <li key={key} style={{ listStyleType: 'none' }}>
                    {key}:
                    <ul>
                      {errorMessages[key].map((message, index) => (
                        <li key={`${key}_${index}`}>{message}</li>
                      ))}
                    </ul>
                  </li>
                );
              })}
            </ul>
          </Alert>
        )}
        {completeMessage && (
          <Alert color="success">
            Upload successful.
            <ul>
              <li>Submission ID: {completeMessage.id}</li>
              <li>Filename: {completeMessage.fileName}</li>
              <li>Service Area: {completeMessage.serviceAreaNumber}</li>
              <li>Type: {reportTypes.find(o => o.id === completeMessage.submissionStreamId).name}</li>
            </ul>
          </Alert>
        )}
      </SimpleModalWrapper>
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    currentUser: state.user.current,
    submissionStreams: state.submissions.streams,
  };
};

export default connect(mapStateToProps, { showValidationErrorDialog })(WorkReportingUpload);
