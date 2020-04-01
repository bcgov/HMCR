import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Alert, Button, Spinner } from 'reactstrap';
import { Formik, Form } from 'formik';
import * as Yup from 'yup';
import moment from 'moment';
import FileSaver from 'file-saver';

import { fetchActivityCodesDropdown, hideErrorDialog } from '../actions';

import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';
import SingleDropdownField from './ui/SingleDropdownField';
import DateRangeField from './ui/DateRangeField';
import MultiDropdownField from './ui/MultiDropdownField';
import { FormInput } from './forms/FormInputs';
import SimpleModalWrapper from './ui/SimpleModalWrapper';
import PageSpinner from './ui/PageSpinner';

import * as Constants from '../Constants';
import * as api from '../Api';

const filterContainerStyle = {
  width: '200px',
  marginRight: '1rem',
};

const defaultSearchFormValues = {
  reportTypeId: '',
  dateFrom: moment()
    .subtract(1, 'months')
    .startOf('month'),
  dateTo: moment()
    .subtract(1, 'months')
    .endOf('month'),
  serviceAreaNumbers: [],
  highwayUnique: '',
  maintenanceTypeIds: [],
  activityNumberIds: [],
  outputFormat: 'csv',
};

const validationSchema = Yup.object({
  reportTypeId: Yup.string()
    .required('Required')
    .trim(),
  dateFrom: Yup.object().required('Required'),
  dateTo: Yup.object().required('Required'),
  outputFormat: Yup.string().required('Reuired'),
});

const EXPORT_STAGE = {
  WAIT: 'WAIT',
  ERROR: 'ERROR',
  NOT_FOUND: 'NOT_FOUND',
  DONE: 'DONE',
};

