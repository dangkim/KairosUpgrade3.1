using Newtonsoft.Json;
using Slot.Model.Formatters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Data.Views.GamePerformance
{
    public class GamePerformance : GamePerformanceBase
    {
        public int NoOfPlayer { get; set; }
                
        public long NoOfSpin { get; set; }
        
        public decimal AvgBetRmb { get; set; }
    }
}
