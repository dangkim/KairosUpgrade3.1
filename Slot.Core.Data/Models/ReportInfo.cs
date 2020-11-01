using Slot.Model.Entity;

namespace Slot.Core.Data.Models
{
    public class ReportInfo : BaseEntity<long>
    {
        public int OffsetId { get; set; }

        public long StartId { get; set; }

        public long LastId { get; set; }

        public long CountItem { get; set; }
    }
}
