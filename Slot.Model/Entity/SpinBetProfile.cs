using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Table("SpinBet", Schema = "Profile")]
    public class SpinBetProfile : CommonEntity
    {
        [Key]
        [ForeignKey("GameTransaction")]
        [Column(Order = 1)]
        public long GameTransactionId { get; set; }

        [Column(Order = 2)]
        public decimal LineBet { get; set; }

        [Column(Order = 3)]
        public int Lines { get; set; }

        [Column(Order = 4)]
        public int Multiplier { get; set; }

        [Column(Order = 5)]
        public decimal TotalBet { get; set; }

        [Column(Order = 6)]
        public bool IsAutoSpin { get; set; }

        public bool IsSideBet { get; set; }

        public virtual GameTransaction GameTransaction { get; set; }
    }
}