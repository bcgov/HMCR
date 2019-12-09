import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button } from 'reactstrap';
import { Formik, Form, Field } from 'formik';

import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import MultiDropdown from './ui/MultiDropdown';
import EditUserForm from './forms/EditUserForm';
import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';

import { setSingleUserSeachCriteria, searchUsers, deleteUser } from '../actions';

import * as Constants from '../Constants';

const defaultSearchFormValues = { serviceAreaIds: [], userTypeIds: [], searchText: '', userStatusIds: [] };
const tableColumns = [
  { heading: 'ID Type', key: 'userType' },
  { heading: 'First Name', key: 'firstName' },
  { heading: 'Last Name', key: 'lastName' },
  { heading: 'User ID', key: 'username' },
  { heading: 'Organization', key: 'businessLegalName' },
  { heading: 'Service Areas', key: 'serviceAreas', nosort: true },
  { heading: 'Active', key: 'isActive', nosort: true },
];

const UserAdmin = ({
  serviceAreas,
  userStatuses,
  userTypes,
  setSingleUserSeachCriteria,
  searchUsers,
  searchResult,
  searchPagination,
  deleteUser,
}) => {
  const [editUserForm, setEditUserForm] = useState({ isOpen: false });

  useEffect(() => {
    searchUsers();
    // eslint-disable-next-line
  }, []);

  const handleSearchFormSubmit = values => {
    const serviceAreas = values.serviceAreaIds.join(',') || null;
    setSingleUserSeachCriteria('serviceAreas', serviceAreas);

    const userTypeIds = values.userTypeIds.join(',') || null;
    setSingleUserSeachCriteria('userTypes', userTypeIds);

    const searchText = values.searchText.trim() || null;
    setSingleUserSeachCriteria('searchText', searchText);

    let isActive = null;
    if (values.userStatusIds.length === 1) {
      isActive = values.userStatusIds[0] === 'ACTIVE';
    }
    setSingleUserSeachCriteria('isActive', isActive);

    searchUsers();
  };

  const onEditClicked = userId => {
    setEditUserForm({ isOpen: true, formType: Constants.FORM_TYPE.EDIT, userId });
  };

  const onDeleteClicked = (userId, endDate) => {
    deleteUser(userId, endDate).then(() => searchUsers());
  };

  const handleEditUserFormClose = refresh => {
    if (refresh) {
      searchUsers();
    }
    setEditUserForm({ isOpen: false });
  };

  const handleChangePage = newPage => {
    setSingleUserSeachCriteria('pageNumber', newPage);
    searchUsers();
  };

  const handleChangePageSize = newSize => {
    setSingleUserSeachCriteria('pageSize', newSize);
    setSingleUserSeachCriteria('pageNumber', 1);
    searchUsers();
  };

  const handleHeadingSortClicked = headingKey => {
    setSingleUserSeachCriteria('pageNumber', 1);
    setSingleUserSeachCriteria('orderBy', headingKey);
    searchUsers();
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
                  <MultiDropdown {...formikProps} items={serviceAreas} name="serviceAreaIds" title="Service Area" />
                </Col>

                <Col>
                  <MultiDropdown {...formikProps} items={userTypes} name="userTypeIds" title="User Type" />
                </Col>
                <Col>
                  <Field type="text" name="searchText" placeholder="User Id/Name" className="form-control" />
                </Col>
                <Col>
                  <MultiDropdown {...formikProps} items={userStatuses} name="userStatusIds" title="User Status" />
                </Col>
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
      <Authorize requires={Constants.PERMISSIONS.USER_W}>
        <Row>
          <Col>
            <Button
              size="sm"
              color="primary"
              className="float-right mb-3"
              onClick={() => setEditUserForm({ isOpen: true, formType: Constants.FORM_TYPE.ADD })}
            >
              Add User
            </Button>
          </Col>
        </Row>
      </Authorize>
      {searchResult.length > 0 && (
        <MaterialCard>
          <DataTableWithPaginaionControl
            dataList={searchResult}
            tableColumns={tableColumns}
            searchPagination={searchPagination}
            onPageNumberChange={handleChangePage}
            onPageSizeChange={handleChangePageSize}
            editable
            editPermissionName={Constants.PERMISSIONS.USER_W}
            onEditClicked={onEditClicked}
            onDeleteClicked={onDeleteClicked}
            onHeadingSortClicked={handleHeadingSortClicked}
          />
        </MaterialCard>
      )}
      {editUserForm.isOpen && <EditUserForm {...editUserForm} toggle={handleEditUserFormClose} />}
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    serviceAreas: Object.values(state.serviceAreas),
    userStatuses: Object.values(state.user.statuses),
    userTypes: Object.values(state.user.types),
    searchResult: Object.values(state.user.list),
    searchPagination: state.user.searchPagination,
  };
};

export default connect(mapStateToProps, { setSingleUserSeachCriteria, searchUsers, deleteUser })(UserAdmin);
