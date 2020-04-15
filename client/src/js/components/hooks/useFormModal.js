import React, { useState } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { Formik, Form } from 'formik';

import SubmitButton from '../ui/SubmitButton';

import * as Constants from '../../Constants';

const useFormModal = (formTitle, formFieldsChildElement, handleFormSubmit) => {
  // This is needed until Formik fixes its own setSubmitting function
  const [submitting, setSubmitting] = useState(false);
  const [initialValues, setInitialValues] = useState(null);
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

  const onFormSubmit = (values) => handleFormSubmit(values, formType);

  const title = formType === Constants.FORM_TYPE.ADD ? `Add ${formTitle}` : `Edit ${formTitle}`;

  const formModal = () => {
    return (
      <Modal isOpen={isOpen} toggle={toggle} backdrop="static">
        <ModalHeader toggle={toggle}>{title}</ModalHeader>
        <Formik
          enableReinitialize={true}
          initialValues={initialValues}
          validationSchema={validationSchema}
          onSubmit={onFormSubmit}
        >
          {({ dirty, values }) => (
            <Form>
              <ModalBody>
                {isOpen &&
                  React.cloneElement(formFieldsChildElement, {
                    ...formOptions,
                    formType,
                    formValues: values,
                    setInitialValues,
                    setValidationSchema,
                  })}
              </ModalBody>
              <ModalFooter>
                <SubmitButton size="sm" submitting={submitting} disabled={submitting || !dirty}>
                  Submit
                </SubmitButton>
                <Button color="secondary" size="sm" onClick={toggle}>
                  Cancel
                </Button>
              </ModalFooter>
            </Form>
          )}
        </Formik>
      </Modal>
    );
  };

  return {
    isOpen,
    openForm,
    closeForm,
    submitting,
    setSubmitting,
    formElement: formModal(),
  };
};

export default useFormModal;
