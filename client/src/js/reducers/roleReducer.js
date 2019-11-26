import _ from 'lodash';

import { FETCH_ROLES } from '../actions/types';

const defaultState = {};

export default (state = defaultState, action) => {
  switch (action.type) {
    case FETCH_ROLES:
      return { ...state, ..._.mapKeys(action.payload, 'id') };
    default:
      return state;
  }
};
