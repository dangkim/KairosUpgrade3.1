using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Slot.Core;
using Slot.Core.Modules.Infrastructure;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Abstractions;
using Slot.Model;
using Slot.WebApiCore.AsyncLock;
using Slot.WebApiCore.Models;
using Slot.WebApiCore.Models.Builders;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Slot.WebApiCore.Controllers
{
    public class GameController : ApiControllerBase
    {
        private readonly LockerManager lockerManager;
        private readonly ILogger<GameController> logger;
        private readonly IGameService gameService;
        private readonly IUserService userService;
        private readonly GetBetsRequestBuilder getBetsRequestBuilder;
        private readonly SpinRequestBuilder spinRequestBuilder;
        private readonly BonusGameRequestBuilder bonusGameRequestBuilder;

        public GameController(LockerManager lockerManager,
                              ILogger<GameController> logger,
                              IGameService gameService,
                              IUserService userService,
                              GetBetsRequestBuilder getBetsRequestBuilder,
                              SpinRequestBuilder spinRequestBuilder,
                              BonusGameRequestBuilder bonusGameRequestBuilder)
        {
            this.lockerManager = lockerManager;
            this.logger = logger;
            this.gameService = gameService;
            this.userService = userService;
            this.getBetsRequestBuilder = getBetsRequestBuilder;
            this.spinRequestBuilder = spinRequestBuilder;
            this.bonusGameRequestBuilder = bonusGameRequestBuilder;
        }

        [HttpGet]
        public async Task<Result<Bets, ErrorCode>> GetBets([FromQuery]GetBetsMessage message)
        {
            var request = await getBetsRequestBuilder.Build(message);
            if (request.IsError)
                return request.Error;
            return await gameService.GetBets(request.Value);
        }

        [HttpGet]
        public async Task<Result<IGameResult, ErrorCode>> Spin([FromQuery]SpinMessage message)
        {
            var session = await userService.GetUserSession(message.Key);
            if (session == null)
            {
                return SessionExpired;
            }
            var userKey = GetUserKeyFromSession(session);
            var result = await LockAsync(userKey, async () =>
            {
                var request = await spinRequestBuilder.Build(message);
                if (request.IsError)
                    return request.Error;
                var x = await gameService.Spin(request.Value);
                return x;
            });
            return result;
        }

        [HttpGet]
        public async Task<Result<IGameResult, ErrorCode>> BonusGame([FromQuery]BonusGameMessage message)
        {
            var session = await userService.GetUserSession(message.Key);
            if (session == null)
            {
                return SessionExpired;
            }
            var userKey = GetUserKeyFromSession(session);
            return await LockAsync(userKey, async () =>
            {
                var request = await bonusGameRequestBuilder.Build(message);
                if (request.IsError)
                    return request.Error;
                var x = await gameService.BonusGame(request.Value);
                return x;
            });
        }

        [HttpGet]
        public IActionResult ListGames()
        {
            return Ok(gameService.ListGameModules().Select(x =>
            {
                var attr = x.GetType().GetCustomAttribute<ModuleInfoAttribute>();
                return new
                {
                    attr.Key,
                    attr.Version
                };
            }));
        }

        private Result<IGameResult, ErrorCode> SessionExpired => ErrorCode.SessionExpired;

        private string GetUserKeyFromSession(UserSession session)
        {
            var userKey = string.Format("{0}-{1}", session.User.OperatorId, session.UserId);
            if (session.IsFunPlay)
            {
                userKey = session.SessionKey;
            }
            return userKey;
        }

        protected async Task<Result<T, ErrorCode>> LockAsync<T>(string key, Func<Task<Result<T, ErrorCode>>> action)
        {
            if (string.IsNullOrWhiteSpace(key))
                return ErrorCode.AccessDenied;

            var locker = lockerManager.Acquire(key);
            await locker.SemaphoreSlim.WaitAsync();
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return ErrorCode.InternalError;
            }
            finally
            {
                locker.SemaphoreSlim.Release();
            }
        }
    }
}
