import { FETCH_MAINTENANCE_TYPES, FETCH_UNIT_OF_MEASURES, FETCH_FEATURE_TYPES, FETCH_LOCATION_CODES } from './types';

import * as api from '../Api';

export const fetchMaintenanceTypes = () => dispatch => {
  return api.getMaintenanceTypes().then(response => {
    const data = response.data;
    dispatch({ type: FETCH_MAINTENANCE_TYPES, payload: data });
  });
};

export const fetchUnitOfMeasures = () => dispatch => {
  return api.getUnitOfMeasures().then(response => {
    const data = response.data;
    dispatch({ type: FETCH_UNIT_OF_MEASURES, payload: data });
  });
};

export const fetchFeatureTypes = () => dispatch => {
  return api.getFeatureTypes().then(response => {
    const data = response.data;
    dispatch({ type: FETCH_FEATURE_TYPES, payload: data });
  });
};

export const fetchLocationCodes = () => dispatch => {
  return api.getLocationCodes().then(response => {
    const data = response.data;
    dispatch({ type: FETCH_LOCATION_CODES, payload: data });
  });
};
