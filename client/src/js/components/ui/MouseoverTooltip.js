import React, { useState } from 'react';
import { Popover, PopoverBody } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const MouseoverTooltip = (props) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <React.Fragment>
      <FontAwesomeIcon
        id={props.id}
        icon="question-circle"
        className="fa-color-primary ml-1 mr-1"
        onMouseOver={() => setIsOpen(true)}
        onMouseOut={() => setIsOpen(false)}
        style={{ cursor: 'pointer' }}
      />
      <Popover placement="top" isOpen={isOpen} target={props.id}>
        <PopoverBody>{props.children}</PopoverBody>
      </Popover>
    </React.Fragment>
  );
};

export default MouseoverTooltip;
