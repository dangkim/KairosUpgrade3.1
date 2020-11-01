using System;

namespace Slot.Core.Data.Models
{
    public class XGameHistory
    {
        public long Id { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public int OperatorId { get; set; }

        public int CurrencyId { get; set; }

        public int UserId { get; set; }

        public int GameId { get; set; }

        public int Level { get; set; }

        public decimal Bet { get; set; }

        public decimal Win { get; set; }

        public decimal ExchangeRate { get; set; }

        public int GameResultType { get; set; }

        public bool IsHistory { get; set; }

        public bool IsReport { get; set; }

        public int PlatformType { get; set; }

        public long GameTransactionId { get; set; }

        public long? SpinTransactionId { get; set; }

        public decimal? LineBet { get; set; }

        public int? Multiplier { get; set; }

        public bool IsFreeGame { get; set; }

        public int FreeRoundId { get; set; }

        public bool IsSideBet { get; set; }
    }
}