using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Slot.Model.Entity;
using System;

namespace Slot.Core.Data
{
    // TODO in future should support db sharding based on operator id
    public class DatabaseManager : IDatabaseManager
    {
        //public const string DEFAULT_SHARD = "shard01";
        public const string DEFAULT_SHARD = "ReadOnlyDatabase";

        private readonly ILoggerFactory loggerFactory;
        private readonly IConfiguration configuration;
        private readonly CachedSettings cachedSettings;

        public DatabaseManager(ILoggerFactory loggerFactory, IConfiguration configuration, CachedSettings cachedSettings)
        {
            this.loggerFactory = loggerFactory;
            this.configuration = configuration;
            this.cachedSettings = cachedSettings;
        }

        public IReadOnlyDatabase GetReadOnlyDatabase()
        {
            var connStr = configuration.GetConnectionString("ReadOnlyDatabase");
            var builder = new DbContextOptionsBuilder<SlotContext>()
                                .UseSqlServer(connStr, options => options.EnableRetryOnFailure(
                                    maxRetryCount: 10,
                                    maxRetryDelay: TimeSpan.FromSeconds(30),
                                    errorNumbersToAdd: null))
                                .UseLoggerFactory(loggerFactory)
                                .ConfigureWarnings(x => x.Throw(RelationalEventId.QueryClientEvaluationWarning))
                                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            return new SlotContext(builder.Options);
        }

        public IReadOnlyDatabase GetReadOnlyDatabase(string operatorId)
        {
            return GetReadOnlyDatabase();
        }

        public IWritableDatabase GetWritableDatabase()
        {
            var connStr = configuration.GetConnectionString("WriteableDatabase");
            var builder = new DbContextOptionsBuilder<SlotContext>()
                                .UseSqlServer(connStr)
                                .UseLoggerFactory(loggerFactory);
            return new SlotContext(builder.Options);
        }

        public IWritableDatabase GetWritableDatabase(string operatorId)
        {
            return GetWritableDatabase();
        }

        private string GetShardId(int? operatorId)
        {
            if (operatorId.HasValue)
            {
                if (cachedSettings.OperatorsById.TryGetValue(operatorId.Value, out Operator op))
                {
                    //return op.Shared;
                    return DEFAULT_SHARD;
                }
                throw new ArgumentException("Incorrect database identity");
            }
            else
            {
                return DEFAULT_SHARD;
            }
        }

        public IWritableDatabase GetWritableDatabase(int? operatorId)
        {
            //return GetSlotContext($"Writable:{GetShardId(operatorId)}");
            return GetWritableDatabase();
        }

        public IReadOnlyDatabase GetReadOnlyDatabase(int? operatorId)
        {
            //return GetSlotContext($"ReadOnly:{GetShardId(operatorId)}");
            return GetReadOnlyDatabase();
        }

        private SlotContext GetSlotContext(string connKey)
        {
            return CreateDbContext(configuration.GetConnectionString(connKey));
        }

        public static SlotContext CreateDbContext(string connectionString)
        {
            var builder = new DbContextOptionsBuilder<SlotContext>().UseSqlServer(connectionString);
            return new SlotContext(builder.Options);
        }
    }
}
