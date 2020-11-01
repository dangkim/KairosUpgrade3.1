using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Serializable]
    [Table("GameSettingGroup")]
    public class GameSettingGroup : BaseEntity<int>
    {
        [Column(Order = 1)]
        [MaxLength(128)]
        public string Name { get; set; }
    }
}