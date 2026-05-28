import React, { useState } from 'react';
import { FormFeedback, FormGroup, Input, Label } from 'reactstrap';
import { FieldArray, useField, useFormikContext } from 'formik';

const MultiSelect = (props) => {
  const { items, name, handleBlur, showSelectAll,selectClass } = props;
  // eslint-disable-next-line
  const [field, meta] = useField(props);
  const selectClassName= (selectClass===null ||selectClass===undefined||selectClass==='')?"form-control multi-select":selectClass;
  
  const { values, setFieldValue } = useFormikContext();
  const selectedValues = values[name];
  
  const [selectAll, setSelectAll] = useState(false);

  const handleItemSelected = (checked, itemId, push, remove) => {
    if (checked) push(itemId);
    else remove(selectedValues.indexOf(itemId));
  };

  const updateSelectAll = () => {
    if (selectedValues.length === items.length && !selectAll) setSelectAll(true);
    else if (selectedValues.length !== items.length && selectAll) setSelectAll(false);
  };

  const handleSelectedAllChecked = (checked) => {
    setSelectAll(checked);

    if (checked)
      setFieldValue(
        name,
        items.map((o) => o.id),
        true
      );
    else setFieldValue(name, [], true);
  };

  updateSelectAll();

  return (
    <React.Fragment>
      <div
        className={`${selectClassName} ${
          meta.touched && meta.error && typeof meta.error === 'string' && meta.value.length === 0 ? 'is-invalid' : ''
        }`}
      >
        {showSelectAll && (
          <FormGroup check className="multiselect-all">
            <Input
              type="checkbox"
              id={`${name}_select_all`}
              value="select_all"
              checked={selectAll}
              onBlur={handleBlur}
              onChange={(e) => {
                handleSelectedAllChecked(e.target.checked);
              }}
            />
            <Label check for={`${name}_select_all`}>
              Select All
            </Label>
          </FormGroup>
        )}
        <FieldArray name={name}>
          {({ push, remove }) =>
            items.map((item) => {
              const description = item.description ? item.description : item.name;
              const displayName = description;
              return (
                <FormGroup check key={item.id}>
                  <Input
                    type="checkbox"
                    id={`${name}_${item.id}`}
                    value={item.id}
                    checked={values[name].includes(item.id)}
                    onBlur={handleBlur}
                    onChange={(e) => {
                      handleItemSelected(e.target.checked, item.id, push, remove);
                    }}
                  />
                  <Label check for={`${name}_${item.id}`}>
                    {displayName}
                  </Label>
                </FormGroup>
              );
            })
          }
        </FieldArray>
      </div>
      {meta.touched && meta.error && typeof meta.error && meta.value.length === 0 && (
        <FormFeedback>{meta.error}</FormFeedback>
      )}
    </React.Fragment>
  );
};

export default MultiSelect;
