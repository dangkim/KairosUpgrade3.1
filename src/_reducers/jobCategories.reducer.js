import { infConstants } from '../_constants';

export function jobCategories(state = {}, action) {
  switch (action.type) {    
    case infConstants.JOBCATEGORY_GETALL_REQUEST:
      return {
        loading: true
      };
    case infConstants.JOBCATEGORY_GETALL_SUCCESS:
      return {
        jobCategories: action.jobCategories
      };
    case infConstants.JOBCATEGORY_GETALL_FAILURE:
      return { 
        error: action.error
      };    
    default:
      return state
  }
}