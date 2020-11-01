using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("ExchangeRate")]
    public class ExchangeRate : BaseEntity<int>
    {
        [Column(Order=1)]
        public DateTime EffectiveTimeUtc { get; set; }

        [Column(Order=2)]
        public int CurrencyId { get; set; }

        [Column(Order = 3)]
        public string CurrencyCode { get; set; }

        [Column(Order=4)]
        public int TargetCurrencyId { get; set; }

        [Column(Order=5)]
        public decimal Rate { get; set; }
    }
}
