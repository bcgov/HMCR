// import _ from 'lodash';

import { SEARCH_ROLES, SET_SINGLE_ROLE_SEARCH_CRITERIA } from '../actions/types';

const defaultState = {
  list: {},
  statuses: { ACTIVE: { id: 'ACTIVE', name: 'ACTIVE' }, INACTIVE: { id: 'INACTIVE', name: 'INACTIVE' } },
  searchCriteria: {
    searchText: null,
    isActive: null,
  },
};

export default (state = defaultState, action) => {
  switch (action.type) {
    case SEARCH_ROLES:
      return {
        ...state,
        list: { ...action.payload },
      };
    case SET_SINGLE_ROLE_SEARCH_CRITERIA:
      return { ...state, searchCriteria: { ...state.searchCriteria, [action.payload.key]: action.payload.value } };
    default:
      return state;
  }
};
