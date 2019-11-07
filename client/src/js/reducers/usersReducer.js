import { FETCH_USER } from '../actions/types';

const defaultState = {};

export default (state = defaultState, action) => {
  switch (action.type) {
    case FETCH_USER:
      return { ...state, [action.payload.id]: action.payload };
    default:
      return state;
  }
};
