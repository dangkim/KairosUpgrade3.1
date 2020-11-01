namespace Slot.WebApiCore.Models
{
    public class SpinMessage : IMessage
    {
        public string Game { get; set; }
        public string Key { get; set; }
        public decimal Bet { get; set; }
        public bool SideBet { get; set; }
        public int Multiplier { get; set; }
    }
}
