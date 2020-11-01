using Microsoft.AspNetCore.Mvc;
using Slot.BackOffice.Data.Authentication;
using Slot.BackOffice.Data.Queries.WinLose;
using Slot.BackOffice.Data.Repositories;
using Slot.BackOffice.Filters;
using System.Threading.Tasks;

namespace Slot.BackOffice.Controllers
{
    [ApiVersion("1.0")]
    [BackOfficeAuthorize(Roles.Administrator, Roles.Manager, Roles.Marketing, Roles.GameAnalysis, Roles.GameAnalysisManager, Roles.Finance)]
    [ValidateQueryOperator]
    public class WinLoseController : BaseController
    {
        private readonly ReportsRepository reportsRepository;

        public WinLoseController(ReportsRepository reportsRepository)
        {
            this.reportsRepository = reportsRepository;
        }

        [HttpGet]
        [Route("/api/[controller]")]
        public async Task<IActionResult> WinLose([FromQuery]WinLoseQuery query) =>
            GetResult(await reportsRepository.WinLose(query));

        [HttpGet]
        public async Task<IActionResult> ByMerchant([FromQuery]WinLoseQuery query) =>
            GetResult(await reportsRepository.WinLoseByMerchant(query));

        [HttpGet]
        public async Task<IActionResult> ByPlatform([FromQuery]WinLoseQuery query) =>
            GetResult(await reportsRepository.WinLoseByPlatform(query));

        [HttpGet]
        public async Task<IActionResult> ByMerchantDetails([FromQuery]WinLoseQuery query) =>
            GetResult(await reportsRepository.WinLoseByMerchantDetails(query));

        [HttpGet]
        public async Task<IActionResult> ByGame([FromQuery]WinLoseQuery query) =>
            GetResult(await reportsRepository.WinLoseByGame(query));

        [HttpGet]
        public async Task<IActionResult> ByMember([FromQuery]WinLoseQuery query) =>
            GetResult(await reportsRepository.WinLoseByMember(query));

        [HttpGet]
        public async Task<IActionResult> ByCurrency([FromQuery]WinLoseQuery query) =>
            GetResult(await reportsRepository.WinLoseByCurrency(query));
    }
}