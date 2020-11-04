export const API_URL = window.RUNTIME_REACT_APP_API_HOST
  ? `${window.location.protocol}//${window.RUNTIME_REACT_APP_API_HOST}/api` //For non-dev environments, CORS is enabled to HMCR urls.
  : `${window.location.protocol}//${process.env.REACT_APP_CLINET_ORIGIN}/api`; //For dev environment, proxy (setupProxy.js) is set up to avoid the same origin policy

const CODE_LOOKUP = '/codelookup';

export const API_PATHS = {
  ACTIVITY_CODES: '/activitycodes',
  ACTIVITY_CODES_LITE: '/activitycodes/lite',
  CODE_LOOKUP: CODE_LOOKUP,
  MAINTENANCE_TYPES: `${CODE_LOOKUP}/maintenancetypes`,
  UNIT_OF_MEASURES: `${CODE_LOOKUP}/unitofmeasures`,
  FEATURE_TYPES: `${CODE_LOOKUP}/featuretypes`,
  THRESHOLD_LEVELS: `${CODE_LOOKUP}/thresholdlevels`,
  LOCATION_CODE: '/locationcodes',
  PERMISSIONS: '/permissions',
  ROLE: '/roles',
  SERVICE_AREAS: '/serviceareas',
  USER: '/users',
  USER_CURRENT: '/users/current',
  USER_TYPES: '/users/usertypes',
  USER_STATUSES: '/users/userstatus',
  USER_BCEID_ACCOUNT: '/users/bceidaccount',
  ROCKFALL_REPORT: '/rockfallreports',
  WILDLIFE_REPORT: '/wildlifereports',
  WORK_REPORT: '/workreports',
  SUBMISSIONS: '/submissionobjects',
  SUBMISSION_STATUS: '/submissionstatus',
  SUBMISSION_STREAMS: '/submissionstreams',
  REPORT_EXPORT: '/exports/report',
  SUPPORTED_FORMATS: '/exports/supportedformats',
  VERSION: '/version',
};

export const REPORT_TYPES = {
  HMR_WORK_REPORT: { api: API_PATHS.WORK_REPORT, name: 'HMR_WORK_REPORT' },
  HMR_ROCKFALL_REPORT: { api: API_PATHS.ROCKFALL_REPORT, name: 'HMR_ROCKFALL_REPORT' },
  HMR_WILDLIFE_REPORT: { api: API_PATHS.WILDLIFE_REPORT, name: 'HMR_WILDLIFE_REPORT' },
};

export const PATHS = {
  UNAUTHORIZED: '/unauthorized',
  HOME: '/',
  ABOUT: '/admin/about',
  API_ACCESS: '/admin/api-access',
  WORK_REPORTING: '/workreporting',
  ADMIN: '/admin',
  ADMIN_ACTIVITIES: '/admin/activities',
  ADMIN_USERS: '/admin/users',
  ADMIN_ROLES: '/admin/roles',
  REPORT_EXPORT: '/export',
  VERSION: '/version',
  DATA_ROOM: 'https://engineering.sp.th.gov.bc.ca/sites/service-areas-portal',
  MANUAL_AND_TEMPLATES:
    'https://www2.gov.bc.ca/gov/content/transportation/transportation-infrastructure/contracting-to-transportation/highway-bridge-maintenance/highway-maintenance/reporting',
};

export const MESSAGE_DATE_FORMAT = 'YYYY-MM-DD hh:mmA';

export const DATE_DISPLAY_FORMAT = 'YYYY-MM-DD';

export const DATE_UTC_FORMAT = 'YYYY-MM-DDTHH:mm';

export const FORM_TYPE = { ADD: 'ADD_FORM', EDIT: 'EDIT_FORM' };

export const PERMISSIONS = {
  CODE_W: 'CODE_W',
  CODE_R: 'CODE_R',
  USER_W: 'USER_W',
  USER_R: 'USER_R',
  ROLE_W: 'ROLE_W',
  ROLE_R: 'ROLE_R',
  FILE_W: 'FILE_W',
  FILE_R: 'FILE_R',
  EXPORT: 'EXPORT',
};

export const USER_TYPE = { INTERNAL: 'INTERNAL', BUSINESS: 'BUSINESS' };

