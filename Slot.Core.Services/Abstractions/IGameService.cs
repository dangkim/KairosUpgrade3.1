using Slot.Core.Modules.Infrastructure;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Model;
using Slot.Model.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Slot.Core.Services.Abstractions
{
    /// <summary>
    /// This interface exposes all the functionality that can be called from the Web Api controller.
    /// It also acts as a place holder to call different slot engine based on the game parameter.
    /// </summary>
    public interface IGameService
    {
        Task<Result<Bets, ErrorCode>> GetBets(RequestContext<GetBetsArgs> requestContext);
        Task<Result<IGameResult, ErrorCode>> Spin(RequestContext<SpinArgs> requestContext);
        Task<Result<IGameResult, ErrorCode>> BonusGame(RequestContext<BonusArgs> requestContext);
        IEnumerable<IGameModule> ListGameModules();
        bool CheckAvailability(string gameKey, int operatorId, out Game game);
    }
}
