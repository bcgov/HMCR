import queryString from 'query-string';
import moment from 'moment';
import _ from 'lodash';

import * as Constants from './Constants';

export const buildActionWithParam = (action, param) => {
  return { action, param };
};

export const buildApiErrorObject = (response) => {
  try {
    const method = response.config.method.toUpperCase();
    const path = response.config.url.replace(response.config.baseURL, '');

    return {
      message: response.data.title,
      statusCode: response.status,
      detail: response.data.detail,
      errors: response.data.errors,
      path,
      method,
    };
  } catch {
    return {
      message: 'Connection to server cannot be established',
    };
  }
};

export const updateQueryParamsFromHistory = (history, newParam, overwrite) => {
  const params = queryString.parse(history.location.search);

  let processedParams = { ..._.pickBy(newParam, _.identity) };
  Object.keys(processedParams).forEach((key) => {
    if (moment.isMoment(processedParams[key]))
      processedParams[key] = processedParams[key].format(Constants.DATE_DISPLAY_FORMAT);
  });

  if (!overwrite) processedParams = { ...params, ...processedParams };

  // remove empty isActive
  if (newParam.isActive === null) processedParams = _.omit(processedParams, ['isActive']);
  else processedParams = { ...processedParams, isActive: newParam.isActive };

  // remove empty searchText
  if (!newParam.searchText) processedParams = _.omit(processedParams, ['searchText']);

  return queryString.stringify(processedParams);
};

export const stringifyQueryParams = (newParam) => {
  return queryString.stringify(newParam);
};

export const buildStatusIdArray = (isActive) => {
  if (isActive === true || isActive === 'true') return [Constants.ACTIVE_STATUS.ACTIVE];

  if (isActive === false || isActive === 'false') return [Constants.ACTIVE_STATUS.INACTIVE];

  return [Constants.ACTIVE_STATUS.ACTIVE, Constants.ACTIVE_STATUS.INACTIVE];
};
