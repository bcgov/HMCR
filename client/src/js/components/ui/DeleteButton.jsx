import { useState, useEffect } from 'react';
import { Popover, PopoverHeader, PopoverBody, ButtonGroup, Button } from 'reactstrap';

import FontAwesomeButton from './FontAwesomeButton';
import SingleDateInput from './SingleDateInput';

const DeleteButton = ({
  buttonId,
  children,
  itemId,
  defaultEndDate,
  onDeleteClicked,
  onComplete,
  permanentDelete,
  ...props
}) => {
  const [popoverOpen, setPopoverOpen] = useState(false);
  const [date, setDate] = useState(null);
  const [buttonText, setButtonText] = useState('Disable');

  useEffect(() => {
    if (defaultEndDate) {
      const parsed = new Date(defaultEndDate);
      setDate(isNaN(parsed.getTime()) ? null : parsed);
      setButtonText('Update');
    } else {
      setDate(null);
      setButtonText('Disable');
    }
  }, [defaultEndDate, popoverOpen]);

  const togglePopover = () => {
    setPopoverOpen(!popoverOpen);
  };

  const handleConfirmDelete = () => {
    togglePopover();
    onDeleteClicked(itemId, date, permanentDelete);
  };

  const iconName = permanentDelete ? 'trash-alt' : 'ban';

  return (
    <>
      <FontAwesomeButton color="danger" icon={iconName} id={buttonId} {...props} className="text-white" />
      <Popover placement="auto-start" isOpen={popoverOpen} target={buttonId} toggle={togglePopover} trigger="legacy">
        <PopoverHeader>Are you sure?</PopoverHeader>
        <PopoverBody>
          {permanentDelete ? (
            <div>
              This will <strong>permanently</strong> delete the record.
            </div>
          ) : (
            <div
              style={{
                marginBottom: '1rem',
              }}
            >
              <SingleDateInput
                id={`${buttonId}_endDate`}
                value={date}
                onChange={setDate}
                placeholder="End Date"
                minDate={new Date()}
              />
            </div>
          )}
          <div className="text-end mt-3">
            <ButtonGroup>
              <Button color="danger" size="sm" onClick={handleConfirmDelete}>
                {permanentDelete ? 'Delete' : buttonText}
              </Button>
              <Button color="secondary" size="sm" onClick={togglePopover}>
                Cancel
              </Button>
            </ButtonGroup>
          </div>
        </PopoverBody>
      </Popover>
    </>
  );
};

export default DeleteButton;
