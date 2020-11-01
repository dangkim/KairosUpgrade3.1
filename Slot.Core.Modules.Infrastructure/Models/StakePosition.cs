using System;
using System.Collections.Generic;

namespace Slot.Core.Modules.Infrastructure.Models
{
    [Serializable]
    public class StakePosition
    {
        public int Line { get; set; }

        public List<int> RowPositions { get; set; }
    }
}
