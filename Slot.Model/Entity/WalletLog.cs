using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("WalletLog")]
    public class WalletLog : BaseEntity<long>
    {
        [Column(Order=2)]
        public int ServerId { get; set; }

        [Column(Order=3)]
        public int OperatorId { get; set; }

        [Column(Order=4)]
        public int Type { get; set; }

        [Column(Order=5)]
        public int Status { get; set; }

        [Column(Order=6)]
        public int Retry { get; set; }

        [Column(Order=7)]
        public string MemberId { get; set; }

        [Column(Order=8)]
        public string TrxId { get; set; }

        [Column(Order=9)]
        public string Request { get; set; }

        [Column(Order=10)]
        public string Rollback { get; set; }
    }
}
