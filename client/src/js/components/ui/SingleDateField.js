import React from 'react';
import DatePicker from 'react-datepicker';
import { Field, useFormikContext } from 'formik';
import moment from 'moment';

const SingleDatePickerWithFormik = ({ field: { name }, placeholder, style, isOutsideRange }) => {
  const { values, setFieldValue } = useFormikContext();
  const current = values[name];

  return (
    <div className="DatePickerWrapper" style={style}>
      <DatePicker
        id={name}
        selected={current ? current.toDate() : null}
        onChange={(date) => setFieldValue(name, date ? moment(date) : null)}
        dateFormat="yyyy-MM-dd"
        placeholderText={placeholder}
        isClearable
        filterDate={isOutsideRange ? (date) => !isOutsideRange(moment(date)) : undefined}
        className="form-control form-control-sm"
      />
    </div>
  );
};

const SingleDateField = (props) => {
  return <Field component={SingleDatePickerWithFormik} {...props} />;
};

export default SingleDateField;
