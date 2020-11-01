using Newtonsoft.Json;
using Slot.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using GameId = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.History
{
    [DataContract]
    public class PaylineAdapter
    {
        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public int Source { get; set; }

        [DataMember]
        public Dictionary<int, byte[]> Paylines { get; set; }
    }

    public class PaylineRepository : BaseRepository
    {
        private static readonly Dictionary<string, Payline> Paylines = new Dictionary<string, Payline>();
        private readonly GameInfoRepository gameInfoRepository;

        public PaylineRepository(GameInfoRepository gameInfoRepository) : base()
        {
            this.gameInfoRepository = gameInfoRepository;
        }

        private Payline GetPayline(string key)
        {
            Payline payline = null;
            string[] resourceNameSegments = null;
            string targetResource = null;

            var resource = Array.Find(manifestResourceNames, name =>
            {
                var isMatch = false;

                if (name.EndsWith(".payline"))
                {
                    var resourceSegments = name.Split('.');

                    if (resourceSegments.Length > 1)
                    {
                        var resourceName = resourceSegments[resourceSegments.Length - 2];
                        isMatch = string.Compare(resourceName, key, true, CultureInfo.InvariantCulture) == 0;

                        if (isMatch)
                        {
                            resourceNameSegments = resourceSegments;
                            targetResource = resourceName;
                        }
                    }
                }

                return isMatch;
            });

            if (!string.IsNullOrWhiteSpace(resource) && resourceNameSegments.Length > 1)
            {
                using (var stream = executingAssembly.GetManifestResourceStream(resource))
                {
                    if (stream != null)
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var adapter = JsonConvert.DeserializeObject<PaylineAdapter>(sr.ReadToEnd());
                            payline = new Payline(
                                            adapter.Paylines,
                                            adapter.Paylines.Count,
                                            GetConfig((PaylineType)adapter.Type));

                            Paylines.Add(targetResource, payline);
                        }
                    }
                }
            }

            return payline;
        }

        public Payline Get(GameId gameId)
        {
            var gameInfo = gameInfoRepository.Get(gameId);

            if (gameInfo != null)
            {
                return Get(gameInfo.PayLineId) ?? Get("local3x5_50");
            }
            else
            {
                return null;
            }
        }

        public Payline Get(string key)
        {
            try
            {
                if (Paylines.TryGetValue(key, out Payline payline))
                {
                    return Paylines[key];

                }
                else if (!string.IsNullOrWhiteSpace(key))
                {
                    return GetPayline(key);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Func<int, int, PaylineConfig> GetConfig(PaylineType paylineType)
        {
            switch (paylineType)
            {
                case PaylineType.SevenByFive:
                    {
                        return (_, code) =>
                        {
                            var r = code % 5;
                            var y = code / 5;
                            return new PaylineConfig(r, y);
                        };
                    }
                case PaylineType.ThreeByThree:
                    {
                        return (_, code) =>
                        {
                            var r = code % 3;
                            var y = code / 3;
                            return new PaylineConfig(r, y);
                        };
                    }
                default:
                    {
                        return (index, code) => new PaylineConfig(index, code);
                    }
            }
        }
    }
}
