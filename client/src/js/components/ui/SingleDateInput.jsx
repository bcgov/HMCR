import { faCalendarAlt, faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { format, isValid } from 'date-fns';
import { useRef, useEffect, useState } from 'react';
import { Calendar } from 'react-date-range';
import { InputGroup, InputGroupText, Input, FormFeedback } from 'reactstrap';
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
  style = {},
}) => {
  const wrapperRef = useRef(null);
  const [open, setOpen] = useState(false);

  useEffect(() => {
    const handleClickOutside = (event) => {
      if (wrapperRef.current && !wrapperRef.current.contains(event.target)) {
        setOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const displayValue = value && isValid(new Date(value)) ? format(new Date(value), 'yyyy-MM-dd') : '';

  return (
    <div
      className={`SingleDateInputWrapper position-relative ${showError ? 'is-invalid' : ''}`}
      ref={wrapperRef}
      style={style}
    >
      <InputGroup
        onClick={() => !disabled && setOpen(!open)}
        style={{
          width: '250px',
          cursor: disabled ? 'not-allowed' : 'pointer',
        }}
      >
        <Input
          readOnly
          value={displayValue}
          placeholder={placeholder}
          className={showError ? 'is-invalid' : ''}
          disabled={disabled}
          id={id}
        />
        {value && !disabled && (
          <InputGroupText
            onClick={(e) => {
              e.stopPropagation();
              onChange(null);
              setOpen(false);
            }}
            style={{
              cursor: 'pointer',
            }}
          >
            <FontAwesomeIcon icon={faTimesCircle} />
          </InputGroupText>
        )}
        <InputGroupText>
          <FontAwesomeIcon icon={faCalendarAlt} />
        </InputGroupText>
      </InputGroup>
      {showError && (
        <FormFeedback
          style={{
            display: 'block',
          }}
        >
          {errorText}
        </FormFeedback>
      )}
      {open && (
        <div
          style={{
            position: 'absolute',
            zIndex: 1000,
          }}
        >
          <Calendar
            date={value && isValid(new Date(value)) ? new Date(value) : new Date()}
            onChange={(date) => {
              onChange(date);
              setOpen(false);
            }}
            minDate={minDate}
            maxDate={maxDate}
            showDateDisplay={false}
            color="#38598a"
          />
        </div>
      )}
    </div>
  );
};

export default SingleDateInput;
