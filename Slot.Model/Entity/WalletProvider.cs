using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Table("WalletProvider")]
    [Serializable]
    public class WalletProvider : BaseEntity<int>
    {
        [Column(Order = 1)]
        public string Name { get; set; }
        
        [Column(Order = 2)]
        public string Description { get; set; }

        [Column(Order = 3)]
        public string URL { get; set; }

        [Column(Order = 4)]
        public string MerchantId { get; set; }

        [Column(Order = 5)]
        public string MerchantPwd { get; set; }

        [Column(Order = 6)]
        public int ApiId { get; set; }

        [Column(Order = 7)]
        public bool UseInternalRate { get; set; }

        [Column(Order = 8)]
        public string GameIdFormat { get; set; }

        [Column(Order = 9)]
        public bool UseGameName { get; set; }

        [Column(Order = 10)]
        public bool NotifyTrxId { get; set; }
    }
}