import { combineReducers } from 'redux';

import errorDialogReducer from './errorDialogReducer';
import roleReducer from './roleReducer';
import serviceAreaReducer from './serviceAreaReducer';
import userReducer from './userReducer';

export default combineReducers({
  errorDialog: errorDialogReducer,
  roles: roleReducer,
  serviceAreas: serviceAreaReducer,
  user: userReducer,
});
