using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Data.Views.GamePerformance
{
    public sealed class GamePerformanceCurrency : GamePerformance
    {
        public int CurrencyId { get; set; }

        public string Currency { get; set; }
    }
}
