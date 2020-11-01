
namespace Slot.Model.Entity
{
    public class FRCoinSetting
    {
        public int Id { get; set; }

        public int FreeRoundId { get; set; }

        public int CurrencyId { get; set; }

        public decimal LineBet { get; set; }

        public decimal TotalBet { get; set; }
    }

    public class FRPlayer
    {
        public int Id { get; set; }
        public int FreeRoundId { get; set; }

        public int UserId { get; set; }
    }
}
