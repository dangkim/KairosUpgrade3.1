using Enyim.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Slot.BackOffice.Configs.AppSettings;
using Slot.BackOffice.Configs.Authentication;
using Slot.BackOffice.Data.Authentication;
using Slot.BackOffice.Filters;
using Slot.BackOffice.HttpClients;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Slot.BackOffice.Controllers
{
    [ApiVersion("1.0")]
    [BackOfficeAuthorize(Roles.Administrator, Roles.Developer)]
    public class GameServicesController : BaseController
    {
        private readonly AppSettingsConfig appSettingsConfig;
        private readonly AuthenticationConfig authenticationConfig;
        private readonly GameServiceClient gameServiceClient;
        private readonly IMemcachedClient memcachedClient;

        public GameServicesController(
            IOptions<AppSettingsConfig> appSettingsConfig,
            IOptions<AuthenticationConfig> authenticationConfig,
            GameServiceClient gameServiceClient,
            IMemcachedClient memcachedClient)
        {
            this.appSettingsConfig = appSettingsConfig.Value;
            this.authenticationConfig = authenticationConfig.Value;
            this.gameServiceClient = gameServiceClient;
            this.memcachedClient = memcachedClient;
        }

        [HttpGet]
        public IActionResult GetResources() =>
            GetResult(appSettingsConfig.GameServices);

        [HttpGet]
        public async Task<IActionResult> ClearCache()
        {
            var token = JwtUser.GetToken(authenticationConfig.Jwt.Key
                                                , authenticationConfig.Jwt.Issuer
                                                , authenticationConfig.Jwt.Issuer
                                                , authenticationConfig.Jwt.Duration);

            foreach (var server in appSettingsConfig.GameServices)
            {
                await gameServiceClient.ClearCache(server.Url, token);
            }

            await memcachedClient.FlushAllAsync();

            return GetResult();
        }

        [HttpGet]
        public async Task<IActionResult> HealthCheck()
        {
            var token = JwtUser.GetToken(authenticationConfig.Jwt.Key
                                                , authenticationConfig.Jwt.Issuer
                                                , authenticationConfig.Jwt.Issuer
                                                , authenticationConfig.Jwt.Duration);


            var server = appSettingsConfig.HealthCheckServices;

            var respone = await gameServiceClient.HealthCheck(server, token);

            return GetResult(respone);
        }
    }
}
