using Slot.Core.Data.Attributes.SqlBuilder;
using System;
using System.ComponentModel.DataAnnotations;

namespace Slot.BackOffice.Data.Queries.TopWinners
{
    public class TopWinnerQuery : BaseQuery, IOperatorQuery
    {
        private int top;

        [Optional(0)]
        public int? OperatorId { get; set; }

        [Excluded]
        public int?[] OperatorIds { get; set; }

        [Excluded]
        public string OperatorTag { get; set; }

        [Optional(0)]
        public int GameId { get; set; }

        [Optional(0)]
        public int CurrencyId { get; set; }

        [SqlBuilder("Username")]
        public string MemberName { get; set; }

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

        public int Top { get => Limit ?? top; set => top = value; }
    }
}
