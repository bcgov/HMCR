import { useState, useEffect } from 'react';
import { connect } from 'react-redux';

import {
  fetchCurrentUser,
  fetchServiceAreas,
  fetchUserStatuses,
  fetchUserTypes,
  fetchMaintenanceTypes,
  fetchUnitOfMeasures,
  fetchLocationCodes,
  fetchFeatureTypes,
  fetchSubmissionStatuses,
  fetchSubmissionStreams,
  fetchThresholdLevels,
  fetchRoadLengthRules,
  fetchSurfaceTypeRules,
  fetchRoadClassRules,
} from '../actions';
import ErrorDialogModal from './ui/ErrorDialogModal';
import PageSpinner from './ui/PageSpinner';

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
  fetchFeatureTypes,
  fetchSubmissionStatuses,
  fetchSubmissionStreams,
  fetchThresholdLevels,
  fetchRoadLengthRules,
  fetchSurfaceTypeRules,
  fetchRoadClassRules,
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
      fetchFeatureTypes(),
      fetchSubmissionStatuses(),
      fetchSubmissionStreams(),
      fetchThresholdLevels(),
      fetchRoadLengthRules(),
      fetchSurfaceTypeRules(),
      fetchRoadClassRules(),
    ]).then(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <>
      {loading ? <PageSpinner /> : children}
      {errorDialog.show && <ErrorDialogModal isOpen={errorDialog.show} {...errorDialog} />}
    </>
  );
};

const mapStateToProps = (state) => {
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
  fetchFeatureTypes,
  fetchSubmissionStatuses,
  fetchSubmissionStreams,
  fetchThresholdLevels,
  fetchRoadLengthRules,
  fetchSurfaceTypeRules,
  fetchRoadClassRules,
})(Main);
