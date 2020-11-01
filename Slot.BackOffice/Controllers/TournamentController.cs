using Microsoft.AspNetCore.Mvc;
using Slot.BackOffice.Data.Authentication;
using Slot.BackOffice.Data.Queries.Tournament;
using Slot.BackOffice.Data.Repositories;
using Slot.BackOffice.Filters;
using System.Threading.Tasks;

namespace Slot.BackOffice.Controllers
{
    [ApiVersion("1.0")]
    [BackOfficeAuthorize(Roles.Administrator, Roles.Manager, Roles.Marketing, Roles.CustomerService)]
    [ValidateQueryOperator]
    public class TournamentController : BaseController
    {
        private readonly TournamentRepository tournamentRepository;

        public TournamentController(TournamentRepository tournamentRepository)
        {
            this.tournamentRepository = tournamentRepository;
        }


        [HttpPost]
        [Route("/api/[controller]")]
        public async Task<IActionResult> GetTournaments(TournamentQuery query) =>
            GetResult(await tournamentRepository.GetGlobalTournaments(query));

        [HttpGet]
        public async Task<IActionResult> GetLeaderboards(int tournamentId) =>
            GetResult(await tournamentRepository.GetGlobalLeaderboard(tournamentId));

    }
}
