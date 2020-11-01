using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Slot.Core;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Abstractions;
using Slot.Games.BullRush.Models;
using Slot.Model;
using Slot.WebApiCore.AsyncLock;
using Slot.WebApiCore.Models;
using Slot.WebApiCore.Models.Builders;

namespace Slot.WebApiCore.hub
{
    public class RaceHub : Hub
    {
        private readonly LockerManager lockerManager;
        private readonly ILogger<RaceHub> logger;
        private readonly IGameService gameService;
        private readonly IUserService userService;
        private readonly GetBetsRequestBuilder getBetsRequestBuilder;
        private readonly SpinRequestBuilder spinRequestBuilder;
        private readonly BonusGameRequestBuilder bonusGameRequestBuilder;
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        public RaceHub(LockerManager lockerManager,
                              ILogger<RaceHub> logger,
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

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                try
                {
                    //Add Logged User
                    var userName = httpContext.Request.Query["user"].ToString();
                    var connId = Context.ConnectionId.ToString();

                    // Current connectionId
                    var currentConnection = _connections.GetConnections(userName).FirstOrDefault();

                    if (string.IsNullOrEmpty(currentConnection))
                    {
                        _connections.Add(userName, connId);
                    }
                    else if (connId.ToLower() != currentConnection.ToLower())
                    {
                        await Clients.Clients(currentConnection).SendAsync("ReceiveMessage", "One connection only");
                    }
                }
                catch (Exception) { }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                //Remove Logged User
                var username = httpContext.Request.Query["user"];

                _connections.Remove(username, Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task BonusGame(BonusGameMessage message)
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                try
                {
                    //await Clients.All.SendAsync("ReceiveMessage", "aaa");
                    var userName = httpContext.Request.Query["user"].ToString();
                    message.Key = userName;

                    logger.LogInformation("userName: " + userName);


                    string ReceiverConnectionids = _connections.GetConnections(message.Key).FirstOrDefault();

                    logger.LogInformation("ReceiverConnectionids: " + ReceiverConnectionids);

                    if (!string.IsNullOrEmpty(ReceiverConnectionids))
                    {
                        var bonus = GetBonusGame(message);

                        string output = JsonConvert.SerializeObject(((BullRushFreeSpinResult)bonus.Result.Value).SpinResult);
                        await Clients.Clients(ReceiverConnectionids).SendAsync("ReceiveMessage", output);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }

        }

        private async Task<Result<IGameResult, ErrorCode>> GetBonusGame(BonusGameMessage message)
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
