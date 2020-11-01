using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Table("FreeRound")]
    public class FreeRound : BaseEntity<int>
    {
        public string Name { get; set; }

        public int OperatorId { get; set; }

        public int GameId { get; set; }

        public int Lines { get; set; }

        public int Multiplier { get; set; }

        public int LimitPerPlayer { get; set; }

        public int RelativeDuration { get; set; }

        public int Platform { get; set; }

        public DateTime StartDateUtc { get; set; }

        public DateTime EndDateUtc { get; set; }

        public string Template { get; set; }

        public string MessageTitle { get; set; }

        public string MessageContent { get; set; }

        public int OwnerId { get; set; }

        public bool IsCancelled { get; set; }
    }
}
