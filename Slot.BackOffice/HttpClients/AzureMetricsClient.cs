using Microsoft.Extensions.Options;
using Slot.BackOffice.Configs.AppSettings;
using System.Net.Http;
using System.Threading.Tasks;

namespace Slot.BackOffice.HttpClients
{
    /// <summary>
    /// HttpClient for Azure Metrics.
    /// </summary>
    public class AzureMetricsClient
    {
        private readonly HttpClient client;
        private readonly AppSettingsConfig appSettingsConfig;

        /// <summary>
        /// Creates an instance of the client.
        /// </summary>
        /// <param name="client"><see cref="HttpClient"/> instance.</param>
        /// <param name="appSettingsConfig"><see cref="AppSettingsConfig"/> options instance.</param>
        public AzureMetricsClient(HttpClient client, IOptions<AppSettingsConfig> appSettingsConfig)
        {
            this.client = client;
            this.appSettingsConfig = appSettingsConfig.Value;
        }

        /// <summary>
        /// Get content from the azure metrics api.
        /// </summary>
        /// <param name="query">Query to be requested to the api.</param>
        /// <returns>Response in JSON string format.</returns>
        public async Task<string> GetContent(string query)
        {
            var requestMsg = new HttpRequestMessage(HttpMethod.Get, appSettingsConfig.AzureMetrics.RequestUrl + query);
            requestMsg.Headers.Add("x-api-key", appSettingsConfig.AzureMetrics.Key);

            var result = await client.SendAsync(requestMsg);
            var response = await result.Content.ReadAsStringAsync();

            return response;
        }
    }
}
