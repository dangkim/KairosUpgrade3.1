using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Slot.Core;
using Slot.Core.Data;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Abstractions;
using Slot.Model;
using System.Threading.Tasks;

namespace Slot.WebApiCore.Models.Builders
{
    public class GetBetsRequestBuilder
    {
        private readonly IUserService userService;
        private readonly IGameService gameService;
        private readonly CachedSettings cachedSettings;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<GetBetsRequestBuilder> logger;

        public GetBetsRequestBuilder(IUserService userService,
                                     IGameService gameService,
                                     CachedSettings cachedSettings,
                                     IHttpContextAccessor httpContextAccessor,
                                     ILogger<GetBetsRequestBuilder> logger)
        {
            this.userService = userService;
            this.gameService = gameService;
            this.cachedSettings = cachedSettings;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        public async Task<Result<RequestContext<GetBetsArgs>, ErrorCode>> Build(GetBetsMessage message)
        {
            var buildRequest = await RequestContextBuilder.Build<GetBetsArgs>(userService, gameService, cachedSettings, httpContextAccessor, message.Key, message.Game);
            if (buildRequest.IsError)
                return buildRequest.Error;

            var request = buildRequest.Value;
            return request;
        }
    }
}
