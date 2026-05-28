import React from 'react';
import DatePicker from 'react-datepicker';
import { Field, useFormikContext } from 'formik';
import { FormFeedback } from 'reactstrap';
import moment from 'moment';

const DateRangePickerWithFormik = ({ name, fromName, toName, isOutsideRange, form: { errors, submitCount } }) => {
  const { values, setFieldValue } = useFormikContext();
  const fromMoment = values[fromName];
  const toMoment = values[toName];
  const fromDate = fromMoment ? fromMoment.toDate() : null;
  const toDate = toMoment ? toMoment.toDate() : null;
  const filterDate = isOutsideRange ? (date) => !isOutsideRange(moment(date)) : undefined;
  const isInvalid = submitCount > 0 && (errors[fromName] || errors[toName]);

  return (
    <div className={`DatePickerWrapper ${isInvalid ? 'is-invalid' : ''}`}>
      <DatePicker
        id={`${name}_${fromName}`}
        selected={fromDate}
        onChange={(date) => setFieldValue(fromName, date ? moment(date) : null)}
        selectsStart
        startDate={fromDate}
        endDate={toDate}
        dateFormat="yyyy-MM-dd"
        placeholderText="Date From"
        isClearable
        filterDate={filterDate}
        className="form-control form-control-sm"
      />
      <DatePicker
        id={`${name}_${toName}`}
        selected={toDate}
        onChange={(date) => setFieldValue(toName, date ? moment(date) : null)}
        selectsEnd
        startDate={fromDate}
        endDate={toDate}
        minDate={fromDate}
        dateFormat="yyyy-MM-dd"
        placeholderText="Date To"
        isClearable
        filterDate={filterDate}
        className="form-control form-control-sm"
      />
      {isInvalid && <FormFeedback style={{ display: 'block' }}>Required</FormFeedback>}
    </div>
  );
};

const DateRangeField = (props) => {
  return <Field component={DateRangePickerWithFormik} {...props} />;
};

export default DateRangeField;
