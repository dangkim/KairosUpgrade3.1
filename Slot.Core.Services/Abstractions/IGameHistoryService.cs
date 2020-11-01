using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Slot.Model;

namespace Slot.Core.Services.Abstractions
{
    public interface IGameHistoryService
    {
        Task<bool> Save(UserGameKey userGameKey, GameResult gr);
    }
}
