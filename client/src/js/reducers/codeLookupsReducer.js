import _ from 'lodash';

import {
  FETCH_MAINTENANCE_TYPES,
  FETCH_UNIT_OF_MEASURES,
  FETCH_FEATURE_TYPES,
  FETCH_LOCATION_CODES,
  FETCH_ACTIVITY_CODES_DROPDOWN,
} from '../actions/types';

const defaultState = {
  maintenanceTypes: [],
  unitOfMeasures: [],
  featureTypes: [],
  locationCodes: [],
  activityCodes: [],
};

export default (state = defaultState, action) => {
  switch (action.type) {
    case FETCH_MAINTENANCE_TYPES:
      return { ...state, maintenanceTypes: _.orderBy(action.payload, ['name']) };
    case FETCH_UNIT_OF_MEASURES:
      return { ...state, unitOfMeasures: _.orderBy(action.payload, ['name']) };
    case FETCH_LOCATION_CODES:
      return { ...state, locationCodes: _.orderBy(action.payload, ['name']) };
    case FETCH_FEATURE_TYPES:
      return { ...state, featureTypes: _.orderBy(action.payload, ['name']) };
    case FETCH_ACTIVITY_CODES_DROPDOWN:
      return { ...state, activityCodes: action.payload };
    default:
      return state;
  }
};
