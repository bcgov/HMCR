import React, { useEffect, useState } from 'react';
import { connect } from 'react-redux';
import moment from 'moment';

import MultiSelect from '../ui/MultiSelect';
import SingleDateField from '../ui/SingleDateField';
import SingleDropdownField from '../ui/SingleDropdownField';
import PageSpinner from '../ui/PageSpinner';
import { FormRow, FormInput } from './FormInputs';

import * as api from '../../Api';
import * as Constants from '../../Constants';

const EditUserFormFields = ({
  setInitialValues,
  formValues,
  setValidationSchema,
  userId,
  serviceAreas,
  userTypes,
  validationSchema,
}) => {
  const [loading, setLoading] = useState(true);
  const [roles, setRoles] = useState([]);

  useEffect(() => {
    setValidationSchema(validationSchema);

    api
      .getUser(userId)
      .then(response => {
        setInitialValues({
          ...response.data,
          endDate: response.data.endDate ? moment(response.data.endDate) : null,
        });

        const userType = response.data.userType;

        return api.getRoles().then(response => {
          const data = response.data.sourceList
            .filter(r => r.isActive === true)
            .map(r => ({ ...r, description: r.name }));

          if (userType === Constants.USER_TYPE.BUSINESS) setRoles(data.filter(r => r.isInternal === false));
          else setRoles(data);
        });
      })
      .then(() => setLoading(false));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (loading || formValues === null) return <PageSpinner />;

  return (
    <React.Fragment>
      <FormRow name="userType" label="User Type*">
        <SingleDropdownField defaultTitle="Select User Type" items={userTypes} name="userType" disabled />
      </FormRow>
      <FormRow name="username" label="User Id*">
        <FormInput type="text" name="username" placeholder="User Id" disabled />
      </FormRow>
      <FormRow name="userRoleIds" label="User Roles*">
        <MultiSelect items={roles} name="userRoleIds" />
      </FormRow>
      <FormRow name="serviceAreaNumbers" label="Service Areas*">
        <MultiSelect items={serviceAreas} name="serviceAreaNumbers" showSelectAll={true} />
      </FormRow>
      <FormRow name="endDate" label="End Date">
        <SingleDateField name="endDate" placeholder="End Date" />
      </FormRow>
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    serviceAreas: Object.values(state.serviceAreas),
    userTypes: Object.values(state.user.types),
  };
};

export default connect(mapStateToProps, null)(EditUserFormFields);
