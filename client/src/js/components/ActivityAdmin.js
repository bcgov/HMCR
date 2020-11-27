import React, { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { connect } from 'react-redux';
import { Row, Col, Button, Alert, Spinner } from 'reactstrap';
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
import EditActivityFormFields from './forms/EditActivityFormFields';

import SimpleModalWrapper from './ui/SimpleModalWrapper';
import { showValidationErrorDialog } from '../actions';
import FileSaver from 'file-saver';

import * as Constants from '../Constants';
import * as api from '../Api';
import { buildStatusIdArray,isValueNotEmpty,toNumberOrNull,toStringOrEmpty,toStringWithCommasOrEmpty,isValueEmpty } from '../utils';

const defaultSearchFormValues = { searchText: '', maintenanceTypeIds: [], statusId: [Constants.ACTIVE_STATUS.ACTIVE] };

const defaultSearchOptions = {
  searchText: '',
  maintenanceTypes: '',
  isActive: true,
  dataPath: Constants.API_PATHS.ACTIVITY_CODES,
};

const tableColumns = [
  { heading: 'Activity Number', key: 'activityNumber' },
  { heading: 'Name', key: 'activityName' },
  { heading: 'Unit', key: 'unitOfMeasure' },
  { heading: 'Maintenance Type', key: 'maintenanceType' },
  { heading: 'Location Code', key: 'locationCode' },
  { heading: 'Feature Type', key: 'featureType' },
  { heading: 'Active', key: 'isActive', nosort: true },
];

const EXPORT_STAGE = {
  WAIT: 'WAIT',
  ERROR: 'ERROR',
  NOT_FOUND: 'NOT_FOUND',
  DONE: 'DONE',
};

const ActivityAdmin = ({ maintenanceTypes, locationCodes, unitOfMeasures,showValidationErrorDialog, hideErrorDialog }) => {
  const location = useLocation();
  const searchData = useSearchData(defaultSearchOptions);
  const [searchInitialValues, setSearchInitialValues] = useState(defaultSearchFormValues);
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

    const searchText = options.searchText || '';

    searchData.updateSearchOptions(options);
    setSearchInitialValues({ ...searchInitialValues, searchText, statusId: buildStatusIdArray(options.isActive) });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSearchFormSubmit = (values) => {
    const searchText = values.searchText.trim() || null;
    const maintenanceTypes = values.maintenanceTypeIds.join(',') || null;

    let isActive = null;
    if (values.statusId.length === 1) {
      isActive = values.statusId[0] === 'ACTIVE';
    }

    const options = { ...searchData.searchOptions, isActive, searchText, maintenanceTypes };
    searchData.updateSearchOptions(options);
  };

  const handleSearchFormReset = () => {
    setSearchInitialValues(defaultSearchFormValues);
    searchData.refresh(true);
  };

  const buildExportParams = () => {
    console.log(searchData);

    const searchText = searchData.searchOptions.searchText;
    const maintenanceTypes = searchData.searchOptions.maintenanceTypes;
    const isActive = searchData.searchOptions.isActive;

    const options = { ...searchData.searchOptions, isActive, searchText, maintenanceTypes };
    return options;
  };

  const onEditClicked = (activityId) => {
    formModal.openForm(Constants.FORM_TYPE.EDIT, { activityId });
  };

  const onDeleteClicked = (activityId, endDate, permanentDelete) => {
    if (permanentDelete) {
      api.deleteActivityCode(activityId).then(() => searchData.refresh());
    } else {
      api
        .getActivityCode(activityId)
        .then((response) =>
          api.putActivityCode(activityId, { ...response.data, endDate }).then(() => searchData.refresh())
        );
    }
  };

  const handleEditFormSubmit = (values, formType) => {
    if (!formModal.submitting) {
      formModal.setSubmitting(true);

      if (values.locationCodeId !== locationCodes.find((location) => location.name === 'C').id) {
        values.featureType = null;
        values.isSiteNumRequired = false;
        values.thresholdLevels = null;
        values.roadClassRule = 0;
        values.roadLengthRule = 0;
        values.surfaceTypeRule = 0;
      }
      values.minValue = toNumberOrNull(values.minValue);
      values.maxValue = toNumberOrNull(values.maxValue);
      values.reportingFrequency = toNumberOrNull(values.reportingFrequency );
      if(isValueNotEmpty(values.minValue) && isValueEmpty(values.maxValue))
      {
        values.maxValue = (['site','num','ea'].includes(values.unitOfMeasure)) ? 999999999:999999999.99;
      }
      if (formType === Constants.FORM_TYPE.ADD) {
        api
          .postActivityCode(values)
          .then(() => {
            values.minValue = toStringWithCommasOrEmpty(values.minValue);
            values.maxValue = toStringWithCommasOrEmpty(values.maxValue);
            values.reportingFrequency = toStringOrEmpty(values.reportingFrequency );
            formModal.closeForm();
            searchData.refresh();
          })
          .catch((error) => showValidationErrorDialog(error.response.data))
          .finally(() => formModal.setSubmitting(false));
      } else {
        api
          .putActivityCode(values.id, values)
          .then(() => {
            values.minValue = toStringWithCommasOrEmpty(values.minValue);
            values.maxValue = toStringWithCommasOrEmpty(values.maxValue);
            values.reportingFrequency = toStringOrEmpty(values.reportingFrequency );
            formModal.closeForm();
            searchData.refresh();
          })
          .catch((error) => showValidationErrorDialog(error.response.data))
          .finally(() => formModal.setSubmitting(false));
      }
    }
  };

  const submitExport = (values) => {
    setExporting(true);
    setShowModal(true);
    setExportStage(EXPORT_STAGE.WAIT);
    api
      .getActivityCodeExport(buildExportParams(values))
      .then((response) => {
        const fileExtensionHeaders = response.headers['content-disposition'].match(/.csv/i);
        
        let fileName = `activitycode_export`;
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

  const formModal = useFormModal('Activity', <EditActivityFormFields />, handleEditFormSubmit,'xl');

  const data = searchData.data.map((item) => ({
    ...item,
    locationCode: locationCodes.find((code) => code.id === item.locationCodeId).name,
    maintenanceType: maintenanceTypes.find((type) => type.id === item.maintenanceType).name,
    unitOfMeasures: unitOfMeasures.find((uom) => uom.id === item.unitOfMeasure).name,
    canDelete: !item.isReferenced,
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
        <UIHeader>Activity Number Management</UIHeader>
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
                  <Field type="text" name="searchText" placeholder="Activity Number/Name" className="form-control" />
                </Col>
                <Col>
                  <MultiDropdownField
                    {...formikProps}
                    title="Maintenance Type"
                    items={maintenanceTypes}
                    name="maintenanceTypeIds"
                  />
                </Col>
                <Col>
                  <MultiDropdownField
                    {...formikProps}
                    title="Activity Status"
                    items={Constants.ACTIVE_STATUS_ARRAY}
                    name="statusId"
                  />
                </Col>
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
      <Authorize requires={Constants.PERMISSIONS.CODE_W}>
        <Row>
          <Col>
            <div className="float-right mb-3">
              <Button
                size="sm"
                color="primary"
                className="mr-2"
                onClick={() => formModal.openForm(Constants.FORM_TYPE.ADD)}
              >
                Add Activity
              </Button>
              <Button size="sm" color="primary" onClick={(values) => submitExport(searchData)}>
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
              editPermissionName={Constants.PERMISSIONS.CODE_W}
              onEditClicked={onEditClicked}
              onDeleteClicked={onDeleteClicked}
              onHeadingSortClicked={searchData.handleHeadingSortClicked}
            />
          )}
          {data.length <= 0 && <div>No records found</div>}
        </MaterialCard>
      )}
      {formModal.formElement}
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
  return {
    maintenanceTypes: state.codeLookups.maintenanceTypes,
    locationCodes: state.codeLookups.locationCodes,
    unitOfMeasures: state.codeLookups.unitOfMeasures,
  };
};

export default connect(mapStateToProps, { showValidationErrorDialog })(ActivityAdmin);
