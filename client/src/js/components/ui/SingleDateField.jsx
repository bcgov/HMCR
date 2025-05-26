import React, { useState, useRef, useEffect } from 'react';
import { Calendar } from 'react-date-range';
import { Field, useFormikContext } from 'formik';
import { InputGroup, InputGroupText, Input, FormFeedback } from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { format, isValid } from 'date-fns';

import 'react-date-range/dist/styles.css';
import 'react-date-range/dist/theme/default.css';
import * as Constants from '../../Constants';

const SingleDatePickerWithFormik = ({
  field: { name },
  placeholder,
  style,
  isOutsideRange, // not used here — you can map it to minDate/maxDate if needed
  form: { errors, submitCount, touched },
}) => {
  const { values, setFieldValue } = useFormikContext();
  const wrapperRef = useRef(null);
  const [open, setOpen] = useState(false);

  const selectedDate = values[name] ? new Date(values[name]) : null;
  const showError = (submitCount > 0 || touched?.[name]) && errors?.[name];

  const handleChange = (date) => {
    setFieldValue(name, date);
    setOpen(false);
  };

  // Close on outside click
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
    <div className={`DatePickerWrapper position-relative ${showError ? 'is-invalid' : ''}`} style={style} ref={wrapperRef}>
      <InputGroup onClick={() => setOpen(!open)} style={{ cursor: 'pointer', width: '250px' }}>
        <Input
          readOnly
          value={isValid(selectedDate) ? format(selectedDate, Constants.DATE_DISPLAY_FORMAT) : ''}
          placeholder={placeholder}
          className={showError ? 'is-invalid' : ''}
        />
        <InputGroupText>
          <FontAwesomeIcon icon="calendar-alt" />
        </InputGroupText>
      </InputGroup>
      {showError && <FormFeedback style={{ display: 'block' }}>{errors[name]}</FormFeedback>}

      {open && (
        <div style={{ position: 'absolute', zIndex: 10 }}>
          <Calendar
            date={selectedDate || new Date()}
            onChange={handleChange}
            showDateDisplay={false}
            color="#38598a"
          />
        </div>
      )}
    </div>
  );
};

const SingleDateField = (props) => {
  return <Field component={SingleDatePickerWithFormik} {...props} />;
};

export default SingleDateField;
