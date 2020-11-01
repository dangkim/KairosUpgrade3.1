using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Serializable]
    [Table("FreeRoundData")]
    public class FreeRoundData : CommonEntity
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

        [Column(Order = 6, TypeName = "datetime")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm:ss.fff}")]
        [DataType(DataType.DateTime)]
        public DateTime? TimeClaimed { get; set; }

        [Column(Order = 7, TypeName = "decimal")]
        public decimal Bet { get; set; }

        [Column(Order = 8, TypeName = "int")]
        public int Multiplier { get; set; }
        
        [Column(Order = 9, TypeName = "int")]
        public int Lines { get; set; }

        [Column(Order = 10, TypeName = "int")]
        public int Counter { get; set; }

        [Range(1, 3), Column(Order = 11, TypeName = "tinyint")]
        public byte State { get; set; }

        [Column(Order = 12, TypeName = "bit")]
        [DefaultValue("0")]
        public bool IsFinish { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("GameId")]
        public virtual Game Game { get; set; }

        [ForeignKey("CampaignId")]
        public virtual FreeRound FreeRound { get; set; }
    }
}