import { brandConstants } from '../_constants';
import { brandService, userService } from '../_services';
import { alertActions } from '.';
import { history } from '../_helpers';
import { toast } from "react-toastify";

export const brandActions = {
    register,
    getAll,
    getBrandByName
};

function register(brandType, userType) {
    return dispatch => {
        dispatch(request(brandType));

        userService.register(userType)
            .then(user => {
                userService.getToken(userType.UserName, userType.Password)
                    .then(token => {
                        brandService.register(brandType)
                            .then(brand => {
                                dispatch(success(brand));
                                
                                // history.push({
                                //     pathname: '/widgets/dashboard-boxes',
                                //     state: { Brand: brand.Brand, type: "Brand" }
                                // })
                                history.replace({ pathname: '/widgets/dashboard-boxes', state: { Brand: brand.Brand, type: "Brand" } });
                                toast.success("Welcome " + userType.UserName + " Let create a Campaign");
                                //dispatch(alertActions.success('Registration successful'));
                            },
                                error => {
                                    dispatch(failure(error.toString()));
                                    dispatch(alertActions.error(error.toString()));
                                }
                            );
                    },
                        error => {
                            dispatch(failure(error.toString()));
                            dispatch(alertActions.error(error.toString()));
                        }
                    )

                //dispatch(alertActions.success('Registration User Successful'));
            },
                error => {
                    dispatch(failure(error.toString()));
                    //dispatch(alertActions.error(error.toString()));
                    toast.warn(userType.UserName + " is existed please use other emails");
                }
            );
    };

    function request(brand) { return { type: brandConstants.BRAND_REGISTER_REQUEST, brand } }
    function success(brand) { return { type: brandConstants.BRAND_REGISTER_SUCCESS, brand } }
    function failure(error) { return { type: brandConstants.BRAND_REGISTER_FAILURE, error } }
}

function getAll() {
    return dispatch => {
        dispatch(request());

        userService.getAll()
            .then(
                brands => dispatch(success(brands)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: brandConstants.BRANDS_GETALL_REQUEST } }
    function success(brands) { return { type: brandConstants.BRANDS_GETALL_SUCCESS, brands } }
    function failure(error) { return { type: brandConstants.BRANDS_GETALL_FAILURE, error } }
}

function getBrandByName(userName) {
    return dispatch => {
        dispatch(request());
        if (userName) {            
            brandService.getBrandByName(userName)
            .then(
                brand => dispatch(success(brand)),
                error => {
                    toast.warn(error.toString() + " Please login again");
                    history.replace({ pathname: '/pages/loginpage' });
                }
            )
            //dispatch(success(brand));
        }
        else {
            const error = "cannot get from brand";
            dispatch(failure(error.toString()));
            //dispatch(alertActions.error(error.toString()));
        }
    };

    function request() { return { type: brandConstants.GET_BRANDBYNAME_REQUEST } }
    function success(brand) { return { type: brandConstants.GET_BRANDBYNAME_SUCCESS, brand } }
    function failure(error) { return { type: brandConstants.GET_BRANDBYNAME_FAILURE, error } }
}