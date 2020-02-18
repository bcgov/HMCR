import { combineReducers } from 'redux';

import codeLookupsReducer from './codeLookupsReducer';
import errorDialogReducer from './errorDialogReducer';
import serviceAreaReducer from './serviceAreaReducer';
import submissionReducer from './submissionReducer';
import userReducer from './userReducer';

export default combineReducers({
  codeLookups: codeLookupsReducer,
  errorDialog: errorDialogReducer,
  serviceAreas: serviceAreaReducer,
  submissions: submissionReducer,
  user: userReducer,
});
