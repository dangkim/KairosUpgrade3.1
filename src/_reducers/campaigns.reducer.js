import { campaignConstants } from '../_constants';

export function campaigns(state = {}, action) {
  switch (action.type) {
    case campaignConstants.CAMS_REGISTER_REQUEST:
      return {
        loading: true
      };
    case campaignConstants.CAMS_REGISTER_SUCCESS:
      return {
        campaigns: action.campaigns
      };
    case campaignConstants.CAMS_REGISTER_FAILURE:
      return { 
        error: action.error
      };
    case campaignConstants.CAMS_GETALL_REQUEST:
      return {
        loading: true
      };
    case campaignConstants.CAMS_GETALL_SUCCESS:
      return {
        campaigns: action.campaigns
      };
    case campaignConstants.CAMS_GETALL_FAILURE:
      return { 
        error: action.error
      };    
    default:
      return state
  }
}