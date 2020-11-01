
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Table("FreeRoundGameHistory")]
    public class FreeRoundGameHistory
    {
        [Key]
        public long Id { get; set; }
        public long GameHistoryId { get; set; }
        public int FreeRoundId { get; set; }
    }
}
