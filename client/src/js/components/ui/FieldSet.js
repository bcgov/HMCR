import React, { useState } from 'react';
import {FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import {Tooltip } from 'reactstrap';

const helpicon = <FontAwesomeIcon color='#38598a' icon={'question-circle'}/>;

const FieldSet = (props) => {
  const { legendname,children,targetId,tips } = props;
  const tipId = (targetId===undefined||targetId===null||targetId==='')?'TooltipForFieldsetId':targetId;
  const [tooltipOpen, setTooltipOpen] = useState(false);

  const toggle = () => setTooltipOpen(!tooltipOpen);

  return (
    <fieldset className='form-control fieldset'>
      <legend className='form-control legend'>{legendname}
      <span style={{paddingLeft:"4px"}} href="#" id={tipId}>{helpicon}</span><Tooltip placement="bottom" className="fieldset-tooltip" isOpen={tooltipOpen} target={tipId} toggle={toggle}>
       {tips}
      </Tooltip>
     </legend>
      {children}
    </fieldset>

  );
}
export default FieldSet;