using Microsoft.AspNetCore.Mvc;
using Slot.BackOffice.Data.Authentication;
using Slot.BackOffice.Data.Queries.GamePerformance;
using Slot.BackOffice.Data.Repositories;
using Slot.BackOffice.Filters;
using System.Threading.Tasks;

namespace Slot.BackOffice.Controllers
{
    [ApiVersion("1.0")]
    [BackOfficeAuthorize(Roles.Administrator, Roles.Manager, Roles.Compliance, Roles.Marketing, Roles.GameAnalysis, Roles.GameAnalysisManager)]
    [ValidateQueryOperator]
    public class GamePerformanceController : BaseController
    {
        private readonly ReportsRepository reportsRepository;

        public GamePerformanceController(ReportsRepository reportsRepository)
        {
            this.reportsRepository = reportsRepository;
        }

        [HttpGet]
        [Route("/api/[controller]")]
        public async Task<IActionResult> GamePerformance([FromQuery]GamePerformanceQuery query) =>
            GetResult(await reportsRepository.GamePerformance(query));
    }
}
