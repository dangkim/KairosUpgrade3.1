using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("GameState")]
    public class GameState
    {
        [Column(Order = 1)]
        public int UserId { get; set; }

        [Column(Order = 2)]
        public int GameId { get; set; } 

        [Column(Order = 3)]
        public GameStateType Type { get; set; }

        [Column(Order = 4)]
        public long? LastRoundId { get; set; }

        //[Column(Order = 4, TypeName = "bit")]
        //[DefaultValue("0")]
        //public bool IsFreeGame { get; set; }

        public virtual Game Game { get; set; }
    }
}