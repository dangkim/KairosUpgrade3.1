using Newtonsoft.Json;
using Slot.Model.Formatters;

namespace Slot.Core.Data.Views.WinLose
{
    public class WinLoseByGame : WinLoseBase
    {
        public int GameId { get; set; }

        public string Game { get; set; }
        
        public int NoOfPlayer { get; set; }
    }
}
