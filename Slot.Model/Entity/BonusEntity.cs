using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("Bonus")]
    public class BonusEntity : CommonEntity
    {
        [Key, Column(Order = 1)]
        public int UserId { get; set; }

        [Key, Column(Order = 2)]
        public int GameId { get; set; }

        [Column(Order = 3), MaxLength(32)]
        public string Guid { get; set; }
        
        [Column(Order = 4)]
        public byte[] Data { get; set; }

        [Column(Order = 6), MaxLength(128)]
        public string BonusType { get; set; }

        [Column(Order = 7)]
        public int Version { get; set; }

        [Column(Order = 8)]
        public bool IsOptional { get; set; }

        [Column(Order = 9)]
        public bool IsStarted { get; set; }

        [DefaultValue("0")]
        public long RoundId { get; set; }

        [DefaultValue("0")]
        public int BnsClsId { get; set; }

        [DefaultValue("0")]
        public int ClientId { get; set; }

        [DefaultValue("0")]
        public int Order { get; set; }

        [MaxLength(255)]
        public string BetReference { get; set; }

        [DefaultValue("0")]
        public bool IsFreeGame { get; set; }

        [DefaultValue("0")]
        public int CampaignId { get; set; }

        public virtual User User { get; set; }
    }
}