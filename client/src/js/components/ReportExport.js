import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Button, Spinner } from 'reactstrap';
import { Formik, Form } from 'formik';
import * as Yup from 'yup';
import moment from 'moment';

import { fetchActivityCodesDropdown } from '../actions';

import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';
import SingleDropdownField from './ui/SingleDropdownField';
import DateRangeField from './ui/DateRangeField';
import MultiDropdownField from './ui/MultiDropdownField';
import { FormInput } from './forms/FormInputs';
import SimpleModalWrapper from './ui/SimpleModalWrapper';

import * as Constants from '../Constants';
import * as api from '../Api';

const exportFormats = [
  { id: 'csv', name: 'CSV' },
  { id: 'application/vnd.google-earth.kml+xml', name: 'KML' },
  // { id: 'application/vnd.google-earth.kmz', name: 'KMZ' },
  { id: 'application/json', name: 'GeoJSON' },
  { id: 'application/gml+xml; version=3.2', name: 'GML' },
];

const reportSpecificDateField = {
  HMR_WORK_REPORT: 'END_DATE',
  HMR_ROCKFALL_REPORT: 'REPORT_DATE',
  HMR_WILDLIFE_REPORT: 'ACCIDENT_DATE',
};

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

const ReportExport = ({
  reportTypes,
  maintenanceTypes,
  serviceAreas,
  currentUser,
  activityCodes,
  fetchActivityCodesDropdown,
}) => {
  const [validServiceAreas, setValidServiceAreas] = useState([]);
  const [submitting, setSubmitting] = useState(false);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    const currentUserSAIds = currentUser.serviceAreas.map(sa => sa.id);
    setValidServiceAreas(serviceAreas.filter(sa => currentUserSAIds.includes(sa.id)));

    if (activityCodes.length === 0) {
      fetchActivityCodesDropdown();
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const disableFutureDates = date => date.isAfter(moment());

  const isRequiredFieldsSet = formikProps =>
    formikProps.values.reportTypeId && formikProps.values.dateFrom && formikProps.values.dateTo;

  const submitExport = values => {
    setSubmitting(true);
    setShowModal(true);

    const queryParams = {};

    queryParams.typeName = `hmr:${values.reportTypeId}_VW`;
    queryParams.outputFormat = values.outputFormat;
    queryParams.cql_filter = '';

    const dateFrom = values.dateFrom.startOf('day').format(Constants.DATE_DISPLAY_FORMAT);
    const dateTo = values.dateTo.endOf('day').format(Constants.DATE_DISPLAY_FORMAT);
    const serviceAreas = values.serviceAreaNumbers.join(',');
    const highwayUnique = values.highwayUnique.trim();

    queryParams.cql_filter += `${reportSpecificDateField[values.reportTypeId]} BETWEEN ${dateFrom} AND ${dateTo}`;

    if (values.serviceAreaNumbers.length > 0) {
      queryParams.cql_filter += ` AND SERVICE_AREA IN (${serviceAreas})`;
      queryParams.serviceAreas = serviceAreas;
    }

    if (highwayUnique.length > 0) {
      queryParams.cql_filter += ` AND (HIGHWAY_UNIQUE LIKE '${highwayUnique}%' OR HIGHWAY_UNIQUE_NAME LIKE '${highwayUnique}%')`;
    }

    if (values.reportTypeId === 'HMR_WORK_REPORT') {
      // Maintenance Type is stored as Record Type in the table/view
      // Maintenance Type and Record Type are used interchangeably elsewhere
      const recordTypes = values.maintenanceTypeIds.join(',');
      const activityNumbers = values.activityNumberIds.join(',');

      if (values.maintenanceTypeIds.length > 0) {
        queryParams.cql_filter += ` AND RECORD_TYPE IN (${recordTypes})`;
      }

      if (values.activityNumberIds.length > 0) {
        queryParams.cql_filter += ` AND ACTIVITY_NUMBER IN (${activityNumbers})`;
      }
    }

    api.getReportExport(queryParams);

    console.log(values);
    console.log(queryParams);
  };

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
                  <SingleDropdownField defaultTitle="Export Format" items={exportFormats} name="outputFormat" />
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
        <div className="text-center">
          <Spinner color="primary" />
          <div className="mt-2">
            <div>Please wait while your report is being generated.</div>
            <div>This may take a few minutes.</div>
          </div>
        </div>
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
    activityCodes: state.codeLookups.activityCodes,
  };
};

export default connect(mapStateToProps, { fetchActivityCodesDropdown })(ReportExport);
