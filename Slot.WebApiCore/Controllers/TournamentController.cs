using Microsoft.AspNetCore.Mvc;
using Slot.Core;
using Slot.Core.Data.Models;
using Slot.Core.Services.Abstractions;
using Slot.Core.Services.Models;
using Slot.Model;
using System.Threading.Tasks;

namespace Slot.WebApiCore.Controllers
{
    public class TournamentController : ApiControllerBase
    {
        private readonly ITournamentService tournamentService;

        public TournamentController(ITournamentService tournamentService)
        {
            this.tournamentService = tournamentService;
        }

        [HttpGet]
        public async Task<Result<TournamentInfo, ErrorCode>> GetInfo(string op, string game)
        {
            return await tournamentService.GetInfo(op, game);
        }
    }
}
