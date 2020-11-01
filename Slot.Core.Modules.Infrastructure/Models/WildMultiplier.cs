using System.Collections.Generic;

namespace Slot.Core.Modules.Infrastructure.Models
{
    public class WildMultiplier
    {
        private readonly IDictionary<int, IReadOnlyList<SortedWeighted>> reelMultipliers;

        public IReadOnlyList<SortedWeighted> this [int level] => reelMultipliers[level];

        public static implicit operator WildMultiplier(Dictionary<int, IReadOnlyList<SortedWeighted>> dictionary)
        {
            return new WildMultiplier(dictionary);
        }

        protected WildMultiplier(IDictionary<int, IReadOnlyList<SortedWeighted>> reelMultipliers)
        {
            this.reelMultipliers = reelMultipliers;
        }
    }
}