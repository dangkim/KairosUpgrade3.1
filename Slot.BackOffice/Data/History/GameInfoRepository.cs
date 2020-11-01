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
    public class GameInfoAdapter
    {
        [DataMember]
        public int GameId { get; set; }

        [DataMember]
        public string PayLineId { get; set; }

        [DataMember]
        public int PayLineType { get; set; }
    }

    public class GameInfoRepository : BaseRepository
    {
        private static readonly Dictionary<GameId, GameInfoAdapter> GameInfo = new Dictionary<GameId, GameInfoAdapter>();

        public GameInfoAdapter Get(GameId gameId)
        {
            if (!GameInfo.TryGetValue(gameId, out GameInfoAdapter gameInfo))
            {
                var gameName = gameId.ToString();
                var gameResource = Array.Find(manifestResourceNames, name =>
                {
                    var isMatch = false;

                    if (name.EndsWith(".slotgame"))
                    {
                        var gameSegments = name.Split(".");

                        if (gameSegments.Length > 1)
                        {
                            var gameNameSegment = gameSegments[gameSegments.Length - 2];

                            isMatch = string.Compare(gameName, gameNameSegment, true, CultureInfo.InvariantCulture) == 0;
                        }
                    }

                    return isMatch;
                });

                if (gameResource != null)
                {
                    using (var stream = executingAssembly.GetManifestResourceStream(gameResource))
                    {
                        if (stream != null)
                        {
                            using (var sr = new StreamReader(stream))
                            {
                                gameInfo = JsonConvert.DeserializeObject<GameInfoAdapter>(sr.ReadToEnd());
                                GameInfo.Add((GameId)gameInfo.GameId, gameInfo);
                            }
                        }
                    }
                }
            }

            return gameInfo;
        }

        public PaylineType GetPayLineType(GameId gameId)
        {
            return GameInfo.ContainsKey(gameId) ? (PaylineType)GameInfo[gameId].PayLineType : PaylineType.ThreeByFive;
        }
    }
}
