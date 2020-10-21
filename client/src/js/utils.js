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

export const isValueEmpty=(v)=>{
  if(v === null || v === undefined || v === '') return true;
  return false;
};

export const isValueNotEmpty=(v)=>{
  if(v !== null && v !== undefined && v !== '') return true;
  return false;
};

export const toNumberOrNull=(v)=>{
  return isValueNotEmpty(v) ? _.toNumber(removeStringCommas(v)): null;
};
export const toStringOrEmpty=(v)=>{
  return isValueNotEmpty(v) ? _.toString(v): '';
};
export const toStringWithCommasOrEmpty=(v)=>{
  return isValueNotEmpty(v) ? _.toString(addCommasToNumber(v)): '';
};
export const removeStringCommas=(v)=>{
  return v.toString().replace(/,/g, '');
}
export const addCommasToNumber=(n) =>{
  if(isValueEmpty(n)) return n;
  let s = removeStringCommas(n).split('.');
  if (s[0].length >= 4) {
      s[0] = s[0].replace(/(\d)(?=(\d{3})+$)/g, '$1,');
  }
  return s.join('.');
}
export const isValidDecimal=(v,digits) =>{
  if(isValueEmpty(v)) return true;
  const d = _.toInteger(digits);
  let s = removeStringCommas(v).split('.');
  if(s.length <2) return true;
  if (s[1].length <= d) return true;
  return false;
}
