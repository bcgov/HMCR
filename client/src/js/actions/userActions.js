import * as api from '../Api';
import * as Constants from '../Constants';

import { FETCH_CURRENT_USER, FETCH_USER_TYPES, FETCH_USER_STATUSES } from './types';

export const fetchCurrentUser = () => dispatch => {
  return new Promise((resolve, reject) => {
    api.instance
      .get(Constants.API_PATHS.USER_CURRENT)
      .then(response => {
        const data = response.data;
        dispatch({ type: FETCH_CURRENT_USER, payload: data });
        resolve();
      })
      .catch(e => {
        reject(e);
      });
  });
};

export const fetchUserStatuses = () => dispatch => {
  return new Promise((resolve, reject) => {
    api.instance
      .get(Constants.API_PATHS.USER_STATUSES)
      .then(response => {
        const data = response.data;
        dispatch({ type: FETCH_USER_STATUSES, payload: data });
        resolve();
      })
      .catch(e => {
        reject(e);
      });
  });
};

export const fetchUserTypes = () => dispatch => {
  return new Promise((resolve, reject) => {
    api.instance
      .get(Constants.API_PATHS.USER_TYPES)
      .then(response => {
        const data = response.data;
        dispatch({ type: FETCH_USER_TYPES, payload: data });
        resolve();
      })
      .catch(e => {
        reject(e);
      });
  });
};
