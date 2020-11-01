using Microsoft.EntityFrameworkCore;
using Slot.Core.Data;
using Slot.Core.Data.Attributes.SqlBuilder;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.BackOffice.Data.Queries.Members
{
    public class MemberHistoryQuery : MemberPagedQuery, IOperatorQuery, IMemberQuery
    {
        public MemberHistoryQuery() { }

        public MemberHistoryQuery(MemberHistoryQuery query) : this()
        {
            OperatorId = query.OperatorId;
            OperatorIds = query.OperatorIds;
            OperatorTag = query.OperatorTag;
            GameId = query.GameId;
            MemberId = query.MemberId;
            MemberName = query.MemberName;
            RoundId = query.RoundId;
            TransactionId = query.TransactionId;
            GameTransactionType = query.GameTransactionType;
            StartDate = query.StartDate;
            EndDate = query.EndDate;
            IsDemo = query.IsDemo;
            PlatformType = query.PlatformType;
            PageIndex = query.PageIndex;
            PageSize = query.PageSize;
        }

        public MemberHistoryQuery(MemberHistoryQuery query, bool isNextPageQuery) : this(query)
        {
            if (isNextPageQuery)
            {
                SetForNextPageCount();
            }
        }

        [Optional(0)]
        public int? OperatorId { get; set; }

        [Excluded]
        public int?[] OperatorIds { get; set; }

        [Excluded]
        public string OperatorTag { get; set; }

        [Optional(0)]
        public int? GameId { get; set; }

        [Optional(0, "UserId")]
        public int? MemberId { get; set; }

        [SqlBuilder("Username")]
        public string MemberName { get; set; }

        [Optional(0)]
        public long? RoundId { get; set; }

        [Optional(0, "TrxId")]
        public long? TransactionId { get; set; }

        [Optional(0, "GameTrxType")]
        public int GameTransactionType { get; set; }

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

        public bool? IsDemo { get; set; }

        public string PlatformType { get; set; }

        [SqlBuilder("OffsetRows")]
        public override int Offset { get => base.Offset; set => base.Offset = value; }

        [SqlBuilder("PageSize")]
        public override int PageSize { get => base.PageSize; set => base.PageSize = value; }

        public async Task GetUserId(IReadOnlyDatabase db)
        {
            var op = OperatorId.Value;
            MemberId = MemberId ?? await db.Users
                                            .AsNoTracking()
                                            .Where(user => user.OperatorId == op && user.Name == MemberName)
                                            .Select(user => user.Id)
                                            .FirstOrDefaultAsync();
        }
    }
}
