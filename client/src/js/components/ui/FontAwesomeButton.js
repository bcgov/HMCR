import React from 'react';
import { Button } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const FontAwesomeButton = props => {
  return (
    <Button
      size="xs"
      color={props.color ? props.color : 'primary'}
      className={`fontawesome-button ${props.className}`}
      onClick={props.onClick}
      id={props.id}
    >
      <FontAwesomeIcon icon={props.icon} />
    </Button>
  );
};

export default FontAwesomeButton;
