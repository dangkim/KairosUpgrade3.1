using Microsoft.AspNetCore.Mvc;
using Slot.BackOffice.Data.Authentication;
using Slot.BackOffice.Data.Queries.TopWinners;
using Slot.BackOffice.Data.Repositories;
using Slot.BackOffice.Filters;
using System.Threading.Tasks;

namespace Slot.BackOffice.Controllers
{
    [ApiVersion("1.0")]
    [BackOfficeAuthorize(Roles.Administrator, Roles.Manager, Roles.Compliance)]
    [ValidateQueryOperator]
    public class TopWinnersController : BaseController
    {
        private readonly ReportsRepository reportsRepository;

        public TopWinnersController(ReportsRepository reportsRepository)
        {
            this.reportsRepository = reportsRepository;
        }

        [HttpGet]
        [Route("/api/[controller]")]
        public async Task<IActionResult> TopWinners([FromQuery]TopWinnerQuery query) =>
            GetResult(await reportsRepository.TopWinners(query));

        [HttpGet]
        public async Task<IActionResult> Details([FromQuery]TopWinnerDetailQuery query) =>
            GetResult(await reportsRepository.TopWinnerDetail(query));
    }
}