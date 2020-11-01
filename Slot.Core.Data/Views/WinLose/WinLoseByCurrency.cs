using Newtonsoft.Json;
using Slot.Model.Formatters;

namespace Slot.Core.Data.Views.WinLose
{
    public class WinLoseByCurrency : WinLoseBase
    {
        public int CurrencyId { get; set; }

        public string Currency { get; set; }
    }
}
