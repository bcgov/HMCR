import React from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, FormGroup, Label, ListGroup, ListGroupItem } from 'reactstrap';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import _ from 'lodash';

import { fetchCountries, fetchProvinces, createProvince } from '../actions';

class HelloWorld extends React.Component {
  state = { selectedCountry: null };

  componentDidMount() {
    Promise.all([this.props.fetchCountries(), this.props.fetchProvinces()]).then(() => {});
  }

  render() {
    const { countries, provinces, createProvince, fetchProvinces } = this.props;
    return (
      <div>
        <h1>Hello World</h1>
        <Formik
          initialValues={{ countryId: '', provinceCode: '', description: '' }}
          onSubmit={(values, { setSubmitting, resetForm }) => {
            createProvince(values).then(() => {
              setSubmitting(false);
              fetchProvinces();
              resetForm();
            });
          }}
        >
          {({ values, isSubmitting, handleChange, handleBlur }) => (
            <Form>
              <FormGroup>
                <Label for="countryId">Country</Label>
                <select
                  id="countryId"
                  name="countryId"
                  className="form-control"
                  value={values.color}
                  onChange={e => {
                    handleChange(e);
                    this.setState({ selectedCountry: e.target.value });
                  }}
                  onBlur={handleBlur}
                >
                  <option value="">Select Country</option>
                  {Object.values(countries).map(country => {
                    return (
                      <option value={country.id} key={country.id}>
                        {`${country.countryCode} - ${country.description}`}
                      </option>
                    );
                  })}
                </select>
                <ErrorMessage name="countryId" component="div" />
              </FormGroup>

              {this.state.selectedCountry && (
                <React.Fragment>
                  <h4>Existing Provinces</h4>
                  <ListGroup className="mb-3">
                    {Object.values(
                      _.filter(provinces, province => {
                        return province.countryId === this.state.selectedCountry;
                      })
                    ).map(province => {
                      return (
                        <ListGroupItem key={province.id}>
                          {`${province.id} - ${province.provinceCode} - ${province.description}`}
                        </ListGroupItem>
                      );
                    })}
                  </ListGroup>
                </React.Fragment>
              )}

              <h4>Add Province</h4>
              <Row form>
                <Col>
                  <FormGroup>
                    <Label for="provinceCode">Province Code</Label>
                    <Field type="text" name="provinceCode" className="form-control" />
                    <ErrorMessage name="provinceCode" component="div" />
                  </FormGroup>
                </Col>
                <Col>
                  <FormGroup>
                    <Label for="description">Province Description</Label>
                    <Field type="text" name="description" className="form-control" />
                    <ErrorMessage name="description" component="div" />
                  </FormGroup>
                </Col>
              </Row>

              <Button type="submit" color="primary" disabled={isSubmitting}>
                Add
              </Button>
            </Form>
          )}
        </Formik>
      </div>
    );
  }
}

const mapStateToProps = state => {
  return {
    provinces: state.hello.provinces,
    countries: state.hello.countries,
  };
};

export default connect(
  mapStateToProps,
  { fetchCountries, fetchProvinces, createProvince }
)(HelloWorld);
