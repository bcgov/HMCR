import React, { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { connect } from 'react-redux';
import { Row, Col, Button } from 'reactstrap';
import { Formik, Form, Field } from 'formik';
import queryString from 'query-string';

import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';
import MultiDropdownField from './ui/MultiDropdownField';
import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';
import SubmitButton from './ui/SubmitButton';
import PageSpinner from './ui/PageSpinner';
import useSearchData from './hooks/useSearchData';
import useFormModal from './hooks/useFormModal';
import EditRoleFormFields from './forms/EditRoleFormFields';

import { showValidationErrorDialog } from '../actions';

import * as Constants from '../Constants';
import * as api from '../Api';
import { buildStatusIdArray } from '../utils';

const defaultSearchFormValues = { searchText: '', statusId: [Constants.ACTIVE_STATUS.ACTIVE] };

const defaultSearchOptions = {
  searchText: '',
  isActive: true,
  dataPath: Constants.API_PATHS.ROLE,
};

const tableColumns = [
  { heading: 'Role Name', key: 'name' },
  { heading: 'Role Description', key: 'description' },
  { heading: 'Active', key: 'isActive', nosort: true },
];

const RoleAdmin = ({ showValidationErrorDialog }) => {
  const location = useLocation();
  const searchData = useSearchData(defaultSearchOptions);
  const [searchInitialValues, setSearchInitialValues] = useState(defaultSearchFormValues);

  // Run on load, parse URL query params
  useEffect(() => {
    const params = queryString.parse(location.search);

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

    const options = { ...searchData.searchOptions, isActive, searchText, pageNumber: 1 };
    searchData.updateSearchOptions(options);
  };

  const handleSearchFormReset = () => {
    setSearchInitialValues(defaultSearchFormValues);
    searchData.refresh(true);
  };

  const onEditClicked = roleId => {
    formModal.openForm(Constants.FORM_TYPE.EDIT, { roleId });
  };

  const onDeleteClicked = (roleId, endDate) => {
    api.deleteRole(roleId, endDate).then(() => searchData.refresh());
  };

  const handleEditFormSubmit = (values, formType) => {
    if (!formModal.submitting) {
      formModal.setSubmitting(true);

      if (formType === Constants.FORM_TYPE.ADD) {
        api
          .postRole(values)
          .then(() => {
            formModal.closeForm();
            searchData.refresh();
          })
          .catch(error => showValidationErrorDialog(error.response.data.errors))
          .finally(() => formModal.setSubmitting(false));
      } else {
        api
          .putRole(values.id, values)
          .then(() => {
            formModal.closeForm();
            searchData.refresh();
          })
          .catch(error => showValidationErrorDialog(error.response.data.errors))
          .finally(() => formModal.setSubmitting(false));
      }
    }
  };

  const formModal = useFormModal('Role', <EditRoleFormFields />, handleEditFormSubmit);

  return (
    <React.Fragment>
      <MaterialCard>
        <UIHeader>Role and Permissions Management</UIHeader>
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
                  <MultiDropdownField
                    {...formikProps}
                    title="Role Status"
                    items={Constants.ACTIVE_STATUS_ARRAY}
                    name="statusId"
                  />
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
              onClick={() => formModal.openForm(Constants.FORM_TYPE.ADD)}
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
      {formModal.formElement}
    </React.Fragment>
  );
};

export default connect(null, { showValidationErrorDialog })(RoleAdmin);
