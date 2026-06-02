import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Formik, Form } from 'formik';
import moment from 'moment';
import * as api from '../Api';
import * as Constants from '../Constants';
import { showValidationErrorDialog } from '../actions';
import useFormModal from './hooks/useFormModal';
import useSearchData from './hooks/useSearchData';
import PageSpinner from './ui/PageSpinner';
import SimpleModalWrapper from './ui/SimpleModalWrapper';
import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';
import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';
import Authorize from './fragments/Authorize';
import AddSaltReportFormFields from './forms/saltreport/AddSaltReportFormFields';
import { Col, FormGroup, Label, Alert, Button, Row } from 'reactstrap';

const SaltReporting = ({ currentUser }) => {
  const [loading, setLoading] = useState(false);
  const [showSaltReportStatusModal, setShowSaltReportStatusModal] = useState(false);
  const [saltReportCompleteMessage, setSaltReportCompleteMessage] = useState(null);
  const [saltReportSuccess, setSaltReportSuccess] = useState(false);

  var defaultSearchOptions = {
    fromDate: moment().subtract(1, 'years').format('YYYY-MM-DD'),
    toDate: moment().format('YYYY-MM-DD'),
    serviceAreas: currentUser.serviceAreas.map((sa) => sa.id).join(','),
    pageNumber: 1,
    isActive: true,
    pageSize: 5,
    dataPath: Constants.API_PATHS.SALT_REPORT,
  };

  const searchData = useSearchData(defaultSearchOptions);

  useEffect(() => {
    searchData.updateSearchOptions({
      ...defaultSearchOptions,
    });
  }, []);

  const handleSaltReportSubmit = async (values, formType) => {
    const isEditMode = formType === Constants.FORM_TYPE.EDIT;

    try {
      saltReportFormModal.closeForm();
      setShowSaltReportStatusModal(true);
      setLoading(true);

      const stagingTableName = 'HMR_SALT_REPORT';
      const apiPath = Constants.REPORT_TYPES[stagingTableName].api;
      const response = isEditMode
        ? await api.putSaltReport(values.saltReportId, values)
        : await api.instance.post(apiPath, values);
      setLoading(false);

      setSaltReportCompleteMessage(
        `Report successfully ${isEditMode ? 'updated' : 'created'}. Details: ${response.status} ${response.statusText}.`
      );
      setSaltReportSuccess(true);
      searchData.refresh();
    } catch (error) {
      setLoading(false);
      console.error(error);
      setSaltReportCompleteMessage(`Report submission failed.  ${error.response?.data.error || error.response?.data || ''}`);
      setSaltReportSuccess(false);
    } finally {
      setLoading(false);
    }
  };

  const handleSaltReportEditClicked = async (saltReportId) => {
    try {
      setLoading(true);
      const response = await api.getSaltReport(saltReportId);
      saltReportFormModal.openForm(Constants.FORM_TYPE.EDIT, { initialReport: response.data });
    } catch (error) {
      console.error(error);
      setSaltReportCompleteMessage(`Unable to load salt report. ${error.response?.data.error || error.response?.data || ''}`);
      setSaltReportSuccess(false);
      setShowSaltReportStatusModal(true);
    } finally {
      setLoading(false);
    }
  };

  const tableColumns = [
    { heading: 'Report ID', key: 'saltReportId' },
    { heading: 'Service Area', key: 'serviceArea' },
    { heading: 'Contact Name/Title', key: 'contactName' },
    { heading: 'Date Created', key: 'appCreateTimestamp', format: (date) => moment.utc(date).local().format('LLL') },
  ];

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
                        className="me-2"
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
        {searchData.loading && <PageSpinner />}
        {!searchData.loading && (
          <MaterialCard>
            <UIHeader>Past Submissions</UIHeader>
            {searchData.data.length > 0 ? (
              <DataTableWithPaginaionControl
                dataList={searchData.data}
                tableColumns={tableColumns}
                searchPagination={searchData.pagination}
                onPageNumberChange={searchData.handleChangePage}
                onPageSizeChange={searchData.handleChangePageSize}
                editable={true}
                editPermissionName={Constants.PERMISSIONS.FILE_W}
                editKey="saltReportId"
                deletable={false}
                onEditClicked={handleSaltReportEditClicked}
                onHeadingSortClicked={searchData.handleHeadingSortClicked}
                showExportButton={true}
                onExportClicked={(saltReportId) => {
                  console.log(`Exporting report with ID: ${saltReportId}`);
                  api.downloadSaltReportPdf(saltReportId);
                }}
              />
            ) : (
              <Alert color="warning">No reports found.</Alert>
            )}
          </MaterialCard>
        )}
      </Authorize>
      {saltReportFormModal.formElement}
      <SimpleModalWrapper
        isOpen={showSaltReportStatusModal}
        toggle={() => setShowSaltReportStatusModal(false)}
        backdrop="static"
        title="Report Submission"
        disableClose={loading}
      >
        {!loading && saltReportCompleteMessage ? (
          <Alert color={saltReportSuccess ? 'success' : 'danger'}>
            {saltReportCompleteMessage}
            {saltReportSuccess && (
              <>
                Provide a copy of current Salt Management Plan following form submission to:{' '}
                <a href="mailto:Maintenance.Programs@gov.bc.ca">Maintenance.Programs@gov.bc.ca</a>
              </>
            )}
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
