using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("Game")]
    public class Game : BaseEntity<int>
    {
        [Column(Order = 1)]
        public GameType GameType { get; set; }

        [Column(Order = 5)]
        public bool IsDisabled { get; set; }

        [Column(Order = 3)]
        public int Lines { get; set; }

        [Column(Order = 2), MaxLength(128)]
        public string Name { get; set; }

        [Column(Order = 4)]
        public int RtpLevel { get; set; }

        [NotNull]
        [Column(Order = 6)]
        public string DisableOperators { get; set; }

        public bool IsBetAllLines { get; set; }

        [MaxLength(255)]
        public string Params { get; set; }

        public bool IsSideBet { get; set; }

        //public DateTime ReleaseDate { get; set; }

        public bool IsFreeRoundEnabled { get; set; }

    }
}