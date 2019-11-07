export const API_URL = process.env.REACT_APP_API_HOST
  ? process.env.REACT_APP_API_HOST
  : `${window.location.protocol}//${window.location.host}/api`;

export const API_PATHS = { PROVINCE: '/provinces', COUNTRY: '/countries', USER: '/users' };

export const PATHS = {
  UNAUTHORIZED: '/unauthorized',
  HOME: '/',
  ABOUT: '/admin/about',
  WORK_REPORTING: '/workreporting',
  ADMIN: '/admin',
  ADMIN_ACTIVITIES: '/admin/activities',
  ADMIN_USERS: '/admin/users',
  ADMIN_ROLES: '/admin/roles',
  HELLOWORLD: '/helloworld',
};

export const MESSAGE_DATE_FORMAT = 'YYYY-MM-DD hh:mmA';

export const FORM_TYPE = { ADD: 'ADD_FORM', EDIT: 'EDIT_FORM' };

export const PERMISSIONS = {
  ADMIN: 'ADMIN_PERMISSION',
  CONTRACTOR: 'CONTRACTOR_PERMISSION',
  UPLOAD: 'UPLOAD_PERMISSION',
};
