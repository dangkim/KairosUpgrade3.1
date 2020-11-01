using Newtonsoft.Json;
using Slot.Model.Formatters;

namespace Slot.Core.Data.Views.WinLose
{
    public class WinLoseByPeriodBase : WinLoseBase
    {        
        public int NoOfPlayer { get; set; }

        public string Game { get; set; }
    }
}
