import { SHOW_ERROR_DIALOG_MODAL, HIDE_ERROR_DIALOG_MODAL } from './types';

export const showValidationErrorDialog = (errors) => {
  // Do not show validation error if not status 422
  if (errors.status !== 422) return { type: '' };

  return {
    type: SHOW_ERROR_DIALOG_MODAL,
    payload: {
      title: 'Validation Failed',
      message: 'The server has failed to validate the submitted data.  Please see below for details.',
      errors: errors.errors,
      hidePrimaryButton: true,
    },
  };
};

export const hideErrorDialog = () => {
  return {
    type: HIDE_ERROR_DIALOG_MODAL,
  };
};
