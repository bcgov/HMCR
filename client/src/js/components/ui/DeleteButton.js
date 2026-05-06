import React, { useState, useEffect } from 'react';
import { Popover, PopoverHeader, PopoverBody, ButtonGroup, Button } from 'reactstrap';
import DatePicker from 'react-datepicker';
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
            <div className="DatePickerWrapper">
              <DatePicker
                id={`${buttonId}_endDate`}
                selected={date ? date.toDate() : null}
                onChange={(d) => setDate(d ? moment(d) : null)}
                dateFormat="yyyy-MM-dd"
                placeholderText="End Date"
                isClearable
                className="form-control form-control-sm"
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
