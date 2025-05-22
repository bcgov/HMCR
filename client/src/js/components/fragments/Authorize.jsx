import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';

const Authorize = ({ requires, children, currentUser, unauthorizedError }) => {
  const authorized = currentUser.permissions.includes(requires);

  return authorized ? children : unauthorizedError;
};

Authorize.propTypes = {
  currentUser: PropTypes.object.isRequired,
  children: PropTypes.node.isRequired,
  requires: PropTypes.string.isRequired,
  unauthorizedError: PropTypes.node,
};

Authorize.defaultProps = {
  unauthorizedError: <></>,
};

function mapStateToProps(state) {
  return {
    currentUser: state.user.current,
  };
}

export default connect(mapStateToProps)(Authorize);
