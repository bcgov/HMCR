import React from 'react';

const MaterialCard = ({ children, ...rest }) => {
  return (
    <div className="material-card-container" {...rest}>
      {children}
    </div>
  );
};

export default MaterialCard;
