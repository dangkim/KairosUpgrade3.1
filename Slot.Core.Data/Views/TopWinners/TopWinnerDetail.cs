using Newtonsoft.Json;
using Slot.Model.Formatters;

namespace Slot.Core.Data.Views.TopWinners
{
    public abstract class TopWinnerDetail
    {
        private decimal totalNetWin;

        public int UserId { get; set; }

        public string Operator { get; set; }

        public string Currency { get; set; }

        public int NoOfTransaction { get; set; }

        public int NoOfSpin { get; set; }

        public decimal AvgBet { get; set; }

        public decimal TotalBet { get; set; }

        /// <remarks>
        /// As per PDT - BO project, changed this to company perspective "Game Income"
        /// </remarks>
        public decimal TotalNetWin { get => totalNetWin; set => totalNetWin = value * -1; }

        public decimal CompanyWLPercentage { get; set; }
    }
}
