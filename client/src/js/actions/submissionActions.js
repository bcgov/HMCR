import * as api from '../Api';
import * as Constants from '../Constants';

import { FETCH_SUBMISSION_STATUSES } from './types';

export const fetchSubmissionStatuses = () => dispatch => {
  return api.instance.get(Constants.API_PATHS.SUBMISSION_STATUS).then(response => {
    const data = response.data;
    dispatch({ type: FETCH_SUBMISSION_STATUSES, payload: data });
  });
};
