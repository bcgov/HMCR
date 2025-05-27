import { parseISO } from 'date-fns';
import React, { useEffect, useState } from 'react';
import * as Yup from 'yup';

import { FormRow, FormInput, FormCheckboxInput } from './FormInputs';
import * as api from '../../Api';
import * as Constants from '../../Constants';
import MultiSelect from '../ui/MultiSelect';
import PageSpinner from '../ui/PageSpinner';
import SingleDateField from '../ui/SingleDateField';

const defaultValues = {
  name: '',
  description: '',
  permissions: [],
  isInternal: false,
  endDate: null,
};

const validationSchema = Yup.object({
  name: Yup.string().required('Required').max(30).trim(),
  description: Yup.string().required('Required').max(150).trim(),
  permissions: Yup.array().required('Require at least one permission'),
});

const EditRoleFormFields = ({ setInitialValues, formValues, setValidationSchema, formType, roleId }) => {
  const [permissionIds, setPermissionIds] = useState([]);
  const [loading, setLoading] = useState(true);
  const [isReferenced, setIsReferenced] = useState(false);

  useEffect(() => {
    setValidationSchema(validationSchema);

    api.getPermissions().then((response) => {
      setInitialValues(defaultValues);
      setPermissionIds(response.data);

      if (formType === Constants.FORM_TYPE.ADD) {
        setLoading(false);
      } else {
        return api.getRole(roleId).then((response) => {
          setInitialValues({
            ...response.data,
            endDate: response.data.endDate ? parseISO(response.data.endDate) : null,
          });
          setIsReferenced(response.data.isReferenced);
          setLoading(false);
        });
      }
    });
  }, []);

  if (loading || formValues === null) return <PageSpinner />;

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
      <FormRow name="isInternal" label="Internal">
        <FormCheckboxInput name="isInternal" disabled={isReferenced} />
      </FormRow>
      <FormRow name="endDate" label="End Date">
        <SingleDateField name="endDate" placeholder="End Date" />
      </FormRow>
    </React.Fragment>
  );
};

export default EditRoleFormFields;
