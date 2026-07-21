import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { Alert, Button, Collapse, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

import { hideErrorDialog } from '../../actions';

const HTTP_ERROR_CONTENT = {
  400: {
    label: 'Bad request',
    message: 'The request could not be completed because some information was not accepted by the server.',
    action: 'Check the information on the page and try again.',
  },
  401: {
    label: 'Session timed out',
    message: 'Your sign-in session appears to have expired.',
    action: 'Reload the page, sign in again if asked, then try again.',
  },
  403: {
    label: 'Access denied',
    message: 'Your account does not have permission to complete this action.',
    action: 'Contact your administrator if you need access.',
  },
  404: {
    label: 'File or page not found',
    message: 'The file, page, or record could not be found.',
    action: 'Refresh the page and try again. If the problem continues, contact support.',
  },
  408: {
    label: 'Request timed out',
    message: 'The server took too long to respond.',
    action: 'Check your connection and try again.',
  },
  409: {
    label: 'Conflict',
    message: 'The request conflicts with the current information in the system.',
    action: 'Refresh the page and try again.',
  },
  413: {
    label: 'File too large',
    message: 'The file or request is larger than the server can accept.',
    action: 'Reduce the file size and try again.',
  },
  415: {
    label: 'Unsupported file type',
    message: 'The file or request format is not supported.',
    action: 'Use a supported file type and try again.',
  },
  429: {
    label: 'Too many requests',
    message: 'The system received too many requests in a short time.',
    action: 'Wait a few minutes and try again.',
  },
  500: {
    label: 'Server error',
    message: 'Something went wrong while the server was processing the request.',
    action: 'Try again. If the problem continues, contact support with the support ID below.',
  },
  502: {
    label: 'Service unavailable',
    message: 'The system could not reach one of the services it needs.',
    action: 'Try again in a few minutes.',
  },
  503: {
    label: 'Service unavailable',
    message: 'The service is temporarily unavailable.',
    action: 'Try again in a few minutes.',
  },
  504: {
    label: 'Service timed out',
    message: 'A service took too long to respond.',
    action: 'Try again in a few minutes.',
  },
};

const DEFAULT_HTTP_ERROR_CONTENT = {
  label: 'Unexpected error',
  message: 'The request could not be completed.',
  action: 'Try again. If the problem continues, contact support with the support ID below.',
};

const NETWORK_ERROR_CONTENT = {
  label: 'Connection problem',
  message: 'The system could not connect to the server.',
  action: 'Check your connection and try again.',
};

const getHttpErrorContent = (statusCode) => {
  if (!statusCode) return NETWORK_ERROR_CONTENT;

  return HTTP_ERROR_CONTENT[statusCode] || DEFAULT_HTTP_ERROR_CONTENT;
};

const getStatusText = (statusCode, label) => (statusCode ? `${statusCode} - ${label}` : label);

const hasErrorDetails = (errors) => errors && Object.keys(errors).length > 0;

const renderErrors = (errors) => (
  <ul>
    {Object.entries(errors).map(([field, error], index) => (
      <li key={`${field}-${index}`} style={{ marginLeft: '-32px' }}>
        {Array.isArray(error) ? error.join(' ') : String(error)}
      </li>
    ))}
  </ul>
);

const ErrorDialogModal = ({
  isOpen,
  title,
  message,
  statusCode,
  detail,
  errors,
  path,
  method,
  supportId,
  errorCode,
  correlationId,
  timestampUtc,
  hideErrorDialog,
  hidePrimaryButton,
}) => {
  const [clicked, setClicked] = useState(false);
  const [showDetails, setShowDetails] = useState(false);

  useEffect(() => {
    setClicked(false);
    setShowDetails(false);
  }, [isOpen]);

  const handleOnClick = (reload) => {
    setClicked(true);

    if (reload) window.location.reload();
    else hideErrorDialog();
  };

  const errorContent = getHttpErrorContent(statusCode);
  const hasStatusCode = Boolean(statusCode);
  const hasDetails =
    message ||
    detail ||
    hasStatusCode ||
    (path && method) ||
    errorCode ||
    correlationId ||
    timestampUtc ||
    hasErrorDetails(errors);
  const modalTitle = title || errorContent.label;

  return (
    <div>
      <Modal isOpen={isOpen}>
        <ModalHeader toggle={hideErrorDialog}>{modalTitle}</ModalHeader>
        <ModalBody>
          {hasStatusCode ? (
            <Alert color="danger">
              <p className="mb-1">
                <strong>{getStatusText(statusCode, errorContent.label)}</strong>
              </p>
              <p className="mb-1">{errorContent.message}</p>
              <p className="mb-0">
                <strong>What to do:</strong> {errorContent.action}
              </p>
            </Alert>
          ) : (
            message && (
              <p>
                <strong>Error:</strong> {message}
              </p>
            )
          )}
          {supportId && (
            <p>
              <strong>Support ID:</strong> <code style={{ wordBreak: 'break-all' }}>{supportId}</code>
            </p>
          )}
          {!hasStatusCode && hasErrorDetails(errors) && (
            <Alert color="danger">
              {renderErrors(errors)}
            </Alert>
          )}
          {hasDetails && (
            <React.Fragment>
              <Button
                size="sm"
                color="link"
                className="p-0"
                onClick={() => setShowDetails(!showDetails)}
                aria-expanded={showDetails}
              >
                {showDetails ? 'Hide technical details' : 'Show technical details'}
              </Button>
              <Collapse isOpen={showDetails}>
                <Alert color="secondary" className="mt-3 mb-0">
                  {message && (
                    <p>
                      <small>
                        <strong>Error:</strong> {message}
                      </small>
                    </p>
                  )}
                  {detail && (
                    <p>
                      <small>
                        <strong>Detail:</strong> {detail}
                      </small>
                    </p>
                  )}
                  {statusCode && path && method && (
                    <p>
                      <small>
                        A <strong>{method}</strong> request to <strong className="text-primary">{path}</strong> has
                        returned a <strong className="text-danger">{statusCode}</strong> status code.
                      </small>
                    </p>
                  )}
                  {statusCode && (!path || !method) && (
                    <p>
                      <small>
                        <strong>Status code:</strong> <code>{statusCode}</code>
                      </small>
                    </p>
                  )}
                  {errorCode && (
                    <p>
                      <small>
                        <strong>Error code:</strong> <code>{errorCode}</code>
                      </small>
                    </p>
                  )}
                  {correlationId && (
                    <p>
                      <small>
                        <strong>Correlation ID:</strong>{' '}
                        <code style={{ wordBreak: 'break-all' }}>{correlationId}</code>
                      </small>
                    </p>
                  )}
                  {timestampUtc && (
                    <p>
                      <small>
                        <strong>Timestamp UTC:</strong> <code>{timestampUtc}</code>
                      </small>
                    </p>
                  )}
                  {hasStatusCode && hasErrorDetails(errors) && <Alert color="danger">{renderErrors(errors)}</Alert>}
                </Alert>
              </Collapse>
            </React.Fragment>
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
  detail: PropTypes.string,
  errors: PropTypes.object,
  supportId: PropTypes.string,
  errorCode: PropTypes.string,
  correlationId: PropTypes.string,
  timestampUtc: PropTypes.string,
  hidePrimaryButton: PropTypes.bool,
};

export default connect(null, { hideErrorDialog })(ErrorDialogModal);
