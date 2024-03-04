import React, { useState } from 'react';
import { Col, FormGroup, Label, Alert, Button, Row } from 'reactstrap';

import _ from 'lodash';
import AddSaltReportFormFields from './forms/saltreport/AddSaltReportFormFields';
import * as api from '../Api';
import { Formik, Form } from 'formik';

import * as Constants from '../Constants';
import useFormModal from './hooks/useFormModal';
import { showValidationErrorDialog } from '../actions';
import SimpleModalWrapper from './ui/SimpleModalWrapper';
import PageSpinner from './ui/PageSpinner';
import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';

const SaltReporting = ({ currentUser }) => {
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  const [showSaltReportStatusModal, setShowSaltReportStatusModal] = useState(false);
  const [saltReportCompleteMssage, setSaltReportCompleteMessage] = useState(null);

  const handleSaltReportSubmit = async (values) => {
    try {
      saltReportFormModal.closeForm();
      setLoading(true);
      setShowSaltReportStatusModal(true);

      const stagingTableName = 'HMR_SALT_REPORT';
      const apiPath = Constants.REPORT_TYPES[stagingTableName].api;
      const response = await api.instance.post(apiPath, values);

      console.log(response);
      setSaltReportCompleteMessage(`Report successfully created. Details: ${(response.status, response.statusText)}`);
    } catch (error) {
      // Handle errors
      console.error(error);
      showValidationErrorDialog(error.response?.data || 'An unexpected error occurred');
    } finally {
      setLoading(false);
      setSubmitting(false);
    }
  };

  const saltReportFormModal = useFormModal(
    'Annual Salt Report',
    <AddSaltReportFormFields />,
    handleSaltReportSubmit,
    'xl'
  );

  return (
    <>
      <Authorize requires={Constants.PERMISSIONS.FILE_W}>
        <MaterialCard>
          <Row>
            <Col lg="8">
              <FormGroup row>
                <Label for="reportFileBrowser" sm={3}>
                  Fill Report
                </Label>
                <Col sm={9}>
                  <Alert color="info">Changes are automatically saved within the browser session</Alert>
                  <Button
                    size="sm"
                    color="primary"
                    className="mr-2"
                    type="button"
                    onClick={() => saltReportFormModal.openForm(Constants.FORM_TYPE.ADD)}
                  >
                    Open Form
                  </Button>
                  {/* <Button type="button" onClick={testExportApi}>
          Export Test
        </Button> */}
                </Col>
              </FormGroup>
            </Col>
            <Col lg="4" />
          </Row>
        </MaterialCard>
      </Authorize>
      {saltReportFormModal.formElement}
      <SimpleModalWrapper
        isOpen={showSaltReportStatusModal}
        toggle={() => {
          if (!submitting) setShowSaltReportStatusModal(false);
        }}
        backdrop="static"
        title="Report Submission"
        disableClose={submitting}
      >
        {saltReportCompleteMssage || <PageSpinner />}
      </SimpleModalWrapper>
    </>
  );
};

export default SaltReporting;
