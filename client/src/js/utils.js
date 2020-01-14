// import store from './store';
import queryString from 'query-string';

// import * as Constants from './Constants';

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

export const updateQueryParams = (history, newParam) => {
  const params = queryString.parse(history.location.search);

  return queryString.stringify({ ...params, ...newParam });
};
