using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Table("Tournament")]
    public class Tournament : BaseEntity<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int OperatorId { get; set; }

        public int OwnerId { get; set; }

        public int Flags { get; set; }

        public int MinHands { get; set; }

        public bool IsAllMembers
        {
            get { return (this.Flags & (int)TournamentFlag.AllMembers) != 0; }
        }

        public string ErrorTitle { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsCancelled { get; set; }

        public DateTime? CancelledOnUtc { get; set; }

        public int PrizeCopyFrom { get; set; }
    }
}
