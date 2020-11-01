using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    public abstract class BaseEntity<T> : BaseEntity
    {
        [Column(Order = 0)]
        public T Id { get; set; }
    }

    [Serializable]
    public abstract class BaseEntity
    {
        public bool IsDeleted { get; set; }

        [MaxLength(128)]
        public string CreatedBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedOnUtc { get; set; }

        [MaxLength(128)]
        public string UpdatedBy { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }

        [MaxLength(128)]
        public string DeletedBy { get; set; }

        public DateTime? DeletedOnUtc { get; set; }
    }
}
