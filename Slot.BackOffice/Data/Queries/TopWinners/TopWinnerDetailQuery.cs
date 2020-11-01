using Slot.Core.Data.Attributes.SqlBuilder;
using System;

namespace Slot.BackOffice.Data.Queries.TopWinners
{
    public class TopWinnerDetailQuery : BaseQuery, IOperatorQuery
    {
        [Optional(0)]
        public int? OperatorId { get; set; }

        [Excluded]
        public int?[] OperatorIds { get; set; }

        [Excluded]
        public string OperatorTag { get; set; }

        [Optional(0)]
        public int GameId { get; set; }

        public int UserId { get; set; }

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

        public Enums.TopWinnerFormat Format { get; set; }
    }
}
