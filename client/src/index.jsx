import React, { lazy, Suspense } from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';

/* eslint-disable import/first */
const App = lazy(() => import('./js/App'));

import store from './js/store';
import * as Keycloak from './js/Keycloak';

Keycloak.init(() => {
  ReactDOM.render(
    <Provider store={store}>
      <Suspense fallback={<div></div>}>
        <App />
      </Suspense>
    </Provider>,
    document.getElementById('root')
  );
});
