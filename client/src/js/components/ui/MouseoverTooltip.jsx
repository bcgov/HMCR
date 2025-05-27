import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React, { useState } from 'react';
import { Popover, PopoverBody } from 'reactstrap';

const MouseoverTooltip = (props) => {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <React.Fragment>
      <FontAwesomeIcon
        id={props.id}
        icon="question-circle"
        className="fa-color-primary ms-1 me-1 fieldset-tooltip"
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
