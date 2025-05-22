import React, { useState, useEffect } from 'react';
import { DropdownToggle, DropdownMenu, UncontrolledDropdown, DropdownItem, FormFeedback } from 'reactstrap';

const SingleDropdown = (props) => {
  const {
    items,
    defaultTitle,
    value,
    disabled,
    handleOnChange,
    handleOnBlur,
    isInvalidClassName,
    fieldMeta,
    errorStyle,
  } = props;
  const [title, setTitle] = useState(defaultTitle);

  useEffect(() => {
    const item = items.find((o) => {
      // disable strict type checking
      // eslint-disable-next-line
      return o.id == value;
    });

    if (item) setTitle(item.name);
  }, [value, items]);

  useEffect(() => {
    setTitle(defaultTitle);
  }, [defaultTitle]);

  const handleOnSelect = (item) => {
    if (handleOnChange) handleOnChange(item.id);

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
        className={`form-control form-input ${disabled ? 'disabled' : ''} ${isInvalidClassName}`}
        disabled={disabled}
        style={{ padding: '0' }}
      >
        <DropdownToggle caret onBlur={handleOnBlur}>
          {title}
        </DropdownToggle>
        <DropdownMenu>{renderMenuItems()}</DropdownMenu>
      </UncontrolledDropdown>
      {fieldMeta && fieldMeta.touched && fieldMeta.error && (
        <FormFeedback style={errorStyle}>{fieldMeta.error}</FormFeedback>
      )}
    </div>
  );
};

export default SingleDropdown;
