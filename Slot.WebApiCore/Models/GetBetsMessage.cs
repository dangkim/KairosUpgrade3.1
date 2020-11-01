using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.WebApiCore.Models
{
    public class GetBetsMessage : IMessage
    {
        public string Key { get; set; }
        public string Game { get; set; }
    }
}
