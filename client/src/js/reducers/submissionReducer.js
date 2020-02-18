import _ from 'lodash';

import { FETCH_SUBMISSION_STATUSES } from '../actions/types';

const defaultState = {
  statuses: {},
};

export default (state = defaultState, action) => {
  switch (action.type) {
    case FETCH_SUBMISSION_STATUSES:
      return { ...state, statuses: { ..._.mapKeys(action.payload, 'statusCode') } };
    default:
      return state;
  }
};
