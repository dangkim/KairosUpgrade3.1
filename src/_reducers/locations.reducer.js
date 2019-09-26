import { campaignConstants } from '../_constants';

export function locations(state = {}, action) {
  switch (action.type) {    
    case campaignConstants.LOCATION_GETALL_REQUEST:
      return {
        loading: true
      };
    case campaignConstants.LOCATION_GETALL_SUCCESS:
      return {
        locations: action.locations
      };
    case campaignConstants.LOCATION_GETALL_FAILURE:
      return { 
        error: action.error
      };    
    default:
      return state
  }
}