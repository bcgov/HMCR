import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button } from 'reactstrap';
import { Formik, Form, Field } from 'formik';

import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import SingleDropdown from './ui/SingleDropdown';
import EditRoleForm from './forms/EditRoleForm';
import DataTableControl from './ui/DataTableControl';

import { setSingleRoleSeachCriteria, searchRoles } from '../actions';

import * as Constants from '../Constants';
import * as api from '../Api';

const defaultSearchFormValues = { searchText: '', isActive: 'ACTIVE' };

const tableColumns = [
  { heading: 'Role Name', key: 'name', nosort: true },
  { heading: 'Role Description', key: 'description', nosort: true },
  { heading: 'Active', key: 'isActive', nosort: true },
];

const RoleAdmin = ({ roleStatuses, setSingleRoleSeachCriteria, searchRoles, searchResult }) => {
  const [editRoleForm, setEditRoleForm] = useState({ isOpen: false });

  useEffect(() => {
    searchRoles();
    // eslint-disable-next-line
  }, []);

  const handleSearchFormSubmit = values => {
    const searchText = values.searchText.trim() || null;
    setSingleRoleSeachCriteria('searchText', searchText);

    setSingleRoleSeachCriteria('isActive', values.isActive === 'ACTIVE');

    searchRoles();
  };

  const onEditClicked = roleId => {
    setEditRoleForm({ isOpen: true, formType: Constants.FORM_TYPE.EDIT, roleId });
  };

  const onDeleteClicked = (roleId, endDate) => {
    api.deleteRole(roleId, endDate).then(() => searchRoles());
  };

  const handleEditUserFormClose = refresh => {
    if (refresh) {
      searchRoles();
    }
    setEditRoleForm({ isOpen: false });
  };

  return (
    <React.Fragment>
      <MaterialCard>
        <Formik
          initialValues={defaultSearchFormValues}
          onSubmit={values => {
            handleSearchFormSubmit(values);
          }}
        >
          {formikProps => (
            <Form>
              <Row form>
                <Col>
                  <Field type="text" name="searchText" placeholder="Role/Description" className="form-control" />
                </Col>
                <Col>
                  <SingleDropdown {...formikProps} defaultTitle="Select Status" items={roleStatuses} name="isActive" />
                </Col>
                <Col />
                <Col />
                <Col>
                  <div className="float-right">
                    <Button type="submit" color="primary" className="mr-2">
                      Search
                    </Button>
                    <Button type="reset">Clear</Button>
                  </div>
                </Col>
              </Row>
            </Form>
          )}
        </Formik>
      </MaterialCard>
      <Authorize requires={Constants.PERMISSIONS.ROLE_W}>
        <Row>
          <Col>
            <Button
              size="sm"
              color="primary"
              className="float-right mb-3"
              onClick={() => setEditRoleForm({ isOpen: true, formType: Constants.FORM_TYPE.ADD })}
            >
              Add Role
            </Button>
          </Col>
        </Row>
      </Authorize>
      {searchResult.length > 0 && (
        <MaterialCard>
          <DataTableControl
            dataList={searchResult}
            tableColumns={tableColumns}
            editable
            editPermissionName={Constants.PERMISSIONS.ROLE_W}
            onEditClicked={onEditClicked}
            onDeleteClicked={onDeleteClicked}
          />
        </MaterialCard>
      )}
      {editRoleForm.isOpen && <EditRoleForm {...editRoleForm} toggle={handleEditUserFormClose} />}
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    roleStatuses: Object.values(state.roles.statuses),
    searchResult: Object.values(state.roles.list),
  };
};

export default connect(mapStateToProps, { setSingleRoleSeachCriteria, searchRoles })(RoleAdmin);
