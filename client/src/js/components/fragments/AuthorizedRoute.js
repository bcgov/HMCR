import React from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { Route, Redirect } from 'react-router-dom';

import * as Constant from '../../Constants';

const AuthorizedRoute = ({ children, currentUser, requires, userType, ...rest }) => {
  return (
    <Route
      {...rest}
      render={({ location }) =>
        currentUser.permissions.includes(requires) && currentUser.userType === userType ? (
          children
        ) : (
          <Redirect
            to={{
              pathname: Constant.PATHS.UNAUTHORIZED,
              state: { from: location },
            }}
          />
        )
      }
    />
  );
};

AuthorizedRoute.propTypes = {
  requires: PropTypes.string.isRequired,
  userType: PropTypes.string.isRequired,
};

const mapStateToProps = (state) => {
  return {
    currentUser: state.user.current,
  };
};

export default connect(mapStateToProps, null)(AuthorizedRoute);
