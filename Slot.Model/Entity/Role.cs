using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Table("Role")]
    public class Role : BaseEntity<int>
    {
        public bool Active { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }
        
        public int OperatorId { get; set; }
    }
}