using Slot.Core.Data.Attributes.SqlBuilder;
using System;
using static Slot.BackOffice.Data.Enums;

namespace Slot.BackOffice.Data.Queries.GamePerformance
{
    public class GamePerformanceQuery : BaseQuery, IOperatorQuery
    {
        [Optional(0)]
        public int? OperatorId { get; set; }

        [Excluded]
        public int?[] OperatorIds { get; set; }

        [Excluded]
        public string OperatorTag { get; set; }

        [Optional(0)]
        public int? GameId { get; set; }

        [Optional(0)]
        public int? CurrencyId { get; set; }

        [SqlBuilder("IsDemo")]
        public bool? IsDemoAccount { get; set; }

        public FilterDateType FilterDateType { get; set; }

        [Excluded]
        public string CustomSearchType { get; set; }

        [Excluded]
        public DateTime StartDate { get; set; }

        public DateTime StartDateInUTC
        {
            get => StartDate.ToUniversalTime();
        }

        [Excluded]
        public DateTime EndDate { get; set; }

        public DateTime EndDateInUTC
        {
            get => EndDate.ToUniversalTime();
        }
    }
}
