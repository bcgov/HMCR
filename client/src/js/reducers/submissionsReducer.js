import moment from 'moment';

import { SEARCH_SUBMISSIONS, SET_SINGLE_SUBMISSIONS_SEARCH_CRITERIA } from '../actions/types';

import * as Constants from '../Constants';

const defaultState = {
  list: {},
  searchCriteria: {
    dateFrom: moment()
      .subtract(1, 'months')
      .format(Constants.DATE_FORMAT),
    dateTo: moment().format(Constants.DATE_FORMAT),
    serviceAreaNumber: null,
    pageSize: Constants.DEFAULT_PAGE_SIZE,
    pageNumber: 1,
  },
  searchPagination: { currentPage: null, pageSize: null, pageCount: null, hasPreviousPage: null, hasNextPage: null },
};

export default (state = defaultState, action) => {
  switch (action.type) {
    case SEARCH_SUBMISSIONS:
      const { hasPreviousPage, hasNextPage } = action.payload;
      const pageNumber = parseInt(action.payload.pageNumber);
      const pageSize = parseInt(action.payload.pageSize);
      const totalCount = parseInt(action.payload.totalCount);
      const pageCount = parseInt(action.payload.pageCount);
      return {
        ...state,
        list: { ...action.payload.sourceList },
        searchPagination: { hasPreviousPage, hasNextPage, pageNumber, pageSize, totalCount, pageCount },
      };
    case SET_SINGLE_SUBMISSIONS_SEARCH_CRITERIA:
      return { ...state, searchCriteria: { ...state.searchCriteria, [action.payload.key]: action.payload.value } };
    default:
      return state;
  }
};
