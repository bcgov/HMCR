import React from 'react';
import PropTypes from 'prop-types';
import { NavLink as RRNavLink } from 'react-router-dom';
import { NavItem, NavLink } from 'reactstrap';

const NavLinkWithMatch = ({ to, text, hideNavbar }) => {
  const exactPathMatch = (match) => {
    return match && match.isExact;
  };

  return (
    <NavItem>
      <NavLink tag={RRNavLink} to={to} onClick={hideNavbar} isActive={exactPathMatch}>
        {text}
      </NavLink>
    </NavItem>
  );
};

NavLinkWithMatch.propTypes = {
  text: PropTypes.string.isRequired,
  to: PropTypes.string.isRequired,
  hideNavbar: PropTypes.func.isRequired,
};

export default NavLinkWithMatch;
