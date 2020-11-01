using Newtonsoft.Json;
using Slot.Model;
using Slot.Model.Formatters;
using Slot.Model.Utilities;
using System;

namespace Slot.Core.Data.Views.Members
{
    public class MemberHistory
    {
        public long Id { get; set; }

        public long RoundId { get; set; }

        public long GameTransactionId { get; set; }

        [JsonConverter(typeof(DataFormatter), Formats.DateTime)]
        public DateTime CreatedOn { get => CreatedOnUtc.ToLocalTime(); }

        [JsonConverter(typeof(DataFormatter), Formats.DateTime)]
        public DateTime CreatedOnUtc { get; set; }

        public int Type { get; set; }

        public string TypeString { get => DataConverter.Description((GameResultType)Type); }

        public decimal Bet { get; set; }

        public decimal Win { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Currency { get; set; }

        public int GameId { get; set; }

        public string GameName { get; set; }

        public string OperatorTag { get; set; }

        public int PlatformType { get; set; }

        public string PlatformTypeString { get => DataConverter.Description((PlatformType)PlatformType); }

        public bool IsVoid { get; set; }

        public bool IsFreeGame { get; set; }

        public bool IsSideBet { get; set; }

        public decimal TotalTransaction { get; set; }

        public decimal TotalBet { get; set; }

        public decimal TotalWin { get; set; }

        public decimal TotalBetRmb { get; set; }

        public decimal TotalWinRmb { get; set; }
    }
}
