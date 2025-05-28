import { Field, useFormikContext } from 'formik';

import SingleDateInput from './SingleDateInput';

const FormikDateWrapper = ({
  field: { name },
  form: { errors, touched, submitCount },
  placeholder,
  style,
  minDate,
  maxDate,
  disabled,
  id,
}) => {
  const { setFieldValue, values } = useFormikContext();
  const value = values[name];

  const showError = (submitCount > 0 || touched?.[name]) && errors?.[name];

  return (
    <SingleDateInput
      value={value}
      onChange={(date) => setFieldValue(name, date)}
      showError={!!showError}
      errorText={errors?.[name]}
      placeholder={placeholder}
      disabled={disabled}
      minDate={minDate}
      maxDate={maxDate}
      style={style}
      id={id}
    />
  );
};

const SingleDateField = (props) => <Field component={FormikDateWrapper} {...props} />;

export default SingleDateField;
