import React, { createContext, useContext, useState } from 'react';

const TooltipContext = createContext();

export function useTooltip() {
  return useContext(TooltipContext);
}

export const TooltipProvider = ({ children }) => {
  const [openTooltip, setOpenTooltip] = useState(null);

  const toggleTooltip = (id) => {
    if (openTooltip === id) {
      setOpenTooltip(null);
    } else {
      setOpenTooltip(id);
    }
  };

  return (
    <TooltipContext.Provider value={{ openTooltip, toggleTooltip }}>
      {children}
    </TooltipContext.Provider>
  );
};
