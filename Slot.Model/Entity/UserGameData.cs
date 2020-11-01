using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("UserGameData")]
    public class UserGameData : CommonEntity
    {
        [Column(Order = 1)]
        public int UserId { get; set; }

        [Column(Order = 2)]
        public int GameId { get; set; }

        [Column(Order = 3)]
        [MaxLength(32)]
        public string TimeStamp { get; set; }

        public decimal Bet { get; set; }

        public int MP { get; set; }

        public int Lines { get; set; }

        [DefaultValue("1")]
        public int GameMode { get; set; }
    }
}