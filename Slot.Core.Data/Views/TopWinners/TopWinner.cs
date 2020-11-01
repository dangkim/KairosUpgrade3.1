using Newtonsoft.Json;
using Slot.Model.Formatters;
using System;


namespace Slot.Core.Data.Views.TopWinners
{
    public class TopWinner
    {
        private decimal totalNetWin;
        private decimal allTimeTotalNetWin;

        public string Currency { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Operator { get; set; }
        
        public int NoOfTransaction { get; set; }
        
        public decimal TotalBet { get; set; }

        /// <remarks>
        /// As per PDT - BO project, changed this to company perspective "Game Income"
        /// </remarks>
        
        public decimal TotalNetWin { get => totalNetWin; set => totalNetWin = value * -1; }
                
        public int NoOfSpin { get; set; }
        
        public decimal AvgBet { get; set; }

        public decimal CompanyWLPercentage { get; set; }
        
        public int AllTimeNoOfTransaction { get; set; }
        
        public decimal AllTimeTotalBet { get; set; }

        /// <remarks>
        /// As per PDT - BO project, changed this to company perspective "Game Income"
        /// </remarks>
        
        public decimal AllTimeTotalNetWin { get => allTimeTotalNetWin; set => allTimeTotalNetWin = value * -1; }
                
        public int AllTimeNoOfSpin { get; set; }
        
        public decimal AllTimeAvgBet { get; set; }

        public decimal AllTimeCompanyWLPercentage { get; set; }

        public DateTime JoinDate { get; set; }
    }
}
