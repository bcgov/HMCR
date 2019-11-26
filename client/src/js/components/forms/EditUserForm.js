import React, { useEffect, useState } from 'react';
import { connect } from 'react-redux';
import {
  // CustomInput,
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
import moment from 'moment';

import MultiSelect from '../ui/MultiSelect';
import SingleDateField from '../ui/SingleDateField';
import Spinner from '../ui/Spinner';

import { fetchRoles, createUser, editUser } from '../../actions';

import * as api from '../../Api';
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

// const FormCheckbox = ({ children, ...props }) => {
//   const [field] = useField({ ...props, type: 'checkbox' });
//   return <CustomInput type="switch" id={props.name} {...field} {...props} />;
// };

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

const EditUserForm = ({
  toggle,
  isOpen,
  fetchRoles,
  roles,
  userTypes,
  serviceAreas,
  createUser,
  editUser,
  formType,
  userId,
}) => {
  // This is needed until Formik fixes its own setSubmitting function
  const [submitting, setSubmitting] = useState(false);
  const [loading, setLoading] = useState(true);
  const [initialValues, setInitialValues] = useState({});
  const [disableEdit, setDisableEdit] = useState(false);

  const defaultValues = {
    userType: '',
    username: '',
    firstName: '',
    lastName: '',
    email: '',
    userRoleIds: [],
    serviceAreaNumbers: [],
    active: true,
    endDate: null,
  };

  useEffect(() => {
    fetchRoles()
      .then(() => {
        if (formType === Constants.FORM_TYPE.ADD) setLoading(false);
        else {
          return api.instance.get(`${Constants.API_PATHS.USER}/${userId}`).then(response => {
            setInitialValues({
              ...response.data,
              endDate: response.data.endDate ? moment(response.data.endDate) : null,
            });
            setDisableEdit(response.data.hasLogInHistory);
          });
        }
      })
      .then(() => setLoading(false));
  }, [fetchRoles, formType, userId]);

  const handleFormSubmit = values => {
    setSubmitting(true);

    if (formType === Constants.FORM_TYPE.ADD) {
      createUser(values).then(() => {
        setSubmitting(false);
        toggle(true);
      });
    } else {
      editUser(userId, values).then(() => {
        setSubmitting(false);
        toggle(true);
      });
    }
  };

  const renderFormBody = formikProps => {
    return (
      <React.Fragment>
        <FormRow name="userType" label="User Type*">
          <FormInput name="userType" type="select" disabled={disableEdit}>
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
          <FormInput type="text" name="username" placeholder="User Id" disabled={disableEdit} />
        </FormRow>
        <FormRow name="firstName" label="First Name*">
          <FormInput type="text" name="firstName" placeholder="First Name" disabled={disableEdit} />
        </FormRow>
        <FormRow name="lastName" label="Last Name*">
          <FormInput type="text" name="lastName" placeholder="Last Name" disabled={disableEdit} />
        </FormRow>
        <FormRow name="email" label="Email*">
          <FormInput type="email" name="email" placeholder="Email" />
        </FormRow>
        <FormRow name="userRoleIds" label="User Roles*">
          <MultiSelect {...formikProps} items={roles} name="userRoleIds" showId={false} />
        </FormRow>
        <FormRow name="serviceAreaNumbers" label="Service Areas*">
          <MultiSelect {...formikProps} items={serviceAreas} name="serviceAreaNumbers" showId={true} />
        </FormRow>
        {/* <FormRow name="active" label="Active">
          <FormCheckbox name="active" />
        </FormRow> */}
        <FormRow name="endDate" label="End Date">
          <SingleDateField {...formikProps} name="endDate" placeholder="End Date" />
        </FormRow>
      </React.Fragment>
    );
  };

  const title = formType === Constants.FORM_TYPE.ADD ? 'Add User' : 'Edit User';

  return (
    <Modal isOpen={isOpen} toggle={toggle} backdrop="static">
      <Formik
        enableReinitialize={true}
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
        })}
        onSubmit={handleFormSubmit}
      >
        {formikProps => (
          <Form>
            <ModalHeader toggle={toggle}>{title}</ModalHeader>
            <ModalBody>{loading ? <Spinner /> : renderFormBody(formikProps)}</ModalBody>
            <ModalFooter>
              <Button color="primary" size="sm" type="submit" disabled={submitting || !formikProps.dirty}>
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

export default connect(mapStateToProps, { fetchRoles, createUser, editUser })(EditUserForm);
