using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Serializable]
    [Table("UserGameSpinData")]
    public class UserGameSpinData
    {
        [Key, Column(Order = 0)]
        public long Id { get; set; }

        public int UserId { get; set; }

        public int GameId { get; set; }

        public int Type { get; set; }

        public byte[] Data { get; set; }

        public UserGameSpinData()
        {
            
        }

        public UserGameSpinData(UserGameKey ugk, SpinDataType type)
        {
            this.UserId = ugk.UserId;
            this.GameId = ugk.GameId;
            this.Type = (int)type;
        }
    }
}