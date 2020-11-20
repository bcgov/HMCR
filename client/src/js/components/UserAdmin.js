import React, { useState, useEffect } from 'react';

import { useLocation } from 'react-router-dom';
import { connect } from 'react-redux';
import { Row, Col, Button,Alert, Spinner } from 'reactstrap';
import { Formik, Form, Field } from 'formik';
import queryString from 'query-string';
import * as Yup from 'yup';

import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import UIHeader from './ui/UIHeader';
import MultiDropdownField from './ui/MultiDropdownField';
import AddUserWizard from './forms/AddUserWizard';
import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';
import SubmitButton from './ui/SubmitButton';
import PageSpinner from './ui/PageSpinner';
import useSearchData from './hooks/useSearchData';
import useFormModal from './hooks/useFormModal';
import EditUserFormFields from './forms/EditUserFormFields';
import SimpleModalWrapper from './ui/SimpleModalWrapper';
import { showValidationErrorDialog,hideErrorDialog } from '../actions';
import FileSaver from 'file-saver';
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

const validationSchema = Yup.object({
  userType: Yup.string().required('Required').max(30).trim(),
  username: Yup.string().required('Required').max(32).trim(),
  userRoleIds: Yup.array().required('Require at least one role'),
  serviceAreaNumbers: Yup.array().when('userType', {
    is: Constants.USER_TYPE.BUSINESS,
    then: Yup.array().required('Required'),
  }),
});

const EXPORT_STAGE = {
  WAIT: 'WAIT',
  ERROR: 'ERROR',
  NOT_FOUND: 'NOT_FOUND',
  DONE: 'DONE',
};

