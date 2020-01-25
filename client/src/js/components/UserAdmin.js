import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button } from 'reactstrap';
import { Formik, Form, Field } from 'formik';
import queryString from 'query-string';

import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import MultiDropdown from './ui/MultiDropdown';
import EditUserForm from './forms/EditUserForm';
import AddUserWizard from './forms/AddUserWizard';
import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';
import SubmitButton from './ui/SubmitButton';
import PageSpinner from './ui/PageSpinner';
import useSearchData from './hooks/useSearchData';

import * as Constants from '../Constants';
import * as api from '../Api';
import { buildStatusIdArray } from '../utils';

const defaultSearchFormValues = {
  serviceAreaIds: [],
  userTypeIds: [],
  searchText: '',
  statusId: [Constants.ACTIVE_STATUS.ACTIVE],
};

const defaultSearchOptions = {
  searchText: '',
  isActive: true,
  serviceAreas: '',
  userTypes: '',
  dataPath: Constants.API_PATHS.USER,
};

const tableColumns = [
  { heading: 'User Type', key: 'userType' },
  { heading: 'First Name', key: 'firstName' },
  { heading: 'Last Name', key: 'lastName' },
  { heading: 'User ID', key: 'username' },
  { heading: 'Organization', key: 'businessLegalName' },
  { heading: 'Service Areas', key: 'serviceAreas', nosort: true, maxWidth: '100px' },
  { heading: 'Active', key: 'isActive', nosort: true },
];

const UserAdmin = ({ serviceAreas, userStatuses, userTypes, history }) => {
  const searchData = useSearchData(defaultSearchOptions, history);
  const [searchInitialValues, setSearchInitialValues] = useState(defaultSearchFormValues);

  const [editUserForm, setEditUserForm] = useState({ isOpen: false });
  const [addUserWizardIsOpen, setAddUserWizardIsOpen] = useState(false);

  // Run on load, parse URL query params
  useEffect(() => {
    const params = queryString.parse(history.location.search);

    const options = {
      ...defaultSearchOptions,
      ...params,
    };

    searchData.updateSearchOptions(options);

    const searchText = options.searchText || '';
    const serviceAreaIds = options.serviceAreas
      ? options.serviceAreas.split(',').map(id => parseInt(id))
      : defaultSearchFormValues.serviceAreaIds;
    const userTypeIds = options.userTypes ? options.userTypes.split(',') : defaultSearchFormValues.userTypeIds;

    setSearchInitialValues({
      ...searchInitialValues,
      searchText,
      statusId: buildStatusIdArray(options.isActive),
      serviceAreaIds,
      userTypeIds,
    });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSearchFormSubmit = values => {
    const serviceAreas = values.serviceAreaIds.join(',') || null;
    const userTypeIds = values.userTypeIds.join(',') || null;
    const searchText = values.searchText.trim() || null;

    let isActive = null;
    if (values.statusId.length === 1) {
      isActive = values.statusId[0] === 'ACTIVE';
    }

    const options = { ...searchData.searchOptions, isActive, searchText, serviceAreas, userTypes: userTypeIds };
    searchData.updateSearchOptions(options);
  };

  const handleSearchFormReset = () => {
    setSearchInitialValues(defaultSearchFormValues);
    searchData.refresh(true);
  };

  const onEditClicked = userId => {
    setEditUserForm({ isOpen: true, userId });
  };

  const onDeleteClicked = (userId, endDate) => {
    api.deleteUser(userId, endDate).then(() => searchData.refresh());
  };

  const handleEditUserFormClose = refresh => {
    if (refresh === true) {
      searchData.refresh();
    }
    setEditUserForm({ isOpen: false });
    setAddUserWizardIsOpen(false);
  };

  const data = Object.values(searchData.data).map(user => ({
    ...user,
    userType: userTypes.find(type => type.id === user.userType).name,
  }));

  return (
    <React.Fragment>
      <MaterialCard>
        <Formik
          initialValues={searchInitialValues}
          enableReinitialize={true}
          onSubmit={values => handleSearchFormSubmit(values)}
          onReset={handleSearchFormReset}
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
                  <MultiDropdown {...formikProps} items={userStatuses} name="statusId" title="User Status" />
                </Col>
                <Col>
                  <div className="float-right">
                    <SubmitButton className="mr-2" disabled={searchData.loading} submitting={searchData.loading}>
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
      {searchData.loading && <PageSpinner />}
      {!searchData.loading && (
        <MaterialCard>
          {data.length > 0 && (
            <DataTableWithPaginaionControl
              dataList={data}
              tableColumns={tableColumns}
              searchPagination={searchData.pagination}
              onPageNumberChange={searchData.handleChangePage}
              onPageSizeChange={searchData.handleChangePageSize}
              editable
              editPermissionName={Constants.PERMISSIONS.USER_W}
              onEditClicked={onEditClicked}
              onDeleteClicked={onDeleteClicked}
              onHeadingSortClicked={searchData.handleHeadingSortClicked}
            />
          )}
          {searchData.data.length <= 0 && <div>No records found</div>}
        </MaterialCard>
      )}
      {editUserForm.isOpen && <EditUserForm {...editUserForm} toggle={handleEditUserFormClose} />}
      {addUserWizardIsOpen && <AddUserWizard isOpen={addUserWizardIsOpen} toggle={handleEditUserFormClose} />}
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  const userTypes = Object.values(state.user.types);
  return {
    serviceAreas: Object.values(state.serviceAreas),
    userStatuses: Object.values(state.user.statuses),
    userTypes,
  };
};

export default connect(mapStateToProps)(UserAdmin);
