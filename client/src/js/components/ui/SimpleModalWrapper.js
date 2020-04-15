import React from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

const SimpleModalWrapper = ({ isOpen, toggle, title, children, disableClose, onComplete, ...props }) => {
  return (
    <Modal isOpen={isOpen} {...props}>
      {disableClose ? <ModalHeader>{title}</ModalHeader> : <ModalHeader toggle={toggle}>{title}</ModalHeader>}
      <ModalBody>{children}</ModalBody>
      <ModalFooter>
        <Button
          color="primary"
          size="sm"
          onClick={() => {
            toggle();
            if (onComplete) onComplete();
          }}
          disabled={disableClose}
        >
          Close
        </Button>
      </ModalFooter>
    </Modal>
  );
};

export default SimpleModalWrapper;
