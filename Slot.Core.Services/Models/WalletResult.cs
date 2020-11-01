namespace Slot.Core.Services.Models
{
    public class WalletResult
    {
        public int ErrorCode { get; set; }

        public decimal Balance { get; set; }

        public decimal ExchangeRate { get; set; }

        public string Response { get; set; }

        public string TransactionId { get; set; }

        public string BetReference { get; set; }

        public string Guid { get; set; }
    }
}
