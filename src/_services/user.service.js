import configContent from 'configContent';
import configOrchardCore from 'configOrchardCore';
import { authHeader, history } from '../_helpers';

var qs = require('qs');

export const userService = {
    //login,
    getToken,
    logout,
    getContentType,
    register,
    //getAll,
    //getById,
    //update,
    //delete: _delete
};

const client_id = 'kolviet';
const client_secret = 'kolviet';
const grant_type = 'password';

function getToken(username, password) {
    //
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: qs.stringify({
            grant_type: grant_type,
            username: username,
            password: password,
            client_id: client_id,
            client_secret: client_secret
        })
    };

    return fetch(`${configContent.apiUrl}connect/token`, requestOptions).then(handleTokenResponse);
}

function getContentType(token) {
    const requestOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': token
        },
    };

    return fetch(`${configOrchardCore.apiUrl}content/Post04`, requestOptions).then(handleContentTypeResponse);
}

// function login(userName, password) {
//     const requestOptions = {
//         method: 'POST',
//         headers: { 'Content-Type': 'application/json' },
//         body: JSON.stringify({ userName, password })
//     };

//     return fetch(`${configContent.apiUrl}/users/authenticate`, requestOptions)
//         .then(handleResponse)
//         .then(user => {
//             // store user details and jwt token in local storage to keep user logged in between page refreshes
//             localStorage.setItem('user', JSON.stringify(user));
//             return user;
//         });
// }

function logout() {
    debugger;
    // remove user from local storage to log user out
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    localStorage.removeItem('campaign');
    localStorage.removeItem('job');
    localStorage.removeItem('dateValue');
    localStorage.removeItem('selectedOptionLocation');
    localStorage.removeItem('selectedOptionInteresting');
    localStorage.removeItem('selectedOptionJobCategory');
    history.index = -1;
    history.replace({ pathname: '/pages/loginpage' });
}

// function getAll() {
//     const requestOptions = {
//         method: 'GET',
//         headers: authHeader()
//     };

//     return fetch(`${configContent.apiUrl}/users`, requestOptions).then(handleResponse);
// }

// function getById(id) {
//     const requestOptions = {
//         method: 'GET',
//         headers: authHeader()
//     };

//     return fetch(`${configContent.apiUrl}/users/${id}`, requestOptions).then(handleResponse);
// }

function register(user) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
    };

    return fetch(`${configOrchardCore.apiUrl}content/Post02`, requestOptions).then(handleResponseRegisterUser);
}

// function update(user) {
//     const requestOptions = {
//         method: 'PUT',
//         headers: { ...authHeader(), 'Content-Type': 'application/json' },
//         body: JSON.stringify(user)
//     };

//     return fetch(`${configContent.apiUrl}/users/${user.id}`, requestOptions).then(handleResponse);;
// }

// // prefixed function name with underscore because delete is a reserved word in javascript
// function _delete(id) {
//     const requestOptions = {
//         method: 'DELETE',
//         headers: authHeader()
//     };

//     return fetch(`${configContent.apiUrl}/users/${id}`, requestOptions).then(handleResponse);
// }

function handleResponseRegisterUser(response) {
    return response.text().then(text => {
        const data = text;
        if (!response.ok) {
            if (response.status === 401) {
                // auto logout if 401 response returned from api
                logout();
                //location.reload(true);
            }
            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }

        if (response.status === 204) {
            const error = response.statusText;
            return Promise.reject(error);
        }

        return data;
    });
}

function handleTokenResponse(response) {
    return response.json().then(data => {
        const token = 'Bearer ' + data.access_token;
        if (!response.ok) {
            if (response.status === 401) {
                // auto logout if 401 response returned from api
                logout();
                //location.reload(true);
            }

            const error = response.statusText;
            return Promise.reject(error);
        }

        localStorage.setItem('token', token);
        return token;
    });
}


function handleContentTypeResponse(response) {
    return response.text().then(text => {
        const data = text;
        if (!response.ok) {
            if (response.status === 401) {
                // auto logout if 401 response returned from api
                logout();
                //location.reload(true);
            }

            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }

        return data;
    });
}