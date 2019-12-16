import React from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { Formik, Form } from 'formik';

import SubmitButton from '../ui/SubmitButton';

const FormModal = ({ toggle, isOpen, children, title, submitting, initialValues, validationSchema, onSubmit }) => {
  return (
    <Modal isOpen={isOpen} toggle={toggle} backdrop="static">
      <ModalHeader toggle={toggle}>{title}</ModalHeader>
      <Formik
        enableReinitialize={true}
        initialValues={initialValues}
        validationSchema={validationSchema}
        onSubmit={onSubmit}
      >
        {({ dirty }) => (
          <Form>
            <ModalBody>{children}</ModalBody>
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

export default FormModal;
