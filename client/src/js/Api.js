import axios from 'axios';

import store from './store';

import { SHOW_ERROR_DIALOG_MODAL } from './actions/types';
import { buildApiErrorObject } from './utils';
import * as Constants from './Constants';

export const instance = axios.create({
    baseURL: `${Constants.API_URL}`,
    headers: {
        Pragma: 'no-cache',
    },
});

const CLIENT_SESSION_KEY = 'hmcrClientSessionId';

const randomId = () => {
    if (window.crypto && window.crypto.randomUUID) return window.crypto.randomUUID();
    return `${Date.now().toString(36)}-${Math.random().toString(36).substring(2, 12)}`;
};

const setHeader = (headers, name, value) => {
    if (headers.set) headers.set(name, value);
    else headers[name] = value;
};

const getClientSessionId = () => {
    let sessionId = sessionStorage.getItem(CLIENT_SESSION_KEY);
    if (!sessionId) {
        sessionId = `hmcr-session-${randomId()}`;
        sessionStorage.setItem(CLIENT_SESSION_KEY, sessionId);
    }

    return sessionId;
};

export const createSupportId = () => {
    const now = new Date();
    const date = now.toISOString().replace(/[-:]/g, '').replace(/\..+/, '');
    return `HMCR-${date.substring(0, 8)}-${date.substring(9, 15)}-${randomId().replace(/-/g, '').substring(0, 8).toUpperCase()}`;
};

const createCorrelationId = () => `hmcr-client-${randomId()}`;

export const reportClientError = (error, details = {}) => {
    const supportId = details.supportId || createSupportId();
    const payload = {
        supportId,
        errorCode: details.errorCode || 'HMCR-CLIENT-UNEXPECTED',
        message: details.message || error?.message || 'Client error',
        stack: details.stack || error?.stack,
        componentStack: details.componentStack,
        route: window.location.pathname,
        clientSessionId: getClientSessionId(),
        clientTraceId: details.clientTraceId || createCorrelationId(),
        correlationId: details.correlationId,
        httpMethod: details.httpMethod,
        url: details.url || window.location.href,
        statusCode: details.statusCode,
        userAgent: navigator.userAgent,
        appVersion: Constants.APP_VERSION,
        timestampUtc: new Date().toISOString(),
    };

    instance
        .post(Constants.API_PATHS.CLIENT_LOGS, payload, {
            skipClientErrorDialog: true,
            skipClientErrorLog: true,
        })
        .catch(() => {});

    return supportId;
};

export const registerGlobalClientErrorHandlers = () => {
    const onError = (event) => {
        reportClientError(event.error, {
            message: event.message,
            url: event.filename,
        });
    };

    const onUnhandledRejection = (event) => {
        const reason = event.reason;
        reportClientError(reason, {
            message: reason?.message || 'Unhandled promise rejection',
            stack: reason?.stack,
        });
    };

    window.addEventListener('error', onError);
    window.addEventListener('unhandledrejection', onUnhandledRejection);

    return () => {
        window.removeEventListener('error', onError);
        window.removeEventListener('unhandledrejection', onUnhandledRejection);
    };
};

instance.interceptors.request.use((config) => {
    const correlationId = config.correlationId || createCorrelationId();
    config.headers = config.headers || {};
    config.metadata = { ...(config.metadata || {}), correlationId };
    setHeader(config.headers, Constants.CORRELATION_HEADER, correlationId);

    return config;
});

instance.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        const apiError = buildApiErrorObject(error);

        if (!error.response && !error.config?.skipClientErrorLog) {
            apiError.supportId = reportClientError(error, {
                message: apiError.message,
                correlationId: error.config?.metadata?.correlationId,
                httpMethod: error.config?.method?.toUpperCase(),
                url: error.config?.url,
            });
        }

        if (!error.config?.skipClientErrorDialog && (!error.response || error.response.status !== 422))
            store.dispatch({ type: SHOW_ERROR_DIALOG_MODAL, payload: apiError });

        return Promise.reject(error);
    }
);

export const getCurrentUser = () => instance.get(Constants.API_PATHS.USER_CURRENT);
export const getUser = (id) => instance.get(`${Constants.API_PATHS.USER}/${id}`);
export const getUserStatuses = () => instance.get(Constants.API_PATHS.USER_STATUSES);
export const getUserTypes = () => instance.get(Constants.API_PATHS.USER_TYPES);
export const postUser = (userData) => instance.post(Constants.API_PATHS.USER, userData);
export const putUser = (id, userData) => instance.put(`${Constants.API_PATHS.USER}/${id}`, userData);
export const deleteUser = (id, endDate) =>
    instance.delete(`${Constants.API_PATHS.USER}/${id}`, { data: { id, endDate } });
export const searchUsers = (params) => instance.get(Constants.API_PATHS.USER, { params: {...params } });
export const getUserBceidAccount = (userType, username) =>
    instance.get(`${Constants.API_PATHS.USER_BCEID_ACCOUNT}/${userType}/${username}`);
