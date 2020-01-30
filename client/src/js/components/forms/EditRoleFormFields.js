import React, { useEffect, useState } from 'react';
import * as Yup from 'yup';
import moment from 'moment';

import MultiSelect from '../ui/MultiSelect';
import SingleDateField from '../ui/SingleDateField';
import PageSpinner from '../ui/PageSpinner';
import { FormRow, FormInput, FormSwitchInput } from './FormInputs';

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

const EditRoleFormFields = ({ disableEdit, setInitialValues, setValidationSchema, formType, roleId }) => {
  const [permissionIds, setPermissionIds] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setValidationSchema(validationSchema);

    api.getPermissions().then(response => {
      setInitialValues(defaultValues);
      setPermissionIds(response.data);

      if (formType === Constants.FORM_TYPE.ADD) {
        setLoading(false);
      } else {
        return api.getRole(roleId).then(response => {
          setInitialValues({
            ...response.data,
            endDate: response.data.endDate ? moment(response.data.endDate) : null,
          });
          setLoading(false);
        });
      }
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (loading) return <PageSpinner />;

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

export default EditRoleFormFields;
