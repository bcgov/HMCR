import { SHOW_ERROR_DIALOG_MODAL, HIDE_ERROR_DIALOG_MODAL } from '../actions/types';

const defaultState = {
  show: false,
  title: undefined,
  message: undefined,
  statusCode: undefined,
  detail: undefined,
  errors: undefined,
  path: undefined,
  method: undefined,
  hidePrimaryButton: false,
};

const errorDialogReducer = (state = defaultState, action) => {
  switch (action.type) {
    case SHOW_ERROR_DIALOG_MODAL:
      return { show: true, ...action.payload };
    case HIDE_ERROR_DIALOG_MODAL:
      return defaultState;
    default:
      return state;
  }
};

export default errorDialogReducer;