export const getUserReportExport = (params) => instance.get(Constants.API_PATHS.USER_REPORT_EXPORT, { params: {...params } });

export const getRoles = () => instance.get(Constants.API_PATHS.ROLE);
export const getRole = (id) => instance.get(`${Constants.API_PATHS.ROLE}/${id}`);
export const searchRoles = (params) => instance.get(Constants.API_PATHS.ROLE, { params: {...params } });
export const postRole = (roleData) => instance.post(Constants.API_PATHS.ROLE, roleData);
export const putRole = (id, roleData) => instance.put(`${Constants.API_PATHS.ROLE}/${id}`, roleData);
export const deleteRole = (id, endDate) =>
    instance.delete(`${Constants.API_PATHS.ROLE}/${id}`, { data: { id, endDate } });

export const getPermissions = () => instance.get(Constants.API_PATHS.PERMISSIONS);

export const getSubmissionStreams = () => instance.get(Constants.API_PATHS.SUBMISSION_STREAMS);
export const searchSubmissions = (params) => instance.get(Constants.API_PATHS.SUBMISSIONS, { params: {...params } });
export const getSubmissionResult = (id) => instance.get(`${Constants.API_PATHS.SUBMISSIONS}/${id}/result`);
export const getSubmissionFile = (id) =>
    instance.get(`${Constants.API_PATHS.SUBMISSIONS}/${id}/file`, { responseType: 'blob' });

export const getMaintenanceTypes = () => instance.get(Constants.API_PATHS.MAINTENANCE_TYPES);
export const getUnitOfMeasures = () => instance.get(Constants.API_PATHS.UNIT_OF_MEASURES);
export const getFeatureTypes = () => instance.get(Constants.API_PATHS.FEATURE_TYPES);
export const getLocationCodes = () => instance.get(Constants.API_PATHS.LOCATION_CODE);
export const getThresholdLevels = () => instance.get(Constants.API_PATHS.THRESHOLD_LEVELS);

export const searchActivityCodes = (params) =>
    instance.get(Constants.API_PATHS.ACTIVITY_CODES, { params: {...params } });
export const getActivityCodesLite = () => instance.get(Constants.API_PATHS.ACTIVITY_CODES_LITE);
export const getActivityCode = (id) => instance.get(`${Constants.API_PATHS.ACTIVITY_CODES}/${id}`);
export const postActivityCode = (data) => instance.post(Constants.API_PATHS.ACTIVITY_CODES, data);
export const putActivityCode = (id, data) => instance.put(`${Constants.API_PATHS.ACTIVITY_CODES}/${id}`, data);
export const deleteActivityCode = (id) => instance.delete(`${Constants.API_PATHS.ACTIVITY_CODES}/${id}`);
export const getActivityCodeExport = (params) => instance.get(Constants.API_PATHS.ACTIVITY_CODES_EXPORT, { params: {...params } });

export const getRoadLengthRules = () => instance.get(Constants.API_PATHS.RULE_ROAD_LENGTH);
export const getSurfaceTypeRules = () => instance.get(Constants.API_PATHS.RULE_SURFACE_TYPE);
export const getRoadClassRules = () => instance.get(Constants.API_PATHS.RULE_ROAD_CLASS);

export const getReportExport = (params) => instance.get(Constants.API_PATHS.REPORT_EXPORT, { params: {...params } });
export const getSaltReports = (params) => instance.get(Constants.API_PATHS.SALT_REPORT, { params: {...params }, responseType: 'blob' });
export const getSaltReportsJson = (params) => instance.get(Constants.API_PATHS.SALT_REPORT, { params: {...params }});
export const getExportSupportedFormats = () => instance.get(Constants.API_PATHS.SUPPORTED_FORMATS);

export const getApiClient = () => instance.get(`${Constants.API_PATHS.USER}/api-client`);
export const createApiClient = () => instance.post(`${Constants.API_PATHS.USER}/api-client`);
export const resetApiClientSecret = () => instance.post(`${Constants.API_PATHS.USER}/api-client/secret`);

export const getVersion = () => instance.get(Constants.API_PATHS.VERSION);

export const getSaltReportById = async (id, params) => {
    try {
      const response = await instance.get(`${Constants.API_PATHS.SALT_REPORT}/${id}`, {
        params: { ...params },
        responseType: 'blob',
      });
  
      // Trigger download
      if (params.isPdf) {
        const blob = new Blob([response.data], { type: 'application/pdf' });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `salt_report_${id}.pdf`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      } else {
        // Return JSON for other cases
        return response.data;
      }
    } catch (error) {
      if (!error.response) {
        reportClientError(error, {
          message: 'Error fetching salt report',
          httpMethod: 'GET',
          url: `${Constants.API_PATHS.SALT_REPORT}/${id}`,
          correlationId: error.config?.metadata?.correlationId,
        });
      }
      throw error;
    }
  };
