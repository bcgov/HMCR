import _ from 'lodash';

import { FETCH_COUNTRIES, FETCH_PROVINCES } from '../actions/types';

const defaultState = { countries: {}, provinces: {} };

export default (state = defaultState, action) => {
  switch (action.type) {
    case FETCH_COUNTRIES:
      return { ...state, countries: { ...state.countries, ..._.mapKeys(action.payload, 'id') } };
    case FETCH_PROVINCES:
      return { ...state, provinces: { ...state.provinces, ..._.mapKeys(action.payload, 'id') } };
    default:
      return state;
  }
};
