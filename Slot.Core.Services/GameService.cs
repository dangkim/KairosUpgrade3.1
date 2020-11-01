using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Slot.Core.Data;
using Slot.Core.Modules.Infrastructure;
using Slot.Core.Modules.Infrastructure.Exceptions;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Abstractions;
using Slot.Core.Services.Exceptions;
using Slot.Core.Services.Extensions;
using Slot.Core.Services.Validation;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.Core.Services
{
    public sealed class GameService : IGameService
    {
        private readonly GameModuleCollection gameModules;
        private readonly IUserService userService;
        private readonly IBonusService bonusService;
        private readonly IPayoutService payoutService;
        private readonly IGameHistoryService gameHistoryService;
        private readonly IGameTransactionService transactionService;
        private readonly IDistributedCache cache;
        private readonly CachedSettings cachedSettings;
        private readonly IValidationStrategy validator;
        private readonly ILogger<GameService> logger;

        public GameService(GameModuleCollection gameModules,
                           IUserService userService,
                           IBonusService bonusService,
                           IPayoutService payoutService,
                           IGameHistoryService gameHistoryService,
                           IGameTransactionService transactionService,
                           IDistributedCache cache,
                           CachedSettings cachedSettings,
                           IValidationStrategy validator,
                           ILogger<GameService> logger)
        {
            this.gameModules = gameModules;
            this.userService = userService;
            this.bonusService = bonusService;
            this.payoutService = payoutService;
            this.gameHistoryService = gameHistoryService;
            this.transactionService = transactionService;
            this.cache = cache;
            this.cachedSettings = cachedSettings;
            this.validator = validator;
            this.logger = logger;
        }

        private IGameModule GetGameModule(string moduleName)
        {
            if (!gameModules.TryGetModule(moduleName, out IGameModule module))
            {
                logger.LogWarning($"Game Module {moduleName} not found!!!");
            }
            return module;
        }

        public bool TryGetGameId(string gameName, out int gameId)
        {
            gameId = 0;
            var module = GetGameModule(gameName);
            if (null == module)
                return false;

            gameId = module.GameId;
            return true;
        }

        public async Task<Result<Bets, ErrorCode>> GetBets(RequestContext<GetBetsArgs> requestContext)
        {
            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["SessionKey"] = requestContext.UserSession.SessionKey,
                ["UserId"] = requestContext.UserSession.UserId,
                ["GameKey"] = requestContext.GameKey
            }))
            {
                logger.LogInformation("User call getbets");

                var userGameKey = requestContext.UserGameKey;

                var module = gameModules.GetModule(requestContext.GameKey);

                var lastSpinData = await userService.GetLastSpinData(requestContext.UserSession, requestContext.Game);

                var gameSetting = requestContext.GameSetting;

                var level = await userService.GetLevel(userGameKey);
                var extraSettings = module.GetExtraSettings(level, lastSpinData);

                var bets = new Bets
                {
                    Wheel = module.InitialRandomWheel(),
                    FunPlayDemo = requestContext.UserSession.IsFunPlay && requestContext.Operator.FunPlayDemo,
                    Coins = gameSetting.CoinsDenomination.Split(';').Select(decimal.Parse).ToList(),
                    Multipliers = gameSetting.CoinsMultiplier.Split(';').Select(int.Parse).ToList(),
                    ExtraGameSettings = extraSettings,
                    ServerTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

                if (!requestContext.UserSession.IsFunPlay)
                {
                    var userGameData = await userService.GetUserGameData(requestContext.UserSession.UserId, requestContext.Game.Id);
                    if (userGameData != null)
                    {
                        bets.Bet = userGameData.Bet;
                    }
                    var bonus = await bonusService.GetUnfinishBonus(requestContext.UserSession, requestContext.Game.Id);
                    bets.Bonus = bonus == null ? null : module.ConvertToBonus(bonus);
                }

                return bets;
            }
        }

        public async Task<Result<IGameResult, ErrorCode>> Spin(RequestContext<SpinArgs> requestContext)
        {
            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["SessionKey"] = requestContext.UserSession.SessionKey,
                ["UserId"] = requestContext.UserSession.UserId,
                ["GameKey"] = requestContext.GameKey,
                ["Platform"] = requestContext.Platform,
                ["BettingLines"] = requestContext.Parameters.BettingLines,
                ["LineBet"] = requestContext.Parameters.LineBet,
                ["Multiplier"] = requestContext.Parameters.Multiplier
            }))
            {
                var module = gameModules.GetModule(requestContext.GameKey);
                var userSession = requestContext.UserSession;
                SpinResult result = null;
                var level = await userService.GetLevel(requestContext.UserGameKey);
                try
                {
                    var transaction = await transactionService.GenerateGameTransactionId(requestContext.UserGameKey, GameTransactionType.Spin);
                    requestContext.GameTransaction = transaction;

                    var roundId = await transactionService.GenerateAutoNumber(CounterType.RoundId);
                    requestContext.CurrentRound = roundId;

                    var lastSpinData = await userService.GetLastSpinData(userSession, requestContext.Game);

                    var totalBet = module.CalculateTotalBet(lastSpinData, requestContext);

                    logger.LogInformation("User spin on level {Level}, trx id {GameTransactionId}, round {CurrentRoundId}, total bet {TotalBet}", level, transaction.Id, roundId, totalBet);

                    var deductBet = await payoutService.DeductBetFromWallet(requestContext, totalBet);
                    // TODO we should check the wallet result if it's success

                    var spinResult = module.ExecuteSpin(level, lastSpinData, requestContext);
                    if (spinResult.IsError)
                    {
                        return spinResult.Error;
                    }

                    result = spinResult.Value;
                    result.Bet = totalBet;
                    result.RoundId = roundId;
                    result.TransactionId = transaction.Id;
                    result.UniqueID = result.TransactionId.ToString();
                    result.DateTimeUtc = transaction.DateTimeUtc;
                    result.ExchangeRate = deductBet.ExchangeRate;
                    result.Balance = Balance.Create(deductBet.Balance);

                    if (result.HasBonus)
                    {
                        logger.LogInformation("User got bonus");
                        var bonusCreated = module.CreateBonus(result);
                        if (bonusCreated.IsError)
                        {
                            logger.LogWarning("Create bonus got error {Error} with {SpinResult}", bonusCreated.Error, JsonHelper.FullString(result));
                            return ErrorCode.InternalError;
                        }

                        var bonus = bonusCreated.Value;
                        bonus.SpinTransactionId = transaction.Id;
                        bonus.GameResult = result;
                        var entity = new BonusEntity
                        {
                            UserId = userSession.UserId,
                            GameId = requestContext.Game.Id,
                            Guid = bonus.Guid.ToString("N"),
                            Data = bonus.ToByteArray(),
                            BonusType = bonus.GetType().Name,
                            Version = 3,
                            IsOptional = bonus.IsOptional,
                            IsStarted = bonus.IsStarted,
                            RoundId = roundId,
                            BetReference = deductBet.BetReference
                        };
                        await bonusService.UpdateBonus(userSession, entity);
                        await userService.UpdateGameState(requestContext, UserGameState.ForBonus(roundId));
                    }
                    else
                    {
                        await userService.UpdateGameState(requestContext, UserGameState.ForNormal(roundId));
                    }

                    var paied = await payoutService.PayoutToUser(requestContext, result, new BonusExtraInfo { RoundId = roundId, BetId = deductBet.BetReference }, !result.HasBonus, deductBet.Guid);
                    result.ExchangeRate = deductBet.ExchangeRate;
                    result.Balance = Balance.Create(paied.Balance);

                    await transactionService.ProfileSpinBet(requestContext);
                    await userService.UpdateLastSpinData(userSession, requestContext.Game, result);
                    if (!userSession.IsFunPlay)
                    {
                        await userService.UpdateUserGameData(new UserGameData
                        {
                            UserId = userSession.UserId,
                            GameId = requestContext.Game.Id,
                            Bet = requestContext.Parameters.LineBet,
                            Lines = requestContext.Game.Lines,
                        });
                    }
                }
                catch (InsufficientBalanceException ex)
                {
                    logger.LogInformation(ex, ex.Message);
                    return ErrorCode.InsufficientCredit;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    throw;
                }
                finally
                {
                    await TrySaveGameHistory(requestContext, result);
                }
                return result;
            }
        }

        public async Task<Result<IGameResult, ErrorCode>> BonusGame(RequestContext<BonusArgs> requestContext)
        {

            if (!gameModules.TryGetModule(requestContext.GameKey, out IGameModule module))
            {
                return ErrorCode.InvalidGame;
            }

            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["SessionKey"] = requestContext.UserSession.SessionKey,
                ["UserId"] = requestContext.UserSession.UserId,
                ["GameKey"] = requestContext.GameKey,
                ["Platform"] = requestContext.Platform,
                ["BonusParams"] = requestContext.Parameters.Param
            }))
            {
                try
                {
                    var bonus = await bonusService.GetUnfinishBonus(requestContext.UserSession, requestContext.Game.Id);
                    if (bonus == null)
                    {
                        logger.LogWarning("User play bonus game but no bonus found.");
                        return ErrorCode.NonexistenceBonus;
                    }

                    var level = await userService.GetLevel(requestContext.UserGameKey);
                    logger.LogInformation("User play bonus game with level {Level}, id {BonusId}", level, bonus.Guid);

                    var bonusGameResult = module.ExecuteBonus(level, bonus, requestContext);
                    if (bonusGameResult.IsError)
                    {
                        logger.LogWarning("Execute bonus got error {Error}", bonusGameResult.Error);
                        return bonusGameResult.Error;
                    }

                    var result = bonusGameResult.Value;
                    if (result.TransactionType == GameTransactionType.None)
                    {
                        throw new InvalidGameModuleImplementation(requestContext.GameKey, $"Must set transaction type to bonus result for ExecuteBonus");
                    }
                    if (!result.IsCompleted && result.Bonus == null)
                    {
                        throw new InvalidGameModuleImplementation(requestContext.GameKey, "If bonus is not completed, the BonusResult.Bonus can not be null.");
                    }

                    var transaction = await payoutService.GetGameTransaction(requestContext.UserGameKey, result.TransactionType);
                    result.TransactionId = transaction.Id;
                    result.UniqueID = result.TransactionId.ToString();
                    result.DateTimeUtc = transaction.DateTimeUtc;
                    result.RoundId = bonus.RoundId;

                    if (result.IsCompleted)
                    {
                        await bonusService.RemoveBonus(requestContext.UserSession, bonus);
                        await userService.UpdateGameState(requestContext, UserGameState.ForNormal(result.RoundId));
                    }
                    else
                    {
                        bonus.Data = result.Bonus.ToByteArray();
                        await bonusService.UpdateBonus(requestContext.UserSession, bonus);
                    }

                    await payoutService.PayoutToUser(requestContext, result, new BonusExtraInfo { BetId = bonus.BetReference, RoundId = bonus.RoundId }, result.IsCompleted);
                    await TrySaveGameHistory(requestContext, result);

                    //if (result.IsCompleted)
                    //{
                    //    await payoutService.NotifyEndGame(requestContext, result);
                    //}

                    return result;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }
                return ErrorCode.InternalError;
            }
        }

        public bool CheckAvailability(string gameKey, int operatorId, out Game game)
        {
            game = null;
            if (TryGetGameId(gameKey, out int gameId))
            {
                if (cachedSettings.Games.TryGetValue(gameId, out game))
                {
                    if (game != null && !game.IsDisabled)
                    {
                        return !validator.IsDisableOperator(operatorId, game.DisableOperators);
                    }
                }
            }
            return false;
        }

        private async Task TrySaveGameHistory<T>(RequestContext<T> requestContext, GameResult result)
        {
            if (requestContext.UserSession.IsFunPlay)
                return;

            var userGameKey = requestContext.UserGameKey;
            try
            {
                // Don't save any transaction within Insufficient exception and any wallet access exception.
                if (result == null || result.ErrorCode == ErrorCode.InsufficientCredit)
                    return;

                if (result.ErrorSource == ErrorSource.Wallet)
                {
                    // log game history to log file
                    logger.LogWarning($"[Got error from wallet] SessionKey<{requestContext.SessionKey}> UserId<{userGameKey.UserId}> GameId<{userGameKey.GameId}> Transaction<{result.TransactionId}>");
                    logger.LogWarning(JsonHelper.FullString(result));
                    return;
                }

                await gameHistoryService.Save(userGameKey, result);
            }
            catch (Exception ex)
            {
                logger.LogError($"[FAULT SAVE GAMEHISTORY] SessionKey<{requestContext.SessionKey}> UserId<{userGameKey.UserId}> GameId<{userGameKey.GameId}> Transaction<{result.TransactionId}>");
                logger.LogError(JsonHelper.FullString(result));
                logger.LogError(ex, ex.Message);
            }
        }

        public IEnumerable<IGameModule> ListGameModules()
        {
            return gameModules.ListModules();
        }
    }
}
