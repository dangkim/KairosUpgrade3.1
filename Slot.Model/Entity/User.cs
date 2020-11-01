using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("User")]
    public class User : BaseEntity<int>
    {
        public virtual Currency Currency { get; set; }

        [Column(Order = 4)]
        public int CurrencyId { get; set; }

        [Column(Order = 1)]
        [MaxLength(255)]
        public string ExternalId { get; set; }

        [Column(Order = 8)]
        public bool IsBlocked { get; set; }

        [Column(Order = 7)]
        public bool IsDemo { get; set; }

        [Column(Order = 2)]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column(Order = 3)]
        [MaxLength(32)]
        public string Password { get; set; }

        [Column(Order = 5)]
        public int OperatorId { get; set; }

        public bool IsBonus { get; set; }

        [Column(Order = 6)]
        public UserRole UserRole { get; set; }

        public virtual Operator Operator { get; set; }

        public virtual ICollection<UserSessionLog> SessionLog { get; set; }

        public virtual ICollection<UserGameData> UserGameData { get; set; }

        public virtual UserGameRtpSetting UserGameRtpSetting { get; set; }
    }
}