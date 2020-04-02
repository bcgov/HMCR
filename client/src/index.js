import 'react-app-polyfill/ie11';
import 'react-app-polyfill/stable';

import React, { lazy, Suspense } from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';

import * as serviceWorker from './serviceWorker';

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

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
