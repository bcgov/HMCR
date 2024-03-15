import React, { useState, useEffect } from 'react';
import { Col, FormGroup, Label, Alert, Button, Row, Table } from 'reactstrap';
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
import UIHeader from './ui/UIHeader';
import moment from 'moment';
import { connect } from 'react-redux';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const SaltReporting = ({ currentUser, serviceAreas }) => {
  const [loading, setLoading] = useState(false);
  const [reportsData, setReportsData] = useState([]);
  const [showSaltReportStatusModal, setShowSaltReportStatusModal] = useState(false);
  const [saltReportCompleteMessage, setSaltReportCompleteMessage] = useState(null);

  useEffect(() => {
    const fetchSaltReports = async () => {
      setLoading(true);
      const currentUserSAIds = currentUser.serviceAreas.map((sa) => sa.id).join();
      try {
        const response = await api.getSaltReportsJson({
          fromDate: moment().subtract(5, 'years').format('YYYY-MM-DD HH:mm:ss'),
          toDate: moment().format('YYYY-MM-DD HH:mm:ss'),
          serviceAreas: currentUserSAIds,
        });
        setReportsData(response.data || []);
      } catch (error) {
        console.error('Fetching salt reports failed:', error);
        setReportsData([]);
      } finally {
        setLoading(false);
      }
    };

    fetchSaltReports();
  }, [currentUser.serviceAreas]);

  const handleSaltReportSubmit = async (values) => {
    try {
      saltReportFormModal.closeForm();
      setShowSaltReportStatusModal(true);
      setLoading(true);

      const stagingTableName = 'HMR_SALT_REPORT';
      const apiPath = Constants.REPORT_TYPES[stagingTableName].api;
      const response = await api.instance.post(apiPath, values);

      setSaltReportCompleteMessage(`Report successfully created. Details: ${response.status} ${response.statusText}.`);
    } catch (error) {
      console.error('Submitting salt report failed:', error);
      showValidationErrorDialog(error.response?.data || 'An unexpected error occurred');
    } finally {
      setLoading(false);
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
          <UIHeader>Annual Salt Report Form</UIHeader>
          <Formik>
            <Form>
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
                      <Button
                        size="sm"
                        color="primary"
                        className="mr-2"
                        type="button"
                        onClick={() => saltReportFormModal.openForm(Constants.FORM_TYPE.ADD)}
                      >
                        Open Form
                      </Button>
                    </Col>
                  </FormGroup>
                </Col>
                <Col lg="4" />
              </Row>
            </Form>
          </Formik>
        </MaterialCard>
        <MaterialCard>
          <UIHeader>Past Submissions</UIHeader>
          <Alert color="info">
            Download is not available as we're currently working on making the forms you've submitted downloadable as PDF.
          </Alert>
          {loading ? (
            <PageSpinner />
          ) : reportsData.length > 0 ? (
            <Table responsive>
              <thead>
                <tr>
                  <th>Report ID</th>
                  <th>Service Area</th>
                  <th>Contact Name</th>
                  <th>Date Created</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {reportsData.map((item, index) => (
                  <tr key={index}>
                    <td>{item.SaltReportId}</td>
                    <td>{item.ServiceArea}</td>
                    <td>{item.ContactName}</td>
                    <td>{moment(item.AppCreateTimestamp).format('LL')}</td>
                    <td>
                      <Button disabled size="sm" color="primary" className="mr-2" title="Download as PDF">
                        <FontAwesomeIcon icon="download" />
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
          ) : (
            <Alert color="warning">No reports found.</Alert>
          )}
        </MaterialCard>
      </Authorize>
      {saltReportFormModal.formElement}
      <SimpleModalWrapper
        isOpen={showSaltReportStatusModal}
        toggle={() => setShowSaltReportStatusModal(false)}
        backdrop="static"
        title="Report Submission"
        disableClose={loading}
      >
        {saltReportCompleteMessage ? (
          <Alert color="info">
            {saltReportCompleteMessage} Provide a copy of current Salt Management Plan following form submission to:{' '}
            <a href="mailto: Maintenance.Programs@gov.bc.ca">Maintenance.Programs@gov.bc.ca</a>
          </Alert>
        ) : (
          <PageSpinner />
        )}
      </SimpleModalWrapper>
    </>
  );
};

const mapStateToProps = (state) => ({
  currentUser: state.user.current,
  serviceAreas: Object.values(state.serviceAreas),
});

export default connect(mapStateToProps)(SaltReporting);
