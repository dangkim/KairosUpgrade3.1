using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Model.Entity
{
    [Table("TRelation")]
    public class TRelation
    {
        public int TournamentId { get; set; }

        public TournamentRelationType RelationType { get; set; }

        public int RelationId { get; set; }

        public decimal RelationValue { get; set; }

        public int Count { get; set; }

        public int Rank { get; set; }
    }
}
