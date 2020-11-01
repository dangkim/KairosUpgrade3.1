using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    public class GameHistoryX
    {
        //[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("GETUTCDATE()")]   
        public DateTime DateTimeUtc { get; set; }

        public GameTransactionType GameTransactionType { get; set; }

        public decimal LineBet { get; set; }

        public int Lines { get; set; }

        public int Multiplier { get; set; }

        public bool IsAutoSpin { get; set; }

        public int UserId { get; set; }

        public int GameId { get; set; }

        public int Level { get; set; }

        public int Group { get; set; }

        public decimal Bet { get; set; }

        public decimal Win { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal EndBalance { get; set; }

        [MaxLength(255)]
        public string WalletReference { get; set; }

        public GameResultType GameResultType { get; set; }

        public XmlType XmlType { get; set; }

        [MaxLength(8000)]
        public string ResultXml { get; set; }

        [MaxLength(8000)]
        public string ResultJson { get; set; }

        public bool IsSideBet { get; set; }

        [Column(TypeName = "bit")]
        [DefaultValue("0")]
        public bool IsFreeGame { get; set; }

        //[DefaultValue("0")]
        //public int FreeRoundId { get; set; }

        public bool IsHistory { get; set; }

        public bool IsReport { get; set; }

        public PlatformType PlatformType { get; set; }

        public int Resolution { get; set; }

        public long RoundId { get; set; }

        [MaxLength(255)]
        public string BetReference { get; set; }

        public decimal JackpotCon { get; set; }

        public decimal JackpotCov { get; set; }

        public decimal JackpotCovExtra { get; set; }
    }
}