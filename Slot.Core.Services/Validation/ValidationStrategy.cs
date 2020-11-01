using Slot.Model;
using Slot.Model.Entity;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;

namespace Slot.Core.Services.Validation
{
    public class ValidationStrategy : IValidationStrategy
    {
        private readonly IDistributedCache cache;

        public ValidationStrategy(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public bool IsCoinValid(string coinsDenomination, SpinBet bet)
        {
            return coinsDenomination.Split(';')
                .Select(decimal.Parse)
                .ToList()
                .Any(s => s == bet.LineBet);
        }

        public bool IsMultiplierValid(string coinsMultiplier, SpinBet bet)
        {
            return coinsMultiplier
                .Split(';')
                .Select(int.Parse)
                .ToList()
                .Any(s => s == bet.Multiplier);
        }

        public bool IsLineBetValid(Game game, SpinBet bet)
        {
            var isValid = game.IsBetAllLines && bet.Lines == game.Lines
                || bet.Lines > 0 && bet.Lines <= game.Lines;
            return isValid;
        }

        public bool IsSideBetValid(Game game, SpinBetX bet)
        {
            return game.IsSideBet && bet.IsSideBet == game.IsSideBet;
        }

        // Never use, disable first
        //public bool IsTransactionIdValid(int userId, int gameId, string tid)
        //{
        //    long transactionId;
        //    if (!long.TryParse(tid, out transactionId))
        //        return true;

        //    var key = StorageEntityTransactionId.GetKeyFormat(userId, gameId);

        //    var storedEntity = StorageEngine.Instance.Get<StorageEntityTransactionId>(key);

        //    if (storedEntity == null)
        //        return true;

        //    return (transactionId == storedEntity.TransactionId || transactionId == 0);
        //}

        public bool IsDisableOperator(int operatorId, string listMerchants)
        {
            return !string.IsNullOrEmpty(listMerchants) && listMerchants.Split(';').Contains(operatorId.ToString());
        }
    }
}