using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Data.Views.GamePerformance
{
    public sealed class GamePerformanceGeneral : GamePerformance
    {
        public int GameId { get; set; }

        public string Game { get; set; }
    }
}
