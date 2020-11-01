using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Slot.Core;
using Slot.Core.Data;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Abstractions;
using Slot.Model;
using System.Linq;
using System.Threading.Tasks;
using Slot.Core.Extensions;

namespace Slot.WebApiCore.Models.Builders
{
    public class SpinRequestBuilder
    {
        private readonly IUserService userService;
        private readonly IGameService gameService;
        private readonly CachedSettings cachedSettings;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<SpinRequestBuilder> logger;

        public SpinRequestBuilder(IUserService userService,
                                  IGameService gameService,
                                  CachedSettings cachedSettings,
                                  IHttpContextAccessor httpContextAccessor,
                                  ILogger<SpinRequestBuilder> logger)
        {
            this.userService = userService;
            this.gameService = gameService;
            this.cachedSettings = cachedSettings;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        private void EnsureBetMultiplier(SpinMessage message, string coinsMultiplier)
        {
            if (string.IsNullOrEmpty(coinsMultiplier)
                || message.Multiplier < 1
                || !coinsMultiplier.Split(';').Select(int.Parse).ToList().Any(s => s == message.Multiplier))
            {
                message.Multiplier = 1;
            }
        }

        public async Task<Result<RequestContext<SpinArgs>, ErrorCode>> Build(SpinMessage message)
        {
            var buildRequest = await RequestContextBuilder.Build<SpinArgs>(userService, gameService, cachedSettings, httpContextAccessor, message.Key, message.Game);
            if (buildRequest.IsError)
                return buildRequest.Error;

            var request = buildRequest.Value;

            var gameState = await userService.GetGameState(request);
            if (gameState.Type == GameStateType.SlotStateBonus)
                return ErrorCode.IncorrectState;
            request.LastGameState = gameState;

            if (message.Bet < 0m)
            {
                return ErrorCode.IncorrectBet;
            }

            if (!request.GameSetting.CoinsDenomination
                .Split(';')
                .Select(decimal.Parse).ToList().Any(s => s == message.Bet))
            {
                return ErrorCode.IncorrectBet;
            }

            EnsureBetMultiplier(message, request.GameSetting.CoinsMultiplier);

            if (message.SideBet && !request.Game.IsSideBet)
            {
                return ErrorCode.WrongParameter;
            }
            request.Query.TryGetInt32("cheat", out int cheat);
            var args = new SpinArgs
            {
                LineBet = message.Bet,
                SideBet = message.SideBet,
                FunPlayDemoKey = cheat,
                IsAutoSpin = false,
                BettingLines = 0,
                Multiplier = message.Multiplier
            };
            request.Parameters = args;

            return request;
        }
    }
}
