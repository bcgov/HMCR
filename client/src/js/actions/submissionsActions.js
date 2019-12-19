import * as api from '../Api';

import { SEARCH_SUBMISSIONS, SET_SINGLE_SUBMISSIONS_SEARCH_CRITERIA } from './types';

export const searchSubmissions = () => (dispatch, getState) => {
  const params = { ...getState().submissions.searchCriteria };

  return api.searchSubmissions(params).then(response => {
    const data = response.data;
    dispatch({ type: SEARCH_SUBMISSIONS, payload: data });
  });
};

export const setSingleSubmissionsSeachCriteria = (key, value) => {
  return {
    type: SET_SINGLE_SUBMISSIONS_SEARCH_CRITERIA,
    payload: { key, value },
  };
};
