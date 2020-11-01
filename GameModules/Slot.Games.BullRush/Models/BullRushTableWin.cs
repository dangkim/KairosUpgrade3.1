using System;

namespace Slot.Games.BullRush.Models
{
    /// <summary>Represents the xml element <c><tablewin></tablewin></c>.</summary>
    [Serializable]
    public class BullRushTableWin
    {
        public int Card { get; set; }

        public int Count { get; set; }

        public int Wild { get; set; }

        public int WildMultiplier { get; set; }

        public decimal Win { get; set; }
    }
}