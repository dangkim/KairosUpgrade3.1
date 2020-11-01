using Slot.Core.Modules.Infrastructure.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Threading.Tasks;

namespace Slot.Core.Services.Abstractions
{
    public interface IGameTransactionService
    {
        Task<long> GenerateAutoNumber(CounterType counterType);
        Task<GameTransaction> GenerateGameTransactionId(UserGameKey userGameKey, GameTransactionType gameTransactionType);
        Task UpdateGameTransaction(GameTransaction gameTransaction);
        void UpdateGameTransactionException(long id, Exception exception);
        Task ProfileSpinBet(RequestContext<SpinArgs> requestContext);
    }
}
