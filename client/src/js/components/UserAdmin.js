import React from 'react';
import { Row, Col, Button } from 'reactstrap';
import { Formik, Form, Field } from 'formik';

import MaterialCard from './ui/MaterialCard';
import MultiDropdown from './forms/MultiDropdown';
import SingleDateField from './forms/SingleDateField';

const serviceAreas = [];
for (var i = 1; i <= 21; i++) {
  serviceAreas.push({ id: i, name: `SA ${i}` });
}

// const userTypes = [{ id: 1, name: 'IDIR' }, { id: 2, name: 'BCeID' }];

const userStatus = [{ id: 1, name: 'Active' }, { id: 2, name: 'Inactive' }];

const UserAdmin = () => {
  return (
    <React.Fragment>
      <MaterialCard>
        <Formik
          initialValues={{ serviceAreaIds: [], userTypeIds: [], userId: '', userStatusIds: [], dateId: null }}
          onSubmit={(values, { setSubmitting }) => {
            setTimeout(() => {
              alert(JSON.stringify(values, null, 2));
              setSubmitting(false);
            }, 400);
          }}
        >
          {formikProps => (
            <Form>
              <Row form>
                <Col>
                  <MultiDropdown {...formikProps} items={serviceAreas} name="serviceAreaIds" title="Service Area" />
                </Col>

                <Col>
                  {/* <MultiDropdown {...formikProps} items={userTypes} name="userTypeIds" title="User Type" /> */}
                  <Field type="text" name="userId" placeholder="User Id/Name" className="form-control" />
                </Col>
                <Col>
                  {/* <Field type="text" name="userId" placeholder="User Id/Name" className="form-control" /> */}
                  <SingleDateField {...formikProps} name="dateId" />
                </Col>
                <Col>
                  <MultiDropdown {...formikProps} items={userStatus} name="userStatusIds" title="User Status" />
                </Col>
                <Col>
                  <Button type="submit" color="primary" className="mr-2">
                    Search
                  </Button>
                  <Button type="reset">Clear</Button>
                </Col>
              </Row>
            </Form>
          )}
        </Formik>
      </MaterialCard>
      <MaterialCard></MaterialCard>
    </React.Fragment>
  );
};

export default UserAdmin;
