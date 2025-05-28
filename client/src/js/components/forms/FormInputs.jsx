import { useField } from 'formik';
import { Col, FormGroup, Label, Input, FormFeedback } from 'reactstrap';

export const FormRow = ({ name, label, children }) => {
  return (
    <FormGroup row>
      <Col sm={3}>
        <Label for={name}>{label}</Label>
      </Col>
      <Col sm={9}>{children}</Col>
    </FormGroup>
  );
};

export const FormSwitchInput = ({ children, ...props }) => {
  const [field] = useField({ ...props, type: 'checkbox' });
  return <Input type="switch" id={props.name} {...field} {...props} />;
};

export const FormCheckboxInput = ({ children, ...props }) => {
  const [field] = useField({ ...props, type: 'checkbox' });
  return <Input type="checkbox" id={props.name} {...field} {...props} />;
};

export const FormInput = ({ children, ...props }) => {
  const [field, meta] = useField({ ...props, type: 'checkbox' });

  return (
    <>
      <Input {...field} {...props} invalid={meta.error && meta.touched}>
        {children}
      </Input>
      {meta.error && meta.touched && <FormFeedback>{meta.error}</FormFeedback>}
    </>
  );
};

export const FormNumberInput = ({ children, name, ...props }) => {
  const [field, meta, helpers] = useField({ ...props, name, type: 'number' });
  const { setValue } = helpers;

  const handleChange = (e) => {
    let value = e.target.value;

    if (value === '') {
      // Set the value to null if the input is cleared
      setValue(null);
    } else {
      // Only parse the value as a number if it's not empty
      value = props.type === 'number' ? parseFloat(value) : value;
      // Check if parsing succeeded; otherwise, revert to original string
      // This avoids setting the field to NaN for incomplete number inputs like "-"
      setValue(!isNaN(value) ? value : e.target.value);
    }
  };

  return (
    <>
      <Input type="number" {...field} {...props} invalid={meta.error && meta.touched} onChange={handleChange}>
        {children}
      </Input>
      {meta.error && meta.touched && <FormFeedback>{meta.error}</FormFeedback>}
    </>
  );
};

export const FormRadioInput = ({ label, ...props }) => {
  const [field, meta] = useField({ ...props, type: 'radio' });
  const inputId = props.id || `${props.name}_${props.value}`;

  return (
    <div className="form-check">
      <Input
        type="radio"
        className="form-check-input"
        id={inputId}
        {...field}
        {...props}
        invalid={meta.touched && meta.error ? true : false}
      />
      <label className="form-check-label" htmlFor={inputId}>
        {label}
      </label>
      {meta.touched && meta.error && <FormFeedback>{meta.error}</FormFeedback>}
    </div>
  );
};
