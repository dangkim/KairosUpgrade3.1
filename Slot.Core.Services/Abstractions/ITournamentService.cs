using Slot.Core.Data.Models;
using Slot.Model;
using System.Threading.Tasks;

namespace Slot.Core.Services.Abstractions
{
    public interface ITournamentService
    {
        Task<Result<TournamentInfo, ErrorCode>> GetInfo(string merchant, string game);
    }
}
