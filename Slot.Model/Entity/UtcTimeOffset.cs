using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Table("UtcTimeOffset")]
    public class UtcTimeOffset : BaseEntity<int>
    {
        [Column(Order = 1)]
        public string Offset { get; set; }

        [Column(Order = 2)]
        public bool IsDisabled { get; set; }
    }
}