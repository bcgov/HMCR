import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Button, ButtonGroup, Spinner } from 'reactstrap';
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

// import * as Constants from '../Constants';

const exportFormats = [
  { id: 'csv', name: 'CSV' },
  { id: 'application/vnd.google-earth.kml+xml', name: 'KML' },
  { id: 'application/vnd.google-earth.kmz', name: 'KMZ' },
  { id: 'json', name: 'GeoJSON' },
  { id: 'application/gml+xml; version=3.2', name: 'GML' },
];

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
  highwayUniqueId: '',
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

const Reports = ({
  reportTypes,
  maintenanceTypes,
  serviceAreas,
  currentUser,
  activityCodes,
  fetchActivityCodesDropdown,
}) => {
  const [validServiceAreas, setValidServiceAreas] = useState([]);
  const [submitting, setSubmitting] = useState(true);
  const [showModal, setShowModal] = useState(true);

  useEffect(() => {
    const currentUserSAIds = currentUser.serviceAreas.map(sa => sa.id);
    setValidServiceAreas(serviceAreas.filter(sa => currentUserSAIds.includes(sa.id)));

    if (activityCodes.length === 0) {
      fetchActivityCodesDropdown();
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function disableFutureDates(date) {
    return date.isAfter(moment());
  }

  function isRequiredFieldsSet(formikProps) {
    return formikProps.values.reportTypeId && formikProps.values.dateFrom && formikProps.values.dateTo;
  }

  return (
    <React.Fragment>
      <UIHeader>Report Export</UIHeader>
      <Formik
        initialValues={defaultSearchFormValues}
        enableReinitialize={true}
        onSubmit={values => {
          console.log(values);
        }}
        onReset={() => {}}
        validationSchema={validationSchema}
      >
        {formikProps => (
          <React.Fragment>
            <MaterialCard>
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
                        <FormInput type="text" name="highwayUniqueId" placeholder="Highway Unique" />
                      </div>
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
                <ButtonGroup>
                  <Button color="primary" size="sm" type="button" onClick={formikProps.submitForm}>
                    Export
                  </Button>
                  <Button color="secondary" size="sm" type="button" onClick={formikProps.resetForm}>
                    Reset
                  </Button>
                </ButtonGroup>
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
    reportTypes: Object.values(state.submissions.streams),
    maintenanceTypes: state.codeLookups.maintenanceTypes,
    serviceAreas: Object.values(state.serviceAreas),
    currentUser: state.user.current,
    activityCodes: state.codeLookups.activityCodes,
  };
};

export default connect(mapStateToProps, { fetchActivityCodesDropdown })(Reports);
