import { createStore, applyMiddleware, compose } from 'redux';
import reduxThunk from 'redux-thunk';

import reducers from './reducers';

const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;

const middleware =
  process.env.NODE_ENV !== 'production'
    ? [require('redux-immutable-state-invariant').default(), reduxThunk]
    : [reduxThunk];

const store = createStore(reducers, composeEnhancers(applyMiddleware(...middleware)));

export default store;
