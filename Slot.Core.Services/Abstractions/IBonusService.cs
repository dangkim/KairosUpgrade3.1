using Slot.Core.Modules.Infrastructure.Models;
using Slot.Model.Entity;
using System.Threading.Tasks;

namespace Slot.Core.Services.Abstractions
{
    public interface IBonusService
    {
        Task<BonusEntity> GetUnfinishBonus(UserSession userSession, int gameId);

        Task<bool> UpdateBonus(UserSession userSession, BonusEntity bonus);

        Task<bool> RemoveBonus(UserSession userSession, BonusEntity bonus);
    }
}
