import { imageConstants } from '../_constants';
import { influencerService } from '../_services';

export const imageActions = {
    getImageData
};

function getImageData(url) {
    return dispatch => {
        dispatch(request(url));
        influencerService.getImageData(url)
            .then(
                imageData => dispatch(success(imageData)),
                error => {
                    toast.error("Please try again");
                }
            );
    };

    function request(url) { return { type: imageConstants.INFS_GET_IMAGEDATA_REQUEST, url } }
    function success(imageData) { return { type: imageConstants.INFS_GET_IMAGEDATA_SUCCESS, imageData } }
    function failure(error) { return { type: imageConstants.INFS_GET_IMAGEDATA_FAILURE, error } }
}