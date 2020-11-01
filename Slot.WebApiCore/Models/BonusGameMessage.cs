namespace Slot.WebApiCore.Models
{
    public class BonusGameMessage : IMessage
    {
        public string Game { get; set; }
        public string Key { get; set; }
        public string Bonus { get; set; }
        public int Param { get; set; }
    }
}
