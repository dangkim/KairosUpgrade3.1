using Newtonsoft.Json;
using Slot.Model.Formatters;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slot.Core.Data.Views.WinLose
{
    public class WinLoseByMember : WinLoseBase
    {
        public int MemberId { get; set; }

        public string MemberName { get; set; }

        [Column("Operator")]
        public string OperatorTag { get; set; }

        public string Currency { get; set; }
    }
}
