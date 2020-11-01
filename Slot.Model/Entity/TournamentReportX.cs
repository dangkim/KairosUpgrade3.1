using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Slot.Model.Entity
{
    [Table("TournamentReportX")]
    public class TournamentReportX : CommonEntity
    {
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column(Order = 2)]
        public int OperatorId { get; set; }

        public string SummaryJson { get; set; }

        public string DetailJson { get; set; }

        public string LeaderboardJson { get; set; }
    }
}