namespace Slot.Core.Modules.Infrastructure.Models
{
    using System.Collections.Generic;
    using System;
    using Slot.Core.RandomNumberGenerators;

    public class ReelStripsWeighted : ReelStripsWeighted<BaseStrips>
    {
        protected ReelStripsWeighted(Tuple<Weighted, IDictionary<int, IReadOnlyList<BaseStrips>>> tuple) : base(tuple) {}

        public static implicit operator ReelStripsWeighted(Tuple<Weighted, IDictionary<int, IReadOnlyList<BaseStrips>>> tuple)
        {
            return new ReelStripsWeighted(tuple);
        }
    }
}