import React, { useState, useRef, useEffect } from 'react';
import { Calendar } from 'react-date-range';
import { InputGroup, InputGroupText, Input, FormFeedback } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { format, isValid } from 'date-fns';

import 'react-date-range/dist/styles.css';
import 'react-date-range/dist/theme/default.css';

const SingleDateInput = ({
  value,
  onChange,
  minDate,
  maxDate,
  showError = false,
  errorText = 'Required',
  placeholder = 'Select Date',
  disabled = false,
  id,
}) => {
  const wrapperRef = useRef(null);
  const [open, setOpen] = useState(false);

  const handleOutsideClick = (e) => {
    if (wrapperRef.current && !wrapperRef.current.contains(e.target)) {
      setOpen(false);
    }
  };

  useEffect(() => {
    document.addEventListener('mousedown', handleOutsideClick);
    return () => document.removeEventListener('mousedown', handleOutsideClick);
  }, []);

  const displayValue = isValid(new Date(value)) ? format(new Date(value), 'yyyy-MM-dd') : '';

  return (
    <div
      className={`SingleDateInputWrapper position-relative ${showError ? 'is-invalid' : ''}`}
      ref={wrapperRef}
    >
      <InputGroup
        onClick={() => !disabled && setOpen(!open)}
        style={{ width: '250px', cursor: disabled ? 'not-allowed' : 'pointer' }}
      >
        <Input
          readOnly
          value={displayValue}
          placeholder={placeholder}
          className={showError ? 'is-invalid' : ''}
          disabled={disabled}
          id={id}
        />
        <InputGroupText>
          <FontAwesomeIcon icon="calendar-alt" />
        </InputGroupText>
      </InputGroup>

      {showError && <FormFeedback style={{ display: 'block' }}>{errorText}</FormFeedback>}

      {open && (
        <div style={{ position: 'absolute', zIndex: 1000 }}>
          <Calendar
            date={isValid(new Date(value)) ? new Date(value) : new Date()}
            onChange={(selectedDate) => {
              onChange(selectedDate);
              setOpen(false);
            }}
            color="#38598a"
            minDate={minDate}
            maxDate={maxDate}
            showDateDisplay={false}
          />
        </div>
      )}
    </div>
  );
};

export default SingleDateInput;
