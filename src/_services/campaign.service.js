import configOrchardCore from 'configOrchardCore';
import { authHeader } from '../_helpers';

export const campaignService = {
    register,
    getAll
};

function register(campaignType) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(campaignType)
    };

    return fetch(`${configOrchardCore.apiUrl}/content`, requestOptions).then(handleContentResponse);
}

function getAll() {
    const GET_ALL_COMPAIGN = `
    {
        campaigns{
          title:displayText,
          bag {
            contentItems {
              ... on Campaign {
                title:displayText,
                campaignName,
                campaignTarget,
                currency,
                budget,
                fromAge,
                toAge,
                fromDate,
                toDate,
                gender,
                productInfo,
                modifiedUtc
              }
            }
          }
        }
      }
    `;
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/graphql' },
        body: GET_ALL_COMPAIGN
    };

    return fetch(`${configOrchardCore.apiUrl}/graphql`, requestOptions).then(handleGraphResponse);
}

function handleGraphResponse(response) {
    return response.json().then(text => {
        const data = text.data;
        debugger;
        
        if (!response.ok) {
            if (response.status === 401) {
                // auto logout if 401 response returned from api
                logout();
                location.reload(true);
            }

            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }

        return data;
    });
}

function handleContentResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            if (response.status === 401) {
                // auto logout if 401 response returned from api
                logout();
                location.reload(true);
            }

            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }

        return data;
    });    
}