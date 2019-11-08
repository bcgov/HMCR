// import store from './store';

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

// export const isCurrentUserAdmin = () => {
//   const state = store.getState();
//   try {
//     const user = state.users.all[state.users.current.id];
//     const role = state.roles[user.roleId];

//     return role.name === Constants.ROLE.ADMIN;
//   } catch {
//     console.error('Unable to verify admin status.');
//   }

//   return false;
// };
