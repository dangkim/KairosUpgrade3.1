using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Model.Entity.Pagination
{
    public interface IPaginatedResult
    {
        int PageIndex { get; }

        int PageSize { get; }

        int Offset { get; }

        int TotalPages { get; }

        int TotalResults { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }
    }
}
