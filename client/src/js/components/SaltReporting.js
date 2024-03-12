import React, { useState, useEffect, useRef, forwardRef } from 'react';
import { Col, FormGroup, Label, Alert, Button, Row, Table } from 'reactstrap';

import _ from 'lodash';
import AddSaltReportFormFields from './forms/saltreport/AddSaltReportFormFields';
import * as api from '../Api';
import { Formik, Form } from 'formik';
import { connect } from 'react-redux';

import * as Constants from '../Constants';
import useFormModal from './hooks/useFormModal';
import { showValidationErrorDialog } from '../actions';
import SimpleModalWrapper from './ui/SimpleModalWrapper';
import PageSpinner from './ui/PageSpinner';
import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';
import moment from 'moment';

const SaltReporting = ({ currentUser, serviceAreas }) => {
  const [loading, setLoading] = useState(false);
  const [reportsData, setReportsData] = useState([]);
  const [reportData, setReportData] = useState({});
  const [submitting, setSubmitting] = useState(false);

  const [showSaltReportStatusModal, setShowSaltReportStatusModal] = useState(false);
  const [saltReportCompleteMessage, setSaltReportCompleteMessage] = useState(null);

  useEffect(() => {
    setKey((prevKey) => prevKey + 1);
  }, [reportData]);

  useEffect(() => {
    const currentUserSAIds = currentUser.serviceAreas.map((sa) => sa.id).join();
    console.log(currentUser)

    api
      .getSaltReportsJson({
        headers: {
          Accept: 'application/json',
        },
        fromDate: moment().subtract(5, 'years').format('YYYY-MM-DD HH:mm:ss'),
        toDate: moment().format('YYYY-MM-DD HH:mm:ss'),
        format: 'json',
        serviceAreas: currentUserSAIds,
      })
      .then((response) => {
        if (response.data) {
          setReportsData(response.data);
        } else {
          console.error('Expected an array but received:', response.data);
          setReportsData([]); // Reset to empty array or handle as needed
        }
      })
      .catch((error) => {
        if (error.response) {
          const response = error.response;

          if (response.status === 422) {
          } else if (response.status === 404) {
          }
        }
      });
  }, []);

  const handleSaltReportSubmit = async (values) => {
    try {
      saltReportFormModal.closeForm();
      setLoading(true);
      setShowSaltReportStatusModal(true);

      const stagingTableName = 'HMR_SALT_REPORT';
      const apiPath = Constants.REPORT_TYPES[stagingTableName].api;
      const response = await api.instance.post(apiPath, values);

      setSaltReportCompleteMessage(`Report successfully created. Details: ${(response.status, response.statusText)}`);
    } catch (error) {
      // Handle errors
      showValidationErrorDialog(error.response?.data || 'An unexpected error occurred');
    } finally {
      setLoading(false);
      setSubmitting(false);
    }
  };

  const toObjectCamelCase = (obj) => {
    if (Array.isArray(obj)) {
      return obj.map(toObjectCamelCase);
    } else if (obj !== null && obj.constructor === Object) {
      return Object.keys(obj).reduce((result, key) => {
        const camelCaseKey = key
          .replace(/(?:^\w|[A-Z]|\b\w)/g, (word, index) => (index === 0 ? word.toLowerCase() : word.toUpperCase()))
          .replace(/\s+/g, '');
        result[camelCaseKey] = toObjectCamelCase(obj[key]);
        return result;
      }, {});
    }
    return obj;
  };

  const childRendered = () => {
    console.log('setting')
    setFormRendered(true);
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
          <UIHeader>Annual Salt Report Form</UIHeader>
          <Row>
            <Col lg="8">
              <FormGroup row>
                <Label for="reportFileBrowser" sm={3}>
                  Fill Report
                </Label>
                <Col sm={9}>
                  <Alert color="info">
                    Changes are automatically saved within the browser tab and discarded when this browser tab is
                    closed.
                    <br />
                    <br />
                    Provide a copy of current Salt Management Plan following form submission to:{' '}
                    <a href="mailto: Maintenance.Programs@gov.bc.ca">Maintenance.Programs@gov.bc.ca</a>
                  </Alert>
                  <Button size="sm" color="primary" className="mr-2" type="button" onClick={() => openSaltFormModal()}>
                    Open Form
                  </Button>
                </Col>
              </FormGroup>
            </Col>
            <Col lg="4" />
          </Row>
        </MaterialCard>
        <MaterialCard>
          {reportsData.length ? (
            <Table responsive>
              <thead>
                <tr>
                  <th>Report ID</th>
                  <th>Service Area</th>
                  <th>Contact Name</th>
                  <th>Date created</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {reportsData.map((item, index) => (
                  <tr key={index}>
                    <td>{item.SaltReportId}</td>
                    <td>{item.ServiceArea}</td>
                    <td>{item.ContactName}</td>
                    <td>{moment(item.AppCreateTimestamp).toString()}</td>
                    <td>
                      <Button color="primary" onClick={() => downloadPdf(index)}>
                        Download
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
          ) : (
            <PageSpinner />
          )}
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
        {saltReportCompleteMessage ? (
          <Alert color="info">
            {saltReportCompleteMessage}
            <br />
            <br />
            Provide a copy of current Salt Management Plan following form submission to:{' '}
            <a href="mailto: Maintenance.Programs@gov.bc.ca">Maintenance.Programs@gov.bc.ca</a>
          </Alert>
        ) : (
          <PageSpinner />
        )}
      </SimpleModalWrapper>
    </>
  );
};

const mapStateToProps = (state) => {
  return {
    currentUser: state.user.current,
    serviceAreas: Object.values(state.serviceAreas),
  };
};

export default connect(mapStateToProps, null)(SaltReporting);
