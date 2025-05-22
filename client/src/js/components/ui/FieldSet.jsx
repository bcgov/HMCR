import React,{ useState }  from 'react';
import { Tooltip } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const FieldSet = (props) => {
  const { legendname,children,targetId,tips } = props;
  const tipId = (targetId===undefined||targetId===null||targetId==='')?'TooltipForFieldsetId':targetId;
  const [tooltipOpen, setTooltipOpen] = useState(false);
  const toggle = () => setTooltipOpen(!tooltipOpen);
  return (
    <fieldset className="form-control fieldset">
      <legend className="form-control legend">{legendname}
        <span style={{paddingLeft:"4px"}} href="#" id={tipId}><FontAwesomeIcon
          id={'question-circle'+ props.id}
          icon="question-circle"
          className="fa-color-primary ml-1 mr-1"/>
        </span>
        <Tooltip placement="bottom" className="fieldset-tooltip" autohide={false} isOpen={tooltipOpen} target={tipId} toggle={toggle}>
          <div className="fieldset-tooltip-body">{tips}</div></Tooltip>
     </legend>
      {children}
    </fieldset>
  );
}
export default FieldSet;