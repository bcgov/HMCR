import React, { useState } from 'react';
import { SingleDatePicker } from 'react-dates';
import { Field, useFormikContext } from 'formik';

const SingleDatePickerWithFormik = ({ field: { name }, placeholder }) => {
  const [focusedInput, setFocusedInput] = useState(false);
  const [focusClassName, setFocusClassName] = useState('');
  const { values, setFieldValue } = useFormikContext();

  const handleFocusChanged = focused => {
    setFocusedInput(focused);

    focused ? setFocusClassName('focused') : setFocusClassName('');
  };

  return (
    <div className={`DatePickerWrapper ${focusClassName}`}>
      <SingleDatePicker
        id={name}
        date={values[name]}
        onDateChange={date => setFieldValue(name, date)}
        focused={focusedInput}
        onFocusChange={({ focused }) => handleFocusChanged(focused)}
        hideKeyboardShortcutsPanel={true}
        numberOfMonths={1}
        transitionDuration={0}
        small
        block
        noBorder
        showDefaultInputIcon={true}
        inputIconPosition="after"
        placeholder={placeholder}
      />
    </div>
  );
};

const SingleDateField = props => {
  return <Field component={SingleDatePickerWithFormik} {...props} />;
};

export default SingleDateField;
