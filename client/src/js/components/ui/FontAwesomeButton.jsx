import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React from 'react';
import { Button } from 'reactstrap';

const FontAwesomeButton = (props) => {
  return (
    <Button
      size={props.size || 'xs'}
      color={props.color || 'primary'}
      className={`fontawesome-button ${props.className}`}
      onClick={props.onClick}
      id={props.id}
      disabled={props.disabled}
      title={props.title}
    >
      <FontAwesomeIcon icon={props.icon} spin={props.spin} />
    </Button>
  );
};

export default FontAwesomeButton;
