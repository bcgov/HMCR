import * as api from '../Api';

import {
  FETCH_CURRENT_USER,
  FETCH_USER_TYPES,
  FETCH_USER_STATUSES,
  CREATE_USER,
  EDIT_USER,
  DELETE_USER,
  SET_SINGLE_USER_SEARCH_CRITERIA,
  SEARCH_USERS,
} from './types';

export const fetchCurrentUser = () => dispatch => {
  return api.getCurrentUser().then(response => {
    const data = response.data;
    dispatch({ type: FETCH_CURRENT_USER, payload: data });
  });
};

export const fetchUserStatuses = () => dispatch => {
  return api.getUserStatuses().then(response => {
    const data = response.data;
    dispatch({ type: FETCH_USER_STATUSES, payload: data });
  });
};

export const fetchUserTypes = () => dispatch => {
  return api.getUserTypes().then(response => {
    const data = response.data;
    dispatch({ type: FETCH_USER_TYPES, payload: data });
  });
};

export const createUser = userData => dispatch => {
  return api.postUser(userData).then(response => {
    const data = response.data;
    dispatch({ type: CREATE_USER, payload: data });
  });
};

export const editUser = (id, userData) => dispatch => {
  return api.putUser(id, userData).then(response => {
    const data = response.data;
    dispatch({ type: EDIT_USER, payload: data });
  });
};

export const deleteUser = (id, endDate) => dispatch => {
  return api.deleteUser(id, endDate).then(response => {
    const data = response.data;
    dispatch({ type: DELETE_USER, payload: data });
  });
};

export const searchUsers = () => (dispatch, getState) => {
  const params = { ...getState().user.searchCriteria };

  return api.searchUsers(params).then(response => {
    const data = response.data;
    dispatch({ type: SEARCH_USERS, payload: data });
  });
};

export const setSingleUserSeachCriteria = (key, value) => {
  return {
    type: SET_SINGLE_USER_SEARCH_CRITERIA,
    payload: { key, value },
  };
};
