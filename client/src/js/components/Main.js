import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';

import Spinner from './ui/Spinner';
import ErrorDialogModal from './ui/ErrorDialogModal';

import { fetchCurrentUser, fetchServiceAreas, fetchUserStatuses, fetchUserTypes } from '../actions';

const Main = ({ errorDialog, children, fetchCurrentUser, fetchServiceAreas, fetchUserStatuses, fetchUserTypes }) => {
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([fetchServiceAreas(), fetchCurrentUser(), fetchUserStatuses(), fetchUserTypes()]).then(() =>
      setLoading(false)
    );
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <React.Fragment>
      {loading ? <Spinner /> : children}
      {errorDialog.show && <ErrorDialogModal isOpen={errorDialog.show} {...errorDialog} />}
    </React.Fragment>
  );
};

const mapStateToProps = state => {
  return {
    errorDialog: state.errorDialog,
  };
};

export default connect(
  mapStateToProps,
  { fetchCurrentUser, fetchServiceAreas, fetchUserStatuses, fetchUserTypes }
)(Main);
