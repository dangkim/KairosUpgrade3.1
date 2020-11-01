using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("UserGameRtpSetting")]
    public class UserGameRtpSetting : CommonEntity
    {
        [Key]
        [ForeignKey("User")]
        [Column(Order = 1)]
        public int UserId { get; set; }

        [Column(Order = 2)]
        public int Level { get; set; }

        public virtual User User { get; set; }
    }
}