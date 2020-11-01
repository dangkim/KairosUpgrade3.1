using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.BackOffice.Data.History
{
    public class GameSymbols
    {
        public int GameId { get; set; }

        public string GameName { get; set; }

        public List<Symbol> Symbols { get; set; }

        public class Symbol
        {
            public Symbol() { }

            public Symbol(int name, string data)
            {
                Name = name;
                Data = data;
            }

            public int Name { get; set; }

            public string Data { get; set; }
        }
    }
}
