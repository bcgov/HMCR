import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';

import PageSpinner from './ui/PageSpinner';
import ErrorDialogModal from './ui/ErrorDialogModal';

import { fetchCurrentUser, fetchServiceAreas, fetchUserStatuses, fetchUserTypes } from '../actions';
import { ErrorDialogContext } from '../contexts';

const Main = ({ errorDialog, children, fetchCurrentUser, fetchServiceAreas, fetchUserStatuses, fetchUserTypes }) => {
  const [loading, setLoading] = useState(true);

  const showLocalErrorDialog = error => {};

  useEffect(() => {
    Promise.all([fetchServiceAreas(), fetchCurrentUser(), fetchUserStatuses(), fetchUserTypes()]).then(() =>
      setLoading(false)
    );
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <ErrorDialogContext.Provider value={showLocalErrorDialog}>
      {loading ? <PageSpinner /> : children}
      {errorDialog.show && <ErrorDialogModal isOpen={errorDialog.show} {...errorDialog} />}
    </ErrorDialogContext.Provider>
  );
};

const mapStateToProps = state => {
  return {
    errorDialog: state.errorDialog,
  };
};

export default connect(mapStateToProps, { fetchCurrentUser, fetchServiceAreas, fetchUserStatuses, fetchUserTypes })(
  Main
);
