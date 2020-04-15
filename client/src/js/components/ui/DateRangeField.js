import React, { useState } from 'react';
import { DateRangePicker } from 'react-dates';
import { Field, useFormikContext } from 'formik';
import { FormFeedback } from 'reactstrap';

import * as Constants from '../../Constants';

const DateRangePickerWithFormik = ({ name, fromName, toName, isOutsideRange, form: { errors, submitCount } }) => {
  const { values, setFieldValue } = useFormikContext();
  const [focusedInput, setFocusedInput] = useState(null);
  const [touched, setTouched] = useState(false);

  const handleFocused = (focusedInput) => {
    setFocusedInput(focusedInput);
    setTouched(true);
  };

  const isInvalid = () => {
    return (submitCount > 0 || touched) && !focusedInput && (errors[fromName] || errors[toName]);
  };

  return (
    <div className={`DatePickerWrapper ${isInvalid() ? 'is-invalid' : ''}`}>
      <DateRangePicker
        startDate={values[fromName]}
        startDateId={`${name}_${fromName}`}
        endDate={values[toName]}
        endDateId={`${name}_${toName}`}
        onDatesChange={({ startDate, endDate }) => {
          setFieldValue(fromName, startDate);
          setFieldValue(toName, endDate);
        }}
        focusedInput={focusedInput}
        onFocusChange={handleFocused}
        startDatePlaceholderText="Date From"
        endDatePlaceholderText="Date To"
        hideKeyboardShortcutsPanel={true}
        small
        showDefaultInputIcon={true}
        inputIconPosition="after"
        displayFormat={Constants.DATE_DISPLAY_FORMAT}
        isOutsideRange={isOutsideRange}
        minimumNights={0}
      />
      {isInvalid() && <FormFeedback style={{ display: 'block' }}>Required</FormFeedback>}
    </div>
  );
};

const DateRangeField = (props) => {
  return <Field component={DateRangePickerWithFormik} {...props} />;
};

export default DateRangeField;
