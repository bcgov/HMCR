import * as api from '../Api';
import * as Constants from '../Constants';

import { FETCH_SUBMISSION_STATUSES, FETCH_SUBMISSION_STREAMS } from './types';

export const fetchSubmissionStatuses = () => dispatch => {
  return api.instance.get(Constants.API_PATHS.SUBMISSION_STATUS).then(response => {
    const data = response.data;
    dispatch({ type: FETCH_SUBMISSION_STATUSES, payload: data });
  });
};

export const fetchSubmissionStreams = () => dispatch => {
  return api.instance.get(Constants.API_PATHS.SUBMISSION_STREAMS).then(response => {
    const data = response.data;
    dispatch({ type: FETCH_SUBMISSION_STREAMS, payload: data });
  });
};
