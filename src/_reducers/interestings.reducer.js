import { campaignConstants } from '../_constants';

export function interestings(state = {}, action) {
  switch (action.type) {    
    case campaignConstants.INTERESTING_GETALL_REQUEST:
      return {
        loading: true
      };
    case campaignConstants.INTERESTING_GETALL_SUCCESS:
      return {
        interestings: action.interestings
      };
    case campaignConstants.INTERESTING_GETALL_FAILURE:
      return { 
        error: action.error
      };
    default:
      return state
  }
}