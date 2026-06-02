import React from 'react';
import { Col, FormGroup, Label, Input, FormFeedback } from 'reactstrap';
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

export const FormSwitchInput = ({ label, children, ...props }) => {
  const [field] = useField({ ...props, type: 'checkbox' });
  return (
    <FormGroup switch>
      <Input type="switch" role="switch" id={props.name} {...field} {...props} />
      {label && <Label check for={props.name}>{label}</Label>}
    </FormGroup>
  );
};

export const FormCheckboxInput = ({ children, ...props }) => {
  const [field] = useField({ ...props, type: 'checkbox' });
  return <Input type="checkbox" id={props.name} {...field} {...props} />;
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
    const value = e.target.value;

    if (value === '') {
      setValue(null);
    } else {
      const numericValue = Number(value);
      setValue(!Number.isNaN(numericValue) ? numericValue : value);
    }
  };

  return (
    <React.Fragment>
      <Input
        type="number"
        {...field}
        {...props}
        value={field.value ?? ''}
        invalid={meta.error && meta.touched}
        onChange={handleChange}
      >
        {children}
      </Input>
      {meta.error && meta.touched && <FormFeedback>{meta.error}</FormFeedback>}
    </React.Fragment>
  );
};

export const FormRadioInput = ({ label, ...props }) => {
  const [field, meta] = useField({ ...props, type: 'radio' });
  return (
    <FormGroup check>
      <Input type="radio" {...field} {...props} id={props.id || props.name} invalid={meta.touched && meta.error ? true : false} />
      <Label check for={props.id || props.name}>
        {label}
      </Label>
      {meta.touched && meta.error && <FormFeedback>{meta.error}</FormFeedback>}
    </FormGroup>
  );
};
