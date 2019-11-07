import * as api from '../Api';
import * as Constants from '../Constants';

import { FETCH_USER } from './types';

export const fetchUser = id => dispatch => {
  return new Promise((resolve, reject) => {
    if (id) {
      api.instance
        .get(`${Constants.API_PATHS.USER}/${id}`)
        .then(response => {
          const data = response.data;
          dispatch({ type: FETCH_USER, payload: data });
          resolve();
        })
        .catch(e => {
          reject(e);
        });
    } else {
      resolve();
    }
  });
};
