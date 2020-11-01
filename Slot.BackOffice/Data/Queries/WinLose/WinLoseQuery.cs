using System;
using Slot.Core.Data.Attributes.SqlBuilder;
using static Slot.BackOffice.Data.Enums;

namespace Slot.BackOffice.Data.Queries.WinLose
{
    public class WinLoseQuery : BaseQuery, IOperatorQuery
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
        public int? UserId { get; set; }

        [Optional(0)]
        public int? CurrencyId { get; set; }

        public Enums.ReportFormat ReportFormat { get; set; }

        public Model.PlatformType? PlatformType { get; set; }

        [Optional(0)]
        public int? Platform
        {
            get
            {
                if(PlatformType.HasValue)
                {
                    return (int)PlatformType;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool? IsFreeRounds { get; set; }

        [Excluded]
        public DateTime StartDate { get; set; }

        public DateTime StartDateInUTC
        {
            get
            {
                return StartDate.ToUniversalTime();
            }
        }

        [Excluded]
        public DateTime EndDate { get; set; }

        public DateTime EndDateInUTC
        {
            get
            {
                return EndDate.ToUniversalTime();
            }
        }

        public FilterDateType FormatFilterType { get; set; }

        public bool? IsDemo { get; set; }
    }
}