export const UPLOAD_STATE = {
  RESUB_CHECK: 'RESUB_CHECK',
  SAVING: 'SAVING',
  ERROR: 'ERROR',
  COMPLETE: 'COMPLETE',
};

export const UPLOAD_STATE_STATUS = {
  START: 'START',
  COMPLETE: 'COMPLETE',
  ERROR: 'ERROR',
  WARNING: 'WARNING',
};

export const ACTIVE_STATUS = {
  ACTIVE: 'ACTIVE',
  INACTIVE: 'INACTIVE',
};

export const ACTIVE_STATUS_ARRAY = Object.keys(ACTIVE_STATUS).map((key) => ({
  id: ACTIVE_STATUS[key],
  name: ACTIVE_STATUS[key],
}));

export const SORT_DIRECTION = {
  ASCENDING: 'asc',
  DESCENDING: 'desc',
};

export const DEFAULT_PAGE_SIZE_OPTIONS = process.env.REACT_APP_DEFAULT_PAGE_SIZE_OPTIONS.split(',').map((o) =>
  parseInt(o)
);

export const DEFAULT_PAGE_SIZE = parseInt(process.env.REACT_APP_DEFAULT_PAGE_SIZE);

export const REPORT_EXPORT_FIELDS = {
  HMR_WORK_REPORT: [
    'RECORD_TYPE',
    'SERVICE_AREA',
    'RECORD_NUMBER',
    'ACTIVITY_NUMBER',
    'ACTIVITY_NAME',
    'START_DATE',
    'END_DATE',
    'ACCOMPLISHMENT',
    'UNIT_OF_MEASURE',
    'POSTED_DATE',
    'HIGHWAY_UNIQUE',
    'HIGHWAY_UNIQUE_NAME',
    'HIGHWAY_UNIQUE_LENGTH',
    'LANDMARK',
    'START_OFFSET',
    'END_OFFSET',
    'START_LATITUDE',
    'START_LONGITUDE',
    'START_VARIANCE',
    'END_LATITUDE',
    'END_LONGITUDE',
    'END_VARIANCE',
    'WORK_LENGTH',
    'STRUCTURE_NUMBER',
    'SITE_NUMBER',
    'VALUE_OF_WORK',
    'COMMENTS',
    'SUBMISSION_OBJECT_ID',
  ],
  HMR_WILDLIFE_REPORT: [
    'RECORD_TYPE',
    'SERVICE_AREA',
    'ACCIDENT_DATE',
    'TIME_OF_KILL',
    'LATITUDE',
    'LONGITUDE',
    'SPATIAL_VARIANCE',
    'HIGHWAY_UNIQUE',
    'LANDMARK',
    'OFFSET',
    'NEAREST_TOWN',
    'WILDLIFE_SIGN',
    'QUANTITY',
    'SPECIES',
    'SEX',
    'AGE',
    'COMMENT',
    'SUBMISSION_OBJECT_ID',
  ],
  HMR_ROCKFALL_REPORT: [
    'MCRR_INCIDENT_NUMBER',
    'RECORD_TYPE',
    'SERVICE_AREA',
    'ESTIMATED_ROCKFALL_DATE',
    'ESTIMATED_ROCKFALL_TIME',
    'START_LATITUDE',
    'START_LONGITUDE',
    'START_VARIANCE',
    'END_LATITUDE',
    'END_LONGITUDE',
    'END_VARIANCE',
    'HIGHWAY_UNIQUE',
    'HIGHWAY_UNIQUE_NAME',
    'LANDMARK',
    'LANDMARK_NAME',
    'START_OFFSET',
    'END_OFFSET',
    'DIRECTION_FROM_LANDMARK',
    'LOCATION_DESCRIPTION',
    'DITCH_VOLUME',
    'TRAVELLED_LANES_VOLUME',
    'OTHER_DITCH_VOLUME',
    'OTHER_TRAVELLED_LANES_VOLUME',
    'HEAVY_PRECIP',
    'FREEZE_THAW',
    'DITCH_SNOW_ICE',
    'VEHICLE_DAMAGE',
    'COMMENTS',
    'REPORTER_NAME',
    'MC_PHONE_NUMBER',
    'MC_NAME',
    'REPORT_DATE',
    'SUBMISSION_OBJECT_ID',
  ],
};
