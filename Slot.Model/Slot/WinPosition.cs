using System;
using System.Collections.Generic;
using System.Linq;

namespace Slot.Model
{
    [Serializable]
    public class WinPosition
    {
        public int Line { get; set; }

        public int Multiplier { get; set; }

        public List<int> RowPositions { get; set; }

        public int Symbol { get; set; }

        public int Count { get; set; }

        public int WildCount { get; set; }

        public int WildMultiplier { get; set; }

        public decimal Win { get; set; }

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

            return obj.GetType() == this.GetType() && this.Equals((WinPosition)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Line;
                hashCode = (hashCode * 397) ^ this.Multiplier;
                hashCode = (hashCode * 397) ^ (this.RowPositions != null ? this.RowPositions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Win.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(WinPosition other)
        {
            return this.Line == other.Line && this.Multiplier == other.Multiplier
                   && this.RowPositions.SequenceEqual(other.RowPositions) && this.Win == other.Win;
        }
    }

    [Serializable]
    public class WinPositionType : WinPosition
    {
        // x: for random multiplier
        // r: for special scatter / combination at reel
        public string Type { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Line;
                hashCode = (hashCode * 397) ^ this.Multiplier;
                hashCode = (hashCode * 397) ^ (this.RowPositions != null ? this.RowPositions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Win.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Type.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(WinPositionType other)
        {
            return this.Line == other.Line
                && this.Multiplier == other.Multiplier
                && this.RowPositions.SequenceEqual(other.RowPositions)
                && this.Win == other.Win
                && this.Type == other.Type;
        }
    }

    [Serializable]
    public class WinPositionExpanding : WinPosition
    {
        public bool IsExpanded { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Line;
                hashCode = (hashCode * 397) ^ this.Multiplier;
                hashCode = (hashCode * 397) ^ (this.RowPositions != null ? this.RowPositions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Win.GetHashCode();
                hashCode = (hashCode * 397) ^ this.IsExpanded.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(WinPositionExpanding other)
        {
            return this.Line == other.Line
                && this.Multiplier == other.Multiplier
                && this.RowPositions.SequenceEqual(other.RowPositions)
                && this.Win == other.Win
                && this.IsExpanded == other.IsExpanded;
        }
    }
}