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
    public class BonusGameRequestBuilder
    {
        private readonly IUserService userService;
        private readonly IGameService gameService;
        private readonly CachedSettings cachedSettings;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<BonusGameRequestBuilder> logger;

        public BonusGameRequestBuilder(IUserService userService,
                                       IGameService gameService,
                                       CachedSettings cachedSettings,
                                       IHttpContextAccessor httpContextAccessor,
                                       ILogger<BonusGameRequestBuilder> logger)
        {
            this.userService = userService;
            this.gameService = gameService;
            this.cachedSettings = cachedSettings;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        public async Task<Result<RequestContext<BonusArgs>, ErrorCode>> Build(BonusGameMessage message)
        {
            var buildRequest = await RequestContextBuilder.Build<BonusArgs>(userService, gameService, cachedSettings, httpContextAccessor, message.Key, message.Game);
            if (buildRequest.IsError)
                return buildRequest.Error;

            var request = buildRequest.Value;

            var gameState = await userService.GetGameState(request);
            if (gameState == null || gameState.Type == GameStateType.SlotStateNormal)
                return ErrorCode.IncorrectState;
            request.LastGameState = gameState;

            request.Parameters = new BonusArgs
            {
                Bonus = message.Bonus,
                Param = message.Param,
                Step = 0
            };

            return request;
        }
    }
}
