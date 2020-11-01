using System;
using System.Collections.Generic;

namespace Slot.Model
{
    [Serializable]
    public class ReelSet : Wheel
    {
        public ReelSet(int width, int height) 
            : base(width, height)
        {
            WinPositions = new List<WinPosition>();
            TableWins = new HashSet<TableWin>();
            BonusPositions = new List<BonusPosition>();
            BonusElement = new BonusElement();
        }

        public int Id { get; set; }

        public List<WinPosition> WinPositions { get; set; }

        public HashSet<TableWin> TableWins { get; set; }

        public List<BonusPosition> BonusPositions { get; set; }

        public BonusElement BonusElement { get; set; }

        public int? Counter { get; set; }

        public int? TotalSpin { get; set; }

        public int GetValue(int position)
        {
            var reel = position / this.Height;
            var row = position % this.Height;
            return this.Reels[reel][row];
        }

        public void SetValue(int position, int value)
        {
            var reel = position / this.Height;
            var row = position % this.Height;
            this.Reels[reel][row] = value;
        }
    }
}