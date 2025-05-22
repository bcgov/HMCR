import { configureStore } from '@reduxjs/toolkit';
import thunk from 'redux-thunk';

import reducers from './reducers';

const isDev = import.meta.env.NODE_ENV !== 'production';

const store = configureStore({
  reducer: reducers,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      thunk: true,
      immutableCheck: isDev,
      serializableCheck: false,
    }).concat(thunk),
  devTools: isDev,
});

export default store;
