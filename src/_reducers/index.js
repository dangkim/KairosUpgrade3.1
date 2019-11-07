// import ThemeOptions from './ThemeOptions';

// export default {
//     ThemeOptions
// };
import { combineReducers } from 'redux';

import { authentication } from './authentication.reducer';
import { registration } from './registration.reducer';
import { users } from './users.reducer';
import { brands } from './brands.reducer';
import { influencers } from './influencers.reducer';
import { campaign } from './campaign.reducer';
import { locations } from './locations.reducer';
import { interestings } from './interestings.reducer';
import { jobCategories } from './jobCategories.reducer';
import { alert } from './alert.reducer';
import { theme } from './theme.reducer';

const rootReducer = combineReducers({
  authentication,
  registration,
  users,
  brands,
  influencers,
  campaign,
  locations,
  interestings,
  jobCategories,
  alert,
  theme
});

export default rootReducer;