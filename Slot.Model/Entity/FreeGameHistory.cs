using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slot.Model.Entity
{
    [Serializable]
    public class FreeGameHistory : CommonEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0, TypeName = "bigint")]
        public long Id { get; set; }

        [Column(Order = 1, TypeName = "int")]
        public int CampaignId { get; set; }

        [Column(Order = 2, TypeName = "int")]
        public int UserId { get; set; }

        [Column(Order = 3, TypeName = "int")]
        public int GameId { get; set; }
        public GameTransactionType GameTransactionType { get; set; }
        
        [Column(Order = 4, TypeName = "datetime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm:ss.fff}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("GETUTCDATE()")]
        [DataType(DataType.DateTime)]
        public DateTime TimeStart { get; set; }

        [Column(Order = 5, TypeName = "datetime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm:ss.fff}")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("GETUTCDATE()")]
        [DataType(DataType.DateTime)]
        public DateTime TimeEnd { get; set; }

        [Column(Order = 6, TypeName = "decimal")]
        public decimal Bet { get; set; }

        [Column(Order = 7, TypeName = "int")]
        public int Multiplier { get; set; }

        [Column(Order = 8, TypeName = "int")]
        public int Counter { get; set; }
        public decimal LineBet { get; set; }
        public int Lines { get; set; }
        public bool IsAutoSpin { get; set; }

        public int Level { get; set; }

        public int Group { get; set; }

        public decimal Win { get; set; }

        public decimal? ExchangeRate { get; set; }

        public decimal EndBalance { get; set; }

        [MaxLength(255)]
        public string WalletReference { get; set; }

        public GameResultType GameResultType { get; set; }

        public bool IsHistory { get; set; }

        public bool IsReport { get; set; }

        public PlatformType PlatformType { get; set; }

        public int Resolution { get; set; }

        public long RoundId { get; set; }
        public XmlType XmlType { get; set; }

        [MaxLength(8000)]
        public string ResultXml { get; set; }

        [MaxLength(255)]
        public string BetReference { get; set; }

        public decimal JackpotCon { get; set; }

        public decimal JackpotCov { get; set; }

        public decimal JackpotCovExtra { get; set; }
    }
}
