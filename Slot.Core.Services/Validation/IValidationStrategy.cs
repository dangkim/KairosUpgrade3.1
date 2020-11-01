using Slot.Model;
using Slot.Model.Entity;

namespace Slot.Core.Services.Validation
{
    public interface IValidationStrategy
    {
        bool IsCoinValid(string coinsDenomination, SpinBet bet);
        bool IsMultiplierValid(string coinsMultiplier, SpinBet bet);
        bool IsLineBetValid(Game game, SpinBet bet);
        bool IsSideBetValid(Game game, SpinBetX bet);
        // Never use, disable first
        //bool IsTransactionIdValid(int userId, int gameId, string tid);
        bool IsDisableOperator(int operatorId, string listMerchants);
    }
}