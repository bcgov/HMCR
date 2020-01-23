import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button } from 'reactstrap';
import { Formik, Form, Field } from 'formik';

import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import MultiDropdown from './ui/MultiDropdown';
import EditUserForm from './forms/EditUserForm';
import AddUserWizard from './forms/AddUserWizard';
import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';
import SubmitButton from './ui/SubmitButton';
import PageSpinner from './ui/PageSpinner';

import { setSingleUserSeachCriteria, searchUsers, deleteUser } from '../actions';

import * as Constants from '../Constants';

const defaultSearchFormValues = { serviceAreaIds: [], userTypeIds: [], searchText: '', userStatusIds: ['ACTIVE'] };
const tableColumns = [
  { heading: 'User Type', key: 'userType' },
  { heading: 'First Name', key: 'firstName' },
  { heading: 'Last Name', key: 'lastName' },
  { heading: 'User ID', key: 'username' },
  { heading: 'Organization', key: 'businessLegalName' },
  { heading: 'Service Areas', key: 'serviceAreas', nosort: true, maxWidth: '100px' },
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
  const [addUserWizardIsOpen, setAddUserWizardIsOpen] = useState(false);
  const [searching, setSearching] = useState(false);

  useEffect(() => {
    const search = async () => {
      setSearching(true);
      await searchUsers();
      setSearching(false);
    };

    search();
  }, [searchUsers]);

  const startSearch = async () => {
    setSearching(true);
    await searchUsers();
    setSearching(false);
  };

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

    startSearch();
  };

  const onEditClicked = userId => {
    setEditUserForm({ isOpen: true, userId });
  };

  const onDeleteClicked = (userId, endDate) => {
    deleteUser(userId, endDate).then(() => searchUsers());
  };

  const handleEditUserFormClose = refresh => {
    if (refresh === true) {
      startSearch();
    }
    setEditUserForm({ isOpen: false });
    setAddUserWizardIsOpen(false);
  };

  const handleChangePage = newPage => {
    setSingleUserSeachCriteria('pageNumber', newPage);
    startSearch();
  };

  const handleChangePageSize = newSize => {
    setSingleUserSeachCriteria('pageSize', newSize);
    setSingleUserSeachCriteria('pageNumber', 1);
    startSearch();
  };

  const handleHeadingSortClicked = headingKey => {
    setSingleUserSeachCriteria('pageNumber', 1);
    setSingleUserSeachCriteria('orderBy', headingKey);
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
                  <Field
                    type="text"
                    name="searchText"
                    placeholder="User Id/Name/Organization"
                    className="form-control"
                  />
                </Col>
                <Col>
                  <MultiDropdown {...formikProps} items={serviceAreas} name="serviceAreaIds" title="Service Area" />
                </Col>
                <Col>
                  <MultiDropdown {...formikProps} items={userTypes} name="userTypeIds" title="User Type" />
                </Col>
                <Col>
                  <MultiDropdown {...formikProps} items={userStatuses} name="userStatusIds" title="User Status" />
                </Col>
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
      <Authorize requires={Constants.PERMISSIONS.USER_W}>
        <Row>
          <Col>
            <Button size="sm" color="primary" className="float-right mb-3" onClick={() => setAddUserWizardIsOpen(true)}>
              Add User
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
            editPermissionName={Constants.PERMISSIONS.USER_W}
            onEditClicked={onEditClicked}
            onDeleteClicked={onDeleteClicked}
            onHeadingSortClicked={handleHeadingSortClicked}
          />
        </MaterialCard>
      )}
      {editUserForm.isOpen && <EditUserForm {...editUserForm} toggle={handleEditUserFormClose} />}
      {addUserWizardIsOpen && <AddUserWizard isOpen={addUserWizardIsOpen} toggle={handleEditUserFormClose} />}
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  const userTypes = Object.values(state.user.types);
  const userList = Object.values(state.user.list).map(user => ({
    ...user,
    userType: state.user.types[user.userType].name,
  }));
  return {
    serviceAreas: Object.values(state.serviceAreas),
    userStatuses: Object.values(state.user.statuses),
    userTypes,
    searchResult: userList,
    searchPagination: state.user.searchPagination,
  };
};

export default connect(mapStateToProps, { setSingleUserSeachCriteria, searchUsers, deleteUser })(UserAdmin);
