import React, { useEffect, useState } from 'react';
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
import { isInteger} from 'lodash';
import {isValueEmpty,isValueNotEmpty,toStringOrEmpty,toStringWithCommasOrEmpty,isValidDecimal} from '../../utils'

const tipAnalyticalValidation = [<ul key ='tipAnalyticalValidation_ul_key_1' style={{ paddingInlineStart: '20px' }}>
  <li style={{margin: '0 0 6px 0'}}>Analytical Validations provide warnings when the activity accomplishment does not meet the defined parameters.</li>
  <li>Minimum Value and Maximum Value check the accomplishment for an activity is within numerical limits, as defined. 
    No tolerances are added to the Minimum Value or Maximum Value calculations.</li>
  <li>Reporting Frequency checks if the activity was reported in the same location, with locational specificity based on the activity location code, 
    within the defined period. A tolerance of 100 metres is added to the start and end points for Location Code C activities to validate against previously 
    reported instances. No time-based tolerance is added to the Reporting Frequency calculation. Users can manually incorporate into the defined Reporting 
    Frequency a time-based tolerance (e.g. by setting the minimum number of days to ‘20’ for an activity that should be completed monthly).</li>
  </ul>];
  
const tipHighwayAttributeValidation = [<ul key ='tipHighwayAttributeValidation_ul_key_1' style={{ paddingInlineStart: '20px' }}>
  <li style={{margin: '0 0 6px 0'}}>Highway Attribute Validations provide warnings for Location Code C activities when the features of the reported 
  location and/or accomplishment do not meet the defined parameters.</li>
  <li style={{margin: '0 0 6px 0'}}>Road Length checks the accomplishment against the road length (either Road KM or Lane KM), 
    or against Guardrail Length [<b>NTD: to confirm Guardrail vs Barrier vernacular based on what types of 
    guardrail/barrier will be included</b>], as defined in each individual rule. Several rules account for the road length 
    to be multiplied by one or more factors to accommodate non kilometre-based units of measure. Multiplying factors 
    include conversion factors (e.g. 1km=1,000m), application rates (e.g. 2.0 litres/m2) or lane width factors to calculate surface area
     (e.g. lane width = 3.5m). A 10% tolerance is added to the Total Road KM (to a 200m maximum) and Total Lane KM (to a 500m maximum) 
     and Barrier Length (to a 200m maximum) for the validation calculations. Point items are calculated with 
     an estimated Road KM length of 40m (30m as permitted by the Reporting Manual and 10m tolerance), with Lane KM calculated based 
     on the number of lanes at the point.</li>
  <li  style={{margin: '0 0 6px 0'}}>Surface Type checks that the location of the record has the appropriate surface type based on the selected rule. 
    For point items, the surface type must match the point exactly and no tolerance is incorporated. For line items, 
    a tolerance of 80% is incorporated (i.e. if the rule is “GPS on Paved Surface” and 80% or more of the surface types 
    within the start and end GPS points are paved, then the record is accepted). Paved surfaces include CHRIS surface types 1-4; 
    non-paved surfaces include CHRIS surface types 5-6; unconstructed roads have a CHRIS surface type of E or F.</li>
  <li >Road Class checks that the location of the record has the appropriate summer or winter road 
    classifications based on the selected rule. For point items, the classification must match exactly and no tolerance 
    is incorporated. For line items, a tolerance of 80% is incorporated (i.e. if the rule is “Not Class 8 or F” and 80% or 
    more of the maintenance class within the start and end GPS points are not 8 or F, then the record is accepted).</li>
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
    .transform((_value, originalValue) => {
      if(isValueEmpty(originalValue.replace(/,/g, ''))) return null;
      return Number(originalValue.replace(/,/g, ''));
    })
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
            message: 'Must be less than or equal to 999,999,999.99',
            path: 'minValue',
            });
        }
        if(!isValidDecimal(this.parent.minValue,2))
        {
          return this.createError({
            message: 'Must be less than or equal to two decimal positions',
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
    .transform((_value, originalValue) => {
      if(isValueEmpty(originalValue.replace(/,/g, ''))) return null;
      return Number(originalValue.replace(/,/g, ''));
    })
    .nullable()
    .typeError('Must be number')
    .test(
      'datamax',
      function() {
        if (isValueEmpty(this.parent.maxValue))
        {
          return true;
        }
        if(Number(this.parent.maxValue) <=0)
        {
          return this.createError({
            message: 'Must be greater than 0',
            path: 'maxValue',
            });
        }
        if(Number(this.parent.maxValue) > 999999999.99)
        {
          return this.createError({
            message: 'Must be less than or equal to 999,999,999.99',
            path: 'maxValue',
            });
        }
        if(!isValidDecimal(this.parent.maxValue,2))
        {
          return this.createError({
            message: 'Must be less than or equal to two decimal positions',
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
          minValue: toStringWithCommasOrEmpty(response.data.minValue),
          maxValue: toStringWithCommasOrEmpty(response.data.maxValue),
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
              <FormInput type="text" name="minValue" placeholder="Minimum Value"/>
            </FormRow>
            <FormRow name="maxValue" label="Maximum Value">
              <FormInput type="text" name="maxValue" placeholder="Maximum Value"/>
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
