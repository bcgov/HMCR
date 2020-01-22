import React, { useEffect, useState } from 'react';
import { connect } from 'react-redux';
import * as Yup from 'yup';
import moment from 'moment';

import MultiSelect from '../ui/MultiSelect';
import SingleDateField from '../ui/SingleDateField';
import PageSpinner from '../ui/PageSpinner';
import { FormRow, FormInput, FormSwitchInput } from './FormInputs';
import FormModal from './FormModal';

import { showValidationErrorDialog } from '../../actions';

import * as api from '../../Api';
import * as Constants from '../../Constants';

const defaultValues = {
  name: '',
  description: '',
  permissions: [],
  internal: false,
  endDate: null,
};

const validationSchema = Yup.object({
  name: Yup.string()
    .required('Required')
    .max(30)
    .trim(),
  description: Yup.string()
    .required('Required')
    .max(150)
    .trim(),
  permissions: Yup.array().required('Require at least one permission'),
});

const EditRoleFormFields = ({ permissionIds, disableEdit }) => {
  return (
    <React.Fragment>
      <FormRow name="name" label="Role Name*">
        <FormInput type="text" name="name" placeholder="Role Name" />
      </FormRow>
      <FormRow name="description" label="Role Description*">
        <FormInput type="text" name="description" placeholder="Role Description" />
      </FormRow>
      <FormRow name="permissions" label="Permissions*">
        <MultiSelect items={permissionIds} name="permissions" />
      </FormRow>
      <FormRow name="internal" label="Internal">
        <FormSwitchInput name="internal" disabled={disableEdit} />
      </FormRow>
      <FormRow name="endDate" label="End Date">
        <SingleDateField name="endDate" placeholder="End Date" />
      </FormRow>
    </React.Fragment>
  );
};

const EditRoleForm = ({ toggle, isOpen, formType, roleId, showValidationErrorDialog }) => {
  // This is needed until Formik fixes its own setSubmitting function
  const [submitting, setSubmitting] = useState(false);
  const [loading, setLoading] = useState(true);
  const [initialValues, setInitialValues] = useState(defaultValues);
  const [permissionIds, setPermissionIds] = useState([]);

  useEffect(() => {
    api.getPermissions().then(response => {
      setPermissionIds(response.data);
      setLoading(false);

      if (formType === Constants.FORM_TYPE.ADD) {
        setLoading(false);
      } else {
        return api.getRole(roleId).then(response => {
          setInitialValues({
            ...response.data,
            endDate: response.data.endDate ? moment(response.data.endDate) : null,
          });
        });
      }
    });
  }, [formType, roleId]);

  const handleFormSubmit = values => {
    if (!submitting) {
      setSubmitting(true);

      if (formType === Constants.FORM_TYPE.ADD) {
        api
          .postRole(values)
          .then(() => {
            toggle(true);
          })
          .catch(error => {
            showValidationErrorDialog(error.response.data.errors);
            setSubmitting(false);
          });
      } else {
        api
          .putRole(roleId, values)
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

  const title = formType === Constants.FORM_TYPE.ADD ? 'Add Role' : 'Edit Role';

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
        <EditRoleFormFields permissionIds={permissionIds} disableEdit={initialValues.inUse} />
      )}
    </FormModal>
  );
};

export default connect(null, { showValidationErrorDialog })(EditRoleForm);
