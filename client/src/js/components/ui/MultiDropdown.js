import React, { useState } from 'react';
import { DropdownToggle, DropdownMenu, UncontrolledDropdown, Label, Input } from 'reactstrap';
import { FieldArray } from 'formik';

const maxSelectedItemDisplay = 2;

const MultiDropdown = ({ values, setFieldValue, items, name, title }) => {
  const [selectAll, setSelectAll] = useState(false);
  const selectedValues = values[name];

  const updateTitle = () => {
    if (selectedValues.length === 0) return title;
    else if (selectedValues.length > maxSelectedItemDisplay) return `(${selectedValues.length}) selected`;
    else return selectedValues.map(v => items.find(o => o.id === v).name).join(', ');
  };

  const updateSelectAll = () => {
    if (selectedValues.length === items.length && !selectAll) setSelectAll(true);
    else if (selectedValues.length !== items.length && selectAll) setSelectAll(false);
  };

  const handleSelectedAllChecked = checked => {
    setSelectAll(checked);

    if (checked)
      setFieldValue(
        name,
        items.map(o => o.id),
        true
      );
    else setFieldValue(name, [], true);
  };

  const handleItemSelected = (checked, itemId, push, remove) => {
    if (checked) push(itemId);
    else remove(selectedValues.indexOf(itemId));
  };

  updateSelectAll();

  return (
    <UncontrolledDropdown className="form-input">
      <DropdownToggle caret>{updateTitle()}</DropdownToggle>
      <DropdownMenu className="multi">
        <div className="multi-item select-all">
          <Label check className="multi-item-label">
            <Input
              name={name}
              type="checkbox"
              checked={selectAll}
              onChange={e => {
                handleSelectedAllChecked(e.target.checked);
              }}
            />
            Select All
          </Label>
        </div>
        <FieldArray name={name}>
          {({ push, remove }) => (
            <div className="multi-menu">
              {items.map(item => {
                const displayName = item.name;
                return (
                  <div key={item.id} className="multi-item">
                    <Label check className="multi-item-label">
                      <Input
                        name={name}
                        type="checkbox"
                        value={item.id}
                        checked={values[name].includes(item.id)}
                        onChange={e => {
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

export default MultiDropdown;
