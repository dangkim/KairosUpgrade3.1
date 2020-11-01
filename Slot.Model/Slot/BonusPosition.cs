using System;
using System.Collections.Generic;

namespace Slot.Model
{
    [Serializable]
    public class BonusPosition
    {
        public int Line { get; set; }

        public int Multiplier { get; set; }

        public List<int> RowPositions { get; set; }

        public decimal Win { get; set; }
    }
}