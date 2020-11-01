using Newtonsoft.Json;
using Slot.Model.Formatters;

namespace Slot.Core.Data.Views.WinLose
{
    public class WinLoseByPeriodAll : WinLoseByPeriodBase
    {
        public WinLoseByPeriodAll()
        {

        }

        public WinLoseByPeriodAll(WinLoseByPeriodAll winLose) : this()
        {
            Date = winLose.Date;
            Game = winLose.Game;
            GamePayoutPer = winLose.GamePayoutPer;
            NoOfPlayer = winLose.NoOfPlayer;
            NoOfSpin = winLose.NoOfSpin;
            NoOfTransaction = winLose.NoOfTransaction;
            AvgBet = winLose.AvgBet;
            TotalBet = winLose.TotalBet;
            TotalWin = winLose.TotalWin;
            GameIncome = winLose.GameIncome;
            AvgBetRmb = winLose.AvgBetRmb;
            TotalBetRmb = winLose.TotalBetRmb;
            TotalWinRmb = winLose.TotalWinRmb;
            GameIncomeRmb = winLose.GameIncomeRmb;

        }

        public string Date { get; set; }
    }
}
