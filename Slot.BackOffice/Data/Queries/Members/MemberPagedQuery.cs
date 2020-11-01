using Slot.Core.Data.Attributes.SqlBuilder;
using Slot.Model.Entity.Pagination;

namespace Slot.BackOffice.Data.Queries.Members
{
    public class MemberPagedQuery : PagedQuery
    {
        [Excluded]
        public override int PageIndex { get; set; } = 1;

        [Excluded]
        public override int PageSize { get; set; } = 20;

        [Excluded]
        public override int Offset { get => base.Offset; set => base.Offset = value; }
    }
}
