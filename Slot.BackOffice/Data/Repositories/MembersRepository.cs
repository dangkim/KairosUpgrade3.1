using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Slot.BackOffice.Data.History;
using Slot.BackOffice.Data.History.HistoryDecode;
using Slot.BackOffice.Data.Queries.Members;
using Slot.Core.Data;
using Slot.Core.Data.Views.Members;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Entity.Pagination;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using GameId = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.Repositories
{
    public class MembersRepository
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IDatabaseManager databaseManager;
        private readonly CachedSettings cachedSettings;
        private readonly GamePayoutEngine gamePayoutEngine;
        private readonly PaylineRepository paylineRepository;
        private readonly GameInfoRepository gameInfoRepository;
        private readonly SymbolRepository symbolRepository;
        private readonly HistoryDecoderFactory historyDecoderFactory;
        private readonly List<int> respinGames = new List<int>
        {
            (int)GameId.GeniesLuck,
            (int)GameId.FuLuShou,
            (int)GameId.GoldenWheel,
            (int)GameId.SevenWonders,
            (int)GameId.FloraSecret,
            (int)GameId.FortuneKoi
        };

        public MembersRepository(
            IHttpContextAccessor httpContextAccessor,
            IDatabaseManager databaseManager,
            CachedSettings cachedSettings,
            GamePayoutEngine gamePayoutEngine,
            PaylineRepository paylineRepository,
            SymbolRepository symbolRepository,
            HistoryDecoderFactory historyDecoderFactory,
            GameInfoRepository gameInfoRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.databaseManager = databaseManager;
            this.cachedSettings = cachedSettings;
            this.gamePayoutEngine = gamePayoutEngine;
            this.paylineRepository = paylineRepository;
            this.symbolRepository = symbolRepository;
            this.gameInfoRepository = gameInfoRepository;
            this.historyDecoderFactory = historyDecoderFactory;
        }

        public async Task<PaginatedList<Member>> GetMemberList(MemberListQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                var memberList = await db.Members
                                            .Query("GETMEMBERLISTDETAILS @MemberId, @MemberName, @OperatorId, @CurrencyId, @IsDemoAccount, @PageNumber, @PageSize", query)
                                            .ToListAsync();

                return PaginatedList<Member>.Create(memberList, query.PageIndex, query.PageSize);
            }
        }

        public async Task<PaginatedList<MemberHistory>> GetMemberHistory(MemberHistoryQuery query)
        {
            if (!query.OperatorId.HasValue)
            {
                return PaginatedList<MemberHistory>.Empty(query.PageIndex, query.PageSize);
            }

            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                if (query.TransactionId.HasValue)
                {
                    var tnxId = query.TransactionId.Value;

                    var historyWithGameUser = await db.GameHistories.Where(trx => trx.GameTransactionId == tnxId)
                        .Select(h => new
                        {
                            h.CreatedOnUtc,
                            h.Bet,
                            h.GameId,
                            h.Id,
                            h.IsFreeGame,
                            h.IsDeleted,
                            h.PlatformType,
                            h.RoundId,
                            h.GameResultType,
                            h.UserId,
                            h.Win,
                            UserName = h.User.Name,
                            GameName = h.Game.Name
                        }).FirstOrDefaultAsync();


                    var memberHistory = historyWithGameUser == null ? new List<MemberHistory>() : new List<MemberHistory> {
                        new MemberHistory {
                            Bet = historyWithGameUser.Bet,
                            CreatedOnUtc = historyWithGameUser.CreatedOnUtc,
                            GameId = historyWithGameUser.GameId,
                            IsSideBet = false,
                            GameTransactionId = tnxId,
                            GameName = historyWithGameUser.GameName,
                            Id = historyWithGameUser.Id,
                            IsFreeGame = historyWithGameUser.IsFreeGame,
                            IsVoid = historyWithGameUser.IsDeleted,
                            PlatformType = (int)historyWithGameUser.PlatformType,
                            RoundId = historyWithGameUser.RoundId,
                            Type = (int)historyWithGameUser.GameResultType,
                            UserId = historyWithGameUser.UserId,
                            UserName = historyWithGameUser.UserName,
                            Win = historyWithGameUser.Win
                        }
                    };

                    return PaginatedList<MemberHistory>.Create(memberHistory, query.PageIndex, query.PageSize);
                }
                else
                {
                    await query.GetUserId(db);
                    var gamesEnum = ((GameId[])System.Enum.GetValues(typeof(GameId))).Select(c => (int)c).ToList();
                    var platformTypes = ((PlatformType[])System.Enum.GetValues(typeof(PlatformType))).Select(c => (int)c).ToList();
                    var gameResultType = ((GameResultType[])System.Enum.GetValues(typeof(GameResultType))).Select(c => (int)c).ToList();

                    var memberHistories = new List<MemberHistory>();
                    var gameHistories = new List<GameHistory>();

                    var gameIds = (query.GameId == null || query.GameId == 0) ? gamesEnum : gamesEnum.Where(i => i == query.GameId.Value).ToList();
                    var platformIds = string.IsNullOrEmpty(query.PlatformType) ? platformTypes : query.PlatformType.Split(',').Select(int.Parse).ToList();
                    var gameResultIds = query.GameTransactionType == 0 ? gameResultType : gameResultType.Where(i => i == query.GameTransactionType).ToList();


                    var sql = db.GameHistories.Where(trx => trx.UserId == query.MemberId &&
                                                            trx.DateTimeUtc >= query.StartDateInUTC &&
                                                            trx.DateTimeUtc <= query.EndDateInUTC);
                    if (gameIds.Count > 0)
                    {
                        sql = sql.Where(trx => gameIds.Contains(trx.GameId));
                    }
                    if (platformIds.Count > 0)
                    {
                        sql = sql.Where(trx => platformIds.Contains((int)trx.PlatformType));
                    }
                    if (gameResultIds.Count > 0)
                    {
                        sql = sql.Where(trx => gameResultIds.Contains((int)trx.GameResultType));
                    }

                    gameHistories = await sql.OrderByDescending(trx => trx.DateTimeUtc)
                                             .Skip(query.Offset)
                                             .Take(query.PageSize)
                                             .ToListAsync();


                    var historyWithGameUser = gameHistories
                        .Select(h => new
                        {
                            h.GameTransactionId,
                            h.CreatedOnUtc,
                            h.Bet,
                            h.GameId,
                            h.Id,
                            h.IsFreeGame,
                            h.IsDeleted,
                            h.PlatformType,
                            h.RoundId,
                            h.GameResultType,
                            h.UserId,
                            h.Win,
                            UserName = query.MemberName,
                            GameName = ((GameId)h.GameId).ToString()
                        }).ToList();

                    var listId = historyWithGameUser.Select(hu => hu.GameTransactionId).ToArray();

                    var profileSideBet = await db.SpinBetProfiles.Where(sb => listId.Contains(sb.GameTransactionId) && sb.IsSideBet).ToListAsync();

                    var listIdSideBet = profileSideBet.Select(sb => sb.GameTransactionId).ToArray();

                    foreach (var item in historyWithGameUser)
                    {
                        var isSideBet = listIdSideBet.Contains(item.GameTransactionId);

                        memberHistories.Add(new MemberHistory
                        {
                            Bet = isSideBet ? item.Bet * 2 : item.Bet,
                            CreatedOnUtc = item.CreatedOnUtc,
                            GameId = item.GameId,
                            IsSideBet = false,
                            GameTransactionId = item.GameTransactionId,
                            GameName = item.GameName,
                            Id = item.Id,
                            IsFreeGame = item.IsFreeGame,
                            IsVoid = item.IsDeleted,
                            PlatformType = (int)item.PlatformType,
                            RoundId = item.RoundId,
                            Type = (int)item.GameResultType,
                            UserId = item.UserId,
                            UserName = item.UserName,
                            Win = item.Win
                        });

                    }

                    return PaginatedList<MemberHistory>.Create(memberHistories, query.PageIndex, query.PageSize);
                }

            }
        }

        public async Task<MemberHistoryResult> GetMemberHistoryResult(MemberHistoryResultQuery query)
        {
            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorTag))
            {
                var history = await db.GameHistories
                                        .Include(gh => gh.Game)
                                        .FirstOrDefaultAsync(gh => gh.GameTransactionId == query.TransactionId);

                if (history != null)
                {
                    var validTransactionId = MemberHistoryResult.GetTransactionId(history);

                    var spinBetProfile = await db.SpinBetProfiles
                                                    .FirstOrDefaultAsync(sbp => sbp.GameTransactionId == validTransactionId);

                    return new MemberHistoryResult(
                                gamePayoutEngine,
                                paylineRepository,
                                gameInfoRepository,
                                history,
                                historyDecoderFactory,
                                spinBetProfile);
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<MemberHistorySummary> GetHistorySummary(MemberHistoryQuery query)
        {
            if (!query.OperatorId.HasValue)
            {
                return new MemberHistorySummary();
            }

            using (var db = databaseManager.GetReadOnlyDatabase(query.OperatorId))
            {
                {
                    await query.GetUserId(db);

                    var memberHistorySummary = await db.MemberHistorySummary
                                                    .Query(@"USERHISTORYSUMMARY @OperatorId, @GameId, @UserId, @UserName, @TrxId, @GameTrxType, @StartDateInUTC, @EndDateInUTC, @IsDemo, @PlatformType", query)
                                                    .ToListAsync();

                    return memberHistorySummary.FirstOrDefault();
                }
            }
        }

        public async Task<GameSymbols> GetGameSymbols(SymbolsQuery query)
        {
            GameSymbols gameSymbols = null;

            using (var db = databaseManager.GetReadOnlyDatabase())
            {
                Game gameRef;
                if (query.GameId.HasValue)
                {
                    gameRef = await db.Games.FirstOrDefaultAsync(game => game.Id == query.GameId);
                }
                else
                {
                    gameRef = await db.Games.FirstOrDefaultAsync(game => game.Name == query.GameName);
                }

                if (gameRef != null)
                {
                    gameSymbols = new GameSymbols
                    {
                        GameId = gameRef.Id,
                        GameName = gameRef.Name,
                        Symbols = new List<GameSymbols.Symbol>()
                    };

                    foreach (var symbol in query.Symbols)
                    {
                        if (query.IsBonus)
                        {
                            using (var bonusItem = symbolRepository.GetBonusIcon(gameSymbols.GameId, symbol))
                            {
                                if (bonusItem != null)
                                {
                                    gameSymbols.Symbols.Add(new GameSymbols.Symbol(symbol, bonusItem.GetBase64()));
                                }
                            }
                        }
                        else
                        {
                            using (var symbolItem = symbolRepository.GetReelIcon(gameSymbols.GameId, symbol))
                            {
                                if (symbolItem != null)
                                {
                                    gameSymbols.Symbols.Add(new GameSymbols.Symbol(symbol, symbolItem.GetBase64()));
                                }
                            }
                        }
                    }
                }
            }

            return gameSymbols;
        }
    }
}
