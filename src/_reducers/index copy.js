import { combineReducers } from 'redux';

import { authentication } from './authentication.reducer';
import { registration } from './registration.reducer';
import { users } from './users.reducer';
import { brands } from './brands.reducer';
import { influencers } from './influencers.reducer';
import { campaigns } from './campaigns.reducer';
import { locations } from './locations.reducer';
import { interestings } from './interestings.reducer';
import { jobCategories } from './jobCategories.reducer';
import { alert } from './alert.reducer';

const rootReducer = combineReducers({
  authentication,
  registration,
  users,
  brands,
  influencers,
  campaigns,
  locations,
  interestings,
  jobCategories,
  alert
});

export default rootReducer;