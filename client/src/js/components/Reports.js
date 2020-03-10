import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button } from 'reactstrap';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';

import MaterialCard from './ui/MaterialCard';
import SingleDropdownField from './ui/SingleDropdownField';
import DateRangeField from './ui/DateRangeField';
import MultiDropdownField from './ui/MultiDropdownField';

import * as Constants from '../Constants';

const defaultSearchFormValues = {
  reportTypeId: '',
  dateFrom: null,
  dateTo: null,
  serviceAreaNumbers: [],
  highwayUniqueId: '',
  maintenanceTypeIds: [],
  activityNumbers: [],
};

const validationSchema = Yup.object({
  reportTypeId: Yup.string()
    .required('Required')
    .trim(),
  dateFrom: Yup.object().required('Required'),
  dateTo: Yup.object().required('Required'),
});

const Reports = ({ reportTypes, maintenanceTypes, serviceAreas, currentUser }) => {
  const [validServiceAreas, setValidServiceAreas] = useState([]);

  useEffect(() => {
    const currentUserSAIds = currentUser.serviceAreas.map(sa => sa.id);
    setValidServiceAreas(serviceAreas.filter(sa => currentUserSAIds.includes(sa.id)));
  }, []);

  console.log(validServiceAreas);
  return (
    <React.Fragment>
      <h1>Reports Export</h1>

      <Formik
        initialValues={defaultSearchFormValues}
        enableReinitialize={true}
        onSubmit={values => {
          console.log(values);
        }}
        onReset={() => {}}
        validationSchema={validationSchema}
      >
        {formikProps => (
          <React.Fragment>
            <MaterialCard>
              <Form>
                <div className="d-flex">
                  <div style={{ width: '200px' }} className="mr-3">
                    <SingleDropdownField defaultTitle="Select Report Type" items={reportTypes} name="reportTypeId" />
                  </div>
                  <DateRangeField name="reportDate" fromName="dateFrom" toName="dateTo" />
                </div>
                {formikProps.values.reportTypeId && formikProps.values.dateFrom && formikProps.values.dateTo && (
                  <React.Fragment>
                    <div className="mt-3 mb-1">
                      <strong>Optional filters</strong>
                    </div>
                    <Row>
                      <Col>
                        <MultiDropdownField
                          {...formikProps}
                          title="Maintenance Type"
                          items={maintenanceTypes}
                          name="maintenanceTypeIds"
                        />
                      </Col>
                      <Col />
                      <Col />
                      <Col />
                    </Row>
                  </React.Fragment>
                )}
              </Form>
            </MaterialCard>
            <Button onClick={formikProps.submitForm}>Submit</Button>
          </React.Fragment>
        )}
      </Formik>
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    reportTypes: Object.values(state.submissions.streams),
    maintenanceTypes: state.codeLookups.maintenanceTypes,
    serviceAreas: Object.values(state.serviceAreas),
    currentUser: state.user.current,
  };
};

export default connect(mapStateToProps)(Reports);
