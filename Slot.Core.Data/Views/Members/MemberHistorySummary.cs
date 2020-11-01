using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.Core.Data.Views.Members
{
    public class MemberHistorySummary
    {
        public int? TotalRecords { get; set; }

        public decimal? TotalBet { get; set; }

        public decimal? TotalWin { get; set; }

        public decimal? TotalBetRmb { get; set; }

        public decimal? TotalWinRmb { get; set; }
    }
}
