using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Serializable]
    [Table("ConfigurationSetting")]
    public class ConfigurationSetting : BaseEntity<int>
    {
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(1024)]
        public string Value { get; set; }
    }
}