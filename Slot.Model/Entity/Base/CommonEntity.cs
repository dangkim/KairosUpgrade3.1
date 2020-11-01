using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Serializable]
    public abstract class CommonEntity
    {
        [MaxLength(128)]
        public string CreatedBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [DefaultValue("GETUTCDATE()")]
        public DateTime CreatedOnUtc { get; set; }

        [MaxLength(128)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }
    }
}