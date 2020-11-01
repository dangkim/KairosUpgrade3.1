using System;


namespace Slot.Model
{
    /// <summary>Represents the xml element <c><tablewin></tablewin></c>.</summary>
    [Serializable]
    public class TableWin
    {
        public int Card { get; set; }

        public int Count { get; set; }

        public int Wild { get; set; }

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

            return obj.GetType() == this.GetType() && this.Equals((TableWin)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Card;
                hashCode = (hashCode * 397) ^ this.Count;
                hashCode = (hashCode * 397) ^ this.Wild;
                return hashCode;
            }
        }

        protected bool Equals(TableWin other)
        {
            return this.Card == other.Card && this.Count == other.Count && this.Wild == other.Wild;
        }
    }
}