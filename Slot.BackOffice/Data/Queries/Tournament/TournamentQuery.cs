using Slot.Core.Data.Attributes.SqlBuilder;
using System;

namespace Slot.BackOffice.Data.Queries.Tournament
{
    public class TournamentQuery : PagedQuery, IOperatorQuery
    {
        public TournamentQuery() { }

        public TournamentQuery(TournamentQuery query) : this()
        {
            OperatorId = query.OperatorId;
            OperatorIds = query.OperatorIds;
            OperatorTag = query.OperatorTag;
            StartDate = query.StartDate;
            EndDate = query.EndDate;
            TournamentName = query.TournamentName;
            Platforms = query.Platforms;
            PageIndex = query.PageIndex;
            PageSize = query.PageSize;
        }

        public TournamentQuery(TournamentQuery query, bool isNextPageQuery) : this(query)
        {
            if (isNextPageQuery)
            {
                SetForNextPageCount();
            }
        }

        [Excluded]
        public int? OperatorId { get; set; }

        [Excluded]
        public int?[] OperatorIds { get; set; }

        [Optional(null, parameterName: "Operator")]
        public string OperatorIdsString { get => OperatorIds != null ? string.Join(",", OperatorIds) : null; }

        [Excluded]
        public string OperatorTag { get; set; }

        [Excluded]
        public DateTime StartDate { get; set; }

        [SqlBuilder("StartDateInUtc")]
        public DateTime StartDateInUTC
        {
            get => StartDate.ToUniversalTime();
        }

        [Excluded]
        public DateTime EndDate { get; set; }

        [SqlBuilder("EndDateInUtc")]
        public DateTime EndDateInUTC
        {
            get => EndDate.ToUniversalTime();
        }

        [SqlBuilder("Name")]
        public string TournamentName { get; set; }

        [Excluded]
        public int?[] Platforms { get; set; }

        [Optional(null, parameterName: "Platform")]
        public string PlatformsString { get => Platforms != null ? string.Join(",", Platforms): string.Empty; }

        [SqlBuilder("PageSize")]
        public override int PageSize { get; set; }

        [SqlBuilder("OffsetRows")]
        public override int Offset { get => base.Offset; set => base.Offset = value; }

        [SqlBuilder("Dir")]
        public string SortDirection { get => "desc"; }

        [SqlBuilder("OrderBy")]
        public string OrderBy { get => "4"; }
    }
}
