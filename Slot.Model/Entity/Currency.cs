using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("Currency")]
    public class Currency : BaseEntity<int>
    {
        [Column(Order = 3), MaxLength(128)]
        public string Description { get; set; }

        [Column(Order = 2), MaxLength(8)]
        public string DisplayCode { get; set; }

        [Column(Order = 4)]
        public decimal ExchangeRateToCredit { get; set; }

        [Column(Order = 5)]
        public bool IsVisible { get; set; }

        [Column(Order = 1), MaxLength(8)]
        public string IsoCode { get; set; }
    }
}