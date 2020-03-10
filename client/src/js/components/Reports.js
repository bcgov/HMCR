import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Button } from 'reactstrap';
import { Formik, Form } from 'formik';
import * as Yup from 'yup';
import moment from 'moment';

import { fetchActivityCodesDropdown } from '../actions';

import MaterialCard from './ui/MaterialCard';
import SingleDropdownField from './ui/SingleDropdownField';
import DateRangeField from './ui/DateRangeField';
import MultiDropdownField from './ui/MultiDropdownField';
import { FormInput } from './forms/FormInputs';

// import * as Constants from '../Constants';

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
};

const validationSchema = Yup.object({
  reportTypeId: Yup.string()
    .required('Required')
    .trim(),
  dateFrom: Yup.object().required('Required'),
  dateTo: Yup.object().required('Required'),
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

  return (
    <React.Fragment>
      <h1>Reports Export</h1>

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
                {formikProps.values.reportTypeId && formikProps.values.dateFrom && formikProps.values.dateTo && (
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
                        />
                      </div>
                    </div>
                  </React.Fragment>
                )}
              </Form>
            </MaterialCard>
            <Button onClick={formikProps.submitForm}>Submit</Button>
          </React.Fragment>
        )}
      </Formik>
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
