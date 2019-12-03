import * as api from '../Api';

import { SEARCH_ROLES, SET_SINGLE_ROLE_SEARCH_CRITERIA } from './types';

// export const fetchRoles = () => dispatch => {
//   return api.getRoles().then(response => {
//     const data = response.data;
//     dispatch({ type: FETCH_ROLES, payload: data });
//   });
// };

export const searchRoles = () => (dispatch, getState) => {
  const params = { ...getState().roles.searchCriteria };

  return api.searchRoles(params).then(response => {
    const data = response.data;
    dispatch({ type: SEARCH_ROLES, payload: data });
  });
};

export const setSingleRoleSeachCriteria = (key, value) => {
  return {
    type: SET_SINGLE_ROLE_SEARCH_CRITERIA,
    payload: { key, value },
  };
};
