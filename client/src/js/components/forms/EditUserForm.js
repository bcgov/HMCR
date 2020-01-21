import React, { useEffect, useState } from 'react';
import { connect } from 'react-redux';
import * as Yup from 'yup';
import moment from 'moment';

import MultiSelect from '../ui/MultiSelect';
import SingleDropdownField from '../ui/SingleDropdownField';
import SingleDateField from '../ui/SingleDateField';
import PageSpinner from '../ui/PageSpinner';
import { FormRow, FormInput } from './FormInputs';
import FormModal from './FormModal';

import { editUser, showValidationErrorDialog } from '../../actions';

import * as api from '../../Api';

const defaultValues = {
  userType: '',
  username: '',
  firstName: '',
  lastName: '',
  email: '',
  userRoleIds: [],
  serviceAreaNumbers: [],
  active: true,
  endDate: null,
};

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

const EditUserFormFields = ({ userTypes, roles, serviceAreas }) => {
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

const EditUserForm = ({ toggle, isOpen, userTypes, serviceAreas, editUser, userId, showValidationErrorDialog }) => {
  // This is needed until Formik fixes its own setSubmitting function
  const [submitting, setSubmitting] = useState(false);
  const [loading, setLoading] = useState(true);
  const [initialValues, setInitialValues] = useState(defaultValues);
  const [roles, setRoles] = useState([]);

  useEffect(() => {
    api
      .getRoles()
      .then(response => {
        setRoles(response.data.sourceList);

        return api.getUser(userId).then(response => {
          setInitialValues({
            ...response.data,
            endDate: response.data.endDate ? moment(response.data.endDate) : null,
          });
        });
      })
      .then(() => setLoading(false));
  }, [userId]);

  const handleFormSubmit = values => {
    if (!submitting) {
      setSubmitting(true);

      editUser(userId, values)
        .then(() => {
          toggle(true);
        })
        .catch(error => {
          showValidationErrorDialog(error.response.data.errors);
          setSubmitting(false);
        });
    }
  };

  return (
    <FormModal
      isOpen={isOpen}
      toggle={toggle}
      title="Edit User"
      initialValues={initialValues}
      validationSchema={validationSchema}
      onSubmit={handleFormSubmit}
      submitting={submitting}
    >
      {loading ? (
        <PageSpinner />
      ) : (
        <EditUserFormFields
          userTypes={userTypes}
          roles={roles}
          serviceAreas={serviceAreas}
          initialValues={initialValues}
        />
      )}
    </FormModal>
  );
};

const mapStateToProps = state => {
  return {
    serviceAreas: Object.values(state.serviceAreas),
    userTypes: Object.values(state.user.types),
  };
};

export default connect(mapStateToProps, { editUser, showValidationErrorDialog })(EditUserForm);
