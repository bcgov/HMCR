import React, { useState, useEffect, useCallback } from 'react';
import { Col, FormGroup, Label, Alert, Button, Row, Table } from 'reactstrap';
import AddSaltReportFormFields from './forms/saltreport/AddSaltReportFormFields';
import * as api from '../Api';
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
import { usePDF } from 'react-to-pdf';
import { pdfExportOptions } from './forms/saltreport/DefaultValues';

const SaltReporting = ({ currentUser }) => {
  const [loading, setLoading] = useState(false);
  const [reportsData, setReportsData] = useState([]);
  const [showSaltReportStatusModal, setShowSaltReportStatusModal] = useState(false);
  const [saltReportCompleteMessage, setSaltReportCompleteMessage] = useState(null);
  const { toPDF, targetRef } = usePDF(pdfExportOptions);

  useEffect(() => {
    const fetchReports = async () => {
      try {
        const currentUserSAIds = currentUser.serviceAreas.map(sa => sa.id).join();
        const response = await api.getSaltReportsJson({
          headers: { Accept: 'application/json' },
          fromDate: moment().subtract(5, 'years').format('YYYY-MM-DD HH:mm:ss'),
          toDate: moment().format('YYYY-MM-DD HH:mm:ss'),
          format: 'json',
          serviceAreas: currentUserSAIds,
        });
        setReportsData(response.data || []);
      } catch (error) {
        console.error('Error fetching salt reports:', error);
        setReportsData([]); // Fallback to empty array
      }
    };

    fetchReports();
  }, [currentUser]);

  const handleSaltReportSubmit = useCallback(async (values) => {
    setLoading(true);
    setShowSaltReportStatusModal(true);
    try {
      const stagingTableName = 'HMR_SALT_REPORT';
      const apiPath = Constants.REPORT_TYPES[stagingTableName].api;
      const response = await api.instance.post(apiPath, values);
      setSaltReportCompleteMessage(`Report successfully created. Details: ${response.status}, ${response.statusText}`);
    } catch (error) {
      showValidationErrorDialog(error.response?.data || 'An unexpected error occurred');
    } finally {
      setLoading(false);
    }
  }, []);

  const saltReportFormModal = useFormModal(
    'Annual Salt Report',
    <AddSaltReportFormFields />,
    handleSaltReportSubmit,
    'xl'
  );

  const downloadPdf = useCallback(async (index) => {
    setShowSaltReportStatusModal(true);
    await toPDF();
    setShowSaltReportStatusModal(false);
  }, [toPDF]);

  const openSaltFormModal = useCallback(() => {
    saltReportFormModal.openForm(Constants.FORM_TYPE.ADD);
  }, [saltReportFormModal]);

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
                    Changes are automatically saved within the browser tab and discarded when this browser tab is closed.
                    <br />
                    Provide a copy of current Salt Management Plan following form submission to: <a href="mailto: Maintenance.Programs@gov.bc.ca">Maintenance.Programs@gov.bc.ca</a>
                  </Alert>
                  <Button size="sm" color="primary" className="mr-2" type="button" onClick={openSaltFormModal}>
                    Open Form
                  </Button>
                </Col>
              </FormGroup>
            </Col>
          </Row>
        </MaterialCard>
        <MaterialCard>
          {loading ? <PageSpinner /> : (
            reportsData.length ? (
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
            ) : "No reports available."
          )}
        </MaterialCard>
      </Authorize>
      {saltReportFormModal.formElement}
      <SimpleModalWrapper
        isOpen={showSaltReportStatusModal}
        toggle={() => setShowSaltReportStatusModal(false)}
        backdrop="static"
        title="Report Submission"
      >
        {saltReportCompleteMessage ? (
          <Alert color="info">{saltReportCompleteMessage}</Alert>
        ) : (
          <PageSpinner />
        )}
      </SimpleModalWrapper>
    </>
  );
};

const mapStateToProps = state => ({
  currentUser: state.user.current,
  serviceAreas: Object.values(state.serviceAreas),
});

export default connect(mapStateToProps)(SaltReporting);
