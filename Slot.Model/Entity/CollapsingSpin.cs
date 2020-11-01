using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Table("CollapsingSpin")]
    public class CollapsingSpin
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        
        public int UserId { get; set; }

        public int GameId { get; set; }

        //public CollapsingSpinType Type { get; set; }

        public byte[] Data { get; set; }
    }
}
