import React from 'react';
import { BrowserRouter, Route, Switch } from 'react-router-dom';
import { Container } from 'reactstrap';

import 'react-dates/initialize';
import 'react-dates/lib/css/_datepicker.css';

import AuthorizedRoute from './components/fragments/AuthorizedRoute';
import Main from './components/Main';
import Footer from './components/fragments/Footer';
import Header from './components/fragments/Header';
import ActivityAdmin from './components/ActivityAdmin';
import UserAdmin from './components/UserAdmin';
import RolePermissionAdmin from './components/RolePermissionAdmin';
import WorkReporting from './components/WorkReporting';
import WorkReportingSubmission from './components/WorkReportingSubmission';
import Home from './components/Home';
import HelloWorld from './components/HelloWorld';

import addIconsToLibrary from './fontAwesome';
import * as Constants from './Constants';

import '../scss/app.scss';

const App = () => {
  addIconsToLibrary();

  return (
    <Main>
      <BrowserRouter>
        <React.Fragment>
          <Header />
          <Container>
            <Switch>
              <Route path={Constants.PATHS.HOME} exact component={Home} />
              <AuthorizedRoute path={Constants.PATHS.ADMIN} requires={Constants.PERMISSIONS.ADMIN}>
                <AdminRoutes />
              </AuthorizedRoute>
              <AuthorizedRoute path={Constants.PATHS.WORK_REPORTING} requires={Constants.PERMISSIONS.CONTRACTOR}>
                <WorkReportingRoutes />
              </AuthorizedRoute>
              <Route path={Constants.PATHS.HELLOWORLD} exact component={HelloWorld} />
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

const WorkReportingRoutes = () => {
  return (
    <Switch>
      <Route path={Constants.PATHS.WORK_REPORTING} exact component={WorkReporting} />
      <Route path={`${Constants.PATHS.WORK_REPORTING}/:submissionId`} component={WorkReportingSubmission} />
    </Switch>
  );
};

const AdminRoutes = () => {
  return (
    <Switch>
      <Route path={Constants.PATHS.ADMIN} exact component={ActivityAdmin} />
      <Route path={Constants.PATHS.ADMIN_ACTIVITIES} exact component={ActivityAdmin} />
      <Route path={Constants.PATHS.ADMIN_USERS} exact component={UserAdmin} />
      <Route path={Constants.PATHS.ADMIN_ROLES} exact component={RolePermissionAdmin} />
      <Route path="*" component={NoMatch} />
    </Switch>
  );
};

export default App;
