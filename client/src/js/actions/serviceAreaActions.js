import * as api from '../Api';
import * as Constants from '../Constants';

import { FETCH_SERVICE_AREAS } from './types';

export const fetchServiceAreas = () => dispatch => {
  return api.instance.get(Constants.API_PATHS.SERVICE_AREAS).then(response => {
    const data = response.data;
    dispatch({ type: FETCH_SERVICE_AREAS, payload: data });
  });
};
