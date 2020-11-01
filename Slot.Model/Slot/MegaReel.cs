namespace Slot.Model {
    using System.Collections.Generic;
    using System;

    [Serializable]
    public class MegaReel : IWheel {
        public int Horizontal {
            get {
                return Reels.Count;
            }
        }
        public List<int[]> Reels { get; }

        public MegaReel(int horizontal) {
            Reels = new List<int[]>(horizontal);
        }

        /// <summary>Gets or sets the list of elements at the specified reel.</summary>
        /// <param name="reel">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException">Index is less than 0 or index is greater than the width of <see cref="Wheel"/>.</exception>
        /// <returns>The list of elements.</returns>
        public int[] this [int reel] {
            get { return Reels[reel]; }

            set {
                if (reel >= 0 && reel < this.Horizontal) {
                    Reels[reel] = value;
                } else {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}