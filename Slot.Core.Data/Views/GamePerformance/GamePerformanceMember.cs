using Newtonsoft.Json;
using Slot.Model.Formatters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Data.Views.GamePerformance
{
    public sealed class GamePerformanceMember : GamePerformanceBase
    {
        [JsonProperty("rowNumber")]
        public long No { get; set; }

        public string MemberName { get; set; }

        public int CurrencyId { get; set; }

        public string Currency { get; set; }

        public int OperatorId { get; set; }

        public string Operator { get; set; }

        public int GameId { get; set; }

        public string Game { get; set; }
        
        public decimal TotalBet { get; set; }
        
        public decimal TotalWin { get; set; }
        
        public decimal GameIncome { get; set; }
    }
}
