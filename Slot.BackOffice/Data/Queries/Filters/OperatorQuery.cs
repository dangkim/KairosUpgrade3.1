using Slot.Core.Data.Attributes.SqlBuilder;

namespace Slot.BackOffice.Data.Queries.Filters
{
    public class OperatorQuery : IOperatorQuery
    {
        public int? OperatorId { get; set; }

        [Excluded]
        public int?[] OperatorIds { get; set; }

        public string OperatorTag { get; set; }
    }
}
