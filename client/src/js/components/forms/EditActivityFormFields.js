import React, { useEffect, useState } from 'react';
import { useFormikContext} from 'formik';
import { connect } from 'react-redux';
import * as Yup from 'yup';
import moment from 'moment';

import SingleDateField from '../ui/SingleDateField';
import MultiSelect from '../ui/MultiSelect';
import SingleDropdownField from '../ui/SingleDropdownField';
import PageSpinner from '../ui/PageSpinner';
import FieldSet from '../ui/FieldSet';

import { FormRow, FormInput, FormCheckboxInput } from './FormInputs';

import * as api from '../../Api';
import * as Constants from '../../Constants';
import { Row,Col} from 'reactstrap';
import { isInteger, toString } from 'lodash';
import {isValueEmpty,isValueNotEmpty,toStringOrEmpty} from '../../utils'

const tipAnalyticalValidation = [<ul key ='tipAnalyticalValidation_ul_key_1' style={{ paddingInlineStart: 10 }}>
  <li>Analytical Validation <em>Help 1</em></li>
  <li>Analytical Validation <em>Help 2</em></li>
  </ul>];
const tipHighwayAttributeValidation = [<ul key ='tipHighwayAttributeValidation_ul_key_1' style={{ paddingInlineStart: 10 }}>
  <li >Highway Attribute Validation Help</li>
  </ul>];

const defaultValues = {
  activityNumber: '',
  activityName: '',
  unitOfMeasure: '',
  maintenanceType: '',
  locationCodeId: '',
  featureType: '',
  spThresholdLevel: '',
  minValue: '',
  maxValue: '',
  reportingFrequency: '',
  roadLengthRule: '',
  surfaceTypeRule: '',
  roadClassRule: '',
  isSiteNumRequired: false,
  serviceAreaNumbers: [],
  endDate: null,
};

