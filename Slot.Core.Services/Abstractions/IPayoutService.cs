using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Models;
using Slot.Model;
using Slot.Model.Entity;
using System.Threading.Tasks;

namespace Slot.Core.Services.Abstractions
{
    public interface IPayoutService
    {
        Task<WalletResult> DeductBetFromWallet<T>(RequestContext<T> requestContext, decimal amount);
        Task<GameTransaction> GetGameTransaction(UserGameKey userGameKey, GameTransactionType gttype);
        Task<bool> NotifyEndGame<T>(RequestContext<T> requestContext, BonusResult bonusResult);
        Task<WalletResult> PayoutToUser<T>(RequestContext<T> requestContext, GameResult gr, BonusExtraInfo bei, bool isEndGame, string debitTrxId = "");
    }
}
