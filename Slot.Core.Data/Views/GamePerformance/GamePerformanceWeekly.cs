using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Data.Views.GamePerformance
{
    public sealed class GamePerformanceWeekly : GamePerformance
    {
        public int Week { get; set; }

        public int Year { get; set; }
    }
}
