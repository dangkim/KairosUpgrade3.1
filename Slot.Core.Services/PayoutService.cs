using Microsoft.Extensions.Logging;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Abstractions;
using Slot.Core.Services.Exceptions;
using Slot.Core.Services.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Slot.Core.Services
{
    public class PayoutService : IPayoutService
    {
        private readonly ILogger<PayoutService> logger;
        private readonly IUserService userService;
        private readonly IGameTransactionService gameTransactionService;
        private readonly IGameHistoryService gameHistoryService;

        public PayoutService(ILogger<PayoutService> logger,
                             IUserService userService,
                             IGameTransactionService gameTransactionService,
                             IGameHistoryService gameHistoryService)
        {
            this.logger = logger;
            this.userService = userService;
            this.gameTransactionService = gameTransactionService;
            this.gameHistoryService = gameHistoryService;
        }

        public async Task<WalletResult> DeductBetFromWallet<T>(RequestContext<T> requestContext, decimal amount)
        {
            // if (requestContext.PreviousRound > 0) logger.LogDebug($"[GAMBLE:ENDGAME] RoundId:{requestContext.PreviousRound}");
            var ugk = requestContext.UserGameKey;
            var gameTransaction = requestContext.GameTransaction;
            var roundId = requestContext.CurrentRound;
            var prevRoundId = requestContext.LastGameState.LastRoundId;
            var platformType = requestContext.Platform;
            var result = new WalletResult();
            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["SessionKey"] = requestContext.UserSession.SessionKey,
                ["UserId"] = requestContext.UserSession.UserId,
                ["GameKey"] = requestContext.GameKey,
                ["Platform"] = requestContext.Platform,
                ["GameTransactionId"] = gameTransaction.Id,
                ["Amount"] = amount,
                ["CurrentRoundId"] = roundId,
                ["PrevRoundId"] = prevRoundId,
            }))
            {
                try
                {
                    logger.LogInformation("Deduct bet from wallet");
                    var wallet = await userService.GetWallet(requestContext.UserSession);
                    result = await wallet.Debit(amount, ugk.GameId, gameTransaction.Id, 0, roundId, (int)platformType, prevRoundId);
                    if (result.ExchangeRate == 0)
                    {
                        result.ExchangeRate = 1;
                    }
                }
                catch (WalletException ex)
                {
                    if (ex.InnerException is InsufficientBalanceException)
                    {
                        logger.LogInformation(ex, $"[BET:DEBIT][InsufficientBalanceException] UserId:{ugk.UserId} GameId:{ugk.GameId} TransactionId:{gameTransaction.Id} Message:{ex.Message}");
                        throw ex.InnerException;
                    }
                    logger.LogError(ex, $"[BET:DEBIT][WalletException] UserId:{ugk.UserId} GameId:{ugk.GameId} TransactionId:{gameTransaction.Id} Message:{ex.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"[BET:DEBIT] UserId:{ugk.UserId} GameId:{ugk.GameId} TransactionId:{gameTransaction.Id} Message:{ex.Message}");
                    throw;
                }
                return result;
            }
        }

        public Task<GameTransaction> GetGameTransaction(UserGameKey userGameKey, GameTransactionType gttype)
        {
            return gameTransactionService.GenerateGameTransactionId(userGameKey, gttype);
        }

        public async Task<bool> NotifyEndGame<T>(RequestContext<T> requestContext, BonusResult bonusResult)
        {
            var w = await userService.GetWallet(requestContext.UserSession);
            return await w.EndGame(requestContext.Game.Id, bonusResult.RoundId);
        }

        public async Task<WalletResult> PayoutToUser<T>(RequestContext<T> requestContext, GameResult gr, BonusExtraInfo bei, bool isEndGame, string debitTrxId = "")
        {
            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["SessionKey"] = requestContext.UserSession.SessionKey,
                ["UserId"] = requestContext.UserSession.UserId,
                ["GameKey"] = requestContext.GameKey,
                ["Platform"] = requestContext.Platform,
                ["GameTransactionId"] = gr.TransactionId,
                ["Amount"] = gr.Win,
                ["CurrentRoundId"] = gr.RoundId,
                ["WalletReference"] = bei.BetId,
            }))
            {
                logger.LogInformation("Payout to user");

                if (gr.RoundId <= 0)
                {
                    var msg = String.Format("RoundID can not be zero or less than zero, please always set RoundID to GameResult. Current value is {0}", gr.RoundId);
                    throw new ArgumentException(msg, "RoundID");
                }

                if (isEndGame && gr.RoundId > 0) logger.LogDebug($"[WIN:ENDGAME] BetId:{bei.BetId} RoundId:{gr.RoundId}");

                gr.Balance = Balance.Create(0);

                var user = requestContext.UserSession.User;
                var game = requestContext.Game;
                WalletResult result;
                try
                {
                    var wallet = await userService.GetWallet(requestContext.UserSession);

                    result = await wallet.Credit(gr.Win, game.Id, gr.TransactionId, 0, gr.RoundId, bei.BetId, (int)gr.PlatformType, debitTrxId, gr.TransactionId.ToString(), isEndGame);

                    if (result.Balance <= 0)
                    {
                        var x = await wallet.GetBalance(game.Id, (int)gr.PlatformType);
                        result.Balance = x.IsError ? 0m : x.Value;
                    }

                    gr.ExchangeRate = result.ExchangeRate;
                }
                catch (Exception ex)
                {
                    gr.ErrorSource = ErrorSource.Wallet;

                    logger.LogError(ex, $"[WIN:CREDIT] UserId:{user.Id} GameId:{game.Id} TransactionId:{gr.TransactionId} BetId:{bei.BetId} RoundId:{gr.RoundId} Message:{ex.Message}");
                    throw;
                }

                if (!gr.ExchangeRate.HasValue)
                    gr.ExchangeRate = 1;

                gr.Balance = Balance.Create(result.Balance);

                return result;
            }
        }
    }
}
