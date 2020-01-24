import queryString from 'query-string';
import moment from 'moment';

import * as Constants from './Constants';

export const buildActionWithParam = (action, param) => {
  return { action, param };
};

export const buildApiErrorObject = response => {
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

export const updateQueryParamsFromHistory = (history, newParam) => {
  const params = queryString.parse(history.location.search);

  const processedParams = { ...newParam };
  Object.keys(processedParams).forEach(key => {
    if (moment.isMoment(processedParams[key]))
      processedParams[key] = processedParams[key].format(Constants.DATE_DISPLAY_FORMAT);
  });

  return queryString.stringify({ ...params, ...processedParams });
};

export const stringifyQueryParams = newParam => {
  return queryString.stringify(newParam);
};
