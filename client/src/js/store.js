import { createStore, applyMiddleware } from 'redux';
import { composeWithDevTools } from 'redux-devtools-extension/developmentOnly';
import reduxThunk from 'redux-thunk';

import reducers from './reducers';

const middleware =
  process.env.NODE_ENV !== 'production'
    ? [require('redux-immutable-state-invariant').default(), reduxThunk]
    : [reduxThunk];

const store = createStore(reducers, composeWithDevTools(applyMiddleware(...middleware)));

export default store;
