import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

import { hideErrorDialog } from '../../actions';

const ErrorDialogModal = ({ isOpen, message, statusCode, path, method, hideErrorDialog }) => {
  const [clicked, setClicked] = useState(false);

  useEffect(() => {
    setClicked(false);
  }, []);

  const handleOnClick = reload => {
    setClicked(true);

    if (reload) window.location.reload();
    else hideErrorDialog();
  };

  return (
    <div>
      <Modal isOpen={isOpen}>
        <ModalHeader toggle={hideErrorDialog}>Server Error</ModalHeader>
        <ModalBody>
          {message && (
            <p>
              <strong>Error:</strong> {message}
            </p>
          )}
          {statusCode && path && method && (
            <p>
              <small>
                A <strong>{method}</strong> request to <strong className="text-primary">{path}</strong> has returned a{' '}
                <strong className="text-danger">{statusCode}</strong> status code.
              </small>
            </p>
          )}
        </ModalBody>
        <ModalFooter>
          <Button
            size="sm"
            color="primary"
            disabled={clicked}
            onClick={() => handleOnClick(true)}
            style={{ minWidth: '50px' }}
          >
            Reload
          </Button>
          <Button size="sm" color="secondary" onClick={() => handleOnClick(false)} disabled={clicked}>
            Close
          </Button>
        </ModalFooter>
      </Modal>
    </div>
  );
};

ErrorDialogModal.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  statusCode: PropTypes.number.isRequired,
  path: PropTypes.string.isRequired,
  method: PropTypes.string.isRequired,
  hideErrorDialog: PropTypes.func.isRequired,
  message: PropTypes.string,
};

export default connect(
  null,
  { hideErrorDialog }
)(ErrorDialogModal);
