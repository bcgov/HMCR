import React, { useState } from 'react';
import { SingleDatePicker } from 'react-dates';
import { Field, useFormikContext } from 'formik';

import * as Constants from '../../Constants';

const SingleDatePickerWithFormik = ({ field: { name }, placeholder, style, isOutsideRange }) => {
  const [focusedInput, setFocusedInput] = useState(false);
  const [focusClassName, setFocusClassName] = useState('');
  const { values, setFieldValue } = useFormikContext();

  const handleFocusChanged = (focused) => {
    setFocusedInput(focused);

    focused ? setFocusClassName('focused') : setFocusClassName('');
  };

  return (
    <div className={`DatePickerWrapper ${focusClassName}`} style={style}>
      <SingleDatePicker
        id={name}
        date={values[name]}
        onDateChange={(date) => setFieldValue(name, date)}
        focused={focusedInput}
        onFocusChange={({ focused }) => handleFocusChanged(focused)}
        hideKeyboardShortcutsPanel={true}
        numberOfMonths={1}
        small
        block
        noBorder
        showDefaultInputIcon={true}
        showClearDate={true}
        inputIconPosition="after"
        placeholder={placeholder}
        isOutsideRange={isOutsideRange}
        displayFormat={Constants.DATE_DISPLAY_FORMAT}
      />
    </div>
  );
};

const SingleDateField = (props) => {
  return <Field component={SingleDatePickerWithFormik} {...props} />;
};

export default SingleDateField;
