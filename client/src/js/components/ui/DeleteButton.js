import React, { useState, useEffect } from 'react';
import { Popover, PopoverHeader, PopoverBody, ButtonGroup, Button } from 'reactstrap';
import { SingleDatePicker } from 'react-dates';
import moment from 'moment';

import FontAwesomeButton from './FontAwesomeButton';

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
  const [focusedInput, setFocusedInput] = useState(false);
  const [focusClassName, setFocusClassName] = useState('');
  const [buttonText, setButtonText] = useState('Disable');

  useEffect(() => {
    if (defaultEndDate) {
      setDate(moment(defaultEndDate));
      setButtonText('Update');
    } else {
      setDate(null);
    }
  }, [defaultEndDate, popoverOpen]);

  const togglePopover = () => {
    setPopoverOpen(!popoverOpen);
  };

  const handleDatePickerFocusChange = (focused) => {
    setFocusedInput(focused);

    focused ? setFocusClassName('focused') : setFocusClassName('');
  };

  const handleConfirmDelete = () => {
    togglePopover();
    onDeleteClicked(itemId, date, permanentDelete);
  };

  const iconName = permanentDelete ? 'trash-alt' : 'ban';

  return (
    <React.Fragment>
      <FontAwesomeButton color="danger" icon={iconName} id={buttonId} {...props} />
      <Popover placement="auto-start" isOpen={popoverOpen} target={buttonId} toggle={togglePopover} trigger="legacy">
        <PopoverHeader>Are you sure?</PopoverHeader>
        <PopoverBody>
          {permanentDelete ? (
            <div>
              This will <strong>permanently</strong> delete the record.
            </div>
          ) : (
            <div className={`DatePickerWrapper ${focusClassName}`}>
              <SingleDatePicker
                id={`${buttonId}_endDate`}
                date={date}
                onDateChange={(date) => setDate(date)}
                focused={focusedInput}
                onFocusChange={({ focused }) => handleDatePickerFocusChange(focused)}
                hideKeyboardShortcutsPanel={true}
                numberOfMonths={1}
                small
                block
                noBorder
                showDefaultInputIcon={true}
                showClearDate={true}
                inputIconPosition="after"
                placeholder="End Date"
              />
            </div>
          )}

          <div className="text-right mt-3">
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
    </React.Fragment>
  );
};

export default DeleteButton;
