import { infConstants } from '../_constants';

export function influencers(state = {}, action) {
  switch (action.type) {
    case infConstants.INF_REGISTER_REQUEST:
      return {
        loading: true
      };
    case infConstants.INF_REGISTER_SUCCESS:
      return {
        loading: false,
        influencer: action.influencer
      };
    case infConstants.INF_REGISTER_FAILURE:
      return {
        error: action.error
      };
    case infConstants.INF_UPDATE_REQUEST:
      return {
        loading: true
      };
    case infConstants.INF_UPDATE_SUCCESS:
      return {
        loading: false,
        influencer: action.influencer
      };
    case infConstants.INF_UPDATE_FAILURE:
      return {
        error: action.error
      };
    case infConstants.INFS_GETALL_REQUEST:
      return {
        loading: true
      };
    case infConstants.INFS_GETALL_SUCCESS:
      return {
        loading: false,
        items: action.influencers
      };
    case infConstants.INFS_GETALL_FAILURE:
      return {
        error: action.error
      };
    case infConstants.INFS_INFINITE_REQUEST:
      return {
        loading: true,
        items: action.previousValues
      };
    case infConstants.INFS_INFINITE_SUCCESS:
      return {
        loading: false,
        items: action.influencers
      };
    case infConstants.INFS_INFINITE_FAILURE:
      return {
        error: action.error
      };
    case infConstants.INFS_GETBYNAME_REQUEST:
      return {
        loading: true
      };
    case infConstants.INFS_GETBYNAME_SUCCESS:
      return {
        loading: false,
        items: action.influencers
      };
    case infConstants.INFS_GETBYNAME_FAILURE:
      return {
        error: action.error
      };
    case infConstants.JOB_REGISTER_REQUEST:
      return {
        loading: true
      };
    case infConstants.JOB_REGISTER_SUCCESS:
      return {
        jobs: action.jobs
      };
    case infConstants.JOB_REGISTER_FAILURE:
      return {
        error: action.error
      };
    case infConstants.INFS_GETCOST_FAILURE:
      return {
        error: action.error
      };
    case infConstants.INFS_GETCOST_SUCCESS:
      return {
        loading: false,
        influencer: action.influencer
      };
    case infConstants.INFS_GETCOST_REQUEST:
      return {
        loading: true
      };
    case infConstants.INFS_GETBYCATEGORY_FAILURE:
      return {
        error: action.error
      };
    case infConstants.INFS_GETBYCATEGORY_SUCCESS:
      return {
        loading: false,
        items: action.influencers
      };
    case infConstants.INFS_GETBYCATEGORY_REQUEST:
      return {
        loading: true,
        items: action.previousValues
      };
    case infConstants.INFS_GET_IMAGEDATA_FAILURE:
      return {
        error: action.error
      };
    case infConstants.INFS_GET_IMAGEDATA_SUCCESS:
      return {
        loading: false,
        imageData: action.imageData
      };
    case infConstants.INFS_GET_IMAGEDATA_REQUEST:
      return {
        loading: true,
        items: action.previousValues
      };
    default:
      return state
  }
}