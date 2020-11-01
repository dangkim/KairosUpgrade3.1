using System.Collections.Generic;
using Symbol = System.Int32;
using Multiplier = System.Int32;

namespace Slot.Core.Modules.Infrastructure.Models
{
    public class PayTable
    {
        private readonly IDictionary<Symbol, IReadOnlyList<Multiplier>> odds;

        public IReadOnlyList<int> this[Symbol symbol] => odds[symbol];

        public static implicit operator PayTable(Dictionary<int, IReadOnlyList<int>> dictionary)
        {
            return new PayTable(dictionary);
        }

        protected PayTable(IDictionary<Symbol, IReadOnlyList<Multiplier>> odds)
        {
            this.odds = odds;
        }

        public int GetOdds(Symbol symbol, int count)
        {
            if (count < 1)
                return 0;
                
            if (!odds.ContainsKey(symbol)) 
                return 0;

            return odds[symbol][count - 1];
        }
    }
}
