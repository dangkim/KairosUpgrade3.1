import { imageConstants } from '../_constants';

export function image(state = {}, action) {
  switch (action.type) {
    case imageConstants.INFS_GET_IMAGEDATA_REQUEST:
      return {
        loading: true
      };

    case imageConstants.INFS_GET_IMAGEDATA_SUCCESS:
      return {
        loading: false,
        imageData: action.imageData
      };

    case imageConstants.INFS_GET_IMAGEDATA_FAILURE:
      return {
        loading: false,
        error: action.error
      };

    default:
      return state
  }
}
