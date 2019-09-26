import { campaignConstants } from '../_constants';

export function campaign(state = {}, action) {
  switch (action.type) {
    case campaignConstants.CAM_REGISTER_REQUEST:
      return {
        loading: true
      };
    case campaignConstants.CAM_REGISTER_SUCCESS:
      return {
        campaign: action.campaign
      };
    case campaignConstants.CAM_REGISTER_FAILURE:
      return { 
        error: action.error
      };
    default:
      return state
  }
}