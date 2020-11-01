using System;
using System.Collections.Generic;
using System.Linq;

namespace Slot.Games.BullRush.Models
{
    [Serializable]
    public class BullRushWinPosition
    {
        public int Line { get; set; }

        public int Multiplier { get; set; }

        public int RandomMultiplier { get; set; }

        public List<int> RowPositions { get; set; }

        public int Symbol { get; set; }

        public int Count { get; set; }

        public decimal Win { get; set; }

        public bool IsAnyComb { get; set; }
    }
}
