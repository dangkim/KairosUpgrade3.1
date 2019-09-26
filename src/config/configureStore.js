// import { createStore, combineReducers } from 'redux';
// import reducers from '../_reducers';

// export default function configureStore() {
//   return createStore(
//     combineReducers({
//       ...reducers
//     }),
//     {},
//   );
// }

// import { createStore, applyMiddleware } from 'redux';
// import thunkMiddleware from 'redux-thunk';
// import { createLogger } from 'redux-logger';
// import rootReducer from '../_reducers';

// const loggerMiddleware = createLogger();

// export const configureStore = createStore(
//     rootReducer,
//     applyMiddleware(
//         thunkMiddleware,
//         loggerMiddleware
//     )
// );