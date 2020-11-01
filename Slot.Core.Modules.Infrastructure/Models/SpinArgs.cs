namespace Slot.Core.Modules.Infrastructure.Models
{
    public class SpinArgs
    {
        public int BettingLines { get; set; }
        public int Multiplier { get; set; }
        public decimal LineBet { get; set; }
        public decimal TotalBet { get; set; }
        public bool IsAutoSpin { get; set; }
        public int FunPlayDemoKey { get; set; }
        public bool SideBet { get; set; }
    }
}
