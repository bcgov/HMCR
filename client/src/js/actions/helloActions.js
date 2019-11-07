import * as api from '../Api';
import * as Constants from '../Constants';

import { FETCH_COUNTRIES, FETCH_PROVINCES } from './types';

export const fetchCountries = () => dispatch => {
  return new Promise((resolve, reject) => {
    api.instance
      .get(Constants.API_PATHS.COUNTRY)
      .then(response => {
        const data = response.data;
        dispatch({ type: FETCH_COUNTRIES, payload: data });
        resolve();
      })
      .catch(e => {
        reject(e);
      });
  });
};

export const fetchProvinces = () => dispatch => {
  return new Promise((resolve, reject) => {
    api.instance
      .get(Constants.API_PATHS.PROVINCE)
      .then(response => {
        const data = response.data;
        dispatch({ type: FETCH_PROVINCES, payload: data });
        resolve();
      })
      .catch(e => {
        reject(e);
      });
  });
};

export const createProvince = province => () => {
  return new Promise((resolve, reject) => {
    api.instance
      .post(Constants.API_PATHS.PROVINCE, province)
      .then(() => {
        resolve();
      })
      .catch(e => {
        reject(e);
      });
  });
};
