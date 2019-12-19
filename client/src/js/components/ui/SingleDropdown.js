import React, { useState, useEffect } from 'react';
import { DropdownToggle, DropdownMenu, UncontrolledDropdown, DropdownItem } from 'reactstrap';

const SingleDropdown = props => {
  const { items, defaultTitle, value, disabled, handleOnChange } = props;
  const [title, setTitle] = useState(defaultTitle);

  useEffect(() => {
    const item = items.find(o => o.id === value);

    if (item) setTitle(item.name);
  }, [value, items]);

  const handleOnSelect = item => {
    handleOnChange(item.id);
    setTitle(item.name);
  };

  const renderMenuItems = () => {
    return items.map((item, index) => {
      const displayName = item.name;

      if (item.type === 'header') {
        return (
          <DropdownItem header key={index}>
            {displayName}
          </DropdownItem>
        );
      } else {
        return (
          <DropdownItem key={index} onClick={() => handleOnSelect(item)}>
            {displayName}
          </DropdownItem>
        );
      }
    });
  };

  return (
    <div style={{ padding: '0' }}>
      <UncontrolledDropdown
        className={`form-control form-input ${disabled ? 'disabled' : ''}`}
        disabled={disabled}
        style={{ padding: '0' }}
      >
        <DropdownToggle caret>{title}</DropdownToggle>
        <DropdownMenu>{renderMenuItems()}</DropdownMenu>
      </UncontrolledDropdown>
    </div>
  );
};

export default SingleDropdown;
