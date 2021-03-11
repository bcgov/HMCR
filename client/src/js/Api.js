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

instance.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        if (!error.response || error.response.status !== 422)
            store.dispatch({ type: SHOW_ERROR_DIALOG_MODAL, payload: buildApiErrorObject(error.response) });

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
export const getExportSupportedFormats = () => instance.get(Constants.API_PATHS.SUPPORTED_FORMATS);

export const getApiClient = () => instance.get(`${Constants.API_PATHS.USER}/api-client`);
export const createApiClient = () => instance.post(`${Constants.API_PATHS.USER}/api-client`);
export const resetApiClientSecret = () => instance.post(`${Constants.API_PATHS.USER}/api-client/secret`);

export const getVersion = () => instance.get(Constants.API_PATHS.VERSION);