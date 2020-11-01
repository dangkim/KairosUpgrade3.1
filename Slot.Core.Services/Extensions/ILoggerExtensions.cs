using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Slot.Core.Modules.Infrastructure.Models;

namespace Slot.Core.Services.Extensions
{
    public static class ILoggerExtensions
    {
        public static void LogRequestContext<T>(this ILogger logger, RequestContext<T> requestContext, LogLevel logLevel = LogLevel.Information)
        {
            logger.Log(logLevel, JsonConvert.SerializeObject(requestContext));
        }

        public static void LogRequestContext(this ILogger logger, RequestContext<SpinArgs> requestContext, LogLevel logLevel = LogLevel.Information)
        {
            var userSession = requestContext.UserSession;
            var spin = requestContext.Parameters;
            logger.Log(logLevel, "Request >> Session {0} UserId {1} Game {2} GameTransactionId {3} Spin {4} {5}",
                                 userSession.SessionKey,
                                 userSession.User?.Id,
                                 requestContext.GameKey,
                                 requestContext.GameTransaction?.Id,
                                 spin?.LineBet,
                                 spin?.BettingLines);
        }

        public static void LogRequestContext(this ILogger logger, RequestContext<BonusArgs> requestContext, LogLevel logLevel = LogLevel.Information)
        {
            var userSession = requestContext.UserSession;
            var bonus = requestContext.Parameters;
            logger.Log(logLevel, "Request >> Session {0} UserId {1} Game {2} GameTransactionId {3} Bonus {4} {5} {6}",
                                 userSession.SessionKey,
                                 userSession.User?.Id,
                                 requestContext.GameKey,
                                 requestContext?.GameTransaction?.Id,
                                 bonus.Bonus,
                                 bonus.Step,
                                 bonus.Param);
        }
    }
}