const UserAdmin = ({ serviceAreas, userStatuses, userTypes, showValidationErrorDialog,hideErrorDialog }) => {
  const location = useLocation();
  const searchData = useSearchData(defaultSearchOptions);
  const [searchInitialValues, setSearchInitialValues] = useState(defaultSearchFormValues);
  const [addUserWizardIsOpen, setAddUserWizardIsOpen] = useState(false);
  const [exporting, setExporting] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [exportStage, setExportStage] = useState(EXPORT_STAGE.WAIT);
  const [exportResult, setExportResult] = useState({});

  // Run on load, parse URL query params
  useEffect(() => {
    const params = queryString.parse(location.search);

    const options = {
      ...defaultSearchOptions,
      ...params,
    };

    searchData.updateSearchOptions(options);

    const searchText = options.searchText || '';
    const serviceAreaIds = options.serviceAreas
      ? options.serviceAreas.split(',').map((id) => parseInt(id))
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

  const handleSearchFormSubmit = (values) => {
    const serviceAreas = values.serviceAreaIds.join(',') || null;
    const userTypeIds = values.userTypeIds.join(',') || null;
    const searchText = values.searchText.trim() || null;

    let isActive = null;
    if (values.statusId.length === 1) {
      isActive = values.statusId[0] === 'ACTIVE';
    }

    const options = {
      ...searchData.searchOptions,
      isActive,
      searchText,
      serviceAreas,
      userTypes: userTypeIds,
      pageNumber: 1,
    };
    searchData.updateSearchOptions(options);
  };

  const handleSearchFormReset = () => {
    setSearchInitialValues(defaultSearchFormValues);
    searchData.refresh(true);
  };

  const buildExportParams = (values) => {
    const serviceAreas = searchData.searchOptions.serviceAreas;
    const userTypeIds = searchData.searchOptions.userTypes;
    const searchText = searchData.searchOptions.searchText;
    const isActive = searchData.searchOptions.isActive;
    const options = {
      ...searchData.searchOptions,
      isActive,
      searchText,
      serviceAreas,
      userTypes: userTypeIds,
    };
    return options;
  };

  const onEditClicked = (userId) => {
    formModal.openForm(Constants.FORM_TYPE.EDIT, { userId });
  };

  const onDeleteClicked = (userId, endDate) => {
    api.deleteUser(userId, endDate).then(() => searchData.refresh());
  };

  const handleAddUserWizardClose = (refresh) => {
    if (refresh === true) {
      searchData.refresh();
    }

    setAddUserWizardIsOpen(false);
  };

  const handleEditFormSubmit = (values) => {
    if (!formModal.submitting) {
      formModal.setSubmitting(true);

      api
        .putUser(values.id, values)
        .then(() => {
          formModal.closeForm();
          searchData.refresh();
        })
        .catch((error) => showValidationErrorDialog(error.response.data))
        .finally(() => formModal.setSubmitting(false));
    }
  };
  
  const submitExport = (values) => {
    setExporting(true);
    setShowModal(true);
    setExportStage(EXPORT_STAGE.WAIT);
    api
      .getUserReportExport(buildExportParams(values))
      .then((response) => {
        const fileExtensionHeaders = response.headers['content-disposition'].match(/.csv/i);

        let fileName = `${values.reportTypeId}_Export_`;
        if (fileExtensionHeaders) fileName += fileExtensionHeaders[0];

        let data = response.data;
        if (fileName.indexOf('.json') > -1) data = JSON.stringify(data);

        FileSaver.saveAs(new Blob([data]), fileName);

        setExportResult({ fileName });
        setExportStage(EXPORT_STAGE.DONE);
      })
      .catch((error) => {
        if (error.response) {
          const response = error.response;

          if (response.status === 422) {
            setExportResult({ error: error.response.data });
            setExportStage(EXPORT_STAGE.ERROR);
          } else if (response.status === 404) {
            hideErrorDialog();
            setExportStage(EXPORT_STAGE.NOT_FOUND);
          }
        }
      })
      .finally(() => setExporting(false));
  };
  const formModal = useFormModal(
    'User',
    <EditUserFormFields validationSchema={validationSchema} />,
    handleEditFormSubmit
  );

  const data = Object.values(searchData.data).map((user) => ({
    ...user,
    userType: userTypes.find((type) => type.id === user.userType).name,
  }));

  const renderContent = () => {
    switch (exportStage) {
      case EXPORT_STAGE.NOT_FOUND:
        return (
          <Alert color="warning">
            <p>
              <strong>No Results Found</strong>
            </p>
            <p>There are no results matching the provided search criterion</p>
          </Alert>
        );
      case EXPORT_STAGE.ERROR:
        return (
          <Alert color="danger">
            <p>
              <strong>{exportResult.error.title}</strong>
            </p>
            <p>{exportResult.error.detail}</p>
          </Alert>
        );
      case EXPORT_STAGE.DONE:
        return (
          <Alert color="success">
            <p>
              <strong>Export Complete</strong>
            </p>
            <p>Your report has been saved to your computer.</p>
            <p>
              <small>{exportResult.fileName}</small>
            </p>
          </Alert>
        );
      default:
        return (
          <div className="text-center">
            <Spinner color="primary" />
            <div className="mt-2">
              <div>Your report is being generated.</div>
              <div>This may take a few minutes.</div>
            </div>
          </div>
        );
    }
  };

  return (
    <React.Fragment>
      <MaterialCard>
        <UIHeader>User Management</UIHeader>
        <Formik
          initialValues={searchInitialValues}
          enableReinitialize={true}
          onSubmit={(values) => handleSearchFormSubmit(values)}
          onReset={handleSearchFormReset}
        >
          {(formikProps) => (
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
                  <MultiDropdownField
                    {...formikProps}
                    items={serviceAreas}
                    name="serviceAreaIds"
                    title="Service Area"
                    searchable={true}
                  />
                </Col>
                <Col>
                  <MultiDropdownField {...formikProps} items={userTypes} name="userTypeIds" title="User Type" />
                </Col>
                <Col>
                  <MultiDropdownField {...formikProps} items={userStatuses} name="statusId" title="User Status" />
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
            <div className="float-right mb-3">
              <Button size="sm" color="primary" className="mr-2" onClick={() => setAddUserWizardIsOpen(true)}>
                Add User
              </Button>
              <Button size="sm" color="primary" onClick={(values) => submitExport(values)}>
                Export
              </Button>
            </div>
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
      {formModal.formElement}
      {addUserWizardIsOpen && (
        <AddUserWizard
          isOpen={addUserWizardIsOpen}
          toggle={handleAddUserWizardClose}
          validationSchema={validationSchema}
        />
      )}
      <SimpleModalWrapper
        isOpen={showModal}
        toggle={() => {
          if (!exporting) setShowModal(false);
        }}
        backdrop="static"
        title="Generating Report"
        disableClose={exporting}
      >
        {renderContent()}
      </SimpleModalWrapper>
    </React.Fragment>
  );
};

const mapStateToProps = (state) => {
  const userTypes = Object.values(state.user.types);
  return {
    serviceAreas: Object.values(state.serviceAreas),
    userStatuses: Object.values(state.user.statuses),
    userTypes,
  };
};

export default connect(mapStateToProps, { showValidationErrorDialog, hideErrorDialog  })(UserAdmin);
