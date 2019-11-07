import { campaignConstants } from '../_constants';

export function campaign(state = {}, action) {
  switch (action.type) {
    case campaignConstants.CAM_REGISTER_REQUEST:
      return {
        loading: true
      };
    case campaignConstants.CAM_REGISTER_SUCCESS:
      return {
        loading: false,
        campaign: action.campaign
      };
    case campaignConstants.CAMS_GETALL_FAILURE:
      return {
        loading: false,
        error: action.error
      };
    case campaignConstants.CAMS_GETALL_REQUEST:
      return {
        loading: true
      };
    case campaignConstants.CAMS_GETALL_SUCCESS:
      return {
        loading: false,
        campaign: action.campaign.campaign
      };
    case campaignConstants.CAM_REGISTER_FAILURE:
      return {
        loading: false,
        error: action.error
      };
    case campaignConstants.CAMS_GETBY_BRAND_REQUEST:
      return {
        loading: true
      };
    case campaignConstants.CAMS_GETBY_BRAND_SUCCESS:
      return {
        loading: false,
        campaign: action.campaign.campaign
      };
    case campaignConstants.CAM_GETBY_BRAND_FAILURE:
      return {
        loading: false,
        error: action.error
      };
    default:
      return state
  }
}