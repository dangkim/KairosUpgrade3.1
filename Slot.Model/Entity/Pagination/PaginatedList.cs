using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slot.Model.Entity.Pagination
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PaginatedList<T> : List<T>, IPaginatedResult
    {
        [JsonProperty]
        public int PageIndex { get; }

        [JsonProperty]
        public int TotalPages { get; }

        [JsonProperty]
        public int TotalResults { get; }

        [JsonProperty]
        public int PageSize { get; }

        [JsonProperty]
        public IEnumerable<T> Items { get => this.ToList(); }

        [JsonProperty]
        public bool HasPreviousPage
        {
            get
            {
                return PageIndex > 1;
            }
        }

        [JsonProperty]
        public bool HasNextPage
        {
            get
            {
                if (TotalPages == 0)
                {
                    return HasNextRecord;
                }
                else
                {
                    return PageIndex < TotalPages;
                }
            }
        }

        public bool HasNextRecord { get => TotalResults == PageSize && TotalResults != 0; }

        public int Offset { get => (PageIndex - 1) * PageSize; }

        public PaginatedList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalResults = count;
            AddRange(items);
        }

        public static PaginatedList<T> Empty(int pageIndex, int pageSize)
        {
            pageIndex = Math.Max(1, pageIndex);
            return new PaginatedList<T>(new List<T>(), 0, pageIndex, pageSize);
        }

        public static PaginatedList<T> Create(
            IList<T> source, int pageIndex, int pageSize)
        {
            IEnumerable<T> items = source;
            pageIndex = Math.Max(1, pageIndex);

            return new PaginatedList<T>(items, source.Count, pageIndex, pageSize);
        }
    }
}
