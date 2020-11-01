using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Slot.BackOffice.Data.Authentication;
using Slot.BackOffice.Data.Queries.AzureMetrics;
using Slot.BackOffice.Data.Queries.GamePerformance;
using Slot.BackOffice.Data.Queries.TopWinners;
using Slot.BackOffice.Data.Queries.WinLose;
using Slot.BackOffice.Data.Repositories;
using Slot.BackOffice.Filters;
using Slot.BackOffice.HttpClients;
using System;
using System.Threading.Tasks;

namespace Slot.BackOffice.Controllers
{
    [ApiVersion("1.0")]
    [BackOfficeAuthorize(Roles.Administrator, Roles.Manager, Roles.GameAnalysis, Roles.GameAnalysisManager)]
    [ValidateQueryOperator]
    [Route("api/dashboard/[action]")]
    public class AzureMetricsController : BaseController
    {
        private readonly DashboardRepository dashboardRepository;
        private readonly ReportsRepository reportsRepository;
        private readonly AzureMetricsClient azureMetricsClient;

        public AzureMetricsController(
            DashboardRepository dashboardRepository,
            ReportsRepository reportsRepository,
            AzureMetricsClient azureMetricsClient)
        {
            this.dashboardRepository = dashboardRepository;
            this.reportsRepository = reportsRepository;
            this.azureMetricsClient = azureMetricsClient;
        }

        [HttpGet]
        public async Task<IActionResult> Regions([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents |
                                where {dashboardRepository.GetTimestampFilter(query.Period)} |
                                project client_CountryOrRegion |
                                where isnotempty(client_CountryOrRegion) |
                                summarize by client_CountryOrRegion |
                                order by client_CountryOrRegion asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> Operators([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents |
                                where {dashboardRepository.GetTimestampFilter(query.Period)} |
                                project operator = tostring(customDimensions[""Operator ID""]) |
                                where isnotempty(operator) {(string.IsNullOrWhiteSpace(query.Operator) ? "" : $"and operator =~ \"{query.Operator}\"")} |
                                summarize by operator |
                                order by operator asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> Currencies([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents |
                                where {dashboardRepository.GetTimestampFilter(query.Period)} |
                                project currency = tostring(customDimensions[""Currency""]) |
                                where isnotempty(currency) |
                                summarize by currency |
                                order by currency asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> UsersByCountry([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents |
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} |
                                project user_Id, client_CountryOrRegion |
                                summarize user_count = round(dcount(user_Id) * {dashboardRepository.GetDataSummaryMultiplier()}) by client_CountryOrRegion |
                                where user_count > 0 |
                                order by user_count asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> UsersByOperatingSystem([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents | 
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} |
                                project user_Id, client_OS |
                                summarize user_count = round(dcount(user_Id) * {dashboardRepository.GetDataSummaryMultiplier()}) by client_OS |
                                where user_count > 0 |
                                order by user_count asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> UsersByBrowsers([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents | 
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} |
                                project user_Id, client_Browser |
                                summarize user_count = round(dcount(user_Id) * {dashboardRepository.GetDataSummaryMultiplier()}) by client_Browser |
                                where user_count > 0 |
                                order by user_count asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> DailyUniqueUsers([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents | 
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} |
                                project user_Id, timestamp |
                                summarize daily_unique_users = round(dcount(user_Id) * {dashboardRepository.GetDataSummaryMultiplier()}) by bin(timestamp, {dashboardRepository.GetPeriodSummaryGrouping(query.Period)}) |
                                where daily_unique_users > 0 |
                                order by timestamp asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> UsersByOperator([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents | 
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} |
                                project user_Id, operator_id = tostring(customDimensions[""Operator ID""]) |
                                where isnotempty(operator_id) |
                                summarize user_count = round(dcount(user_Id) * {dashboardRepository.GetDataSummaryMultiplier()}) by operator_id |
                                where user_count > 0 |
                                order by user_count asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> UsersByCurrency([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents | 
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} |
                                project user_Id, currency = tostring(customDimensions[""Currency""]) |
                                where isnotempty(currency) |
                                summarize user_count = round(dcount(user_Id) * {dashboardRepository.GetDataSummaryMultiplier()}) by currency |
                                where user_count > 0 |
                                order by user_count asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> UsersByGame([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents | 
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} |
                                project user_Id, game_id = tostring(customDimensions[""Game ID""]) |
                                where isnotempty(game_id) |
                                summarize user_count = round(dcount(user_Id) * {dashboardRepository.GetDataSummaryMultiplier()}) by game_id |
                                where user_count > 0 |
                                order by game_id asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> Spins([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents | 
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} | 
                                project spin = tostring(customDimensions[""Spin mode""]), timestamp |
                                where isnotempty(spin) |
                                summarize spin_count = round(count(spin) * {dashboardRepository.GetDataSummaryMultiplier()}) by bin(timestamp, { dashboardRepository.GetPeriodSummaryGrouping(query.Period) }) |
                                where spin_count > 0 |
                                order by timestamp asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> SpinsByOperator([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents | 
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} | 
                                project operator = tostring(customDimensions[""Operator ID""]), spin = tostring(customDimensions[""Spin mode""]) | 
                                where isnotempty(operator) and isnotempty(spin) |
                                evaluate pivot(spin, round(count(operator * {dashboardRepository.GetDataSummaryMultiplier()}))) |
                                where operator > 0 |
                                order by operator asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> SpinsByCurrency([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents | 
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} | 
                                project currency = tostring(customDimensions[""Currency""]), spin = tostring(customDimensions[""Spin mode""]) | 
                                where isnotempty(currency) and isnotempty(spin) |
                                evaluate pivot(spin, round(count(currency * {dashboardRepository.GetDataSummaryMultiplier()}))) |
                                where currency > 0 |
                                order by currency asc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> SpinsByGame([FromQuery]AzureMetricsQuery query)
        {
            var queryString = $@"customEvents | 
                                where {dashboardRepository.GetTimestampFilter(query.Period)} {dashboardRepository.GetQueryFilters(query)} | 
                                project game_id = tostring(customDimensions[""Game ID""]), spin = tostring(customDimensions[""Spin mode""]) | 
                                where isnotempty(game_id) and isnotempty(spin) |
                                summarize spin_count = round(count(spin) * {dashboardRepository.GetDataSummaryMultiplier()}) by game_id |
                                where spin_count > 0 |
                                order by spin_count desc";

            return await GetContent(queryString);
        }

        [HttpGet]
        public async Task<IActionResult> WinLose([FromQuery]WinLoseQuery query) =>
            GetResult(await reportsRepository.WinLose(query));

        [HttpGet]
        public async Task<IActionResult> TopWinners([FromQuery]TopWinnerQuery query) =>
            GetResult(await reportsRepository.TopWinners(query));

        [HttpGet]
        public async Task<IActionResult> GamePerformance([FromQuery]GamePerformanceQuery query) =>
            GetResult(await reportsRepository.GamePerformance(query));

        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> CountriesData([FromQuery(Name = "regions")]string[] regions) =>
            GetResult(await dashboardRepository.GetCountriesData(regions));

        private async Task<IActionResult> GetContent(string queryString)
        {
            var response = await azureMetricsClient.GetContent(queryString);

            return GetResult(JObject.Parse(response), "application/json");
        }
    }
}