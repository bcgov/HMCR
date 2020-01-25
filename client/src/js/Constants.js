export const API_URL = window.RUNTIME_REACT_APP_API_HOST
  ? `${window.location.protocol}//${window.RUNTIME_REACT_APP_API_HOST}/api`
  : process.env.REACT_APP_API_HOST;

export const API_PATHS = {
  PERMISSIONS: '/permissions',
  ROLE: '/roles',
  SERVICE_AREAS: '/serviceareas',
  USER: '/users',
  USER_CURRENT: '/users/current',
  USER_TYPES: '/users/usertypes',
  USER_STATUSES: '/users/userstatus',
  USER_BCEID_ACCOUNT: '/users/bceidaccount',
  ROCKFALL_REPORT: '/rockfallreports',
  SUBMISSIONS: '/submissionobjects',
  SUBMISSION_STREAMS: '/submissionstreams',
  WILDLIFE_REPORT: '/wildlifereports',
  WORK_REPORT: '/workreports',
  VERSION: '/version',
};

export const PATHS = {
  UNAUTHORIZED: '/unauthorized',
  HOME: '/',
  ABOUT: '/admin/about',
  WORK_REPORTING: '/workreporting',
  ADMIN: '/admin',
  ADMIN_ACTIVITIES: '/admin/activities',
  ADMIN_USERS: '/admin/users',
  ADMIN_ROLES: '/admin/roles',
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

export const SORT_DIRECTION = {
  ASCENDING: 'asc',
  DESCENDING: 'desc',
};

export const DEFAULT_PAGE_SIZE_OPTIONS = process.env.REACT_APP_DEFAULT_PAGE_SIZE_OPTIONS.split(',').map(o =>
  parseInt(o)
);

export const DEFAULT_PAGE_SIZE = parseInt(process.env.REACT_APP_DEFAULT_PAGE_SIZE);
