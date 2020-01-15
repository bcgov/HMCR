import { SEARCH_SUBMISSIONS } from '../actions/types';

const defaultState = {
  list: {},
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
    default:
      return state;
  }
};
