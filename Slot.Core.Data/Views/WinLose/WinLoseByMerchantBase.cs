using Newtonsoft.Json;
using Slot.Model.Formatters;

namespace Slot.Core.Data.Views.WinLose
{
    public abstract class WinLoseByMerchantBase : WinLoseBase
    {        
        public int NoOfPlayer { get; set; }

        public string Game { get; set; }

        public int OperatorId { get; set; }

        public string OperatorTag { get; set; }
    }
}
