using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Slot.Core.Data.Exceptions;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slot.Core.Data
{
    public class CachedSettings
    {
        //const string MAIN_READONLY_DATABASE = "ReadOnly:shard01";
        const string MAIN_READONLY_DATABASE = "ReadOnlyDatabase";

        private readonly ILogger<CachedSettings> logger;
        private readonly IConfiguration configuration;

        public CachedSettings(ILogger<CachedSettings> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public Dictionary<string, Operator> OperatorsByName { get; private set; }
        public Dictionary<int, Operator> OperatorsById { get; private set; }
        public Dictionary<string, Currency> CurrenciesByIsoCode { get; private set; }
        public Dictionary<int, Game> Games { get; private set; }
        public Dictionary<string, Game> GamesByName { get; private set; }
        public Dictionary<string, GameSetting> GameSettings { get; private set; }
        public Dictionary<string, ConfigurationSetting> ConfigSettings { get; private set; }

        public void Load()
        {
            using (var db = DatabaseManager.CreateDbContext(configuration.GetConnectionString(MAIN_READONLY_DATABASE)))
            {
                var operators = db.Operators.ToArray();
                OperatorsByName = ToDictionary(operators, x => x.Tag, x => x, StringComparer.OrdinalIgnoreCase);
                OperatorsById = ToDictionary(operators, x => x.Id, x => x);
                CurrenciesByIsoCode = ToDictionary(db.Currencies.ToArray(), x => x.IsoCode, x => x, StringComparer.OrdinalIgnoreCase);
                var games = db.Games.ToArray();
                Games = ToDictionary(games, x => x.Id, x => x);
                GamesByName = ToDictionary(games, x => NormalizeGameName(x.Name), x => x);
                GameSettings = ToDictionary(db.GameSettings.ToArray(), x => $"{x.GameSettingGroupId}-{x.GameId}-{x.CurrencyId}", x => x);
                ConfigSettings = ToDictionary(db.ConfigurationSettings.ToArray(), x => x.Name, x => x);
            }
        }

        private Dictionary<K, V> ToDictionary<K, V, T>(IEnumerable<T> source, Func<T, K> keySelector, Func<T, V> valueSelector, IEqualityComparer<K> comparer = null)
        {
            var d = new Dictionary<K, V>(comparer);
            try
            {
                foreach (var element in source)
                {
                    var key = keySelector(element);
                    if (!d.ContainsKey(key))
                    {
                        d.Add(key, valueSelector(element));
                    }
                    else
                    {
                        var ex = new DuplicateException($"Key\"{key.ToString()}\" is duplicated for {typeof(T).Name}");
                        logger.LogError(ex, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return d;
        }

        private string NormalizeGameName(string name)
        {
            return name.Replace(" ", "").ToLower();
        }
    }
}
