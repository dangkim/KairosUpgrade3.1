
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Table("GameTransactionError")]
    public class GameTransactionError : CommonEntity
    {
        [Key]
        [ForeignKey("GameTransaction")]
        [Column(Order = 1)]
        public long GameTransactionId { get; set; }

        [Column(Order = 2), MaxLength(8000)]
        public string Message { get; set; }

        public virtual GameTransaction GameTransaction { get; set; }
    }
}