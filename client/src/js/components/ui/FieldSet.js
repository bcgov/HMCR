import React,{ useState }  from 'react';
import { Popover, PopoverBody } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
const FieldSet = (props) => {
  const { legendname,children,targetId,tips } = props;
  const tipId = (targetId===undefined||targetId===null||targetId==='')?'TooltipForFieldsetId':targetId;
  const [isOpen, setIsOpen] = useState(false);
  return (
    <fieldset className='form-control fieldset'>
      <legend className='form-control legend'>{legendname}
        <span style={{paddingLeft:"4px"}}></span>
        <FontAwesomeIcon
          id={tipId}
          icon="question-circle"
          className="fa-color-primary ml-1 mr-1"
          onMouseOver={() => setIsOpen(true)}
          onMouseOut={() => setIsOpen(false)}
          style={{ cursor: 'pointer' }}
        />
        <Popover placement="top" isOpen={isOpen} target={tipId} innerClassName='fieldset-tooltip'>
          <PopoverBody className='fieldset-tooltip'>{tips}</PopoverBody>
        </Popover>
     </legend>
      {children}
    </fieldset>
  );
}
export default FieldSet;