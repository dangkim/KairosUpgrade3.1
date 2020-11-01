using Microsoft.EntityFrameworkCore;
using Slot.Core.Data.Models;
using Slot.Core.Data.Views.GamePerformance;
using Slot.Core.Data.Views.Members;
using Slot.Core.Data.Views.TopWinners;
using Slot.Core.Data.Views.Tournament;
using Slot.Core.Data.Views.WinLose;
using Slot.Model.Entity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Slot.Core.Data
{
    public class SlotContext : DbContext, IWritableDatabase
    {
        public SlotContext(DbContextOptions<SlotContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var baseType = typeof(BaseEntity);
            foreach (var entityType in builder.Model.GetEntityTypes().Where(x => baseType.IsAssignableFrom(x.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.CreatedOnUtc))
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("GETUTCDATE()");
            }

            builder.Entity<BonusEntity>()
                .HasKey(x => new { x.UserId, x.GameId });

            builder.Entity<GameSetting>()
                .HasKey(x => new { x.GameSettingGroupId, x.GameId, x.CurrencyId });

            builder.Entity<GameState>()
                .HasKey(x => new { x.UserId, x.GameId });

            builder.Entity<UserGameData>()
                .HasKey(x => new { x.UserId, x.GameId });

            builder.Entity<WalletLog>()
                .HasKey(x => new { x.ServerId, x.OperatorId, x.MemberId, x.TrxId });

            builder.Entity<UserSessionEntity>()
                .HasKey(x => new { x.UserId });
        }

        public override int SaveChanges()
        {
            SetUpdatedOnUtc();
            SoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SetUpdatedOnUtc();
            SoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetUpdatedOnUtc()
        {
            var updatedEntities = ChangeTracker.Entries().Where(entry => entry.State == EntityState.Modified);
            foreach (var updatedEntity in updatedEntities.Where(entry => entry.Entity is CommonEntity))
            {
                ((CommonEntity)updatedEntity.Entity).UpdatedOnUtc = DateTime.UtcNow;
            }
            foreach (var updatedEntity in updatedEntities.Where(entry => entry.Entity is BaseEntity))
            {
                ((BaseEntity)updatedEntity.Entity).UpdatedOnUtc = DateTime.UtcNow;
            }
        }

        private void SoftDelete()
        {
            var deletedEntities =
                ChangeTracker.Entries()
                .Where(entry => entry.State == EntityState.Deleted)
                .Where(entry => entry.Entity is BaseEntity);

            foreach (var deletedEntity in deletedEntities)
            {
                ((BaseEntity)deletedEntity.Entity).IsDeleted = true;
                ((BaseEntity)deletedEntity.Entity).DeletedOnUtc = DateTime.UtcNow;

                deletedEntity.State = EntityState.Modified;
            }
        }

        public void InsertOrUpdate<T>(T entity, params object[] id) where T : class
        {
            var existingEntity = Find<T>(id);
            if (existingEntity == null)
            {
                Add(entity);
            }
            else
            {
                Entry(existingEntity).State = EntityState.Detached;
                Update(entity);
            }
        }

        public DbContext Context => this;

        public DbSet<Account> Accounts { get; set; }

        public DbSet<BonusEntity> Bonuses { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<GameHistory> GameHistories { get; set; }

        public DbSet<GameSetting> GameSettings { get; set; }

        public DbSet<GameState> GameStates { get; set; }

        public DbSet<GameTransactionError> GameTransactionErrors { get; set; }

        public DbSet<GameTransaction> GameTransactions { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<GameSettingGroup> Groups { get; set; }

        public DbSet<IncompleteBonus> IncompleteBonuses { get; set; }

        public DbSet<Operator> Operators { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<SpinBetProfile> SpinBetProfiles { get; set; }

        public DbSet<UserGameRtpSetting> UserGameRtpSettings { get; set; }

        public DbSet<UserSessionEntity> UserSessions { get; set; }

        public DbSet<UserSessionLog> UserSessionLogs { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UtcTimeOffset> UtcTimeOffsets { get; set; }

        public DbSet<WalletProvider> WalletProviders { get; set; }

        public DbSet<WalletTransaction> WalletTransactions { get; set; }

        public DbSet<WalletLog> WalletLog { get; set; }

        public DbSet<UserGameData> UserGameDatas { get; set; }

        public DbSet<ConfigurationSetting> ConfigurationSettings { get; set; }

        public DbSet<Tournament> Tournaments { get; set; }

        public DbSet<Counter> Counters { get; set; }

        public DbSet<GameHistoryX> GameHistoriesX { get; set; }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        public DbSet<FreeRound> FreeGameX { get; set; }

        public DbSet<FreeRoundData> FreeGameData { get; set; }

        public DbSet<GameRtp> GameRtps { get; set; }

        public DbSet<FreeRoundGameHistory> FreeRoundGameHistory { get; set; }

        public DbSet<UserGameSpinData> UserGameSpinDatas { get; set; }

        public DbSet<TournamentReportX> TournamentReportX { get; set; }

        public DbSet<ReportInfo> ReportInfo { get; set; }

        // Queries
        public DbQuery<TournamentInfo> TournamentInfos { get; set; }

        public DbQuery<Member> Members { get; set; }

        public DbQuery<MemberHistory> MemberHistory { get; set; }

        public DbQuery<MemberHistorySummary> MemberHistorySummary { get; set; }

        public DbQuery<TopWinner> TopWinners { get; set; }

        public DbQuery<TopWinnerDetailGame> TopWinnerDetails { get; set; }

        public DbQuery<TopWinnerDetailDaily> TopWinnerDetailsDaily { get; set; }

        public DbQuery<TopWinnerDetailWeekly> TopWinnerDetailsWeekly { get; set; }

        public DbQuery<TopWinnerDetailMonthly> TopWinnerDetailsMonthly { get; set; }

        public DbQuery<WinLoseByPeriodAll> WinLoses { get; set; }

        public DbQuery<WinLoseByPeriodWeekly> WinLosesWeekly { get; set; }

        public DbQuery<WinLoseByPeriodMonthly> WinLosesMonthly { get; set; }

        public DbQuery<WinLoseByGame> WinLosesByGame { get; set; }

        public DbQuery<WinLoseByMember> WinLosesByMember { get; set; }

        public DbQuery<WinLoseByCurrency> WinLosesByCurrency { get; set; }

        public DbQuery<WinLoseByMerchant> WinLosesByMerchant { get; set; }

        public DbQuery<WinLoseByPlatform> WinLosesByPlatform { get; set; }

        public DbQuery<WinLoseByMerchantDetails> WinLosesByMerchantDetails { get; set; }

        public DbQuery<GamePerformanceGeneral> GamePerformancesGeneral { get; set; }

        public DbQuery<GamePerformanceDaily> GamePerformancesDaily { get; set; }

        public DbQuery<GamePerformanceWeekly> GamePerformancesWeekly { get; set; }

        public DbQuery<GamePerformanceMonthly> GamePerformancesMonthly { get; set; }

        public DbQuery<GamePerformanceCurrency> GamePerformancesCurrency { get; set; }

        public DbQuery<GamePerformanceMember> GamePerformancesMember { get; set; }

        public DbQuery<TournamentInfo> TournamentInfo { get; set; }

        public DbQuery<GlobalTournament> GlobalTournament { get; set; }

        public DbQuery<GlobalTournamentLeaderboardBase> GlobalTournamentLeaderboard { get; set; }
    }
}
