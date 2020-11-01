
namespace Slot.Core.Modules.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ratio = System.Double;

    public class SortedWeighted
    {
        private readonly SortedList<Ratio, int> collection;

        public static implicit operator SortedWeighted(SortedList<Ratio, int> dictionary) => new SortedWeighted(dictionary);

        public static implicit operator SortedWeighted(Ratio[] ratios) => new SortedWeighted(ratios);

        public int this[Ratio ratio]
        {
            get => collection.First(pro => ratio <= pro.Key).Value;          
        }

        public SortedWeighted(params double[] ratios)
        {
            var list = ratios.ToList();
            list.Sort();
            var dict = list.ToDictionary(item => item, item => list.IndexOf(item));
            collection = new SortedList<Ratio, int>(dict);
        }

        protected SortedWeighted(IDictionary<Ratio, int> dictionary) => collection = new SortedList<Ratio, int>(dictionary);
    }
}