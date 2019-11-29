import _ from 'lodash';

import {
  FETCH_CURRENT_USER,
  FETCH_USER_TYPES,
  FETCH_USER_STATUSES,
  SET_SINGLE_USER_SEARCH_CRITERIA,
  SEARCH_USERS,
} from '../actions/types';

const defaultState = {
  types: {},
  statuses: {},
  current: {},
  searchCriteria: {
    serviceAreas: null,
    userTypes: null,
    searchText: null,
    isActive: null,
    pageSize: 25,
    pageNumber: 1,
    orderBy: null,
  },
  searchResult: {},
  searchPagination: { currentPage: null, pageSize: null, pageCount: null, hasPreviousPage: null, hasNextPage: null },
};

export default (state = defaultState, action) => {
  switch (action.type) {
    case FETCH_CURRENT_USER:
      return { ...state, current: { ...state.current, ...action.payload } };
    case FETCH_USER_TYPES:
      return { ...state, types: { ...state.types, ..._.mapKeys(action.payload, 'id') } };
    case FETCH_USER_STATUSES:
      return { ...state, statuses: { ...state.statuses, ..._.mapKeys(action.payload, 'id') } };
    case SET_SINGLE_USER_SEARCH_CRITERIA:
      return { ...state, searchCriteria: { ...state.searchCriteria, [action.payload.key]: action.payload.value } };
    case SEARCH_USERS:
      const { hasPreviousPage, hasNextPage } = action.payload;
      const pageNumber = parseInt(action.payload.pageNumber);
      const pageSize = parseInt(action.payload.pageSize);
      const totalCount = parseInt(action.payload.totalCount);
      const pageCount = parseInt(action.payload.pageCount);
      return {
        ...state,
        searchResult: { ...action.payload.sourceList },
        searchPagination: { hasPreviousPage, hasNextPage, pageNumber, pageSize, totalCount, pageCount },
      };
    default:
      return state;
  }
};
