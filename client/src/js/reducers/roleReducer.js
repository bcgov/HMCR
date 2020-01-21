// import _ from 'lodash';

import { SEARCH_ROLES, SET_SINGLE_ROLE_SEARCH_CRITERIA } from '../actions/types';

import * as Constants from '../Constants';

const defaultState = {
  list: {},
  statuses: { ACTIVE: { id: 'ACTIVE', name: 'ACTIVE' }, INACTIVE: { id: 'INACTIVE', name: 'INACTIVE' } },
  searchCriteria: {
    searchText: null,
    isActive: true,
    pageSize: Constants.DEFAULT_PAGE_SIZE,
    pageNumber: 1,
    orderBy: null,
  },
  searchPagination: {
    currentPage: null,
    pageSize: null,
    pageCount: null,
    hasPreviousPage: null,
    hasNextPage: null,
    totalCount: null,
  },
};

export default (state = defaultState, action) => {
  switch (action.type) {
    case SEARCH_ROLES:
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
    case SET_SINGLE_ROLE_SEARCH_CRITERIA:
      return { ...state, searchCriteria: { ...state.searchCriteria, [action.payload.key]: action.payload.value } };
    default:
      return state;
  }
};
