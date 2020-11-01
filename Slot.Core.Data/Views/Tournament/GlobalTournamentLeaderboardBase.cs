using Newtonsoft.Json;
using Slot.Model.Formatters;
using System;

namespace Slot.Core.Data.Views.Tournament
{
    public class GlobalTournamentLeaderboardBase
    {
        public string Operator { get; set; }

        public int UserId { get; set; }

        public int CurrencyId { get; set; }

        public int Rank { get; set; }

        public int Points { get; set; }

        public string MemberName { get; set; }

        [JsonConverter(typeof(DataFormatter), Formats.Currency)]
        public decimal Bet { get; set; }

        [JsonConverter(typeof(DataFormatter), Formats.Currency)]
        public decimal Win { get; set; }

        [JsonConverter(typeof(DataFormatter), Formats.Currency)]
        public decimal BetL { get; set; }

        [JsonConverter(typeof(DataFormatter), Formats.Currency)]
        public decimal WinLoseL { get; set; }

        [JsonConverter(typeof(DataFormatter), Formats.DateTime)]
        public DateTime FirstBet { get; set; }

        [JsonConverter(typeof(DataFormatter), Formats.DateTime)]
        public DateTime LastBet { get; set; }
    }
}
