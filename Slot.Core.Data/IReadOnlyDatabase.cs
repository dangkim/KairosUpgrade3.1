using Microsoft.EntityFrameworkCore;
using Slot.Core.Data.Models;
using Slot.Core.Data.Views.GamePerformance;
using Slot.Core.Data.Views.Members;
using Slot.Core.Data.Views.TopWinners;
using Slot.Core.Data.Views.Tournament;
using Slot.Core.Data.Views.WinLose;
using Slot.Model.Entity;
using System;

namespace Slot.Core.Data
{
    public interface IReadOnlyDatabase : IDisposable
    {
        DbSet<Account> Accounts { get; }
        DbSet<BonusEntity> Bonuses { get; }
        DbSet<ConfigurationSetting> ConfigurationSettings { get; }
        DbSet<Counter> Counters { get; }
        DbSet<Currency> Currencies { get; }
        DbSet<ExchangeRate> ExchangeRates { get; }
        DbSet<FreeRoundData> FreeGameData { get; }
        DbSet<FreeRound> FreeGameX { get; }
        DbSet<FreeRoundGameHistory> FreeRoundGameHistory { get; }
        DbSet<GameHistory> GameHistories { get; }
        DbSet<GameHistoryX> GameHistoriesX { get; }
        DbSet<GameRtp> GameRtps { get; }
        DbSet<Game> Games { get; }
        DbSet<GameSetting> GameSettings { get; }
        DbSet<GameState> GameStates { get; }
        DbSet<GameTransactionError> GameTransactionErrors { get; }
        DbSet<GameTransaction> GameTransactions { get; }
        DbSet<GameSettingGroup> Groups { get; }
        DbSet<IncompleteBonus> IncompleteBonuses { get; }
        DbSet<Operator> Operators { get; }
        DbSet<Role> Roles { get; }
        DbSet<SpinBetProfile> SpinBetProfiles { get; }
        DbSet<TournamentReportX> TournamentReportX { get; }
        DbSet<Tournament> Tournaments { get; }
        DbSet<UserGameData> UserGameDatas { get; }
        DbSet<UserGameRtpSetting> UserGameRtpSettings { get; }
        DbSet<UserGameSpinData> UserGameSpinDatas { get; }
        DbSet<User> Users { get; }
        DbSet<UserSessionEntity> UserSessions { get; }
        DbSet<UserSessionLog> UserSessionLogs { get; }
        DbSet<UtcTimeOffset> UtcTimeOffsets { get; }
        DbSet<WalletLog> WalletLog { get; }
        DbSet<WalletProvider> WalletProviders { get; }
        DbSet<WalletTransaction> WalletTransactions { get; }
        DbSet<ReportInfo> ReportInfo { get; set; }

        DbQuery<Member> Members { get; }
        DbQuery<MemberHistory> MemberHistory { get; set; }
        DbQuery<MemberHistorySummary> MemberHistorySummary { get; set; }
        DbQuery<TopWinner> TopWinners { get; }
        DbQuery<TopWinnerDetailGame> TopWinnerDetails { get; }
        DbQuery<TopWinnerDetailDaily> TopWinnerDetailsDaily { get; }
        DbQuery<TopWinnerDetailWeekly> TopWinnerDetailsWeekly { get; }
        DbQuery<TopWinnerDetailMonthly> TopWinnerDetailsMonthly { get; }
        DbQuery<WinLoseByPeriodAll> WinLoses { get; }
        DbQuery<WinLoseByPeriodWeekly> WinLosesWeekly { get; }
        DbQuery<WinLoseByPeriodMonthly> WinLosesMonthly { get; }
        DbQuery<WinLoseByGame> WinLosesByGame { get; }
        DbQuery<WinLoseByMember> WinLosesByMember { get; }
        DbQuery<WinLoseByCurrency> WinLosesByCurrency { get; }
        DbQuery<WinLoseByMerchant> WinLosesByMerchant { get; }
        DbQuery<WinLoseByPlatform> WinLosesByPlatform { get; }
        DbQuery<WinLoseByMerchantDetails> WinLosesByMerchantDetails { get; }
        DbQuery<GamePerformanceGeneral> GamePerformancesGeneral { get; }
        DbQuery<GamePerformanceDaily> GamePerformancesDaily { get; }
        DbQuery<GamePerformanceWeekly> GamePerformancesWeekly { get; }
        DbQuery<GamePerformanceMonthly> GamePerformancesMonthly { get; }
        DbQuery<GamePerformanceCurrency> GamePerformancesCurrency { get; }
        DbQuery<GamePerformanceMember> GamePerformancesMember { get; }
        DbQuery<TournamentInfo> TournamentInfo { get; }
        DbQuery<GlobalTournament> GlobalTournament { get; }
        DbQuery<GlobalTournamentLeaderboardBase> GlobalTournamentLeaderboard { get; }
    }
}