import React, { useEffect, useState } from 'react';
import { connect } from 'react-redux';
import * as Yup from 'yup';
import moment from 'moment';

import MultiSelect from '../ui/MultiSelect';
import SingleDateField from '../ui/SingleDateField';
import SingleDropdownField from '../ui/SingleDropdownField';
import PageSpinner from '../ui/PageSpinner';
import { FormRow, FormInput } from './FormInputs';

import * as api from '../../Api';
import * as Constants from '../../Constants';

const validationSchema = Yup.object({
  userType: Yup.string()
    .required('Required')
    .max(30)
    .trim(),
  username: Yup.string()
    .required('Required')
    .max(32)
    .trim(),
  userRoleIds: Yup.array().required('Require at least one role'),
});

const EditUserFormFields = ({ setInitialValues, setValidationSchema, userId, serviceAreas, userTypes }) => {
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

          if (userType === Constants.USER_TYPE.BUSINESS) setRoles(data.filter(r => r.internal === false));
          else setRoles(data);
        });
      })
      .then(() => setLoading(false));

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (loading) return <PageSpinner />;

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
