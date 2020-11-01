using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("UserSessionLog")]
    public class UserSessionLog : BaseEntity<int>
    {
        [Column(Order = 3)]
        [MaxLength(128)]
        public string IpAddress { get; set; }

        [Column(Order = 4)]
        public PlatformType PlatformType { get; set; }

        [Column(Order = 2)]
        [MaxLength(512)]
        public string SessionKey { get; set; }

        public virtual User User { get; set; }

        [Column(Order = 1)]
        public int UserId { get; set; }
    }
}