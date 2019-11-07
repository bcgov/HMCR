import { SHOW_ERROR_DIALOG_MODAL, HIDE_ERROR_DIALOG_MODAL } from '../actions/types';

const defaultState = {
  show: false,
  message: undefined,
  statusCode: undefined,
  path: undefined,
  method: undefined,
};

export default (state = defaultState, action) => {
  switch (action.type) {
    case SHOW_ERROR_DIALOG_MODAL:
      return { show: true, ...action.payload };
    case HIDE_ERROR_DIALOG_MODAL:
      return defaultState;
    default:
      return state;
  }
};
