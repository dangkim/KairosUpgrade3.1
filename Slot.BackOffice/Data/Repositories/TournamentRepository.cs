using Microsoft.EntityFrameworkCore;
using Slot.BackOffice.Data.Queries.Tournament;
using Slot.Core.Data;
using Slot.Core.Data.Views.Tournament;
using Slot.Model.Entity;
using Slot.Model.Entity.Pagination;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.BackOffice.Data.Repositories
{
    public class TournamentRepository
    {
        private readonly IDatabaseManager databaseManager;

        public TournamentRepository(IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        public async Task<Tournament> Get(int id)
        {
            using (var db = databaseManager.GetReadOnlyDatabase())
            {
                return await db.Tournaments.FindAsync(id);
            }
        }

        public async Task<List<GlobalTournament>> GetGlobalTournaments(TournamentQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase())
            {
                var globalTournaments = await db.GlobalTournament
                                                    .Query("GETGLOBALTOURNAMENT @StartDateInUtc, @EndDateInUtc, @Operator, @Name, @Platform, @OffsetRows, @PageSize, @OrderBy, @Dir", query)
                                                    .ToListAsync();

                return PaginatedList<GlobalTournament>.Create(globalTournaments, query.PageIndex, query.PageSize);
            }
        }

        public async Task<List<GlobalTournamentLeaderboard>> GetGlobalLeaderboard(int tournamentId)
        {
            using (var db = databaseManager.GetReadOnlyDatabase())
            {

                var leaderboards = await db.GlobalTournamentLeaderboard
                                            .FromSql("BOGETGLOBALLEADERBOARDDETAIL @TournamentId", new SqlParameter("@TournamentId", tournamentId))
                                            .ToListAsync();
                var cid = leaderboards.Select(lb => lb.CurrencyId).ToArray();
                var currencies = await db.Currencies
                                            .Where(currency =>
                                                    cid.Contains(currency.Id)
                                                    && currency.IsVisible
                                                    && currency.IsoCode != "UNK")
                                            .ToListAsync();

                return leaderboards
                        .Select(lb => new GlobalTournamentLeaderboard(lb)
                        {
                            Currency = currencies.First(currency => currency.Id == lb.CurrencyId).IsoCode
                        }).ToList();
            }
        }
    }
}
