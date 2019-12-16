import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { Alert, Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

import { hideErrorDialog } from '../../actions';

const ErrorDialogModal = ({
  isOpen,
  title,
  message,
  statusCode,
  errors,
  path,
  method,
  hideErrorDialog,
  hidePrimaryButton,
}) => {
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
        <ModalHeader toggle={hideErrorDialog}>{title ? title : 'Server Error'}</ModalHeader>
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
          {errors && Object.keys(errors).length > 0 && (
            <Alert color="danger">
              <ul>
                {Object.keys(errors).map(key =>
                  errors[key].map((error, index) => (
                    <li key={`${key}_${index}`} style={{ marginLeft: '-32px' }}>
                      {error}
                    </li>
                  ))
                )}
              </ul>
            </Alert>
          )}
        </ModalBody>
        <ModalFooter>
          {!hidePrimaryButton && (
            <Button
              size="sm"
              color="primary"
              disabled={clicked}
              onClick={() => handleOnClick(true)}
              style={{ minWidth: '50px' }}
            >
              Reload
            </Button>
          )}
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
  statusCode: PropTypes.number,
  path: PropTypes.string,
  method: PropTypes.string,
  hideErrorDialog: PropTypes.func.isRequired,
  title: PropTypes.string,
  message: PropTypes.string,
  errors: PropTypes.object,
  hidePrimaryButton: PropTypes.bool,
};

export default connect(null, { hideErrorDialog })(ErrorDialogModal);
