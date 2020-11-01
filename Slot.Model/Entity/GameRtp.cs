using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Table("GameRtp")]
    public class GameRtp : BaseEntity<int>
    {
        [Column(Order = 1)]
        public int GameId { get; set; }

        [Column(Order = 2)]
        public int RtpLevel { get; set; }

        [Column(Order = 3)]
        public decimal Rtp { get; set; }
    }
}
