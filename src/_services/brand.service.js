import configOrchardCore from 'configOrchardCore';
import { authHeader } from '../_helpers';

export const brandService = {
    register,
    getAll,
    getBrandByName
};

function getBrandByName(userName) {
    const GET_BRANDBYNAME = `
    {
        brand(where: {displayText_contains: "` + userName + `"}, status: LATEST) {
          contentItemId
          brandName
          email
          businessAreas
          fullName
          location
          createdUtc
          published
        }
      }
    `;
    const token = localStorage.getItem('token');
    const requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/graphql',
            'Authorization': token
        },
        body: GET_BRANDBYNAME
    };

    return fetch(`${configOrchardCore.apiUrl}graphql`, requestOptions).then(handleGraphResponse);
}

function getAll() {
    const GET_ALL_BRAND = `
    {
        {
            brand {
                title: displayText,
                contentItemId,
                brandName,
                businessAreas,
                fullName,
                email,
                location,
                phone,
                createdUtc,
            }
          }
    }
    `;

    const token = localStorage.getItem('token');
    const requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/graphql',
            'Authorization': token
        },
        body: GET_ALL_BRAND
    };

    return fetch(`${configOrchardCore.apiUrl}graphql`, requestOptions).then(handleGraphResponse);
}

function register(brandType) {
    const token = localStorage.getItem('token');
    const requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': token
        },
        body: JSON.stringify(brandType)
    };
    return fetch(`${configOrchardCore.apiUrl}content/Post?draft=true`, requestOptions).then(handleContentResponse);
}

function handleGraphResponse(response) {
    if (!response.ok) {
        if (response.status === 401) {
            // auto logout if 401 response returned from api
            logout();
            //location.reload(true);
            const error = response.statusText;            
            return Promise.reject(error);
        }
    }

    return response.json().then(text => {        
        const data = text.data;
        return data;
    });
    // 
    // return response.json().then(text => {
    //     const data = text.data;

    //     if (!response.ok) {
    //         if (response.status === 401) {
    //             // auto logout if 401 response returned from api
    //             logout();
    //             location.reload(true);
    //         }

    //         const error = (data && data.message) || response.statusText;
    //         return Promise.reject(error);
    //     }

    //     return data;
    // });
}

function handleContentResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
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

function logout() {    
    debugger;
    // remove user from local storage to log user out
    localStorage.removeItem('user');
    localStorage.removeItem('token');
}