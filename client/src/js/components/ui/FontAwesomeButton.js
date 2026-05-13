import React from 'react';
import { Button } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const FontAwesomeButton = (props) => {
  return (
    <Button
      size={props.size || 'xs'}
      color={props.color || 'primary'}
      outline={props.noBorder ? false : props.outline !== false}
      className={`fontawesome-button${props.noBorder ? ' no-border' : ''}${props.className ? ` ${props.className}` : ''}`}
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
