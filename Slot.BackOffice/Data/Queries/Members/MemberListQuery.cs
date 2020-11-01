using Slot.Core.Data.Attributes.SqlBuilder;

namespace Slot.BackOffice.Data.Queries.Members
{
    public class MemberListQuery : MemberPagedQuery, IOperatorQuery, IMemberQuery
    {
        public MemberListQuery() { }

        public MemberListQuery(MemberListQuery query) : this()
        {
            OperatorId = query.OperatorId;
            OperatorTag = query.OperatorTag;
            MemberId = query.MemberId;
            MemberName = query.MemberName;
            CurrencyId = query.CurrencyId;
            IsDemoAccount = query.IsDemoAccount;
            PageIndex = query.PageIndex;
            PageSize = query.PageSize;
        }

        public MemberListQuery(MemberListQuery query, bool isNextPageQuery) : this(query)
        {
            if(isNextPageQuery)
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
        public int? MemberId { get; set; }

        public string MemberName { get; set; }

        [Optional(0)]
        public int CurrencyId { get; set; }

        public bool? IsDemoAccount { get; set; }

        [SqlBuilder("PageNumber")]
        public override int PageIndex { get => base.PageIndex; set => base.PageIndex = value; }

        [SqlBuilder("PageSize")]
        public override int PageSize { get => base.PageSize; set => base.PageSize = value; }
    }
}
