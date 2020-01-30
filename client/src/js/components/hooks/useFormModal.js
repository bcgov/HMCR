import React, { useState } from 'react';

import FormModal from '../forms/FormModal';

import * as Constants from '../../Constants';

const useFormModal = (formTitle, formFieldsChildElement, handleFormSubmit) => {
  // This is needed until Formik fixes its own setSubmitting function
  const [submitting, setSubmitting] = useState(false);
  const [initialValues, setInitialValues] = useState({});
  const [isOpen, setIsOpen] = useState(false);
  const [formType, setFormType] = useState(Constants.FORM_TYPE.ADD);
  const [formOptions, setFormOptions] = useState({});
  const [validationSchema, setValidationSchema] = useState({});

  const toggle = () => setIsOpen(false);

  const openForm = (formType, options) => {
    setFormType(formType);
    setFormOptions({ ...options });
    setIsOpen(true);
  };

  const closeForm = () => {
    setFormOptions({});
    toggle();
  };

  const onFormSubmit = values => handleFormSubmit(values, formType);

  const title = formType === Constants.FORM_TYPE.ADD ? `Add ${formTitle}` : `Edit ${formTitle}`;

  return {
    isOpen,
    openForm,
    closeForm,
    submitting,
    setSubmitting,
    formElement: (
      <FormModal
        isOpen={isOpen}
        toggle={toggle}
        title={title}
        initialValues={initialValues}
        validationSchema={validationSchema}
        onSubmit={onFormSubmit}
        submitting={submitting}
      >
        {isOpen &&
          React.cloneElement(formFieldsChildElement, {
            ...formOptions,
            formType,
            setInitialValues,
            setValidationSchema,
          })}
      </FormModal>
    ),
  };
};

export default useFormModal;
