import axios from 'axios';

import store from './store';

import { SHOW_ERROR_DIALOG_MODAL } from './actions/types';
import { buildApiErrorObject } from './utils';
import * as Constants from './Constants';

export const instance = axios.create({
  baseURL: `${Constants.API_URL}`,
  headers: { 'Access-Control-Allow-Origin': '*' },
});

instance.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    if (!error.response || error.response.status !== 422)
      store.dispatch({ type: SHOW_ERROR_DIALOG_MODAL, payload: buildApiErrorObject(error.response) });

    return Promise.reject(error);
  }
);

export const getCurrentUser = () => instance.get(Constants.API_PATHS.USER_CURRENT);
export const getUser = id => instance.get(`${Constants.API_PATHS.USER}/${id}`);
export const getUserStatuses = () => instance.get(Constants.API_PATHS.USER_STATUSES);
export const getUserTypes = () => instance.get(Constants.API_PATHS.USER_TYPES);
export const postUser = userData => instance.post(Constants.API_PATHS.USER, userData);
export const putUser = (id, userData) => instance.put(`${Constants.API_PATHS.USER}/${id}`, userData);
export const deleteUser = (id, endDate) =>
  instance.delete(`${Constants.API_PATHS.USER}/${id}`, { data: { id, endDate } });
export const searchUsers = params => instance.get(Constants.API_PATHS.USER, { params: { ...params } });
export const getUserBceidAccount = (userType, username) =>
  instance.get(`${Constants.API_PATHS.USER_BCEID_ACCOUNT}/${userType}/${username}`);

export const getRoles = () => instance.get(Constants.API_PATHS.ROLE);
export const getRole = id => instance.get(`${Constants.API_PATHS.ROLE}/${id}`);
export const searchRoles = params => instance.get(Constants.API_PATHS.ROLE, { params: { ...params } });
export const postRole = roleData => instance.post(Constants.API_PATHS.ROLE, roleData);
export const putRole = (id, roleData) => instance.put(`${Constants.API_PATHS.ROLE}/${id}`, roleData);
export const deleteRole = (id, endDate) =>
  instance.delete(`${Constants.API_PATHS.ROLE}/${id}`, { data: { id, endDate } });

export const getPermissions = () => instance.get(Constants.API_PATHS.PERMISSIONS);

export const getSubmissionStreams = () => instance.get(Constants.API_PATHS.SUBMISSION_STREAMS);

export const searchSubmissions = params => instance.get(Constants.API_PATHS.SUBMISSIONS, { params: { ...params } });

export const getVersion = () => instance.get(Constants.API_PATHS.VERSION);
