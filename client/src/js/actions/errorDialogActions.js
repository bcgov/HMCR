import { SHOW_ERROR_DIALOG_MODAL, HIDE_ERROR_DIALOG_MODAL } from './types';

export const showErrorDialog = response => {
  const method = response.config.method.toUpperCase();
  const path = response.config.url.replace(response.config.baseURL, '');

  return {
    type: SHOW_ERROR_DIALOG_MODAL,
    payload: {
      message: response.data.title,
      statusCode: response.status,
      detail: response.data.detail,
      errors: response.data.errors,
      path,
      method,
    },
  };
};

export const hideErrorDialog = () => {
  return {
    type: HIDE_ERROR_DIALOG_MODAL,
  };
};
