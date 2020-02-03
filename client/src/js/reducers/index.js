import { combineReducers } from 'redux';

import codeLookupsReducer from './codeLookupsReducer';
import errorDialogReducer from './errorDialogReducer';
import roleReducer from './roleReducer';
import serviceAreaReducer from './serviceAreaReducer';
import submissionsReducer from './submissionsReducer';
import userReducer from './userReducer';

export default combineReducers({
  codeLookups: codeLookupsReducer,
  errorDialog: errorDialogReducer,
  roles: roleReducer,
  serviceAreas: serviceAreaReducer,
  submissions: submissionsReducer,
  user: userReducer,
});
