using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Table("Account")]
    public class Account : BaseEntity<int>
    {
        [Column(Order = 1)]
        public int OperatorId { get; set; }

        [ForeignKey("OperatorId")]
        public virtual Operator Operator { get; set; }

        [Column(Order = 2), MaxLength(128)]
        public string Username { get; set; }

        [Column(Order = 3), MaxLength(256)]
        public string Password { get; set; }

        [Column(Order = 4), MaxLength(255)]
        public string RealName { get; set; }

        [Column(Order = 5)]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [Column(Order = 6)]
        public bool Active { get; set; }

        [Column(Order = 7)]
        public DateTime? LastLoginUtc { get; set; }

        [Column("DefaultOffSet", Order = 8)]
        public int UtcTimeOffsetId { get; set; }

        [ForeignKey("UtcTimeOffsetId")]
        public virtual UtcTimeOffset UtcTimeOffset { get; set; }
    }
}