import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';

const Authorize = ({ requires, children, currentUser }) => {
  const authorized = currentUser.permissions.includes(requires);

  return authorized ? children : <></>;
};

Authorize.propTypes = {
  currentUser: PropTypes.object.isRequired,
  children: PropTypes.node.isRequired,
  requires: PropTypes.string.isRequired,
};

function mapStateToProps(state) {
  return {
    currentUser: state.user.current,
  };
}

export default connect(mapStateToProps)(Authorize);
