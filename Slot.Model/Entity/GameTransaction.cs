using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("GameTransaction")]
    public class GameTransaction : BaseEntity<long>
    {
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("GETUTCDATE()")]
        public DateTime DateTimeUtc { get; set; }

        [Column(Order = 3)]
        public int GameId { get; set; }

        [Column(Order = 4)]
        public GameTransactionType Type { get; set; }

        [Column(Order = 2)]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual Game Game { get; set; }

        public virtual GameTransactionError Error { get; set; }

        public virtual SpinBetProfile SpinBetProfile { get; set; }

    }
}