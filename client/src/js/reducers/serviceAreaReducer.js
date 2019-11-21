import _ from 'lodash';

import { FETCH_SERVICE_AREAS } from '../actions/types';

const defaultState = {};

export default (state = defaultState, action) => {
  switch (action.type) {
    case FETCH_SERVICE_AREAS:
      return { ...state, ..._.mapKeys(action.payload, 'id') };
    default:
      return state;
  }
};
