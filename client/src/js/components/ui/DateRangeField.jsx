import { Field, useFormikContext } from 'formik';
import React from 'react';

import DateRangeInput from './DateRangeInput';

const DateRangePickerWithFormik = ({ fromName, toName, form: { errors, submitCount, touched } }) => {
  const { values, setFieldValue } = useFormikContext();

  const handleChange = ({ startDate, endDate }) => {
    setFieldValue(fromName, startDate, true);
    setFieldValue(toName, endDate, true);
  };

  const showError =
    (submitCount > 0 || touched?.[fromName] || touched?.[toName]) && (errors?.[fromName] || errors?.[toName]);

  return (
    <DateRangeInput
      startDate={values[fromName]}
      endDate={values[toName]}
      onChange={handleChange}
      showError={showError}
    />
  );
};

const DateRangeField = (props) => {
  return <Field component={DateRangePickerWithFormik} {...props} />;
};

export default DateRangeField;
