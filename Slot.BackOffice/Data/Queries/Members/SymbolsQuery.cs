using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Slot.BackOffice.Data.Queries.Members
{
    public class SymbolsQuery
    {
        public string GameName { get; set; }

        public int? GameId { get; set; }

        public bool IsBonus { get; set; }

        public List<int> Symbols { get; set; }
    }
}
