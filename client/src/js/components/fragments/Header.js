import React, { useState } from 'react';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import { Button, Container, Collapse, Navbar, NavbarToggler, NavbarBrand, Nav, NavItem } from 'reactstrap';

import NavLinkWithMatch from '../ui/NavLinkWithMatch';

import * as Constants from '../../Constants';

const Header = ({ currentUser }) => {
  const [collapsed, setCollapsed] = useState(true);

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
      <Navbar expand="lg" className="navbar-dark main-nav">
        <Container>
          <Collapse isOpen={!collapsed} navbar>
            <Nav className="navbar-nav">
              <NavLinkWithMatch hideNavbar={hideNavbar} to={Constants.PATHS.ADMIN_ACTIVITIES} text="Activities" />
              <NavLinkWithMatch hideNavbar={hideNavbar} to={Constants.PATHS.ADMIN_USERS} text="Users" />
              <NavLinkWithMatch hideNavbar={hideNavbar} to={Constants.PATHS.ADMIN_ROLES} text="Roles and Permissions" />
              {/* <NavLinkWithMatch hideNavbar={hideNavbar} to={Constants.PATHS.WORK_REPORTING} text="Work Reporting" /> */}
            </Nav>
            <Nav className="navbar-nav ml-auto">
              <NavItem>
                <Button color="link">{`${currentUser.username},  Logout`} </Button>
              </NavItem>
            </Nav>
          </Collapse>
        </Container>
      </Navbar>
    </header>
  );
};

const mapStateToProps = state => {
  return {
    currentUser: state.user.current,
  };
};

export default connect(mapStateToProps, null)(Header);
