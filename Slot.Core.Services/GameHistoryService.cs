using Microsoft.Extensions.Logging;
using Slot.Core.Data;
using Slot.Core.Services.Abstractions;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Utility;
using System;
using System.Threading.Tasks;

namespace Slot.Core.Services
{
    public class GameHistoryService : IGameHistoryService
    {
        private static readonly XmlHelper xmlhelper = new XmlHelper();
        private readonly ILogger<GameHistoryService> logger;
        private readonly IDatabaseManager databaseManager;

        public GameHistoryService(ILogger<GameHistoryService> logger,
                                  IDatabaseManager databaseManager)
        {
            this.logger = logger;
            this.databaseManager = databaseManager;
        }

        public Task<bool> Save(UserGameKey userGameKey, GameResult gameResult)
        {
            if (gameResult == null) throw new ArgumentNullException("gameResult");

            var gameHistory = CreatGameHistory(userGameKey, gameResult);
            return SaveHistory(gameHistory);
        }

        private async Task<bool> SaveHistory(GameHistory gameHistory)
        {
            try
            {
                using (var db = databaseManager.GetWritableDatabase())
                {
                    db.GameHistories.Add(gameHistory);
                    return await db.SaveChangesAsync() > 0;
                }
            }
            catch (Exception ex)
            {
                using (logger.BeginScope("Store game history fault"))
                {
                    logger.LogError(ex, ex.Message);
                    logger.LogInformation(JsonHelper.ToString(gameHistory));
                }
            }
            return false;
        }

        private GameHistory CreatGameHistory(UserGameKey userGameKey, GameResult result)
        {
            var history = new GameHistory
            {
                DateTimeUtc = result.DateTimeUtc,
                GameTransactionId = result.TransactionId,
                SpinTransactionId = result.SpinTransactionId,
                UserId = userGameKey.UserId,
                GameId = userGameKey.GameId,
                Level = result.Level,
                Bet = result.Bet,
                Win = result.Win,
                ExchangeRate = result.ExchangeRate.GetValueOrDefault(),
                GameResultType = result.GameResultType,
                XmlType = result.XmlType,
                ResponseXml = xmlhelper.Serialize(result.ToResponseXml(ResponseXmlFormat.None)),
                HistoryXml = xmlhelper.Serialize(result.ToResponseXml(ResponseXmlFormat.Legacy | ResponseXmlFormat.History)),
                IsFreeGame = userGameKey.IsFreeGame,
                IsHistory = result.IsHistory,
                IsReport = result.IsReport,
                PlatformType = result.PlatformType,
                RoundId = result.RoundId
            };

            return history;
        }
    }
}
