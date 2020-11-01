using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Model.Slot
{
    [Serializable]
    public class CollapsePosition
    {
        public int Line { get; set; }

        public int Card { get; set; }

        public int Count { get; set; }

        public List<int> RowPositions { get; set; }

        public CollapsePosition()
        {
            RowPositions = new List<int>();
        }
    }
}
