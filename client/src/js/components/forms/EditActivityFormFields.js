import React, { useEffect, useState } from 'react';
import { connect } from 'react-redux';
import * as Yup from 'yup';
import moment from 'moment';

import SingleDateField from '../ui/SingleDateField';
import SingleDropdownField from '../ui/SingleDropdownField';
import PageSpinner from '../ui/PageSpinner';
import { FormRow, FormInput, FormCheckboxInput } from './FormInputs';

import * as api from '../../Api';
import * as Constants from '../../Constants';

const defaultValues = {
  activityNumber: '',
  activityName: '',
  unitOfMeasure: '',
  maintenanceType: '',
  locationCodeId: '',
  pointLineFeature: '',
  isSiteNumRequired: false,
  endDate: null,
};

const validationSchema = Yup.object({
  activityNumber: Yup.string()
    .matches(/^[a-z0-9]+$/i, 'Numbers or letters only')
    .required('Required')
    .max(6)
    .trim(),
  activityName: Yup.string()
    .required('Required')
    .max(150)
    .trim(),
  unitOfMeasure: Yup.string()
    .required('Required')
    .max(12),
  maintenanceType: Yup.string()
    .required('Required')
    .max(12),
  locationCodeId: Yup.number().required('Required'),
});

const EditActivityFormFields = ({
  setInitialValues,
  formValues,
  setValidationSchema,
  formType,
  activityId,
  maintenanceTypes,
  unitOfMeasures,
  locationCodes,
  pointLineFeatures,
}) => {
  const [loading, setLoading] = useState(true);
  const [validLocationCodeValues, setValidLocationCodeValues] = useState(locationCodes);
  const [disableLocationCodeEdit, setDisableLocationCodeEdit] = useState(false);
  const [validPointLineFeatureValues, setValidPointLineFeatureValues] = useState(pointLineFeatures);
  const locationCodeCId = locationCodes.find(code => code.name === 'C').id;

  useEffect(() => {
    // Add validation for point line feature when location code is C.
    // Need to get the id value of location code C
    const defaultValidationSchema = validationSchema.shape({
      pointLineFeature: Yup.string()
        .nullable()
        .when('locationCodeId', {
          is: locationCodeCId,
          then: Yup.string()
            .required('Required')
            .max(12),
        }),
      isSiteNumRequired: Yup.boolean().when('locationCodeId', {
        is: locationCodeCId,
        then: Yup.boolean().required('Required'),
      }),
    });

    setValidationSchema(defaultValidationSchema);

    setLoading(true);

    if (formType === Constants.FORM_TYPE.ADD) {
      setInitialValues(defaultValues);
      setLoading(false);
    } else {
      api.getActivityCode(activityId).then(response => {
        setInitialValues({
          ...response.data,
          endDate: response.data.endDate ? moment(response.data.endDate) : null,
        });

        setValidLocationCodeValues(() => {
          if (formType === Constants.FORM_TYPE.EDIT) {
            if (response.data.locationCodeId === locationCodes.find(code => code.name === 'B').id)
              return locationCodes.filter(location => location.name !== 'C');
          }

          return locationCodes;
        });

        setDisableLocationCodeEdit(() => {
          if (formType === Constants.FORM_TYPE.EDIT) {
            if (response.data.locationCodeId === locationCodes.find(code => code.name === 'A').id) return true;
          }
          return false;
        });

        setValidPointLineFeatureValues(() => {
          if (formType === Constants.FORM_TYPE.EDIT) {
            if (response.data.pointLineFeature === 'Either')
              return pointLineFeatures.filter(feature => feature.id === 'Either');

            if (response.data.pointLineFeature)
              return pointLineFeatures.filter(
                feature => feature.id === 'Either' || feature.id === response.data.pointLineFeature
              );
          }

          return pointLineFeatures;
        });

        setLoading(false);
      });
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (loading || formValues === null) return <PageSpinner />;

  return (
    <React.Fragment>
      <FormRow name="activityNumber" label="Activity Code*">
        <FormInput
          type="text"
          name="activityNumber"
          placeholder="Activity Code"
          disabled={formType === Constants.FORM_TYPE.EDIT}
        />
      </FormRow>
      <FormRow name="activityName" label="Activity Name*">
        <FormInput type="text" name="activityName" placeholder="Activity Name" />
      </FormRow>
      <FormRow name="unitOfMeasure" label="Unit*">
        <SingleDropdownField
          defaultTitle="Select Unit"
          items={unitOfMeasures}
          name="unitOfMeasure"
          disabled={formType === Constants.FORM_TYPE.EDIT}
        />
      </FormRow>
      <FormRow name="maintenanceType" label="Maintenance Type*">
        <SingleDropdownField
          defaultTitle="Select Maintenance Type"
          items={maintenanceTypes}
          name="maintenanceType"
          disabled={formType === Constants.FORM_TYPE.EDIT}
        />
      </FormRow>
      <FormRow name="locationCodeId" label="Location Code*">
        <SingleDropdownField
          defaultTitle="Select Location Code"
          items={validLocationCodeValues}
          name="locationCodeId"
          disabled={disableLocationCodeEdit}
        />
      </FormRow>
      {formValues.locationCodeId === locationCodeCId && (
        <React.Fragment>
          <FormRow name="pointLineFeature" label="Point Line Feature*">
            <SingleDropdownField
              defaultTitle="Select Point Line Feature"
              items={validPointLineFeatureValues}
              name="pointLineFeature"
            />
          </FormRow>
          <FormRow name="isSiteNumRequired" label="Site Number Required">
            <FormCheckboxInput name="isSiteNumRequired" />
          </FormRow>
        </React.Fragment>
      )}
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
    locationCodes: state.codeLookups.locationCodes,
    pointLineFeatures: state.codeLookups.pointLineFeatures,
  };
};

export default connect(mapStateToProps)(EditActivityFormFields);
