import { lazy, Suspense } from 'react';
import { createRoot } from 'react-dom/client';
import { Provider } from 'react-redux';

import * as Keycloak from './js/Keycloak';
import store from './js/store';

// Lazy-load your main App component
const App = lazy(() => import('./js/App'));

// Wait for Keycloak before rendering
Keycloak.init(() => {
  const rootEl = document.getElementById('root');

  createRoot(rootEl).render(
    <Provider store={store}>
      <Suspense fallback={<div></div>}>
        <App />
      </Suspense>
    </Provider>,
  );
});
