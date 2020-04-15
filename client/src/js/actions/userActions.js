import * as api from '../Api';

import { FETCH_CURRENT_USER, FETCH_USER_TYPES, FETCH_USER_STATUSES } from './types';

export const fetchCurrentUser = () => (dispatch) => {
  return api.getCurrentUser().then((response) => {
    const data = response.data;
    dispatch({ type: FETCH_CURRENT_USER, payload: data });
  });
};

export const fetchUserStatuses = () => (dispatch) => {
  return api.getUserStatuses().then((response) => {
    const data = response.data;
    dispatch({ type: FETCH_USER_STATUSES, payload: data });
  });
};

export const fetchUserTypes = () => (dispatch) => {
  return api.getUserTypes().then((response) => {
    const data = response.data;
    dispatch({ type: FETCH_USER_TYPES, payload: data });
  });
};
