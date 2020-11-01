using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;


namespace Slot.Model
{
    public class BonusFreeSpinWeight
    {
        public BonusFreeSpinWeight()
        {
            this.Weights = new SortedDictionary<double, SortedDictionary<double, List<BonusFreeSpinItem>>>();
        }

        public SortedDictionary<double, SortedDictionary<double, List<BonusFreeSpinItem>>> Weights { get; set; }

        public void Add(double outerWeight, List<BonusFreeSpinInnerItem> bonusFreeSpinItems)
        {
            if (bonusFreeSpinItems == null)
            {
                throw new ArgumentNullException("bonusFreeSpinItems");
            }

            if (Math.Abs(bonusFreeSpinItems.Sum(x => x.Weight) - 1) > Constant.Epsilon)
            {
                throw new ArgumentException(@"Sum of inner weights doesn't add up to 1.", "bonusFreeSpinItems");
            }

            double innerWeight = 0;

            var innerWeights = new SortedDictionary<double, List<BonusFreeSpinItem>>();
            foreach (var bonusFreeSpinItem in bonusFreeSpinItems)
            {
                innerWeight += bonusFreeSpinItem.Weight;
                innerWeights[innerWeight] = bonusFreeSpinItem.Items;
            }

            var weight = this.Weights.Keys.LastOrDefault() + outerWeight;
            this.Weights.Add(weight, innerWeights);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.Equals((BonusFreeSpinWeight)obj);
        }

        public List<BonusFreeSpinItem> Get(double outerWeight, double innerWeight)
        {
            var innerWeights = this.Weights.FirstOrDefault(weight => outerWeight <= weight.Key).Value;

            if (innerWeights == null)
            {
                return null;
            }

            var bonusFreeSpinItems = innerWeights.FirstOrDefault(weight => innerWeight <= weight.Key).Value;

            return bonusFreeSpinItems;
        }

        public override int GetHashCode()
        {
            return this.Weights != null ? this.Weights.GetHashCode() : 0;
        }

        public string ToCustomString()
        {
            return JsonConvert.SerializeObject(new { BonusFreeSpinWeight = this }, Formatting.Indented);
        }

        public override string ToString()
        {
            return this.ToCustomString();
        }

        protected bool Equals(BonusFreeSpinWeight other)
        {
            return this.Weights.SequenceEqual(other.Weights, new WeightsEqualityComparer());
        }

        /// <summary>The weights equality comparer.</summary>
        public class WeightsEqualityComparer :
            IEqualityComparer<KeyValuePair<double, SortedDictionary<double, List<BonusFreeSpinItem>>>>
        {
            public bool Equals(
                KeyValuePair<double, SortedDictionary<double, List<BonusFreeSpinItem>>> x, 
                KeyValuePair<double, SortedDictionary<double, List<BonusFreeSpinItem>>> y)
            {
                return Math.Abs(x.Key - y.Key) < Constant.Epsilon
                       && x.Value.SequenceEqual(y.Value, new BonusFreeSpinItemListEqualityComparer());
            }

            public int GetHashCode(KeyValuePair<double, SortedDictionary<double, List<BonusFreeSpinItem>>> obj)
            {
                return 0;
            }

            /// <summary>The bonus free spin item list equality comparer.</summary>
            public class BonusFreeSpinItemListEqualityComparer :
                IEqualityComparer<KeyValuePair<double, List<BonusFreeSpinItem>>>
            {
                public bool Equals(
                    KeyValuePair<double, List<BonusFreeSpinItem>> x, 
                    KeyValuePair<double, List<BonusFreeSpinItem>> y)
                {
                    return Math.Abs(x.Key - y.Key) < Constant.Epsilon && x.Value.SequenceEqual(y.Value);
                }

                public int GetHashCode(KeyValuePair<double, List<BonusFreeSpinItem>> obj)
                {
                    return 0;
                }
            }
        }
    }
}