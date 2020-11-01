using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Table("WalletTransaction")]
    public class WalletTransaction : BaseEntity<long>
    {
        [Column(Order = 3)]
        public decimal Amount { get; set; }

        [Column(Order = 10)]
        public double ElapsedSeconds { get; set; }

        [Column(Order = 9)]
        [MaxLength(8000)]
        public string ErrorMessage { get; set; }

        [Column(Order = 4)]
        public long GameTransactionId { get; set; }

        public virtual GameTransaction GameTransaction { get; set; }

        [Column(Order = 1)]
        [MaxLength(128)]
        public string Guid { get; set; }

        [Column(Order = 8)]
        public bool IsError { get; set; }

        [Column(Order = 2)]
        public WalletTransactionType Type { get; set; }

        public virtual WalletProvider WalletProvider { get; set; }

        [Column(Order = 5)]
        public int WalletProviderId { get; set; }

        [Column(Order = 7)]
        public string WalletProviderResponse { get; set; }

        [Column(Order = 6)]
        public string WalletProviderTransactionId { get; set; }
    }
}