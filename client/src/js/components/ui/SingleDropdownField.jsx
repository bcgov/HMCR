import React, { useState, useEffect } from 'react';
import { useFormikContext, useField } from 'formik';

import SingleDropdown from './SingleDropdown';

const SingleDropdownField = (props) => {
  const { items, name, defaultTitle, disabled } = props;
  const { values, setFieldValue, setFieldTouched } = useFormikContext();
  const [title, setTitle] = useState(values[name] && values[name].length > 0 ? values[name] : defaultTitle);
  const [field, meta] = useField(props);

  useEffect(() => {
    const selectedItem = items.find((o) => o.id === field.value);
    if (selectedItem) {
      setTitle(selectedItem.name);
    } else {
      setTitle(defaultTitle);
    }
  }, [field.value, items, defaultTitle]);

  const handleOnSelect = (item) => {
    setFieldValue(name, item);
  };

  let style = {};
  let isInvalidClassName = '';

  if (meta.touched && meta.error) {
    style = { display: 'block' };
    isInvalidClassName = 'is-invalid';
  }

  return (
    <SingleDropdown
      items={items}
      value={field.value}
      defaultTitle={title}
      handleOnChange={handleOnSelect}
      handleOnBlur={() => setFieldTouched(name, true)}
      disabled={disabled}
      isInvalidClassName={isInvalidClassName}
      errorStyle={style}
      fieldMeta={meta}
    />
  );
};

export default SingleDropdownField;
