import React, { useState } from 'react';
import { DropdownToggle, DropdownMenu, UncontrolledDropdown, DropdownItem, FormFeedback } from 'reactstrap';
import { useFormikContext, useField } from 'formik';

const SingleDropdown = props => {
  const { items, name, defaultTitle, showId, disabled } = props;
  const { values, setFieldValue, setFieldTouched } = useFormikContext();
  const [title, setTitle] = useState(values[name] && values[name].length > 0 ? values[name] : defaultTitle);
  // eslint-disable-next-line
  const [field, meta] = useField(props);

  const handleOnSelect = item => {
    setFieldValue(name, item.id);
    setTitle(item.name);
  };

  const renderMenuItems = () => {
    return items.map((item, index) => {
      const displayName = showId ? `${item.id} - ${item.name}` : item.name;

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

  let style = {};
  let isInvalidClassName = '';

  if (meta.touched && meta.error) {
    style = { display: 'block' };
    isInvalidClassName = 'is-invalid';
  }

  return (
    <div style={{ padding: '0' }}>
      <UncontrolledDropdown
        className={`form-control form-input ${disabled ? 'disabled' : ''} ${isInvalidClassName}`}
        disabled={disabled}
        style={{ padding: '0' }}
      >
        <DropdownToggle caret className={isInvalidClassName} onBlur={() => setFieldTouched(name, true)}>
          {title}
        </DropdownToggle>
        <DropdownMenu>{renderMenuItems()}</DropdownMenu>
      </UncontrolledDropdown>
      {meta.touched && meta.error && <FormFeedback style={style}>{meta.error}</FormFeedback>}
    </div>
  );
};

export default SingleDropdown;
