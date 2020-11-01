using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Model.Slot
{
    [Serializable]
    public class ReelCollapse
    {
        public int Reel { get; set; }

        public List<int> Symbols { get; set; }

        public ReelCollapse()
        {
            Symbols = new List<int>();
        }
    }
}
