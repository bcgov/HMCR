import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';

import PageSpinner from './ui/PageSpinner';
import ErrorDialogModal from './ui/ErrorDialogModal';

import {
  fetchCurrentUser,
  fetchServiceAreas,
  fetchUserStatuses,
  fetchUserTypes,
  fetchMaintenanceTypes,
  fetchUnitOfMeasures,
  fetchLocationCodes,
  fetchPointLineFeatures,
} from '../actions';

const Main = ({
  errorDialog,
  children,
  fetchCurrentUser,
  fetchServiceAreas,
  fetchUserStatuses,
  fetchUserTypes,
  fetchMaintenanceTypes,
  fetchUnitOfMeasures,
  fetchLocationCodes,
  fetchPointLineFeatures,
}) => {
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      fetchServiceAreas(),
      fetchCurrentUser(),
      fetchUserStatuses(),
      fetchUserTypes(),
      fetchMaintenanceTypes(),
      fetchUnitOfMeasures(),
      fetchLocationCodes(),
      fetchPointLineFeatures(),
    ]).then(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <React.Fragment>
      {loading ? <PageSpinner /> : children}
      {errorDialog.show && <ErrorDialogModal isOpen={errorDialog.show} {...errorDialog} />}
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    errorDialog: state.errorDialog,
  };
};

export default connect(mapStateToProps, {
  fetchCurrentUser,
  fetchServiceAreas,
  fetchUserStatuses,
  fetchUserTypes,
  fetchMaintenanceTypes,
  fetchUnitOfMeasures,
  fetchLocationCodes,
  fetchPointLineFeatures,
})(Main);
