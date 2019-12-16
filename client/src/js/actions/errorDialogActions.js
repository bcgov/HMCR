import { SHOW_ERROR_DIALOG_MODAL, HIDE_ERROR_DIALOG_MODAL } from './types';

export const showValidationErrorDialog = errors => {
  return {
    type: SHOW_ERROR_DIALOG_MODAL,
    payload: {
      title: 'Validation Failed',
      message: 'The uploaded file has failed initial validations.  Please see below for details.',
      errors,
      hidePrimaryButton: true,
    },
  };
};

export const hideErrorDialog = () => {
  return {
    type: HIDE_ERROR_DIALOG_MODAL,
  };
};