const ReportExport = ({
  reportTypes,
  maintenanceTypes,
  serviceAreas,
  currentUser,
  activityCodes,
  fetchActivityCodesDropdown,
  hideErrorDialog,
}) => {
  const [validServiceAreas, setValidServiceAreas] = useState([]);
  const [submitting, setSubmitting] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [exportStage, setExportStage] = useState(EXPORT_STAGE.WAIT);
  const [exportResult, setExportResult] = useState({});
  const [loading, setLoading] = useState(true);
  const [supportedFormats, setSupportedFormats] = useState([]);

  useEffect(() => {
    const currentUserSAIds = currentUser.serviceAreas.map(sa => sa.id);
    setValidServiceAreas(serviceAreas.filter(sa => currentUserSAIds.includes(sa.id)));

    if (activityCodes.length === 0) {
      fetchActivityCodesDropdown();
    }

    api
      .getExportSupportedFormats()
      .then(response => setSupportedFormats(response.data))
      .finally(() => setLoading(false));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const disableFutureDates = date => date.isAfter(moment().endOf('day'));

  const isRequiredFieldsSet = formikProps =>
    formikProps.values.reportTypeId && formikProps.values.dateFrom && formikProps.values.dateTo;

  const buildExportParams = (values, dateFrom, dateTo) => {
    const queryParams = {};
    const cql_filters = [];

    queryParams.typeName = `hmr:${values.reportTypeId}_VW`;
    queryParams.format = values.outputFormat;

    const serviceAreas = values.serviceAreaNumbers.join(',');
    const highwayUnique = values.highwayUnique.trim().replace(/\*/g, '%');

    queryParams.fromDate = dateFrom;
    queryParams.toDate = dateTo;

    if (values.serviceAreaNumbers.length > 0) {
      queryParams.serviceAreas = serviceAreas;
    }

    if (highwayUnique.length > 0) {
      cql_filters.push(`(HIGHWAY_UNIQUE LIKE '%${highwayUnique}%' OR HIGHWAY_UNIQUE_NAME LIKE '%${highwayUnique}%')`);
    }

    if (values.reportTypeId === 'HMR_WORK_REPORT') {
      // Maintenance Type is stored as Record Type in the table/view
      // Maintenance Type and Record Type are used interchangeably elsewhere
      const recordTypes = values.maintenanceTypeIds.map(value => `'${value}'`).join(',');
      const activityNumbers = values.activityNumberIds.map(value => `'${value}'`).join(',');

      if (values.maintenanceTypeIds.length > 0) {
        cql_filters.push(`RECORD_TYPE IN (${recordTypes})`);
      }

      if (values.activityNumberIds.length > 0) {
        cql_filters.push(`ACTIVITY_NUMBER IN (${activityNumbers})`);
      }
    }

    if (cql_filters.length > 0) {
      queryParams.cql_filter = cql_filters.join(' AND ');
    }

    return queryParams;
  };

  const submitExport = values => {
    setSubmitting(true);
    setShowModal(true);
    setExportStage(EXPORT_STAGE.WAIT);

    const dateFrom = values.dateFrom.startOf('day').format(Constants.DATE_DISPLAY_FORMAT);
    const dateTo = values.dateTo.endOf('day').format(Constants.DATE_DISPLAY_FORMAT);

    api
      .getReportExport(buildExportParams(values, dateFrom, dateTo))
      .then(response => {
        const fileExtensionHeaders = response.headers['content-disposition'].match(/.csv|.json|.gml|.kml|.kmz/i);

        let fileName = `${values.reportTypeId}_Export_${dateFrom}-${dateTo}`;
        if (fileExtensionHeaders) fileName += fileExtensionHeaders[0];

        let data = response.data;
        if (fileName.indexOf('.json') > -1) data = JSON.stringify(data);

        FileSaver.saveAs(new Blob([data]), fileName);

        setExportResult({ fileName });
        setExportStage(EXPORT_STAGE.DONE);
      })
      .catch(error => {
        if (error.response) {
          const response = error.response;

          if (response.status === 422) {
            setExportResult({ error: error.response.data });
            setExportStage(EXPORT_STAGE.ERROR);
          } else if (response.status === 404) {
            hideErrorDialog();
            setExportStage(EXPORT_STAGE.NOT_FOUND);
          }
        }
      })
      .finally(() => setSubmitting(false));
  };

  const renderContent = () => {
    switch (exportStage) {
      case EXPORT_STAGE.NOT_FOUND:
        return (
          <Alert color="warning">
            <p>
              <strong>No Results Found</strong>
            </p>
            <p>There are no results matching the provided search criterion</p>
          </Alert>
        );
      case EXPORT_STAGE.ERROR:
        return (
          <Alert color="danger">
            <p>
              <strong>{exportResult.error.title}</strong>
            </p>
            <p>{exportResult.error.detail}</p>
          </Alert>
        );
      case EXPORT_STAGE.DONE:
        return (
          <Alert color="success">
            <p>
              <strong>Export Complete</strong>
            </p>
            <p>Your report has been saved to your computer.</p>
            <p>
              <small>{exportResult.fileName}</small>
            </p>
          </Alert>
        );
      default:
        return (
          <div className="text-center">
            <Spinner color="primary" />
            <div className="mt-2">
              <div>Your report is being generated.</div>
              <div>This may take a few minutes.</div>
            </div>
          </div>
        );
    }
  };

  if (loading) {
    return <PageSpinner />;
  }

  return (
    <React.Fragment>
      <Formik
        initialValues={defaultSearchFormValues}
        enableReinitialize={true}
        onSubmit={submitExport}
        onReset={() => {}}
        validationSchema={validationSchema}
      >
        {formikProps => (
          <React.Fragment>
            <MaterialCard>
              <UIHeader>Report Export</UIHeader>
              <Form>
                <div className="d-flex">
                  <div style={filterContainerStyle}>
                    <SingleDropdownField defaultTitle="Select Report Type" items={reportTypes} name="reportTypeId" />
                  </div>
                  <DateRangeField
                    name="reportDate"
                    fromName="dateFrom"
                    toName="dateTo"
                    isOutsideRange={disableFutureDates}
                  />
                </div>
                {isRequiredFieldsSet(formikProps) && (
                  <React.Fragment>
                    <div className="mt-3 mb-1">
                      <strong>Optional filters</strong>
                    </div>
                    <div className="d-flex">
                      <div style={filterContainerStyle}>
                        <MultiDropdownField
                          {...formikProps}
                          title="Service Area"
                          items={validServiceAreas}
                          name="serviceAreaNumbers"
                        />
                      </div>
                      <div style={filterContainerStyle}>
                        <FormInput type="text" name="highwayUnique" placeholder="Highway Unique" />
                      </div>
                      {formikProps.values.reportTypeId === 'HMR_WORK_REPORT' && (
                        <React.Fragment>
                          <div style={filterContainerStyle}>
                            <MultiDropdownField
                              {...formikProps}
                              title="Maintenance Type"
                              items={maintenanceTypes}
                              name="maintenanceTypeIds"
                            />
                          </div>
                          <div style={filterContainerStyle}>
                            <MultiDropdownField
                              {...formikProps}
                              title="Activity Number"
                              items={activityCodes}
                              name="activityNumberIds"
                              searchable={true}
                            />
                          </div>
                        </React.Fragment>
                      )}
                    </div>
                  </React.Fragment>
                )}
              </Form>
            </MaterialCard>
            {isRequiredFieldsSet(formikProps) && (
              <div className="d-flex justify-content-end">
                <div style={{ width: '100px' }} className="mr-2">
                  <SingleDropdownField defaultTitle="Export Format" items={supportedFormats} name="outputFormat" />
                </div>
                <Button color="primary" size="sm" type="button" onClick={formikProps.submitForm} className="mr-2">
                  Export
                </Button>
                <Button color="secondary" size="sm" type="button" onClick={formikProps.resetForm}>
                  Reset
                </Button>
              </div>
            )}
          </React.Fragment>
        )}
      </Formik>
      <SimpleModalWrapper
        isOpen={showModal}
        toggle={() => {
          if (!submitting) setShowModal(false);
        }}
        backdrop="static"
        title="Generating Report"
        disableClose={submitting}
      >
        {renderContent()}
      </SimpleModalWrapper>
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    reportTypes: Object.values(state.submissions.streams).map(item => ({ ...item, id: item.stagingTableName })),
    maintenanceTypes: state.codeLookups.maintenanceTypes,
    serviceAreas: Object.values(state.serviceAreas),
    currentUser: state.user.current,
    activityCodes: state.codeLookups.activityCodes.map(item => ({ ...item, id: item.activityNumber })),
  };
};

export default connect(mapStateToProps, { fetchActivityCodesDropdown, hideErrorDialog })(ReportExport);
