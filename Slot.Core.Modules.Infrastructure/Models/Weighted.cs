namespace Slot.Core.Modules.Infrastructure.Models
{
    using System.Collections.Generic;
    using Level = System.Int32;

    public class Weighted
    {
        private readonly IDictionary<int, SortedWeighted> dictionary;

        public SortedWeighted this[Level level] => dictionary[level];

        public static implicit operator Weighted(Dictionary<int, SortedWeighted> dictionary)
        {
            return new Weighted(dictionary);
        }

        protected Weighted(Dictionary<int, SortedWeighted> dictionary)
        {
            this.dictionary = dictionary;
        }
    }
}