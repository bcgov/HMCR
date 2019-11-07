import { combineReducers } from 'redux';

import errorDialogReducer from './errorDialogReducer';
import userReducer from './userReducer';
import helloReducer from './helloReducer';

export default combineReducers({
  errorDialog: errorDialogReducer,
  user: userReducer,
  hello: helloReducer,
});
