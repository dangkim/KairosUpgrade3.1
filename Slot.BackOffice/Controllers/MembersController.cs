using Microsoft.AspNetCore.Mvc;
using Slot.BackOffice.Data.Authentication;
using Slot.BackOffice.Data.Queries.Members;
using Slot.BackOffice.Data.Repositories;
using Slot.BackOffice.Filters;
using System.Threading.Tasks;

namespace Slot.BackOffice.Controllers
{
    [ApiVersion("1.0")]
    [BackOfficeAuthorize]
    [ValidateQueryOperator]
    public class MembersController : BaseController
    {
        private readonly MembersRepository membersRepository;

        public MembersController(MembersRepository membersRepository)
        {
            this.membersRepository = membersRepository;
        }

        [HttpGet]
        [BackOfficeAuthorize(Roles.Administrator, Roles.Manager, Roles.Compliance, Roles.Marketing, Roles.CustomerService, Roles.CustomerServiceLimited)]
        public async Task<IActionResult> MemberList([FromQuery] MemberListQuery query) =>
            GetResult(await membersRepository.GetMemberList(query));

        [HttpGet]
        [BackOfficeAuthorize(Roles.Administrator, Roles.Manager, Roles.Compliance, Roles.Marketing, Roles.CustomerService, Roles.CustomerServiceLimited, Roles.GameAnalysis, Roles.GameAnalysisManager)]
        [RestrictMemberQuery]
        public async Task<IActionResult> MemberGameHistory([FromQuery] MemberHistoryQuery query) =>
            GetResult(await membersRepository.GetMemberHistory(query));

        [HttpGet]
        [BackOfficeAuthorize(Roles.Administrator, Roles.Manager, Roles.Compliance, Roles.Marketing, Roles.CustomerService, Roles.CustomerServiceLimited, Roles.GameAnalysis, Roles.GameAnalysisManager)]
        [RestrictMemberQuery]
        public async Task<IActionResult> MemberHistorySummary([FromQuery] MemberHistoryQuery query) =>
            GetResult(await membersRepository.GetHistorySummary(query));

        [HttpGet]
        [BackOfficeAuthorize(Roles.Administrator, Roles.Manager, Roles.Compliance, Roles.Marketing, Roles.CustomerService, Roles.CustomerServiceLimited, Roles.GameAnalysis, Roles.GameAnalysisManager)]
        public async Task<IActionResult> MemberGameHistoryResult([FromQuery] MemberHistoryResultQuery query) =>
            GetResult(await membersRepository.GetMemberHistoryResult(query));

        [HttpGet]
        public async Task<IActionResult> Symbols([FromQuery] SymbolsQuery query) =>
            GetResult(await membersRepository.GetGameSymbols(query));
    }
}