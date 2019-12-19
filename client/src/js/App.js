import React from 'react';
import { connect } from 'react-redux';
import { BrowserRouter, Route, Switch, Redirect } from 'react-router-dom';
import { Container } from 'reactstrap';

import 'react-dates/initialize';
import 'react-dates/lib/css/_datepicker.css';

import AuthorizedRoute from './components/fragments/AuthorizedRoute';
import Main from './components/Main';
import Footer from './components/fragments/Footer';
import Header from './components/fragments/Header';
import ActivityAdmin from './components/ActivityAdmin';
import UserAdmin from './components/UserAdmin';
import RoleAdmin from './components/RoleAdmin';
import WorkReporting from './components/WorkReporting';
import WorkReportingSubmissionDetail from './components/WorkReportingSubmissionDetail';
// import Home from './components/Home';

import addIconsToLibrary from './fontAwesome';
import * as Constants from './Constants';

import '../scss/app.scss';

const App = ({ currentUser }) => {
  addIconsToLibrary();

  return (
    <Main>
      <BrowserRouter>
        <React.Fragment>
          <Header />
          <Container>
            <Switch>
              {Routes(currentUser.userType)}
              <Route path={Constants.PATHS.UNAUTHORIZED} exact component={Unauthorized} />
              <Route path="*" component={NoMatch} />
            </Switch>
          </Container>
          <Footer />
        </React.Fragment>
      </BrowserRouter>
    </Main>
  );
};

const NoMatch = () => {
  return <p>404</p>;
};

const Unauthorized = () => {
  return <p>Unauthorized</p>;
};

const Routes = userType => {
  switch (userType) {
    case Constants.USER_TYPE.INTERNAL:
      return AdminRoutes();
    case Constants.USER_TYPE.BUSINESS:
      return WorkReportingRoutes();
    default:
      return <Redirect to={Constants.PATHS.UNAUTHORIZED} />;
  }
};

const WorkReportingRoutes = () => {
  return (
    <Switch>
      <Route path={Constants.PATHS.ADMIN}>
        <Redirect to={Constants.PATHS.UNAUTHORIZED} />
      </Route>
      <Route path={Constants.PATHS.HOME} exact>
        <Redirect to={Constants.PATHS.WORK_REPORTING} />
      </Route>
      <Route path={Constants.PATHS.WORK_REPORTING} exact component={WorkReporting} />
      <Route path={`${Constants.PATHS.WORK_REPORTING}/:submissionId`} component={WorkReportingSubmissionDetail} />
      <Route path={Constants.PATHS.UNAUTHORIZED} exact component={Unauthorized} />
      <Route path="*" component={NoMatch} />
    </Switch>
  );
};

const AdminRoutes = () => {
  return (
    <Switch>
      <Route path={Constants.PATHS.HOME} exact>
        <Redirect to={Constants.PATHS.ADMIN_USERS} />
      </Route>
      <AuthorizedRoute
        path={Constants.PATHS.ADMIN_ACTIVITIES}
        requires={Constants.PERMISSIONS.FILE_R}
        userType={Constants.USER_TYPE.INTERNAL}
      >
        <Route path={Constants.PATHS.ADMIN_ACTIVITIES} exact component={ActivityAdmin} />
      </AuthorizedRoute>
      <AuthorizedRoute
        path={Constants.PATHS.ADMIN_USERS}
        requires={Constants.PERMISSIONS.USER_R}
        userType={Constants.USER_TYPE.INTERNAL}
      >
        <Route path={Constants.PATHS.ADMIN_USERS} exact component={UserAdmin} />
      </AuthorizedRoute>
      <AuthorizedRoute
        path={Constants.PATHS.ADMIN_ROLES}
        requires={Constants.PERMISSIONS.ROLE_R}
        userType={Constants.USER_TYPE.INTERNAL}
      >
        <Route path={Constants.PATHS.ADMIN_ROLES} exact component={RoleAdmin} />
      </AuthorizedRoute>
      <AuthorizedRoute
        path={Constants.PATHS.WORK_REPORTING}
        requires={Constants.PERMISSIONS.FILE_R}
        userType={Constants.USER_TYPE.INTERNAL}
      >
        <Route path={Constants.PATHS.WORK_REPORTING} exact component={WorkReporting} />
      </AuthorizedRoute>
      <Route path={Constants.PATHS.UNAUTHORIZED} exact component={Unauthorized} />
      <Route path="*" component={NoMatch} />
    </Switch>
  );
};

const mapStateToProps = state => {
  return {
    currentUser: state.user.current,
  };
};

export default connect(mapStateToProps, null)(App);
