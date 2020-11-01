using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Slot.Core.Data;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Abstractions;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Threading.Tasks;

namespace Slot.Core.Services
{
    // TODO for Funplay should move to standalone GameTransactionService
    public class GameTransactionService : IGameTransactionService
    {
        private readonly IDistributedCache cache;
        private readonly IDatabaseManager databaseManager;
        private readonly ILogger<GameTransactionService> logger;

        public GameTransactionService(IDistributedCache cache,
                                      IDatabaseManager databaseManager,
                                      ILogger<GameTransactionService> logger)
        {
            this.cache = cache;
            this.databaseManager = databaseManager;
            this.logger = logger;
        }

        public Task<long> GenerateAutoNumber(CounterType counterType)
        {
            using (var db = databaseManager.GetWritableDatabase())
            using (var conn = db.Context.Database.GetDbConnection())
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    var id = (int)counterType;
                    var sql = string.Format("DECLARE @cnt BIGINT UPDATE COUNTER SET @cnt=value+1, value=value+1 FROM COUNTER WHERE Id={0} SELECT @cnt", id);
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            return Task.FromResult(reader.GetInt64(0));
                        }
                        return Task.FromResult(0L);
                    }
                }
            }
        }

        public async Task<GameTransaction> GenerateGameTransactionId(UserGameKey userGameKey, GameTransactionType gameTransactionType)
        {
            if (userGameKey.IsFunPlay)
                return new GameTransaction { Id = 0, DateTimeUtc = DateTime.UtcNow };

            using (var db = databaseManager.GetWritableDatabase())
            {
                var gameTransaction = new GameTransaction
                {
                    UserId = userGameKey.UserId,
                    GameId = userGameKey.GameId,
                    Type = gameTransactionType
                };
                db.GameTransactions.Add(gameTransaction);
                await db.SaveChangesAsync();
                return gameTransaction;
            }
        }

        public async Task ProfileSpinBet(RequestContext<SpinArgs> requestContext)
        {
            if (requestContext.UserGameKey.IsFunPlay) return;
            using (var db = databaseManager.GetWritableDatabase())
            {
                var sbp = new SpinBetProfile
                {
                    GameTransactionId = requestContext.GameTransaction.Id,
                    LineBet = requestContext.Parameters.LineBet,
                    Lines = requestContext.Parameters.BettingLines,
                    Multiplier = requestContext.Parameters.Multiplier,
                    TotalBet = requestContext.Parameters.TotalBet,
                    IsAutoSpin = requestContext.Parameters.IsAutoSpin,
                    IsSideBet = requestContext.Parameters.SideBet
                };
                db.SpinBetProfiles.Add(sbp);
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateGameTransaction(GameTransaction gameTransaction)
        {
            using (var db = databaseManager.GetWritableDatabase())
            {
                db.GameTransactions.Update(gameTransaction);
                await db.SaveChangesAsync();
            }
        }

        public void UpdateGameTransactionException(long id, Exception exception)
        {
            logger.LogError(exception, "<Game Transaction Error> - Game Transaction Id : {0}, Message : {1}", id, exception.ToString());
        }
    }
}
