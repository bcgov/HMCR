import _ from 'lodash';

import { FETCH_CURRENT_USER, FETCH_USER_TYPES, FETCH_USER_STATUSES } from '../actions/types';

const defaultState = {
  types: {},
  statuses: {},
  current: {},
};

const userReducer = (state = defaultState, action) => {
  switch (action.type) {
    case FETCH_CURRENT_USER:
      return { ...state, current: { ...state.current, ...action.payload } };
    case FETCH_USER_TYPES:
      return { ...state, types: { ...state.types, ..._.mapKeys(action.payload, 'id') } };
    case FETCH_USER_STATUSES:
      return { ...state, statuses: { ...state.statuses, ..._.mapKeys(action.payload, 'id') } };
    default:
      return state;
  }
};

export default userReducer;
