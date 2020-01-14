import moment from 'moment';

import * as api from '../Api';
import * as Constants from '../Constants';

import { SEARCH_SUBMISSIONS, SET_SINGLE_SUBMISSIONS_SEARCH_CRITERIA } from './types';

export const searchSubmissions = params => (dispatch, getState) => {
  // let params = { ...getState().submissions.searchCriteria };
  const searchParams = {
    ...params,
    dateTo: moment(params.dateTo)
      .add(1, 'days')
      .format(Constants.DATE_FORMAT),
    dateFrom: moment(params.dateFrom).format(Constants.DATE_FORMAT),
  };

  return api.searchSubmissions(searchParams).then(response => {
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
