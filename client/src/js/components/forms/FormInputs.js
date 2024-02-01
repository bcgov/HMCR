import React from 'react';
import { CustomInput, Col, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
import { useField } from 'formik';

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
  return <CustomInput type="switch" id={props.name} {...field} {...props} />;
};

export const FormCheckboxInput = ({ children, ...props }) => {
  const [field] = useField({ ...props, type: 'checkbox' });
  return <CustomInput type="checkbox" id={props.name} {...field} {...props} />;
};

export const FormInput = ({ children, ...props }) => {
  const [field, meta] = useField({ ...props, type: 'checkbox' });
  return (
    <React.Fragment>
      <Input {...field} {...props} invalid={meta.error && meta.touched}>
        {children}
      </Input>
      {meta.error && meta.touched && <FormFeedback>{meta.error}</FormFeedback>}
    </React.Fragment>
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
    <React.Fragment>
      <Input type="number" {...field} {...props} invalid={meta.error && meta.touched} onChange={handleChange}>
        {children}
      </Input>
      {meta.error && meta.touched && <FormFeedback>{meta.error}</FormFeedback>}
    </React.Fragment>
  );
};

export const FormRadioInput = ({ label, ...props }) => {
  const [field, meta] = useField({ ...props, type: 'radio' });
  return (
    <div>
      <CustomInput
        type="radio"
        {...field}
        {...props}
        label={label}
        id={props.id || props.name}
        invalid={meta.touched && meta.error ? true : false}
      />
      {meta.touched && meta.error && <FormFeedback>{meta.error}</FormFeedback>}
    </div>
  );
};
