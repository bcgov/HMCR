const MaterialCard = ({ children, ...rest }) => {
  return (
    <div className="material-card-container" {...rest}>
      {children}
    </div>
  );
};

export default MaterialCard;
