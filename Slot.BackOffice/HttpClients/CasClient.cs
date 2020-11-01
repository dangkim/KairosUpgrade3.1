using Microsoft.Extensions.Options;
using Slot.BackOffice.Configs.Authentication;
using Slot.Model.Utilities;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Slot.BackOffice.HttpClients
{
    /// <summary>
    /// HttpClient for CAS.
    /// </summary>
    public class CasClient
    {
        private readonly HttpClient client;
        private readonly AuthenticationConfig authenticationConfig;

        /// <summary>
        /// Creates an instance of the client.
        /// </summary>
        /// <param name="client"><see cref="HttpClient"/> instance.</param>
        /// <param name="authenticationConfig"><see cref="AuthenticationConfig"/> options instance.</param>
        public CasClient(HttpClient client, IOptions<AuthenticationConfig> authenticationConfig)
        {
            this.client = client;
            this.authenticationConfig = authenticationConfig.Value;
        }

        /// <summary>
        /// Perform security check on CAS.
        /// </summary>
        /// <param name="ticket">Authentication ticket from CAS.</param>
        /// <returns>Security check response in JSON string format.</returns>
        public async Task<string> DoSecurityCheck(string ticket)
        {
            var checkSum = HashUtil.SHA1Hash($"{authenticationConfig.CasClientId}{ticket}{authenticationConfig.CasClientPassword}");
            var url = $"{authenticationConfig.CasSecurityCheckUrl}/{authenticationConfig.CasClientId}/{ticket}/{checkSum}";

            var requestMsg = new HttpRequestMessage(HttpMethod.Post, url);
            requestMsg.Headers.Add(nameof(HttpRequestHeader.ContentType), "application/x-www-form-urlencoded");

            var result = await client.SendAsync(requestMsg);
            var response = await result.Content.ReadAsStringAsync();

            return response;
        }
    }
}
