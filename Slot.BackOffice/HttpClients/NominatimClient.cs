using Microsoft.Extensions.Options;
using Slot.BackOffice.Configs.AppSettings;
using System.Net.Http;
using System.Threading.Tasks;

namespace Slot.BackOffice.HttpClients
{
    /// <summary>
    /// HttpClient for Nominatim maps api.
    /// </summary>
    public class NominatimClient
    {
        private readonly HttpClient client;
        private readonly AppSettingsConfig appSettingsConfig;
        private readonly string nominatimUrl;

        /// <summary>
        /// Creates an instance of the client.
        /// </summary>
        /// <param name="client"><see cref="HttpClient"/> instance.</param>
        /// <param name="appSettingsConfig"><see cref="AppSettingsConfig"/> options instance.</param>
        public NominatimClient(HttpClient client, IOptions<AppSettingsConfig> appSettingsConfig)
        {
            this.client = client;
            this.appSettingsConfig = appSettingsConfig.Value;
            nominatimUrl = $"{this.appSettingsConfig.AzureMetrics.NominatimUrl}/search?format=json&q=";
        }

        /// <summary>
        /// Retrieve the specified region data from the nominatim api.
        /// </summary>
        /// <param name="region">Region name.</param>
        /// <returns>Region name JSON string data.</returns>
        public async Task<string> GetCountryData(string region)
        {
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";

            var requestMsg = new HttpRequestMessage(HttpMethod.Get, nominatimUrl + region);
            requestMsg.Headers.Add("user-agent", userAgent);

            var response = await client.SendAsync(requestMsg);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }
    }
}
