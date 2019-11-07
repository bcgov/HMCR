import axios from 'axios';

import store from './store';

import { SHOW_ERROR_DIALOG_MODAL } from './actions/types';
import { buildApiErrorObject } from './utils';
import { API_URL } from './Constants';

export const instance = axios.create({
  baseURL: `${API_URL}`,
  headers: { 'Access-Control-Allow-Origin': '*' },
});

instance.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    store.dispatch({ type: SHOW_ERROR_DIALOG_MODAL, payload: buildApiErrorObject(error.response) });
    return Promise.reject(error);
  }
);
