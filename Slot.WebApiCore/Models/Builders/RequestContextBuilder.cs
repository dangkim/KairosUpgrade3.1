using Microsoft.AspNetCore.Http;
using Slot.Core;
using Slot.Core.Data;
using Slot.Core.Extensions;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Abstractions;
using Slot.Model;
using Slot.Model.Entity;
using System.Threading.Tasks;

namespace Slot.WebApiCore.Models.Builders
{
    public static class RequestContextBuilder
    {
        private static PlatformType GetPlatformType(string platform)
        {
            switch (platform.ToLower())
            {
                case "mobile": return PlatformType.Mobile;
                case "mini": return PlatformType.Mini;
            }
            return PlatformType.Web;
        }

        public static async Task<Result<RequestContext<T>, ErrorCode>> Build<T>(
            IUserService userService,
            IGameService gameService,
            CachedSettings cachedSettings,
            IHttpContextAccessor httpContextAccessor,
            string sessionKey,
            string gameKey)
        {
            var userSession = await userService.GetUserSession(sessionKey);
            if (userSession == null)
                return ErrorCode.SessionExpired;

            var query = httpContextAccessor.HttpContext.Request.Query;
            if (!query.TryGetString("platform", out string platform))
            {
                platform = "web";
            }

            var requestContext = new RequestContext<T>(sessionKey, gameKey, GetPlatformType(platform))
            {
                Query = query,
                UserSession = userSession,
                Currency = userSession.User.Currency
            };

            if (!gameService.CheckAvailability(gameKey, userSession.User.OperatorId, out Game game))
                return ErrorCode.InvalidGame;
            requestContext.Game = game;

            if (!cachedSettings.OperatorsById.TryGetValue(userSession.User.OperatorId, out Operator op))
                return ErrorCode.SessionExpired;
            requestContext.Operator = op;

            var gameSettingKey = string.Format("{0}-{1}-{2}", op.GameSettingGroupId, game.Id, userSession.User.CurrencyId);
            if (!cachedSettings.GameSettings.TryGetValue(gameSettingKey, out GameSetting gameSetting))
                return ErrorCode.MissingGameSetting;
            requestContext.GameSetting = gameSetting;

            requestContext.UserGameKey = new UserGameKey(userSession.User.Id, game.Id);

            return requestContext;
        }
    }
}
