using Slot.Core.Data.Attributes.SqlBuilder;
using Slot.Model.Entity.Pagination;

namespace Slot.BackOffice.Data.Queries
{
    public abstract class PagedQuery : BaseQuery, IPaginatedResult
    {
        private int offset;

        [Excluded]
        public virtual int PageIndex { get; set; } = 1;

        [Excluded]
        public virtual int PageSize { get; set; } = 20;

        [Excluded]
        public virtual int Offset
        {
            get => offset == 0 ? GetPageOffset(PageSize, PageIndex) : offset;
            set => offset = value;
        }

        [Excluded]
        public virtual int TotalPages { get; set; }

        [Excluded]
        public virtual int TotalResults { get; set; }

        [Excluded]
        public virtual bool HasPreviousPage { get; set; }

        [Excluded]
        public virtual bool HasNextPage { get; set; }

        protected virtual void SetForNextPageCount()
        {
            PageIndex++;
            Offset = GetPageOffset(PageSize, PageIndex);
            PageSize = 1;
        }

        protected virtual int GetPageOffset(int pageSize, int pageIndex) => (pageIndex - 1) * pageSize;
    }
}
