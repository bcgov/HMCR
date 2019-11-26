import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button, Table } from 'reactstrap';
import { Formik, Form, Field } from 'formik';

import MaterialCard from './ui/MaterialCard';
import MultiDropdown from './ui/MultiDropdown';
import FontAwesomeButton from './ui/FontAwesomeButton';
import EditUserForm from './forms/EditUserForm';
import DeleteButton from './ui/DeleteButton';

import * as api from '../Api';
import * as Constants from '../Constants';

const renderUserTable = (userList, setEditUserForm, refreshData) => {
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
            <th></th>
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
                    refreshData={refreshData}
                  ></DeleteButton>
                </td>
              </tr>
            );
          })}
        </tbody>
      </Table>
    </MaterialCard>
  );
};

const initialValues = { serviceAreaIds: [], userTypeIds: [], searchText: '', useStatusIds: [] };

const UserAdmin = ({ serviceAreas, userStatuses, userTypes }) => {
  // const [displayPage, setDisplayPage] = useState(1);
  // const [displayPageSize, setDisplayPageSize] = useState(25);
  // const [displayOrderBy, setDisplayOrderBy] = useState(null);

  // eslint-disable-next-line
  const [searchServiceAreas, setSearchServiceAreas] = useState(null);
  // eslint-disable-next-line
  const [searchUserTypes, setSearchUserTypes] = useState(null);
  // eslint-disable-next-line
  const [searchUserStatus, setSearchUserStatus] = useState(null);
  // eslint-disable-next-line
  const [searchUserText, setSearchUserText] = useState(null);

  const [userList, setUserList] = useState([]);

  const [editUserForm, setEditUserForm] = useState({ isOpen: false });

  // Temporary...
  const refreshData = () => {
    api.instance.get(Constants.API_PATHS.USER).then(response => {
      setUserList(response.data.sourceList);
    });
  };

  useEffect(() => {
    refreshData();
  }, []);

  const handleSearchFormSubmit = (values, setSubmitting) => {
    const serviceAreas = values.serviceAreaIds.join(',') || null;
    setSearchServiceAreas(serviceAreas);

    const userTypeIds = values.userTypeIds.join(',') || null;
    setSearchUserTypes(userTypeIds);

    const searchText = values.searchText.trim() || null;
    setSearchUserText(searchText);

    let isActive = null;
    if (values.useStatusIds.length === 1) {
      isActive = values.useStatusIds[0] === 'ACTIVE';
    }
    setSearchUserStatus(isActive);

    api.instance
      .get(Constants.API_PATHS.USER, {
        params: {
          serviceAreas: serviceAreas,
          userTypes: userTypeIds,
          searchText,
          isActive,
          // pageSize: displayPageSize,
          // pageNumber: displayPage,
          // orderBy: displayOrderBy,
        },
      })
      .then(response => {
        setUserList(response.data.sourceList);
      })
      .finally(() => setSubmitting(false));
  };

  const handleEditUserFormClose = refresh => {
    if (refresh) {
      refreshData();
    }
    setEditUserForm({ isOpen: false });
  };

  return (
    <React.Fragment>
      <MaterialCard>
        <Formik
          initialValues={initialValues}
          onSubmit={(values, { setSubmitting }) => {
            handleSearchFormSubmit(values, setSubmitting);
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

      {userList.length > 0 && renderUserTable(userList, setEditUserForm, refreshData)}
      {editUserForm.isOpen && <EditUserForm {...editUserForm} toggle={handleEditUserFormClose} />}
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    serviceAreas: Object.values(state.serviceAreas),
    userStatuses: Object.values(state.user.statuses),
    userTypes: Object.values(state.user.types),
  };
};

export default connect(mapStateToProps, null)(UserAdmin);
