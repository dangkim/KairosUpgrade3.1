using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Data.Views.GamePerformance
{
    public sealed class GamePerformanceMonthly : GamePerformance
    {
        public int Month { get; set; }

        public int Year { get; set; }
    }
}
