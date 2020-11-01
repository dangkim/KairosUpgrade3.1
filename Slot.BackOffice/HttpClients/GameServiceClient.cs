using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Slot.BackOffice.Configs.AppSettings;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Slot.BackOffice.HttpClients
{
    /// <summary>
    /// HttpClient for Game Services.
    /// </summary>
    public class GameServiceClient
    {
        private readonly HttpClient client;

        /// <summary>
        /// Creates an instance of the client.
        /// </summary>
        /// <param name="client"><see cref="HttpClient"/> instance.</param>
        public GameServiceClient(HttpClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Clear the game service cache.
        /// </summary>
        /// <param name="url">The game service URL.</param>
        /// <param name="token">The current user's token to be used for authentication.</param>
        /// <returns>Clear cache api <see cref="HttpResponseMessage"/> response.</returns>
        public Task<HttpResponseMessage> ClearCache(string url, string token)
        {
            var clearCachePath = "/api/admin/reload";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            var response = client.GetAsync(new Uri(url + clearCachePath));
            return response;
        }

        /// <summary>
        /// Clear the game service cache.
        /// </summary>
        /// <param name="url">The game service URL.</param>
        /// <param name="token">The current user's token to be used for authentication.</param>
        /// <returns>Clear cache api <see cref="HttpResponseMessage"/> response.</returns>
        public async Task<object> HealthCheck(HealthCheckServices server, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            var response = await client.GetAsync(new Uri(server.Url));
            var contents = await response.Content.ReadAsStringAsync();
            var healthObject = JObject.Parse(contents);
            healthObject.Add("name", server.Name);
            return healthObject;
        }
    }
}
