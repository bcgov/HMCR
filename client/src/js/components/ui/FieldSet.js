import React from 'react';
import MouseoverTooltip from './MouseoverTooltip';

const FieldSet = (props) => {
  const { legendname,children,targetId,tips } = props;
  const tipId = (targetId===undefined||targetId===null||targetId==='')?'TooltipForFieldsetId':targetId;
  return (
    <fieldset className='form-control fieldset'>
      <legend className='form-control legend'>{legendname}
        <span style={{paddingLeft:"4px"}}></span>
        <MouseoverTooltip id={tipId}>{tips}</MouseoverTooltip>
     </legend>
      {children}
    </fieldset>
  );
}
export default FieldSet;