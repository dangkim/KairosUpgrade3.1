using Slot.Core.Services.Models;
using Slot.Model;
using System.Threading.Tasks;

namespace Slot.Core.Services.Abstractions
{
    public interface IWalletService
    {
        Task<Result<decimal, ErrorCode>> GetBalance(int gameId, int platform);
        Task<WalletResult> Credit(decimal amount, int gameId, long transactionId, decimal jwin, long roundId, string betId, int platform, string debitTrxId, string refTrxId, bool isEndGame = false);
        Task<WalletResult> Debit(decimal amount, int gameId, long transactionId, decimal jcon, long roundId, int platform, long prevRoundId);
        Task<bool> NotifyTrxId(string trx_id, string ref_trx_id);
        Task<bool> EndGame(int gameId, long roundId);
    }
}
