import React, { useState, useEffect } from 'react';
import { DropdownToggle, DropdownMenu, UncontrolledDropdown, DropdownItem, FormFeedback } from 'reactstrap';
import { useFormikContext, useField } from 'formik';

const SingleDropdownField = props => {
  const { items, name, defaultTitle, disabled } = props;
  const { values, setFieldValue, setFieldTouched } = useFormikContext();
  const [title, setTitle] = useState(values[name] && values[name].length > 0 ? values[name] : defaultTitle);
  const [field, meta] = useField(props);

  useEffect(() => {
    if (field.value) setTitle(items.find(o => o.id === field.value).name);
    else setTitle(defaultTitle);
  }, [field.value, items, defaultTitle]);

  const handleOnSelect = item => {
    setFieldValue(name, item.id);
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

export default SingleDropdownField;
