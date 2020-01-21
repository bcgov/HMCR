import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button } from 'reactstrap';
import { Formik, Form, Field } from 'formik';

import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import MultiDropdown from './ui/MultiDropdown';
import EditRoleForm from './forms/EditRoleForm';
import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';
import SubmitButton from './ui/SubmitButton';
import PageSpinner from './ui/PageSpinner';

import { setSingleRoleSeachCriteria, searchRoles } from '../actions';

import * as Constants from '../Constants';
import * as api from '../Api';

const defaultSearchFormValues = { searchText: '', roleStatusId: ['ACTIVE'] };

const tableColumns = [
  { heading: 'Role Name', key: 'name' },
  { heading: 'Role Description', key: 'description' },
  { heading: 'Active', key: 'isActive', nosort: true },
];

const RoleAdmin = ({ roleStatuses, setSingleRoleSeachCriteria, searchRoles, searchResult, searchPagination }) => {
  const [editRoleForm, setEditRoleForm] = useState({ isOpen: false });
  const [searching, setSearching] = useState(false);

  useEffect(() => {
    const search = async () => {
      setSearching(true);
      await searchRoles();
      setSearching(false);
    };

    search();
  }, [searchRoles]);

  const startSearch = async () => {
    setSearching(true);
    await searchRoles();
    setSearching(false);
  };

  const handleSearchFormSubmit = values => {
    const searchText = values.searchText.trim() || null;
    setSingleRoleSeachCriteria('searchText', searchText);

    let isActive = null;
    if (values.roleStatusId.length === 1) {
      isActive = values.roleStatusId[0] === 'ACTIVE';
    }
    setSingleRoleSeachCriteria('isActive', isActive);

    startSearch();
  };

  const onEditClicked = roleId => {
    setEditRoleForm({ isOpen: true, formType: Constants.FORM_TYPE.EDIT, roleId });
  };

  const onDeleteClicked = (roleId, endDate) => {
    api.deleteRole(roleId, endDate).then(() => startSearch());
  };

  const handleEditUserFormClose = refresh => {
    if (refresh === true) {
      startSearch();
    }
    setEditRoleForm({ isOpen: false });
  };

  const handleChangePage = newPage => {
    setSingleRoleSeachCriteria('pageNumber', newPage);
    startSearch();
  };

  const handleChangePageSize = newSize => {
    setSingleRoleSeachCriteria('pageSize', newSize);
    setSingleRoleSeachCriteria('pageNumber', 1);
    startSearch();
  };

  const handleHeadingSortClicked = headingKey => {
    setSingleRoleSeachCriteria('pageNumber', 1);
    setSingleRoleSeachCriteria('orderBy', headingKey);
    startSearch();
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
                  <MultiDropdown {...formikProps} title="Role Status" items={roleStatuses} name="roleStatusId" />
                </Col>
                <Col />
                <Col />
                <Col>
                  <div className="float-right">
                    <SubmitButton className="mr-2" disabled={searching} submitting={searching}>
                      Search
                    </SubmitButton>
                    <Button type="reset">Reset</Button>
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
      {searching && <PageSpinner />}
      {!searching && searchResult.length > 0 && (
        <MaterialCard>
          <DataTableWithPaginaionControl
            dataList={searchResult}
            tableColumns={tableColumns}
            searchPagination={searchPagination}
            onPageNumberChange={handleChangePage}
            onPageSizeChange={handleChangePageSize}
            editable
            editPermissionName={Constants.PERMISSIONS.ROLE_W}
            onEditClicked={onEditClicked}
            onDeleteClicked={onDeleteClicked}
            onHeadingSortClicked={handleHeadingSortClicked}
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
    searchPagination: state.roles.searchPagination,
  };
};

export default connect(mapStateToProps, { setSingleRoleSeachCriteria, searchRoles })(RoleAdmin);
