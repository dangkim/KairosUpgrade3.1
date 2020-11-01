namespace Slot.Core.Modules.Infrastructure.Models
{
    using Slot.Core.RandomNumberGenerators;
    using System;
    using System.Collections.Generic;

    public class ReelStripsWeighted<T> 
    {
        protected readonly IDictionary<int, IReadOnlyList<T>> values;
        protected readonly Weighted weighted;

        public T this[int level]
        {
            get
            {
                var rnd = RandomNumberEngine.NextDouble();
                var ratio = weighted[level];
                var index = ratio[rnd];
                return values[level][index];
            }
        }

        public static implicit operator ReelStripsWeighted<T>(Tuple<Weighted, IDictionary<int, IReadOnlyList<T>>> tuple)
        {
            return new ReelStripsWeighted<T>(tuple);
        }

        protected ReelStripsWeighted(Tuple<Weighted, IDictionary<int, IReadOnlyList<T>>> tuple)
        {
            weighted = tuple.Item1;
            values = tuple.Item2;
        }
    }
}