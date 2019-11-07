import { SHOW_ERROR_DIALOG_MODAL, HIDE_ERROR_DIALOG_MODAL } from './types';

export const showErrorDialog = response => {
  const method = response.config.method.toUpperCase();
  const path = response.config.url.replace(response.config.baseURL, '');

  return {
    type: SHOW_ERROR_DIALOG_MODAL,
    payload: {
      message: response.data.message,
      statusCode: response.status,
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
