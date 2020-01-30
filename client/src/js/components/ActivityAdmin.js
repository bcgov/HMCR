import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';
import { Row, Col, Button } from 'reactstrap';
import { Formik, Form, Field } from 'formik';
import queryString from 'query-string';

import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import MultiDropdown from './ui/MultiDropdown';
import EditActivityForm from './forms/EditActivityForm';
import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';
import SubmitButton from './ui/SubmitButton';
import PageSpinner from './ui/PageSpinner';
import useSearchData from './hooks/useSearchData';

import * as Constants from '../Constants';
import * as api from '../Api';
import { buildStatusIdArray } from '../utils';

const defaultSearchFormValues = { searchText: '', maintenanceTypeIds: [], statusId: [Constants.ACTIVE_STATUS.ACTIVE] };

const defaultSearchOptions = {
  searchText: '',
  maintenanceTypes: '',
  isActive: true,
  dataPath: Constants.API_PATHS.ACTIVITY,
};

const tableColumns = [
  { heading: 'Role Name', key: 'name' },
  { heading: 'Role Description', key: 'description' },
  { heading: 'Active', key: 'isActive', nosort: true },
];

const ActivityAdmin = ({ roleStatuses, history }) => {
  const searchData = useSearchData(defaultSearchOptions, history);
  const [editActivityForm, setEditActivityForm] = useState({ isOpen: false });
  const [searchInitialValues, setSearchInitialValues] = useState(defaultSearchFormValues);

  // Run on load, parse URL query params
  useEffect(() => {
    const params = queryString.parse(history.location.search);

    const options = {
      ...defaultSearchOptions,
      ...params,
    };

    const searchText = options.searchText || '';

    searchData.updateSearchOptions(options);
    setSearchInitialValues({ ...searchInitialValues, searchText, statusId: buildStatusIdArray(options.isActive) });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSearchFormSubmit = values => {
    const searchText = values.searchText.trim() || null;

    let isActive = null;
    if (values.statusId.length === 1) {
      isActive = values.statusId[0] === 'ACTIVE';
    }

    const options = { ...searchData.searchOptions, isActive, searchText };
    searchData.updateSearchOptions(options);
  };

  const handleSearchFormReset = () => {
    setSearchInitialValues(defaultSearchFormValues);
    searchData.refresh(true);
  };

  const onEditClicked = roleId => {
    setEditActivityForm({ isOpen: true, formType: Constants.FORM_TYPE.EDIT, roleId });
  };

  const onDeleteClicked = (roleId, endDate) => {
    api.deleteRole(roleId, endDate).then(() => searchData.refresh());
  };

  const handleEditRoleFormClose = refresh => {
    if (refresh === true) {
      searchData.refresh();
    }
    setEditActivityForm({ isOpen: false });
  };

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
                  <Field type="text" name="searchText" placeholder="Role/Description" className="form-control" />
                </Col>
                <Col>
                  <MultiDropdown {...formikProps} title="Role Status" items={roleStatuses} name="statusId" />
                </Col>
                <Col />
                <Col />
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
      <Authorize requires={Constants.PERMISSIONS.ROLE_W}>
        <Row>
          <Col>
            <Button
              size="sm"
              color="primary"
              className="float-right mb-3"
              onClick={() => setEditActivityForm({ isOpen: true, formType: Constants.FORM_TYPE.ADD })}
            >
              Add Role
            </Button>
          </Col>
        </Row>
      </Authorize>
      {searchData.loading && <PageSpinner />}
      {!searchData.loading && (
        <MaterialCard>
          {searchData.data.length > 0 && (
            <DataTableWithPaginaionControl
              dataList={searchData.data}
              tableColumns={tableColumns}
              searchPagination={searchData.pagination}
              onPageNumberChange={searchData.handleChangePage}
              onPageSizeChange={searchData.handleChangePageSize}
              editable
              editPermissionName={Constants.PERMISSIONS.ROLE_W}
              onEditClicked={onEditClicked}
              onDeleteClicked={onDeleteClicked}
              onHeadingSortClicked={searchData.handleHeadingSortClicked}
            />
          )}
          {searchData.data.length <= 0 && <div>No records found</div>}
        </MaterialCard>
      )}
      {editActivityForm.isOpen && <EditActivityForm {...editActivityForm} toggle={handleEditRoleFormClose} />}
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    roleStatuses: Object.values(state.roles.statuses),
  };
};

export default connect(mapStateToProps)(ActivityAdmin);
