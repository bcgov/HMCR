import React from 'react';

const UIHeader = ({ children }) => {
  return (
    <div className="ui-header">
      <h1>{children}</h1>
    </div>
  );
};

export default UIHeader;
