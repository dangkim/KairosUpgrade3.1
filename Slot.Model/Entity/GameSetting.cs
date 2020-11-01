using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("GameSetting")]
    public class GameSetting : CommonEntity
    {
        [Column(Order = 1)]
        public int GameSettingGroupId { get; set; }

        [Column(Order = 2)]
        public int GameId { get; set; }

        [Column(Order = 3)]
        public int CurrencyId { get; set; }

        [Required, Column(Order = 4)]
        public string CoinsDenomination { get; set; }

        [Required, Column(Order = 5)]
        public string CoinsMultiplier { get; set; }

        [Column(Order = 6)]
        public decimal GambleMinValue { get; set; }

        [Column(Order = 7)]
        public decimal GambleMaxValue { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual Game Game { get; set; }

        public virtual GameSettingGroup Group { get; set; }
    }
}