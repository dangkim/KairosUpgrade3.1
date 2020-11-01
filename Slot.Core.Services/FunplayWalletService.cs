using Microsoft.Extensions.Logging;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Abstractions;
using Slot.Core.Services.Exceptions;
using Slot.Core.Services.Models;
using Slot.Model;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Slot.Core.Services
{
    public class FunplayWalletService : IWalletService
    {
        private static readonly ConcurrentDictionary<string, decimal> userBalances = new ConcurrentDictionary<string, decimal>();
        private readonly UserSession userSession;
        private readonly ILogger<FunplayWalletService> logger;
        // TODO should be load from database
        private readonly decimal initialBalance = 2000m;

        public FunplayWalletService(UserSession userSession,
                                    ILogger<FunplayWalletService> logger)
        {
            this.userSession = userSession;
            this.logger = logger;
        }

        public Task<WalletResult> Credit(decimal amount, int gameId, long transactionId, decimal jwin, long roundId, string betId, int platform, string debitTrxId, string refTrxId, bool isEndGame = false)
        {
            try
            {
                return Task.FromResult(new WalletResult
                {
                    Balance = userBalances.AddOrUpdate(userSession.SessionKey, initialBalance + amount, (key, value) => value + amount)
                });
            }
            catch (Exception ex)
            {
                throw new WalletException(ex.Message, ex);
            }
        }

        public async Task<WalletResult> Debit(decimal amount, int gameId, long transactionId, decimal jcon, long roundId, int platform, long prevRoundId)
        {
            var balance = await GetBalance();
            if (balance.IsError)
                throw new WalletException(balance.Error.ToString());
            if (balance.Value - amount < 0)
                throw new WalletException("Insufficient balance", new InsufficientBalanceException());
            return new WalletResult
            {
                Balance = userBalances.AddOrUpdate(userSession.SessionKey, initialBalance - amount, (key, value) => value - amount)
            };
        }

        public Task<bool> EndGame(int gameId, long roundId)
        {
            return Task.FromResult(true);
        }

        public async Task<Result<decimal, ErrorCode>> GetBalance()
        {
            var task = Task.FromResult(userBalances.GetOrAdd(userSession.SessionKey, initialBalance));
            return await task;
        }

        public Task<Result<decimal, ErrorCode>> GetBalance(int gameId, int platform)
        {
            return GetBalance();
        }

        public Task<bool> NotifyTrxId(string trx_id, string ref_trx_id)
        {
            return Task.FromResult(true);
        }
    }
}
