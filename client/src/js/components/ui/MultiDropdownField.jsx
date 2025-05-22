import React, { useState, useMemo, useEffect } from 'react';
import PropTypes from 'prop-types';
import { DropdownToggle, DropdownMenu, UncontrolledDropdown, Label, Input } from 'reactstrap';
import { FieldArray } from 'formik';

const maxSelectedItemDisplay = 2;

const MultiDropdownField = ({ values, setFieldValue, items, name, title, searchable }) => {
  const [selectAll, setSelectAll] = useState(false);
  const [textFilter, setTextFilter] = useState('');
  const selectedValues = values[name];

  const updateTitle = () => {
    if (selectedValues.length === 0) return title;
    else if (selectedValues.length > maxSelectedItemDisplay) return `(${selectedValues.length}) selected`;
    else return selectedValues.map((v) => items.find((o) => o.id === v).name).join(', ');
  };

  const displayItems = useMemo(() => {
    if (textFilter.trim().length > 0) {
      const pattern = new RegExp(textFilter.trim(), 'i');
      const filteredItems = items.filter((item) => pattern.test(item.name));

      return filteredItems;
    }

    return items;
  }, [items, textFilter]);

  const handleSelectedAllChecked = (checked) => {
    setSelectAll(checked);

    if (checked)
      setFieldValue(
        name,
        displayItems.map((o) => o.id),
        true
      );
    else setFieldValue(name, [], true);
  };

  const handleItemSelected = (checked, itemId, push, remove) => {
    if (checked) push(itemId);
    else remove(selectedValues.indexOf(itemId));
  };

  useEffect(() => {
    if (selectedValues.length === displayItems.length && !selectAll) setSelectAll(true);
    else if (selectedValues.length !== displayItems.length && selectAll) setSelectAll(false);
  }, [selectedValues, displayItems, selectAll]);

  return (
    <UncontrolledDropdown className="form-input">
      <DropdownToggle caret>{updateTitle()}</DropdownToggle>
      <DropdownMenu className="multi">
        {searchable && (
          <div className="multi-item select-all p-2">
            <Input
              name={name}
              type="textbox"
              placeholder="Search"
              value={textFilter}
              onChange={(e) => {
                setTextFilter(e.target.value);
              }}
            />
          </div>
        )}
        <div className="multi-item select-all">
          <Label check className="multi-item-label">
            <Input
              name={name}
              type="checkbox"
              checked={selectAll}
              onChange={(e) => {
                handleSelectedAllChecked(e.target.checked);
              }}
            />
            Select All
          </Label>
        </div>
        <FieldArray name={name}>
          {({ push, remove }) => (
            <div className="multi-menu">
              {displayItems.map((item) => {
                const displayName = item.name;
                return (
                  <div key={item.id} className="multi-item">
                    <Label check className="multi-item-label">
                      <Input
                        name={name}
                        type="checkbox"
                        value={item.id}
                        checked={values[name].includes(item.id)}
                        onChange={(e) => {
                          handleItemSelected(e.target.checked, item.id, push, remove);
                        }}
                      />
                      {displayName}
                    </Label>
                  </div>
                );
              })}
            </div>
          )}
        </FieldArray>
      </DropdownMenu>
    </UncontrolledDropdown>
  );
};

MultiDropdownField.propTypes = {
  // Formik form values
  values: PropTypes.object.isRequired,

  // Formik setFieldValue
  setFieldValue: PropTypes.func.isRequired,

  // Dropdown items
  items: PropTypes.array.isRequired,

  // Formik field name
  name: PropTypes.string.isRequired,

  // Default title of dropdown
  title: PropTypes.string.isRequired,

  // Enable search text field
  searchable: PropTypes.bool,
};

export default MultiDropdownField;
