using System.Collections.Generic;
using System.Linq;


namespace Slot.Model
{
    public class BonusFreeSpinInnerItem
    {
        public BonusFreeSpinInnerItem()
        {
            this.Items = new List<BonusFreeSpinItem>();
        }

        public List<BonusFreeSpinItem> Items { get; set; }

        public double Weight { get; set; }

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

            return obj.GetType() == this.GetType() && this.Equals((BonusFreeSpinInnerItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Items != null ? this.Items.GetHashCode() : 0) * 397) ^ this.Weight.GetHashCode();
            }
        }

        protected bool Equals(BonusFreeSpinInnerItem other)
        {
            return this.Items.SequenceEqual(other.Items) && this.Weight.Equals(other.Weight);
        }
    }
}