using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Slot.BackOffice.Data.Queries.GamePerformance;
using Slot.BackOffice.Data.Queries.TopWinners;
using Slot.BackOffice.Data.Queries.WinLose;
using Slot.BackOffice.Extensions;
using Slot.Core.Data;
using Slot.Core.Data.Views.GamePerformance;
using Slot.Core.Data.Views.TopWinners;
using Slot.Core.Data.Views.WinLose;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.BackOffice.Data.Repositories
{
    public class ReportsRepository
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IDatabaseManager databaseManager;

        public ReportsRepository(IHttpContextAccessor httpContextAccessor, IDatabaseManager databaseManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.databaseManager = databaseManager;
        }

        public async Task<List<TopWinner>> TopWinners(TopWinnerQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                var result = await db.TopWinners
                                        .Query("REPORTTOPWINNER @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @Username, @Top, @CurrencyId", query)
                                        .ToListAsync();

                return result.WithFilters(query);
            }
        }

        public async Task<List<TopWinnerDetail>> TopWinnerDetail(TopWinnerDetailQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                switch (query.Format)
                {
                    case Enums.TopWinnerFormat.Game:
                        {
                            var x = await db.TopWinnerDetails
                                            .Query("REPORTTOPWINNERGAME @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @UserId", query)
                                            .ToListAsync();
                            return x.WithFilters<TopWinnerDetail>(query);
                        }
                    case Enums.TopWinnerFormat.Daily:
                        {
                            var x = await db.TopWinnerDetailsDaily
                                            .Query($"REPORTTOPWINNER{query.Format} @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @UserId", query)
                                            .ToListAsync();
                            return x.WithFilters<TopWinnerDetail>(query);
                        }
                    case Enums.TopWinnerFormat.Weekly:
                        {
                            var x = await db.TopWinnerDetailsWeekly
                                               .Query($"REPORTTOPWINNER{query.Format} @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @UserId", query)
                                               .ToListAsync();
                            return x.WithFilters<TopWinnerDetail>(query);
                        }
                    case Enums.TopWinnerFormat.Monthly:
                        {
                            var x = await db.TopWinnerDetailsMonthly
                                            .Query($"REPORTTOPWINNER{query.Format} @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @UserId", query)
                                            .ToListAsync();
                            return x.WithFilters<TopWinnerDetail>(query);
                        }
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public async Task<List<WinLoseBase>> WinLose(WinLoseQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                switch (query.FormatFilterType)
                {
                    case Enums.FilterDateType.All:
                    case Enums.FilterDateType.Daily:
                        {
                            var x = await db.WinLoses
                                            .Query($"REPORTWINLOSE{query.FormatFilterType} @OperatorId, @GameId, @UserId, @StartDateInUTC, @EndDateInUTC, @IsDemo, @IsFreeRounds, @CurrencyId, @Platform", query)
                                            .Select(data => new WinLoseByPeriodAll(data)
                                            {
                                                Date = query.FormatFilterType == Enums.FilterDateType.All ?
                                                        $"{query.StartDate.ToString("dd MMM yyyy")} - {query.EndDate.ToString("dd MMM yyyy")}" : data.Date
                                            })
                                            .ToListAsync();
                            return x.WithFilters<WinLoseBase>(query);
                        }
                    case Enums.FilterDateType.Weekly:
                        {
                            var x = await db.WinLosesWeekly
                                            .Query($"REPORTWINLOSE{query.FormatFilterType} @OperatorId, @GameId, @UserId, @StartDateInUTC, @EndDateInUTC, @IsDemo, @IsFreeRounds, @CurrencyId, @Platform", query)
                                            .ToListAsync();
                            return x.WithFilters<WinLoseBase>(query);
                        }
                    case Enums.FilterDateType.Monthly:
                        {
                            var x = await db.WinLosesMonthly
                                            .Query($"REPORTWINLOSE{query.FormatFilterType} @OperatorId, @GameId, @UserId, @StartDateInUTC, @EndDateInUTC, @IsDemo, @IsFreeRounds, @CurrencyId, @Platform", query)
                                            .ToListAsync();
                            return x.WithFilters<WinLoseBase>(query);
                        }
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public async Task<List<WinLoseByMerchant>> WinLoseByMerchant(WinLoseQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                var x = await db.WinLosesByMerchant
                                .Query("REPORTWINLOSEOPERATOR @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @IsDemo, @IsFreeRounds, @CurrencyId, @Platform", query)
                                .ToListAsync();
                return x.WithFilters(query);
            }
        }

        public async Task<List<WinLoseByMerchantDetails>> WinLoseByMerchantDetails(WinLoseQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                var x = await db.WinLosesByMerchantDetails
                                .Query("REPORTWINLOSEOPERATORGAME @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @IsDemo, @IsFreeRounds, @CurrencyId, @Platform", query)
                                .ToListAsync();
                return x.WithFilters(query);
            }
        }

        public async Task<List<WinLoseByGame>> WinLoseByGame(WinLoseQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                var x = await db.WinLosesByGame
                                .Query("REPORTWINLOSEGAME @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @IsDemo, @IsFreeRounds, @CurrencyId, @Platform", query)
                                .ToListAsync();
                return x.WithFilters(query);
            }
        }

        public async Task<List<WinLoseByMember>> WinLoseByMember(WinLoseQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                var x = await db.WinLosesByMember
                                .Query("REPORTWINLOSEGAMEMEMBER @OperatorId, @GameId, @CurrencyId, @UserId, @StartDateInUTC, @EndDateInUTC, @IsDemo, @IsFreeRounds, @Platform", query)
                                .ToListAsync();
                return x.WithFilters(query);
            }
        }

        public async Task<List<WinLoseByCurrency>> WinLoseByCurrency(WinLoseQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                var x = await db.WinLosesByCurrency
                                .Query("REPORTWINLOSEGAMECURRENCY @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @IsDemo, @IsFreeRounds, @CurrencyId, @Platform", query)
                                .ToListAsync();
                return x.WithFilters(query);
            }
        }

        public async Task<List<WinLoseByPlatform>> WinLoseByPlatform(WinLoseQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                var x = await db.WinLosesByPlatform
                                .Query("REPORTWINLOSEPLATFORM @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @IsDemo, @IsFreeRounds, @CurrencyId", query)
                                .ToListAsync();
                return x.WithFilters(query);
            }
        }

        public async Task<List<GamePerformanceBase>> GamePerformance(GamePerformanceQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                switch (query.FilterDateType)
                {
                    case Enums.FilterDateType.None:
                        {
                            if (!string.IsNullOrWhiteSpace(query.CustomSearchType)
                                    && string.Compare(query.CustomSearchType, "member", true, CultureInfo.InvariantCulture) == 0)
                            {
                                var x = await db.GamePerformancesMember
                                                    .Query("REPORTGAMEPERFORMANCEMEMBER @OperatorId, @GameId, @CurrencyId, @StartDateInUTC, @EndDateInUTC, @IsDemo", query)
                                                    .ToListAsync();
                                return x.WithFilters(query)
                                        .ToList<GamePerformanceBase>();
                            }
                            else
                            {
                                var x = await db.GamePerformancesGeneral
                                                    .Query("REPORTGAMEPERFORMANCE @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @IsDemo", query)
                                                    .ToListAsync();
                                return x.WithFilters(query)
                                        .ToList<GamePerformanceBase>();
                            }
                        }
                    case Enums.FilterDateType.Daily:
                        {
                            var x = await db.GamePerformancesDaily
                                            .Query("REPORTGAMEPERFORMANCEDAILY @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @IsDemo", query)
                                            .ToListAsync();
                            return x.WithFilters(query)
                                    .ToList<GamePerformanceBase>();
                        }
                    case Enums.FilterDateType.Weekly:
                        {
                            var x = await db.GamePerformancesWeekly
                                            .Query("REPORTGAMEPERFORMANCEWEEKLY @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @IsDemo", query)
                                            .ToListAsync();
                            return x.WithFilters(query)
                                    .ToList<GamePerformanceBase>();
                        }
                    case Enums.FilterDateType.Monthly:
                        {
                            var x = await db.GamePerformancesMonthly
                                            .Query("REPORTGAMEPERFORMANCEMONTHLY @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @IsDemo", query)
                                            .ToListAsync();
                            return x.WithFilters(query)
                                    .ToList<GamePerformanceBase>();
                        }
                    case Enums.FilterDateType.Currency:
                        {
                            var x = await db.GamePerformancesCurrency
                                            .Query("REPORTGAMEPERFORMANCECURRENCY @OperatorId, @GameId, @StartDateInUTC, @EndDateInUTC, @IsDemo", query)
                                            .ToListAsync();
                            return x.WithFilters(query)
                                    .ToList<GamePerformanceBase>();
                        }
                    default:
                        {

                            throw new ArgumentException("Invalid parameters");
                        }
                }
            }
        }
    }
}