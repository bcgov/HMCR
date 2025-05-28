import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { format, isValid } from 'date-fns';
import React, { useState, useRef, useEffect } from 'react';
import { DateRange } from 'react-date-range';
import { InputGroup, InputGroupText, Input } from 'reactstrap';
import 'react-date-range/dist/styles.css';
import 'react-date-range/dist/theme/default.css';

const DateRangeInput = ({
  startDate,
  endDate,
  onChange,
  minDate,
  maxDate,
  showError = false,
  errorText = 'Required',
  disabled = false,
  months = 2,
}) => {
  const wrapperRef = useRef(null);
  const [open, setOpen] = useState(false);

  const [range, setRange] = useState([
    {
      startDate,
      endDate,
      key: 'selection',
    },
  ]);

  useEffect(() => {
    setRange([
      {
        startDate,
        endDate,
        key: 'selection',
      },
    ]);
  }, [startDate, endDate]);

  const handleChange = (ranges) => {
    const { startDate, endDate } = ranges.selection;
    setRange([ranges.selection]);
    if (onChange) {
      onChange({ startDate, endDate });
    }
  };

  // Close when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target)) {
        setOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <div className={`DateRangeInputWrapper position-relative ${showError ? 'is-invalid' : ''}`} ref={wrapperRef}>
      <InputGroup onClick={() => !disabled && setOpen(!open)} style={{ width: '250px', cursor: 'pointer' }}>
        <Input
          readOnly
          value={`${isValid(range[0].startDate) ? format(range[0].startDate, 'yyyy-MM-dd') : ''} - ${isValid(range[0].endDate) ? format(range[0].endDate, 'yyyy-MM-dd') : ''}`}
          className={showError ? 'is-invalid' : ''}
          disabled={disabled}
        />
        <InputGroupText>
          <FontAwesomeIcon icon="calendar-alt" />
        </InputGroupText>
      </InputGroup>
      {showError && (
        <div className="invalid-feedback" style={{ display: 'block' }}>
          {errorText}
        </div>
      )}
      {open && (
        <div style={{ position: 'absolute', zIndex: 1000 }}>
          <DateRange
            ranges={range}
            onChange={handleChange}
            moveRangeOnFirstSelection={false}
            editableDateInputs={true}
            rangeColors={['#38598a']}
            direction="horizontal"
            months={months}
            minDate={minDate}
            maxDate={maxDate}
          />
        </div>
      )}
    </div>
  );
};

export default DateRangeInput;
