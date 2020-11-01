using Newtonsoft.Json;
using Slot.Model.Formatters;

namespace Slot.Core.Data.Views.WinLose
{
    public abstract class WinLoseBase
    {        
        public long NoOfTransaction { get; set; }
                
        public long NoOfSpin { get; set; }
        
        public decimal AvgBet { get; set; }
        
        public decimal TotalBet { get; set; }
        
        public decimal TotalWin { get; set; }
        
        public decimal GameIncome { get; set; }
        
        public decimal AvgBetRmb { get; set; }
        
        public decimal TotalBetRmb { get; set; }
        
        public decimal TotalWinRmb { get; set; }
        
        public decimal GameIncomeRmb { get; set; }

        public decimal GamePayoutPer { get; set; }
    }
}
