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

import { createUser, editUser, showValidationErrorDialog } from '../../actions';

import * as api from '../../Api';
import * as Constants from '../../Constants';

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
  firstName: Yup.string()
    .required('Required')
    .max(150)
    .trim(),
  lastName: Yup.string()
    .required('Required')
    .max(150)
    .trim(),
  email: Yup.string()
    .email('Invalid email address')
    .required('Required')
    .max(100)
    .trim(),
  userRoleIds: Yup.array().required('Require at least one role'),
});

const EditUserFormFields = ({ userTypes, roles, serviceAreas, disableEdit }) => {
  return (
    <React.Fragment>
      <FormRow name="userType" label="User Type*">
        <SingleDropdownField defaultTitle="Select User Type" items={userTypes} name="userType" disabled={disableEdit} />
      </FormRow>
      <FormRow name="username" label="User Id*">
        <FormInput type="text" name="username" placeholder="User Id" disabled={disableEdit} />
      </FormRow>
      <FormRow name="firstName" label="First Name*">
        <FormInput type="text" name="firstName" placeholder="First Name" disabled={disableEdit} />
      </FormRow>
      <FormRow name="lastName" label="Last Name*">
        <FormInput type="text" name="lastName" placeholder="Last Name" disabled={disableEdit} />
      </FormRow>
      <FormRow name="email" label="Email*">
        <FormInput type="email" name="email" placeholder="Email" />
      </FormRow>
      <FormRow name="userRoleIds" label="User Roles*">
        <MultiSelect items={roles} name="userRoleIds" />
      </FormRow>
      <FormRow name="serviceAreaNumbers" label="Service Areas*">
        <MultiSelect items={serviceAreas} name="serviceAreaNumbers" showSelectAll={true} />
      </FormRow>
      {/* <FormRow name="active" label="Active">
    <FormCheckbox name="active" />
  </FormRow> */}
      <FormRow name="endDate" label="End Date">
        <SingleDateField name="endDate" placeholder="End Date" />
      </FormRow>
    </React.Fragment>
  );
};

const EditUserForm = ({
  toggle,
  isOpen,
  userTypes,
  serviceAreas,
  createUser,
  editUser,
  formType,
  userId,
  showValidationErrorDialog,
}) => {
  // This is needed until Formik fixes its own setSubmitting function
  const [submitting, setSubmitting] = useState(false);
  const [loading, setLoading] = useState(true);
  const [initialValues, setInitialValues] = useState(defaultValues);
  const [disableEdit, setDisableEdit] = useState(false);
  const [roles, setRoles] = useState([]);

  useEffect(() => {
    api
      .getRoles()
      .then(response => {
        setRoles(response.data);

        if (formType === Constants.FORM_TYPE.ADD) {
          setLoading(false);
        } else {
          return api.getUser(userId).then(response => {
            setInitialValues({
              ...response.data,
              endDate: response.data.endDate ? moment(response.data.endDate) : null,
            });
            setDisableEdit(response.data.hasLogInHistory);
          });
        }
      })
      .then(() => setLoading(false));
  }, [formType, userId]);

  const handleFormSubmit = values => {
    if (!submitting) {
      setSubmitting(true);

      if (formType === Constants.FORM_TYPE.ADD) {
        createUser(values)
          .then(() => {
            toggle(true);
          })
          .catch(error => {
            showValidationErrorDialog(error.response.data.errors);
            setSubmitting(false);
          });
      } else {
        editUser(userId, values)
          .then(() => {
            toggle(true);
          })
          .catch(error => {
            showValidationErrorDialog(error.response.data.errors);
            setSubmitting(false);
          });
      }
    }
  };

  const title = formType === Constants.FORM_TYPE.ADD ? 'Add User' : 'Edit User';

  return (
    <FormModal
      isOpen={isOpen}
      toggle={toggle}
      title={title}
      initialValues={initialValues}
      validationSchema={validationSchema}
      onSubmit={handleFormSubmit}
      submitting={submitting}
    >
      {loading ? (
        <PageSpinner />
      ) : (
        <EditUserFormFields userTypes={userTypes} roles={roles} serviceAreas={serviceAreas} disableEdit={disableEdit} />
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

export default connect(mapStateToProps, { createUser, editUser, showValidationErrorDialog })(EditUserForm);
