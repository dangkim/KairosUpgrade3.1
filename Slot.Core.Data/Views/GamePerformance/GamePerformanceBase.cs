using Newtonsoft.Json;
using Slot.Model.Formatters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Data.Views.GamePerformance
{
    public abstract class GamePerformanceBase
    {        
        public long NoOfTransaction { get; set; }
        
        public decimal TotalBetRmb { get; set; }
        
        public decimal TotalWinRmb { get; set; }
        
        public decimal GameIncomeRmb { get; set; }

        public decimal GamePayoutPer { get; set; }
    }
}
