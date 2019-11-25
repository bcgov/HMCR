import React, { useEffect, useState } from 'react';
import { connect } from 'react-redux';
import {
  CustomInput,
  Col,
  Button,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter,
  FormGroup,
  Label,
  Input,
  FormFeedback,
} from 'reactstrap';
import { Formik, Form, useField } from 'formik';
import * as Yup from 'yup';

import MultiSelect from '../ui/MultiSelect';
import SingleDateField from '../ui/SingleDateField';

import { fetchRoles, createUser } from '../../actions';

import * as Constants from '../../Constants';

const FormRow = ({ name, label, children }) => {
  return (
    <FormGroup row>
      <Col sm={3}>
        <Label for={name}>{label}</Label>
      </Col>
      <Col sm={9}>{children}</Col>
    </FormGroup>
  );
};

const FormCheckbox = ({ children, ...props }) => {
  const [field] = useField({ ...props, type: 'checkbox' });
  return <CustomInput type="switch" id={props.name} {...field} {...props} />;
};

const FormInput = ({ children, ...props }) => {
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

const AddUserForm = ({
  toggle,
  isOpen,
  fetchRoles,
  roles,
  userTypes,
  serviceAreas,
  createUser,
  formType,
  initialValues,
}) => {
  useEffect(() => {
    fetchRoles();
  }, [fetchRoles]);

  // This is needed until Formik fixes its own setSubmitting function
  const [submitting, setSubmitting] = useState(false);

  const defaultValues = {
    userType: '',
    username: '',
    firstName: '',
    lastName: '',
    email: '',
    userRoleIds: [],
    ServiceAreaNumbers: [],
    active: true,
    endDate: null,
  };

  return (
    <Modal isOpen={isOpen} toggle={toggle} backdrop="static">
      <Formik
        initialValues={formType === Constants.FORM_TYPE.ADD ? defaultValues : initialValues}
        validationSchema={Yup.object({
          userType: Yup.string()
            .required('Required')
            .max(30)
            .trim(),
          username: Yup.string()
            .required('Required')
            .max(32)
            .trim(),
          firstName: Yup.string()
            .required('Required')
            .max(150)
            .trim(),
          lastName: Yup.string()
            .required('Required')
            .max(150)
            .trim(),
          email: Yup.string()
            .email('Invalid email address')
            .required('Required')
            .max(100)
            .trim(),
          userRoleIds: Yup.array().required('Require at least one role'),
          ServiceAreaNumbers: Yup.array().required('Require at least one service area'),
        })}
        onSubmit={values => {
          setSubmitting(true);
          createUser(values).then(() => {
            setSubmitting(false);
            toggle(true);
          });
        }}
      >
        {formikProps => (
          <Form>
            <ModalHeader toggle={toggle}>Add User</ModalHeader>
            <ModalBody>
              <FormRow name="userType" label="User Type*">
                <FormInput name="userType" type="select">
                  <option value="">--Select User Type--</option>
                  {userTypes.map(userType => {
                    return (
                      <option key={userType.id} value={userType.id}>
                        {userType.name}
                      </option>
                    );
                  })}
                </FormInput>
              </FormRow>
              <FormRow name="username" label="User Id*">
                <FormInput type="text" name="username" placeholder="User Id" />
              </FormRow>
              <FormRow name="firstName" label="First Name*">
                <FormInput type="text" name="firstName" placeholder="First Name" />
              </FormRow>
              <FormRow name="lastName" label="Last Name*">
                <FormInput type="text" name="lastName" placeholder="Last Name" />
              </FormRow>
              <FormRow name="email" label="Email*">
                <FormInput type="email" name="email" placeholder="Email" />
              </FormRow>
              <FormRow name="userRoleIds" label="User Roles*">
                <MultiSelect {...formikProps} items={roles} name="userRoleIds" showId={false} />
              </FormRow>
              <FormRow name="ServiceAreaNumbers" label="Service Areas*">
                <MultiSelect {...formikProps} items={serviceAreas} name="ServiceAreaNumbers" showId={true} />
              </FormRow>
              <FormRow name="active" label="Active">
                <FormCheckbox name="active" />
              </FormRow>
              <FormRow name="endDate" label="End Date">
                <SingleDateField {...formikProps} name="endDate" placeholder="End Date" />
              </FormRow>
            </ModalBody>
            <ModalFooter>
              <Button color="primary" size="sm" type="submit" disabled={submitting}>
                Submit
              </Button>
              <Button color="secondary" size="sm" onClick={toggle}>
                Cancel
              </Button>
            </ModalFooter>
          </Form>
        )}
      </Formik>
    </Modal>
  );
};

const mapStateToProps = state => {
  return {
    serviceAreas: Object.values(state.serviceAreas),
    userTypes: Object.values(state.user.types),
    roles: Object.values(state.roles),
  };
};

export default connect(mapStateToProps, { fetchRoles, createUser })(AddUserForm);
