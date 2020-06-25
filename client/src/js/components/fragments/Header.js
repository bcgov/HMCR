import React, { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import {
  Button,
  Container,
  Collapse,
  Navbar,
  NavbarToggler,
  NavbarBrand,
  Nav,
  NavItem,
  UncontrolledDropdown,
  DropdownToggle,
  DropdownMenu,
  DropdownItem,
} from 'reactstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

import NavLinkWithMatch from '../ui/NavLinkWithMatch';
import Authorize from '../fragments/Authorize';

import * as Keycloak from '../../Keycloak';
import * as Constants from '../../Constants';
import * as api from '../../Api';

const Header = ({ currentUser }) => {
  const location = useLocation();
  const [collapsed, setCollapsed] = useState(true);
  const [version, setVersion] = useState(null);

  useEffect(() => {
    api.getVersion().then((response) => setVersion(response.data.environment.toLowerCase()));
  }, []);

  useEffect(() => {
    localStorage.setItem('lastVisitedPath', location.pathname);
  }, [location.pathname]);

  const toggleNavbar = () => {
    setCollapsed(!collapsed);
  };

  const hideNavbar = () => {
    setCollapsed(true);
  };

  return (
    <header className="mb-3">
      <Navbar expand="lg" className="navbar-dark">
        <Container>
          <NavbarBrand tag={Link} onClick={hideNavbar} to="/">
            <img
              className="img-fluid d-none d-md-block"
              src={`${process.env.PUBLIC_URL}/images/bcid-logo-rev-en.svg`}
              width="181"
              height="44"
              alt="B.C. Government Logo"
            />
            <img
              className="img-fluid d-md-none"
              src={`${process.env.PUBLIC_URL}/images/bcid-symbol-rev.svg`}
              width="64"
              height="44"
              alt="B.C. Government Logo"
            />
          </NavbarBrand>
          <div className="navbar-brand">MoTI Maintenance Contractor Reporting</div>
          <NavbarToggler onClick={toggleNavbar} />
          {/* <Collapse> is needed to keep justify-content: space-between maintain correct spacing */}
          <Collapse isOpen={!collapsed} navbar />
        </Container>
      </Navbar>
      <Navbar expand="lg" className={`navbar-dark main-nav ${version}`}>
        <Container>
          <Collapse isOpen={!collapsed} navbar>
            <Nav className="navbar-nav">
              {currentUser.userType === Constants.USER_TYPE.INTERNAL && (
                <React.Fragment>
                  <Authorize requires={Constants.PERMISSIONS.CODE_R}>
                    <NavLinkWithMatch hideNavbar={hideNavbar} to={Constants.PATHS.ADMIN_ACTIVITIES} text="Activities" />
                  </Authorize>
                  <Authorize requires={Constants.PERMISSIONS.USER_R}>
                    <NavLinkWithMatch hideNavbar={hideNavbar} to={Constants.PATHS.ADMIN_USERS} text="Users" />
                  </Authorize>
                  <Authorize requires={Constants.PERMISSIONS.ROLE_R}>
                    <NavLinkWithMatch
                      hideNavbar={hideNavbar}
                      to={Constants.PATHS.ADMIN_ROLES}
                      text="Roles and Permissions"
                    />
                  </Authorize>
                </React.Fragment>
              )}
              <Authorize requires={Constants.PERMISSIONS.FILE_R}>
                <NavLinkWithMatch hideNavbar={hideNavbar} to={Constants.PATHS.WORK_REPORTING} text="Work Reporting" />
              </Authorize>
              <Authorize requires={Constants.PERMISSIONS.EXPORT}>
                <NavLinkWithMatch hideNavbar={hideNavbar} to={Constants.PATHS.REPORT_EXPORT} text="Report Export" />
              </Authorize>
              <UncontrolledDropdown nav inNavbar>
                <DropdownToggle nav caret>
                  Quick Links
                </DropdownToggle>
                <DropdownMenu>
                  {currentUser.userType === Constants.USER_TYPE.INTERNAL && (
                    <DropdownItem tag={Link} to={Constants.PATHS.API_ACCESS}>
                      API Access
                    </DropdownItem>
                  )}
                  <DropdownItem tag={Link} to={Constants.PATHS.VERSION}>
                    Version
                  </DropdownItem>
                  <DropdownItem href={Constants.PATHS.DATA_ROOM} target="_blank">
                    Data Room
                  </DropdownItem>
                  <DropdownItem href={Constants.PATHS.MANUAL_AND_TEMPLATES} target="_blank">
                    Manual and Templates
                  </DropdownItem>
                </DropdownMenu>
              </UncontrolledDropdown>
            </Nav>
            <Nav className="navbar-nav ml-auto">
              <NavItem>
                <Button color="link" onClick={() => Keycloak.logout()}>
                  <FontAwesomeIcon icon="user" /> {`${currentUser.username},  Logout`}
                </Button>
              </NavItem>
            </Nav>
          </Collapse>
        </Container>
      </Navbar>
    </header>
  );
};

const mapStateToProps = (state) => {
  return {
    currentUser: state.user.current,
  };
};

export default connect(mapStateToProps, null)(Header);
