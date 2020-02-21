import React, { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { connect } from 'react-redux';
import { Row, Col, Button } from 'reactstrap';
import { Formik, Form, Field } from 'formik';
import queryString from 'query-string';

import Authorize from './fragments/Authorize';
import MaterialCard from './ui/MaterialCard';
import MultiDropdown from './ui/MultiDropdown';
import DataTableWithPaginaionControl from './ui/DataTableWithPaginaionControl';
import SubmitButton from './ui/SubmitButton';
import PageSpinner from './ui/PageSpinner';
import useSearchData from './hooks/useSearchData';
import useFormModal from './hooks/useFormModal';
import EditActivityFormFields from './forms/EditActivityFormFields';

import { showValidationErrorDialog } from '../actions';

import * as Constants from '../Constants';
import * as api from '../Api';
import { buildStatusIdArray } from '../utils';

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
  { heading: 'Point Line Feature', key: 'pointLineFeature' },
  { heading: 'Active', key: 'isActive', nosort: true },
];

const ActivityAdmin = ({ maintenanceTypes, locationCodes, unitOfMeasures, showValidationErrorDialog }) => {
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

  const onEditClicked = activityId => {
    formModal.openForm(Constants.FORM_TYPE.EDIT, { activityId });
  };

  const onDeleteClicked = (activityId, endDate, permanentDelete) => {
    if (permanentDelete) {
      api.deleteActivityCode(activityId).then(() => searchData.refresh());
    } else {
      api
        .getActivityCode(activityId)
        .then(response =>
          api.putActivityCode(activityId, { ...response.data, endDate }).then(() => searchData.refresh())
        );
    }
  };

  const handleEditFormSubmit = (values, formType) => {
    if (!formModal.submitting) {
      formModal.setSubmitting(true);

      if (values.locationCodeId !== locationCodes.find(location => location.name === 'C').id) {
        values.pointLineFeature = null;
        values.isSiteNumRequired = false;
      }

      if (formType === Constants.FORM_TYPE.ADD) {
        api
          .postActivityCode(values)
          .then(() => {
            formModal.closeForm();
            searchData.refresh();
          })
          .catch(error => showValidationErrorDialog(error.response.data.errors))
          .finally(() => formModal.setSubmitting(false));
      } else {
        api
          .putActivityCode(values.id, values)
          .then(() => {
            formModal.closeForm();
            searchData.refresh();
          })
          .catch(error => showValidationErrorDialog(error.response.data.errors))
          .finally(() => formModal.setSubmitting(false));
      }
    }
  };

  const formModal = useFormModal('Activity', <EditActivityFormFields />, handleEditFormSubmit);

  const data = searchData.data.map(item => ({
    ...item,
    locationCode: locationCodes.find(code => code.id === item.locationCodeId).name,
    maintenanceType: maintenanceTypes.find(type => type.id === item.maintenanceType).name,
    unitOfMeasures: unitOfMeasures.find(uom => uom.id === item.unitOfMeasure).name,
    canDelete: !item.isReferenced,
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
                  <Field type="text" name="searchText" placeholder="Activity Number/Name" className="form-control" />
                </Col>
                <Col>
                  <MultiDropdown
                    {...formikProps}
                    title="Maintenance Type"
                    items={maintenanceTypes}
                    name="maintenanceTypeIds"
                  />
                </Col>
                <Col>
                  <MultiDropdown
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
            <Button
              size="sm"
              color="primary"
              className="float-right mb-3"
              onClick={() => formModal.openForm(Constants.FORM_TYPE.ADD)}
            >
              Add Activity
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
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    maintenanceTypes: state.codeLookups.maintenanceTypes,
    locationCodes: state.codeLookups.locationCodes,
    unitOfMeasures: state.codeLookups.unitOfMeasures,
  };
};

export default connect(mapStateToProps, { showValidationErrorDialog })(ActivityAdmin);
