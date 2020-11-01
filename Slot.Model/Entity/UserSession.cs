using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("UserSession")]
    public class UserSessionEntity : CommonEntity
    {
       [Column(Order = 2)]
       [MaxLength(512)]
       public string SessionKey { get; set; }

       public virtual User User { get; set; }

       [Key]
       [ForeignKey("User")]
       [Column(Order = 1)]
       public int UserId { get; set; }

       [Column(Order = 3)]
       [MaxLength(1024)]
       public string ExtraInfo { get; set; }
    }
}