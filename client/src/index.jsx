import React, { lazy, Suspense } from 'react';
import { createRoot } from 'react-dom/client';
import { Provider } from 'react-redux';

/* eslint-disable import/first */
const App = lazy(() => import('./js/App'));

import store from './js/store';
import * as Keycloak from './js/Keycloak';

Keycloak.init(() => {
  const root = createRoot(document.getElementById('root'));
  root.render(
    <Provider store={store}>
      <Suspense fallback={<div></div>}>
        <App />
      </Suspense>
    </Provider>
  );
});
