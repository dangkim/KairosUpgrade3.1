import configOrchardCore from 'configOrchardCore';
import { authHeader } from '../_helpers';

export const campaignService = {
    register,
    //getAll,
    getAllInteresting,
    getAllLocation
};

function register(campaignType) {
    const token = localStorage.getItem('token');
    const requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': token
        },
        body: JSON.stringify(campaignType)
    };

    return fetch(`${configOrchardCore.apiUrl}content/Post?draft=true`, requestOptions).then(handleContentResponse);
}

// function getAll() {
//     const GET_ALL_COMPAIGN = `
//     {
//         campaigns{
//           title:displayText,
//           bag {
//             contentItems {
//               ... on Campaign {
//                 title:displayText,
//                 campaignName,
//                 campaignTarget,
//                 currency,
//                 budget,
//                 fromAge,
//                 toAge,
//                 fromDate,
//                 toDate,
//                 gender,
//                 productInfo,
//                 modifiedUtc
//               }
//             }
//           }
//         }
//       }
//     `;
//     const requestOptions = {
//         method: 'POST',
//         headers: { 'Content-Type': 'application/graphql' },
//         body: GET_ALL_COMPAIGN
//     };

//     return fetch(`${configOrchardCore.apiUrl}graphql`, requestOptions).then(handleGraphResponse);
// }

function getAllInteresting() {
    const GET_ALL_INTERESTING = `
    {
        interestingList {
        contentItemId,
        contentItemVersionId,
        contentType,
        displayText,
        latest,
        published,
        modifiedUtc,
        publishedUtc,
        createdUtc,
        owner,
        author,
        interesting
        }
      }
    `;
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/graphql' },
        body: GET_ALL_INTERESTING
    };

    return fetch(`${configOrchardCore.apiUrl}graphql`, requestOptions).then(handleGraphInterestingResponse);
}

function getAllLocation() {
    const GET_ALL_LOCATION = `
    {
        locations{
        contentItemId,
        contentItemVersionId,
        contentType,
        displayText,
        latest,
        published,
        modifiedUtc,
        publishedUtc,
        createdUtc,
        owner,
        author,
        location
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
        body: GET_ALL_LOCATION
    };

    return fetch(`${configOrchardCore.apiUrl}graphql`, requestOptions).then(handleGraphLocationResponse);
}

function handleGraphLocationResponse(response) {
    return response.json().then(text => {
        const data = text.data.locations;
        //
        
        if (!response.ok) {
            if (response.status === 401) {
                // auto logout if 401 response returned from api
                //logout();
                //location.reload(true);
            }

            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }

        return data;
    });
}

function handleGraphInterestingResponse(response) {
    return response.json().then(text => {
        const data = text.data.interestingList;
        //
        
        if (!response.ok) {
            if (response.status === 401) {
                // auto logout if 401 response returned from api
                //logout();
                //location.reload(true);
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
                //logout();
                //location.reload(true);
            }

            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }

        return data;
    });    
}