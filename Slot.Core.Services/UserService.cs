using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Slot.Core.Data;
using Slot.Core.Data.Exceptions;
using Slot.Core.Extensions;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Security;
using Slot.Core.Services.Abstractions;
using Slot.Core.Services.Extensions;
using Slot.Core.Services.Mappings;
using Slot.Core.Services.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Slot.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IDatabaseManager databaseManager;
        private readonly IDistributedCache cache;
        private readonly CachedSettings cachedSettings;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IAuthenticationService authenticationService;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<UserService> logger;

        public UserService(IDatabaseManager databaseManager,
            IDistributedCache cache,
            CachedSettings cachedSettings,
            IHttpClientFactory httpClientFactory,
            IAuthenticationService authenticationService,
            ILoggerFactory loggerFactory)
        {
            this.databaseManager = databaseManager;
            this.cache = cache;
            this.cachedSettings = cachedSettings;
            this.httpClientFactory = httpClientFactory;
            this.authenticationService = authenticationService;
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger<UserService>();
        }

        public async Task<Result<AuthenticateResult, ErrorCode>> Authenticate(string operatorId, string token, string ipAddress)
        {
            try
            {
                if (!cachedSettings.OperatorsByName.TryGetValue(operatorId, out Operator op))
                {
                    return ErrorCode.WrongParameter;
                }

                var result = await authenticationService.Authenticate(operatorId, token);
                if (result.IsError) return result.Error;

                var auth = result.Value;

                using (var db = databaseManager.GetWritableDatabase())
                {
                    var user = db.Users
                        .Where(x => x.OperatorId == op.Id)
                        .Where(u => u.ExternalId == auth.MemberId)
                        .Include(x => x.Currency)
                        .Include(x => x.Operator)
                        .AsNoTracking()
                        .FirstOrDefault();
                    if (user == null)
                    {
                        logger.LogInformation($"Trying to create new user for {auth.MemberId} {auth.MemberName} - {op.Tag}");
                        user = await CreateUser(db, auth.Currency, auth.MemberId, auth.MemberName, op.Id, auth.IsTestAccount);
                    }
                    if (user == null)
                    {
                        logger.LogWarning($"Create user failed for {auth.MemberId} {auth.MemberName} - {op.Tag}");
                        return ErrorCode.SessionExpired;
                    }
                    if (user.IsBlocked)
                    {
                        return ErrorCode.AccessDenied;
                    }
                    if (IsMismatchMemberName(operatorId, user.Name, auth.MemberName))
                    {
                        return ErrorCode.AccessDenied;
                    }

                    var sessionKey = op.EncodeToken ? HttpUtility.UrlDecode(auth.SessionKey) : auth.SessionKey;
                    await SaveUserSession(db, user, sessionKey, ipAddress, auth.ExtraInfo);

                    return auth;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return ErrorCode.SessionExpired;
        }

        private async Task SaveUserSession(IWritableDatabase db, User user, string sessionKey, string ipAddress, string extraInfo)
        {
            db.UserSessionLogs.Add(new UserSessionLog
            {
                UserId = user.Id,
                SessionKey = sessionKey,
                IpAddress = ipAddress,
                PlatformType = PlatformType.Web
            });

            // try to adapt with old structure when the game lobby do authentication
            var entitySession = new UserSessionEntity
            {
                UserId = user.Id,
                SessionKey = sessionKey,
                ExtraInfo = extraInfo
            };

            db.InsertOrUpdate(entitySession, user.Id);
            await db.SaveChangesAsync();
            var userSession = new UserSession
            {
                IsFunPlay = false,
                SessionKey = sessionKey,
                UserId = user.Id,
                User = user,
                ExtraInfo = extraInfo
            };
            await cache.SetAsync(userSession.SessionKey, userSession, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(20)
            });
            // The reason why we cache user id here, because RSlotServer project they store user session 
            // on database, and try to get user id by session key from database. 
            // for the workaround, we cache user id on the cache, so RSlotServer can get it from cache also.
            await cache.SetAsync(userSession.SessionKey + "_UserId", user.Id, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(20)
            });
        }

        // TODO, whats this function for?
        private bool IsMismatchMemberName(string operatorTag, string savedUsername, string memberName)
        {
            if (!cachedSettings.ConfigSettings.TryGetValue("SETS_OPERATOR_CHECKMEMBERNAME", out ConfigurationSetting configSetting))
            {
                logger.LogWarning("Miss configuration for SETS_OPERATOR_CHECKMEMBERNAME");
                return false;
            }
            if (string.IsNullOrWhiteSpace(configSetting.Value))
            {
                logger.LogWarning("Configuration for SETS_OPERATOR_CHECKMEMBERNAME is empty");
                return false;
            }
            var isMatchOperator = configSetting.Value.Equals("All", StringComparison.OrdinalIgnoreCase) ||
                configSetting.Value.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                .Any(x => x.Equals(operatorTag, StringComparison.OrdinalIgnoreCase));
            if (!isMatchOperator) return false;
            return savedUsername != memberName;
        }

        private async Task<User> CreateUser(IWritableDatabase db, string currencyCode, string externalId, string memberName, int operatorId, bool isTestAccount)
        {
            if (IsInvalidMemberName(memberName))
            {
                logger.LogWarning("cust_name: '{0}' is not valid characters", memberName);
                return null;
            }

            if (!cachedSettings.CurrenciesByIsoCode.TryGetValue(currencyCode, out Currency currency))
                cachedSettings.CurrenciesByIsoCode.TryGetValue("UNK", out currency);

            if (currency == null || currency.Id == 0)
            {
                logger.LogError("{0} is not a valid currency code!", currencyCode);
                return null;
            }

            if (!currency.IsVisible)
            {
                currency.IsVisible = true;
                db.Currencies.Update(currency);
                await db.SaveChangesAsync();
            }

            var existingUsername = await db.Users
                .Where(u => u.OperatorId == operatorId)
                .Where(u => u.Name == memberName)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (existingUsername != null)
            {
                logger.LogInformation("[USER EXIST] cust_id: {0}, cust_name: {1}, optag: {2}",
                    existingUsername.ExternalId,
                    existingUsername.Name,
                    existingUsername.Operator.Tag);
                return null;
            }
            try
            {
                var newUser = new User
                {
                    Name = memberName,
                    CurrencyId = currency.Id,
                    ExternalId = externalId,
                    OperatorId = operatorId,
                    IsDemo = isTestAccount
                };
                db.Users.Add(newUser);
                await db.SaveChangesAsync();
            }
            catch (DuplicateException ex)
            {
                logger.LogWarning(ex, "Duplicated exception during create new user {operatorId}-{externalId}-{memberName}", operatorId, externalId, memberName);
            }

            return await db.Users.Where(x => x.ExternalId == externalId)
                .Where(x => x.OperatorId == operatorId)
                .Include(x => x.Operator)
                .Include(x => x.Currency)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        private bool IsInvalidMemberName(string memberName)
        {
            return !memberName.All(x => char.IsLetterOrDigit(x) || x == '_' || x == ' ');
        }

        private async Task<UserGameState> GetGameStateFromCache(string key)
        {
            var gameState = await cache.GetAsync<UserGameState>(key);
            if (gameState == null)
            {
                gameState = UserGameState.InitialState();
                await cache.SetAsync(key, gameState, new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });
            }
            return gameState;
        }

        private async Task<UserGameState> GetGameStateFromDatabase(UserGameKey userGameKey)
        {
            using (var db = databaseManager.GetWritableDatabase())
            {
                var gameState = await db.GameStates.FindAsync(userGameKey.UserId, userGameKey.GameId);
                if (gameState == null)
                {
                    gameState = new GameState
                    {
                        UserId = userGameKey.UserId,
                        GameId = userGameKey.GameId,
                        Type = GameStateType.SlotStateNormal,
                        LastRoundId = 0
                    };
                    db.GameStates.Add(gameState);
                    await db.SaveChangesAsync();
                }
                return GameStateMapper.Map(gameState);
            }
        }

        public async Task<UserGameState> GetGameState<T>(RequestContext<T> requestContext)
        {
            try
            {
                if (requestContext.UserSession.IsFunPlay)
                {
                    return await GetGameStateFromCache(requestContext.SessionKey);
                }
                else
                {
                    return await GetGameStateFromDatabase(requestContext.UserGameKey);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<int> GetLevel(UserGameKey userGameKey)
        {
            if (userGameKey.Level != 0)
                return userGameKey.Level;

            using (var db = databaseManager.GetWritableDatabase())
            {
                // TODO for funplay should move to FunplayUserService
                if (!userGameKey.IsFunPlay)
                {
                    var userGameRtpSetting = await db.UserGameRtpSettings.FindAsync(userGameKey.UserId);
                    if (userGameRtpSetting != null)
                    {
                        var gameRtps = await db.GameRtps.Where(g => g.GameId == userGameKey.GameId)
                            .OrderBy(g => g.Rtp)
                            .ToListAsync();
                        if (gameRtps.Count > 0)
                        {
                            var rtpLevel = 0;

                            if (userGameRtpSetting.Level > gameRtps.Count)
                                rtpLevel = gameRtps.LastOrDefault().RtpLevel;
                            else
                                rtpLevel = gameRtps.ElementAtOrDefault(userGameRtpSetting.Level - 1).RtpLevel;

                            if (rtpLevel > 0)
                                return rtpLevel;
                        }
                    }
                }

                if (cachedSettings.Games.TryGetValue(userGameKey.GameId, out Game game)) return game.RtpLevel;

                //default RTP level
                return 1;
            }
        }

        public async Task<UserSession> GetUserSession(string sessionKey)
        {
            var session = await cache.GetAsync<UserSession>(sessionKey);
            if (session != null)
            {
                return session;
            }
            return null;
        }

        public async Task<bool> UpdateGameState<T>(RequestContext<T> requestContext, UserGameState userGameState)
        {
            try
            {
                if (requestContext.UserSession.IsFunPlay)
                {
                    await cache.SetAsync(requestContext.SessionKey, userGameState, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    });
                    return true;
                }
                else
                {
                    using (var db = databaseManager.GetWritableDatabase())
                    {
                        var state = db.GameStates.Find(requestContext.UserSession.User.Id, requestContext.Game.Id);
                        if (state == null) return false;
                        state.LastRoundId = userGameState.LastRoundId;
                        state.Type = userGameState.Type;
                        db.GameStates.Update(state);
                        return await db.SaveChangesAsync() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogRequestContext(requestContext, LogLevel.Error);
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateUserGameData(UserGameData userGameData)
        {
            try
            {
                if (userGameData.UserId < 0)
                {
                    await cache.SetAsync(userGameData.UserId.ToString(), userGameData, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    });
                    return true;
                }

                using (var db = databaseManager.GetWritableDatabase())
                {
                    db.InsertOrUpdate(userGameData, userGameData.UserId, userGameData.GameId);
                    var result = await db.SaveChangesAsync() > 0;
                    if (!result)
                    {
                        logger.LogWarning("Update user game data to db was failed");
                    }
                    return result;
                }
            }
            catch (Exception err)
            {
                logger.LogError(err, "UpdateUserGameData for UserGame({0}-{1}) encountered exception {2}", userGameData.UserId, userGameData.GameId, err.Message);
                return false;
            }
        }

        public async Task<UserGameData> GetUserGameData(long userId, int gameId)
        {
            using (var db = databaseManager.GetWritableDatabase())
            {
                return await db.UserGameDatas
                               .Where(x => x.UserId == userId && x.GameId == gameId)
                               .FirstOrDefaultAsync();
            }
        }

        public Task<IWalletService> GetWallet(UserSession userSession)
        {
            if (userSession == null)
                throw new ArgumentNullException(nameof(userSession));
            IWalletService wallet;
            if (userSession.IsFunPlay)
            {
                wallet = new FunplayWalletService(userSession,
                    loggerFactory.CreateLogger<FunplayWalletService>());
            }
            else
            {
                wallet = new WalletService(userSession,
                    httpClientFactory,
                    cachedSettings,
                    databaseManager,
                    loggerFactory.CreateLogger<WalletService>());
            }
            return Task.FromResult(wallet);
        }

        public async Task<Result<string, ErrorCode>> GetFunplayKey(string operatorTag)
        {
            if (!cachedSettings.OperatorsByName.TryGetValue(operatorTag, out Operator ops) || !ops.FunPlay)
                return ErrorCode.AccessDenied;

            var user = new User
            {
                Id = -1,
                OperatorId = ops.Id,
                CurrencyId = ops.FunPlayCurrencyId,
                Currency = ops.FunPlayCurrency
            };

            var funplaySession = new UserSession
            {
                IsFunPlay = true,
                SessionKey = SecurityTokenProvider.Create(),
                UserId = user.Id,
                User = user
            };

            // TODO we shouldn't set initial balance here, so remove first.
            // set initial balance
            //var db = new WritableDatabase();
            //var op = db.OperatorRepository.GetById(ops.Id);
            //WalletFunPlay.SetFunplayInitBalance(op.FunPlayInitialBalance);

            await cache.SetAsync(funplaySession.SessionKey, funplaySession, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(20)
            });
            return funplaySession.SessionKey;
        }

        public async Task<Result<GetBalanceResult, ErrorCode>> GetBalance(string token, string gameKey, PlatformType platform)
        {
            var session = await GetUserSession(token);
            if (session == null)
            {
                return ErrorCode.SessionExpired;
            }

            var gameId = 0;
            if (gameKey != null && cachedSettings.GamesByName.TryGetValue(gameKey, out Game game))
            {
                gameId = game.Id;
            }

            var wallet = await GetWallet(session);
            var balance = await wallet.GetBalance(gameId, (int)platform);
            if (balance.IsError)
                return balance.Error;
            return new GetBalanceResult
            {
                Amount = balance.Value,
                Currency = session.User.Currency.DisplayCode
            };
        }

        public async Task<UserGameSpinData> GetLastSpinData(UserSession userSession, Game game)
        {
            if (userSession.IsFunPlay)
            {
                var result = await cache.GetAsync<UserGameSpinData>(userSession.SessionKey + "_" + game.Id.ToString());
                return result;
            }
            else
            {
                using (var db = databaseManager.GetWritableDatabase())
                {
                    var user = userSession.User;
                    var result = await db.UserGameSpinDatas
                        .AsNoTracking()
                        .Where(x => x.UserId == user.Id)
                        .Where(x => x.GameId == game.Id)
                        .FirstOrDefaultAsync();
                    return result;
                }
            }
        }

        public async Task UpdateLastSpinData(UserSession userSession, Game game, SpinResult spinResult)
        {
            var user = userSession.User;
            var spinData = new UserGameSpinData()
            {
                UserId = user.Id,
                GameId = game.Id,
                Data = spinResult.ToByteArray()
            };
            if (userSession.IsFunPlay)
            {
                var key = userSession.SessionKey + "_" + game.Id.ToString();
                await cache.SetAsync(key, spinData, new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                });
            }
            else
            {
                using (var db = databaseManager.GetWritableDatabase())
                {
                    var lastSpin = await db.UserGameSpinDatas
                        .Where(x => x.UserId == user.Id)
                        .Where(x => x.GameId == game.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (lastSpin == null)
                    {
                        db.UserGameSpinDatas.Add(spinData);
                    }
                    else
                    {
                        lastSpin.Data = spinData.Data;
                        db.UserGameSpinDatas.Update(lastSpin);
                    }
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}