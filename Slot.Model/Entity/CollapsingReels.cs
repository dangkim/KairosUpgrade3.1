using System;

namespace Slot.Model.Entity
{
    public class CollapsingReels
    {
        public CollapsingReels()
        {
            this.FullReelCollapse = false;
            this.Multiplier = 1;
            this.IncrementingMultiplier = 0;
        }
        
        public bool FullReelCollapse { get; set; }

        public int Multiplier { get; set; }

        public int IncrementingMultiplier { get; set; }
    }
}
