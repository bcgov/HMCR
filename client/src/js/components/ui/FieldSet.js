import React from 'react';

const FieldSet = ({ legendname,children }) => {
  return (
    <fieldset className='form-control fieldset'>
      <legend className='form-control legend'>{legendname}</legend>
      {children}
    </fieldset>
  );
};

export default FieldSet;
