import React from 'react';
import { CustomInput, FormFeedback } from 'reactstrap';
import { FieldArray, useField, useFormikContext } from 'formik';

const MultiSelect = props => {
  const { items, name, handleBlur } = props;
  // eslint-disable-next-line
  const [field, meta] = useField(props);
  const { values } = useFormikContext();
  const selectedValues = values[name];

  const handleItemSelected = (checked, itemId, push, remove) => {
    if (checked) push(itemId);
    else remove(selectedValues.indexOf(itemId));
  };

  return (
    <React.Fragment>
      <div
        className={`form-control multi-select ${
          meta.touched && meta.error && typeof meta.error === 'string' && meta.value.length === 0 ? 'is-invalid' : ''
        }`}
      >
        <FieldArray name={name}>
          {({ push, remove }) =>
            items.map(item => {
              const description = item.description ? item.description : item.name;
              const displayName = description;
              return (
                <CustomInput
                  key={item.id}
                  type="checkbox"
                  id={`${name}_${item.id}`}
                  label={displayName}
                  value={item.id}
                  checked={values[name].includes(item.id)}
                  onBlur={handleBlur}
                  onChange={e => {
                    handleItemSelected(e.target.checked, item.id, push, remove);
                  }}
                />
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
