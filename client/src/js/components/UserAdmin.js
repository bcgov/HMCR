import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Table } from 'reactstrap';
import { Formik, Form, Field } from 'formik';

import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import MultiDropdown from './ui/MultiDropdown';
import FontAwesomeButton from './ui/FontAwesomeButton';
import EditUserForm from './forms/EditUserForm';
import DeleteButton from './ui/DeleteButton';
import PaginationControl from './ui/PaginationControl';

import { setSingleUserSeachCriteria, searchUsers } from '../actions';

import * as Constants from '../Constants';

const renderUserTable = (
  userList,
  setEditUserForm,
  refreshData,
  searchPagination,
  handleChangePage,
  handleChangePageSize
) => {
  return (
    <MaterialCard>
      <Table size="sm" responsive>
        <thead className="thead-dark">
          <tr>
            <th>ID Type</th>
            <th>Last Name</th>
            <th>Fist Name</th>
            <th>User ID</th>
            <th>Organization</th>
            <th>Service Areas</th>
            <Authorize requires={Constants.PERMISSIONS.USER_W}>
              <th></th>
            </Authorize>
          </tr>
        </thead>
        <tbody>
          {userList.map(user => {
            return (
              <tr key={user.id}>
                <td>{user.userType}</td>
                <td>{user.lastName}</td>
                <td>{user.firstName}</td>
                <td>{user.username}</td>
                <td>{user.businessLegalName}</td>
                <td>{user.serviceAreas}</td>
                <Authorize requires={Constants.PERMISSIONS.USER_W}>
                  <td style={{ width: '1%', whiteSpace: 'nowrap' }}>
                    <FontAwesomeButton
                      icon="edit"
                      className="mr-1"
                      onClick={() =>
                        setEditUserForm({ isOpen: true, formType: Constants.FORM_TYPE.EDIT, userId: user.id })
                      }
                    />
                    <DeleteButton
                      id={`user_${user.id}_delete`}
                      userId={user.id}
                      endDate={user.endDate}
                      onComplete={refreshData}
                    ></DeleteButton>
                  </td>
                </Authorize>
              </tr>
            );
          })}
        </tbody>
      </Table>
      <PaginationControl
        currentPage={searchPagination.pageNumber}
        pageCount={searchPagination.pageCount}
        onPageChange={handleChangePage}
        pageSize={searchPagination.pageSize}
        onPageSizeChange={handleChangePageSize}
      />
    </MaterialCard>
  );
};

const defaultSearchFormValues = { serviceAreaIds: [], userTypeIds: [], searchText: '', useStatusIds: [] };

const UserAdmin = ({
  serviceAreas,
  userStatuses,
  userTypes,
  setSingleUserSeachCriteria,
  searchUsers,
  searchResult,
  searchPagination,
}) => {
  // const [displayPage, setDisplayPage] = useState(1);
  // const [displayPageSize, setDisplayPageSize] = useState(25);
  // const [displayOrderBy, setDisplayOrderBy] = useState(null);

  const [editUserForm, setEditUserForm] = useState({ isOpen: false });

  // Temporary...
  const refreshData = () => {
    searchUsers();
  };

  useEffect(() => {
    refreshData();
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
    if (values.useStatusIds.length === 1) {
      isActive = values.useStatusIds[0] === 'ACTIVE';
    }
    setSingleUserSeachCriteria('isActive', isActive);

    searchUsers();
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
                  <MultiDropdown
                    {...formikProps}
                    items={serviceAreas}
                    name="serviceAreaIds"
                    title="Service Area"
                    showId={true}
                  />
                </Col>

                <Col>
                  <MultiDropdown {...formikProps} items={userTypes} name="userTypeIds" title="User Type" />
                </Col>
                <Col>
                  <Field type="text" name="searchText" placeholder="User Id/Name" className="form-control" />
                </Col>
                <Col>
                  <MultiDropdown {...formikProps} items={userStatuses} name="useStatusIds" title="User Status" />
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

      {searchResult.length > 0 &&
        renderUserTable(
          searchResult,
          setEditUserForm,
          refreshData,
          searchPagination,
          handleChangePage,
          handleChangePageSize
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
    searchResult: Object.values(state.user.searchResult),
    searchPagination: state.user.searchPagination,
  };
};

export default connect(mapStateToProps, { setSingleUserSeachCriteria, searchUsers })(UserAdmin);
