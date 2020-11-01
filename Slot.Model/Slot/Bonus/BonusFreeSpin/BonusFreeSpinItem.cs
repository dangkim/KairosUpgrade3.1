using System;

using Newtonsoft.Json;


namespace Slot.Model
{
    [Serializable]
    public class BonusFreeSpinItem
    {
        public BonusFreeSpinItemType Type { get; set; }

        public int Value { get; set; }

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

            return obj.GetType() == this.GetType() && this.Equals((BonusFreeSpinItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)this.Type * 397) ^ this.Value;
            }
        }

        public string ToCustomString()
        {
            return JsonConvert.SerializeObject(new { BonusFreeSpinItem = this }, Formatting.Indented);
        }

        public override string ToString()
        {
            return this.ToCustomString();
        }

        protected bool Equals(BonusFreeSpinItem other)
        {
            return this.Type == other.Type && this.Value == other.Value;
        }
    }

    [Serializable]
    public class BonusFreeSpinItemX
    {
        public BonusFreeSpinItemType Type { get; set; }

        public int Value { get; set; }

        public int? Prize { get; set; }

        public bool Selected { get; set; }
        
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

            return obj.GetType() == this.GetType() && this.Equals((BonusFreeSpinItemX)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)this.Type * 397) ^ this.Value;
            }
        }

        public string ToCustomString()
        {
            return JsonConvert.SerializeObject(new { BonusFreeSpinItem = this }, Formatting.Indented);
        }

        public override string ToString()
        {
            return this.ToCustomString();
        }

        protected bool Equals(BonusFreeSpinItemX other)
        {
            return this.Type == other.Type && this.Value == other.Value;
        }
    }
}