import React, { useEffect, useState } from 'react';
import { connect } from 'react-redux';
import * as Yup from 'yup';
// import moment from 'moment';

import SingleDateField from '../ui/SingleDateField';
import SingleDropdownField from '../ui/SingleDropdownField';
import PageSpinner from '../ui/PageSpinner';
import { FormRow, FormInput } from './FormInputs';

// import * as api from '../../Api';
import * as Constants from '../../Constants';

// Activity Number - Mandatory, Text input, alpha-numeric
// Activity Name - Mandatory, Text input, alpha-numeric
// Unit - Mandatory, Drop down, single selection
// Maintenance Type - Mandatory, Drop down, single selection
// Location Code - Mandatory, Drop down, single selection
// A
// B
// C
// Point-Line Feature - Mandatory for location code C, Drop down, single selection, not required on the UI for Location Codes A and B
// Point
// Line
// Point or Line

const defaultValues = {
  activityNumber: '',
  activityName: '',
  unit: '',
  maintenanceType: '',
  locationCode: '',
  pointLineFeature: '',
  endDate: null,
};

const validationSchema = Yup.object({
  activityNumber: Yup.string()
    .required('Required')
    .max(6)
    .trim(),
  activityName: Yup.string()
    .required('Required')
    .max(150)
    .trim(),
  unit: Yup.string()
    .required('Required')
    .max(12),
  maintenanceType: Yup.string()
    .required('Required')
    .max(12),
  locationCode: Yup.number().required('Required'),
  pointLineFeature: Yup.string(12).when('locationCode', { is: 'C', then: Yup.string(12).required('Required') }),
});

const EditActivityFormFields = ({
  setInitialValues,
  formValues,
  setValidationSchema,
  formType,
  activityId,
  maintenanceTypes,
  unitOfMeasures,
}) => {
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setValidationSchema(validationSchema);

    setLoading(true);

    if (formType === Constants.FORM_TYPE.ADD) {
      setInitialValues(defaultValues);
      setLoading(false);
    } else {
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (loading || formValues === null) return <PageSpinner />;

  return (
    <React.Fragment>
      <FormRow name="activityNumber" label="Activity Code*">
        <FormInput type="text" name="activityNumber" placeholder="Activity Code" />
      </FormRow>
      <FormRow name="activityName" label="Activity Name*">
        <FormInput type="text" name="activityName" placeholder="Activity Name" />
      </FormRow>
      <FormRow name="unit" label="Unit*">
        <SingleDropdownField defaultTitle="Select Unit" items={unitOfMeasures} name="unit" />
      </FormRow>
      <FormRow name="maintenanceType" label="Maintenance Type*">
        <SingleDropdownField defaultTitle="Select Maintenance Type" items={maintenanceTypes} name="maintenanceType" />
      </FormRow>
      <FormRow name="locationCode" label="Location Code*">
        <SingleDropdownField defaultTitle="Select Location Code" items={[]} name="locationCode" />
      </FormRow>
      <FormRow name="pointLineFeature" label="Point Line Feature*">
        <SingleDropdownField defaultTitle="Select Point Line Feature" items={[]} name="pointLineFeature" />
      </FormRow>
      <FormRow name="endDate" label="End Date">
        <SingleDateField name="endDate" placeholder="End Date" />
      </FormRow>
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    maintenanceTypes: state.codeLookups.maintenanceTypes,
    unitOfMeasures: state.codeLookups.unitOfMeasures,
  };
};

export default connect(mapStateToProps)(EditActivityFormFields);
