import { applyMiddleware, createStore, compose } from 'redux';
import logger from 'redux-logger';
import thunk from 'redux-thunk';
import promise from 'redux-promise';

import config from './config';
import reducers from './reducers';

let middleware = null;

console.log(`running in ${config.NODE_ENV} environment`);

if (['development', 'staging'].indexOf(config.NODE_ENV) >= 0) {
	middleware = applyMiddleware(promise, thunk, logger);
} else {
	middleware = applyMiddleware(promise, thunk);
}

const store = createStore(reducers, compose(middleware));

export default store;