const validationSchema = Yup.object({
  activityNumber: Yup.string()
    .matches(/^[a-z0-9]+$/i, 'Numbers or letters only')
    .required('Required')
    .max(6)
    .trim(),
  activityName: Yup.string().required('Required').max(150).trim(),
  unitOfMeasure: Yup.string().required('Required').max(12),
  maintenanceType: Yup.string().required('Required').max(12),
  locationCodeId: Yup.number().required('Required'),
  serviceAreaNumbers: Yup.array().required('At least one Service Area must be selected'),
  minValue: Yup.number()
    .min(0,'Must be greater than or equal to 0')
    .nullable()
    .typeError('Must be number')
    .test(
      'datamin',
      function() {
        if (isValueEmpty(this.parent.minValue))
        {
          return true;
        }
        if(this.parent.minValue > 999999999.99)
        {
          return this.createError({
            message: 'Must be less than or equal to 999999999.99',
            path: 'minValue',
            });
        }
        if(isValueNotEmpty(this.parent.maxValue))
        {
          if(Number(this.parent.maxValue) !== 0 && this.parent.maxValue < this.parent.minValue)
          {
            return this.createError({
              message: 'Must be less than or equal to the Maximum value',
              path: 'minValue',
              });
          }
        }
        if (['site','num','ea'].includes(this.parent.unitOfMeasure))
        { 
          if(!isInteger(this.parent.minValue))
          {
            return this.createError({
              message: 'Must be whole number',
              path: 'minValue',
              });
          }
        }
        return true;
      }
    ),
  maxValue: Yup.number()
    .min(0,'Must be greater than or equal to 0')
    .nullable()
    .typeError('Must be number')
    .test(
      'datamax',
      function() {
        if (isValueEmpty(this.parent.maxValue))
        {
          return true;
        }
        if(Number(this.parent.maxValue) > 999999999.99)
        {
          return this.createError({
            message: 'Must be less than or equal to 999999999.99',
            path: 'maxValue',
            });
        }
        if (isValueNotEmpty(this.parent.minValue))
        {
          if(Number(this.parent.maxValue) !== 0 && this.parent.maxValue < this.parent.minValue)
          {
            return this.createError({
              message: 'Must be greater than or equal to the Minimum value',
              path: 'maxValue',
            });
          }
        }
        if (['site','num','ea'].includes(this.parent.unitOfMeasure))
        { 
          if(!isInteger(this.parent.maxValue))
          {
            return this.createError({
            message: 'Must be whole number',
            path: 'maxValue',
            });
          }
        }
        return true;
      }
    ),
  reportingFrequency: Yup.number()
    .min(0,'Must be greater than or equal to 0')
    .max(366,'Must be less than or equal to 366')
    .nullable()
    .typeError('Must be whole number')
    .integer('Must be whole number'),
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
  featureTypes,
  thresholdLevels,
  roadLengthRules,
  surfaceTypeRules,
  roadClassRules,
  serviceAreas,
}) => {
  const [loading, setLoading] = useState(true);
  const {setFieldValue} = useFormikContext();
  const [validLocationCodeValues, setValidLocationCodeValues] = useState(locationCodes);
  const [disableLocationCodeEdit, setDisableLocationCodeEdit] = useState(false);
  const [validFeatureTypeValues, setValidFeatureTypeValues] = useState(featureTypes);
  const locationCodeCId = locationCodes.find((code) => code.name === 'C').id;
  const roadLengthRuleDefaultId = roadLengthRules.find((rlr) => rlr.name === 'Not Applicable').id;
  const surfaceTypeRuleDefaultId = surfaceTypeRules.find((str) => str.name === 'Not Applicable').id;
  const roadClassRuleDefaultId =roadClassRules.find((rcr) => rcr.name === 'Not Applicable').id;
  useEffect(() => {
    // Add validation for point line feature when location code is C.
    // Need to get the id value of location code C
    const defaultValidationSchema = validationSchema.shape({
      featureType: Yup.string()
        .nullable()
        .when('locationCodeId', {
          is: locationCodeCId,
          then: Yup.string().required('Required').max(12),
        }),
      spThresholdLevel: Yup.string()
        .nullable()
        .when('locationCodeId', {
          is: locationCodeCId,
          then: Yup.string().required('Required'),
        }),
      isSiteNumRequired: Yup.boolean().when('locationCodeId', {
        is: locationCodeCId,
        then: Yup.boolean().required('Required'),
      }),
    });

    
    setValidationSchema(defaultValidationSchema);
    setLoading(true);

    if (formType === Constants.FORM_TYPE.ADD) {
      defaultValues.roadLengthRule = roadLengthRuleDefaultId;
      defaultValues.surfaceTypeRule = surfaceTypeRuleDefaultId;
      defaultValues.roadClassRule = roadClassRuleDefaultId;
      setInitialValues(defaultValues);
      setLoading(false);
    } else {
      api.getActivityCode(activityId).then((response) => {
        setInitialValues({
          ...response.data,
          endDate: response.data.endDate ? moment(response.data.endDate): null,
          minValue: toStringOrEmpty(response.data.minValue),
          maxValue: toStringOrEmpty(response.data.maxValue),
          reportingFrequency: toStringOrEmpty(response.data.reportingFrequency),
        });
        
        setValidLocationCodeValues(() => {
          if (formType === Constants.FORM_TYPE.EDIT) {
            if (response.data.locationCodeId === locationCodes.find((code) => code.name === 'B').id)
              return locationCodes.filter((location) => location.name !== 'C');
          }

          return locationCodes;
        });
        
        setDisableLocationCodeEdit(() => {
          if (formType === Constants.FORM_TYPE.EDIT) {
            if (response.data.locationCodeId === locationCodes.find((code) => code.name === 'A').id) return true;
          }
          return false;
        });

        setValidFeatureTypeValues(() => {
          const pointLineType = 'Point/Line';

          if (formType === Constants.FORM_TYPE.EDIT) {
            if (response.data.featureType === pointLineType)
              return featureTypes.filter((feature) => feature.id === pointLineType);

            if (response.data.featureType)
              return featureTypes.filter(
                (feature) => feature.id === pointLineType || feature.id === response.data.featureType
              );
          }

          return featureTypes;
        });

        setLoading(false);
      });
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (loading || formValues === null) return <PageSpinner />;
  
  const convertMaxValue = (minv,maxv)=>{
    const minval = minv;
    const maxval = maxv;
    if (isValueNotEmpty(minval) && Number(minval) >=0 && isValueNotEmpty(maxval) && Number(maxval) === 0)
    {
      return (['site','num','ea'].includes(formValues.unitOfMeasure)) ? '999999999': '999999999.99';
    }
    if(isValueEmpty(maxval)) return '';
    return toString(maxval);
  };

  const handleMinValue = (e) => {
    const minv = e.target.value;
    if(formValues)
    {
      let maxv= formValues.maxValue;
      maxv = convertMaxValue(minv,maxv);
      setFieldValue('maxValue',maxv);
    }
    setFieldValue('minValue', minv); 
  };

  const handleMaxValue = (e) => {
    const maxval = e.target.value;
    if(formValues)
    {
      let minval = formValues.minValue;
      setFieldValue('maxValue',convertMaxValue(minval,maxval));
    }
    else{
      setFieldValue('maxValue', maxval);
    }    
  };

  return (
    <React.Fragment>
      <Row>
        <Col>
          <FormRow name="activityNumber" label="Activity Code*">
            <FormInput
              type="text"
              name="activityNumber"
              placeholder="Activity Code"
              disabled={formType === Constants.FORM_TYPE.EDIT}
            />
          </FormRow>
        </Col>
        <Col>
          <FormRow name="activityName" label="Activity Name*">
            <FormInput type="text" name="activityName" placeholder="Activity Name" />
          </FormRow>
        </Col>
      </Row>
      <Row>
        <Col>
          <FormRow name="unitOfMeasure" label="Unit*">
            <SingleDropdownField
              defaultTitle="Select Unit"
              items={unitOfMeasures}
              name="unitOfMeasure"
              disabled={formType === Constants.FORM_TYPE.EDIT}
            />
          </FormRow>
        </Col>
        <Col>
          <FormRow name="maintenanceType" label="Maintenance Type*">
            <SingleDropdownField
              defaultTitle="Select Maintenance Type"
              items={maintenanceTypes}
              name="maintenanceType"
              disabled={formType === Constants.FORM_TYPE.EDIT}
            />
          </FormRow>
        </Col>
      </Row>
      <Row>
        <Col className='col colmargin1'>
          <FormRow name="serviceAreaNumbers" label="Service Areas*">
            <MultiSelect items={serviceAreas} name="serviceAreaNumbers" showSelectAll={true} selectClass="form-control servicearea-large"/>
          </FormRow>
          <FormRow name="locationCodeId" label="Location Code*">
            <SingleDropdownField
              defaultTitle="Select Location Code"
              items={validLocationCodeValues}
              name="locationCodeId"
              disabled={disableLocationCodeEdit}
            />
          </FormRow>
        </Col>
        <Col>
          <FieldSet legendname = "Analytical Validation" tips={tipAnalyticalValidation} targetId='AnalyticalValidationTooltipForFieldsetId'>
            <FormRow name="minValue" label="Minimum Value">
              <FormInput type="text" name="minValue" placeholder="Minimum Value"   onChange= {handleMinValue}/>
            </FormRow>
            <FormRow name="maxValue" label="Maximum Value">
              <FormInput type="text" name="maxValue" placeholder="Maximum Value"   onChange= {handleMaxValue}/>
            </FormRow>
            <FormRow name="reportingFrequency" label="Reporting Frequency">
              <FormInput type="text" name="reportingFrequency" placeholder="Minimum # Days" />
            </FormRow>
          </FieldSet>
        </Col>
      </Row>
      {formValues.locationCodeId === locationCodeCId && (
          <React.Fragment>
            <Row>
              <Col  className='col colmargin1'>           
                <FormRow name="featureType" label="Feature Type*">
                  <SingleDropdownField defaultTitle="Select Feature Type" items={validFeatureTypeValues} name="featureType" />
                </FormRow>
                <FormRow name="spThresholdLevel" label="Location Tolerance Level*">
                  <SingleDropdownField
                    defaultTitle="Select Location Tolerance Level"
                    items={thresholdLevels}
                    name="spThresholdLevel"
                  />
                </FormRow>
                <FormRow name="isSiteNumRequired" label="Site Number Required">
                  <FormCheckboxInput name="isSiteNumRequired" />
                </FormRow>
              </Col>
              <Col>
                <FieldSet legendname = "Highway Attribute Validation" tips={tipHighwayAttributeValidation} targetId='HighwayAttributeValidationTooltipForFieldsetId'>
                  <FormRow name="roadLengthRule" label="Road Length Validation Rule">
                    <SingleDropdownField
                      defaultTitle="Not Applicable"
                      items={roadLengthRules}
                      name="roadLengthRule"
                    />
                  </FormRow>
                  <FormRow name="surfaceTypeRule" label="Surface Type Rule">
                    <SingleDropdownField
                      defaultTitle="Not Applicable"
                      items={surfaceTypeRules}
                      name="surfaceTypeRule"
                    />
                  </FormRow>
                  <FormRow name="roadClassRule" label="Road Class Rule">
                    <SingleDropdownField
                      defaultTitle="Not Applicable"
                      items={roadClassRules}
                      name="roadClassRule"
                    />
                  </FormRow>
                </FieldSet>
              </Col>
            </Row>
            
          </React.Fragment>
         )}
       
       <Row>
          <Col>
          </Col>
          <Col>
            <FormRow name="endDate" label="End Date">
              <SingleDateField name="endDate" placeholder="End Date" />
            </FormRow>
          </Col>
        </Row>
    </React.Fragment>
  );
};

const mapStateToProps = (state) => {
  return {
    maintenanceTypes: state.codeLookups.maintenanceTypes,
    unitOfMeasures: state.codeLookups.unitOfMeasures,
    locationCodes: state.codeLookups.locationCodes,
    featureTypes: state.codeLookups.featureTypes,
    thresholdLevels: state.codeLookups.thresholdLevels,
    roadLengthRules: state.codeLookups.roadLengthRules,
    surfaceTypeRules: state.codeLookups.surfaceTypeRules,
    roadClassRules: state.codeLookups.roadClassRules,
    serviceAreas: Object.values(state.serviceAreas),
    minValue: state.codeLookups.minValue,
    maxValue: state.codeLookups.maxValue,
    reportingFrequency: state.codeLookups.reportingFrequency,
  };
};

export default connect(mapStateToProps)(EditActivityFormFields);
