import { combineReducers } from 'redux';

import errorDialogReducer from './errorDialogReducer';
import serviceAreaReducer from './serviceAreaReducer';
import userReducer from './userReducer';

export default combineReducers({
  errorDialog: errorDialogReducer,
  serviceAreas: serviceAreaReducer,
  user: userReducer,
});
