import React, { useState, useEffect } from 'react';
import { connect } from 'react-redux';

import Spinner from './ui/Spinner';
import ErrorDialogModal from './ui/ErrorDialogModal';

const Main = ({ errorDialog, children }) => {
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(false);
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
  null
)(Main);
