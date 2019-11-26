import * as api from '../Api';
import * as Constants from '../Constants';

import { FETCH_ROLES } from './types';

export const fetchRoles = () => dispatch => {
  return new Promise((resolve, reject) => {
    api.instance
      .get(Constants.API_PATHS.ROLE)
      .then(response => {
        const data = response.data;
        dispatch({ type: FETCH_ROLES, payload: data });
        resolve();
      })
      .catch(e => {
        reject(e);
      });
  });
};
