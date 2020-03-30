import React from 'react';
import { connect } from 'react-redux';
import { BrowserRouter, Route, Switch, Redirect } from 'react-router-dom';
import { Container } from 'reactstrap';
import { toast } from 'react-toastify';

import 'react-dates/initialize';
import 'react-dates/lib/css/_datepicker.css';

import 'react-toastify/dist/ReactToastify.css';

import AuthorizedRoute from './components/fragments/AuthorizedRoute';
import Main from './components/Main';
import Footer from './components/fragments/Footer';
import Header from './components/fragments/Header';
import ActivityAdmin from './components/ActivityAdmin';
import UserAdmin from './components/UserAdmin';
import RoleAdmin from './components/RoleAdmin';
import ReportExport from './components/ReportExport';
import WorkReporting from './components/WorkReporting';
import Version from './components/Version';
import WorkReportingSubmissionDetail from './components/WorkReportingSubmissionDetail';
import ErrorBoundary from './components/ErrorBoundary';

import addIconsToLibrary from './fontAwesome';
import * as Constants from './Constants';

import '../scss/app.scss';

toast.configure({
  position: 'top-center',
  autoClose: 2000,
  hideProgressBar: true,
  closeOnClick: true,
  pauseOnHover: true,
  draggable: true,
});

const App = ({ currentUser }) => {
  addIconsToLibrary();

  return (
    <Main>
      <BrowserRouter>
        <React.Fragment>
          <Header />
          <Container>
            <ErrorBoundary>
              <Switch>
                {Routes(currentUser)}
                <Route path={Constants.PATHS.UNAUTHORIZED} exact component={Unauthorized} />
                <Route path="*" component={NoMatch} />
              </Switch>
            </ErrorBoundary>
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

const Routes = currentUser => {
  switch (currentUser.userType) {
    case Constants.USER_TYPE.INTERNAL:
      return AdminRoutes(currentUser);
    case Constants.USER_TYPE.BUSINESS:
      return ContractorRoutes(currentUser);
    default:
      return <Redirect to={Constants.PATHS.UNAUTHORIZED} />;
  }
};

const defaultPath = currentUser => {
  if (currentUser.permissions.includes(Constants.PERMISSIONS.CODE_R)) return Constants.PATHS.ADMIN_ACTIVITIES;

  if (currentUser.permissions.includes(Constants.PERMISSIONS.USER_R)) return Constants.PATHS.ADMIN_USERS;

  if (currentUser.permissions.includes(Constants.PERMISSIONS.ROLE_R)) return Constants.PATHS.ADMIN_ROLES;

  if (currentUser.permissions.includes(Constants.PERMISSIONS.FILE_R)) return Constants.PATHS.WORK_REPORTING;

  return Constants.PATHS.UNAUTHORIZED;
};

const getLastVistedPath = currentUser => {
  const lastVisitedPath = localStorage.getItem('lastVisitedPath');

  if (lastVisitedPath) return lastVisitedPath;

  return defaultPath(currentUser);
};

const ContractorRoutes = currentUser => {
  return (
    <Switch>
      <Route path={Constants.PATHS.ADMIN}>
        <Redirect to={Constants.PATHS.UNAUTHORIZED} />
      </Route>
      <Route path={Constants.PATHS.HOME} exact>
        <Redirect to={getLastVistedPath(currentUser)} />
      </Route>
      <Route path={Constants.PATHS.WORK_REPORTING} exact component={WorkReporting} />
      <Route path={`${Constants.PATHS.WORK_REPORTING}/:submissionId`} component={WorkReportingSubmissionDetail} />
      <Route path={Constants.PATHS.VERSION} exact component={Version} />
      <Route path={Constants.PATHS.UNAUTHORIZED} exact component={Unauthorized} />
      <Route path="*" component={NoMatch} />
    </Switch>
  );
};

const AdminRoutes = currentUser => {
  return (
    <Switch>
      <Route path={Constants.PATHS.HOME} exact>
        <Redirect to={getLastVistedPath(currentUser)} />
      </Route>
      <AuthorizedRoute
        path={Constants.PATHS.ADMIN_ACTIVITIES}
        requires={Constants.PERMISSIONS.CODE_R}
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
        path={Constants.PATHS.REPORT_EXPORT}
        requires={Constants.PERMISSIONS.EXPORT}
        userType={Constants.USER_TYPE.INTERNAL}
      >
        <Route path={Constants.PATHS.REPORT_EXPORT} exact component={ReportExport} />
      </AuthorizedRoute>
      <AuthorizedRoute
        path={Constants.PATHS.WORK_REPORTING}
        requires={Constants.PERMISSIONS.FILE_R}
        userType={Constants.USER_TYPE.INTERNAL}
      >
        <Route path={Constants.PATHS.WORK_REPORTING} exact component={WorkReporting} />
      </AuthorizedRoute>
      <Route path={Constants.PATHS.VERSION} exact component={Version} />
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
