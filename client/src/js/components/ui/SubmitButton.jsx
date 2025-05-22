import React from 'react';
import { Button, Spinner } from 'reactstrap';

const SubmitButton = ({ submitting, disabled, children, type, ...props }) => {
  return (
    <Button type={type || 'submit'} color="primary" disabled={disabled} {...props}>
      {submitting && <Spinner size="sm" />} {children ? children : 'Submit'}
    </Button>
  );
};

export default SubmitButton;
