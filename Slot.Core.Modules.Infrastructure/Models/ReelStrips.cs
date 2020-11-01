using System.Collections.Generic;

namespace Slot.Core.Modules.Infrastructure.Models
{
    public class ReelStrips
    {
        private readonly IDictionary<int, IReadOnlyList<IReadOnlyList<int>>> reelStrips;

        public IReadOnlyList<IReadOnlyList<int>> this[int level] => reelStrips[level];

        public static implicit operator ReelStrips(Dictionary<int, IReadOnlyList<IReadOnlyList<int>>> dictionary)
        {
            return new ReelStrips(dictionary);
        }

        protected ReelStrips(IDictionary<int, IReadOnlyList<IReadOnlyList<int>>> reelStrips)
        {
            this.reelStrips = reelStrips;
        }
    }
}