using Slot.Core.Data.Attributes.SqlBuilder;

namespace Slot.BackOffice.Data.Queries.Members
{
    public class MemberHistoryResultQuery : BaseQuery, IOperatorQuery
    {
        [Excluded]
        public int? OperatorId { get; set; }

        [Excluded]
        public int?[] OperatorIds { get; set; }

        public string OperatorTag { get; set; }

        public long TransactionId { get; set; }
    }
}
