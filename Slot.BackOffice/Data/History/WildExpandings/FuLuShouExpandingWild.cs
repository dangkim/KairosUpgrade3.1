using Slot.Model;
using System.Collections.Generic;
using System.Linq;

namespace Slot.BackOffice.Data.History.WildExpandings
{
    public class FuLuShouExpandingWild : IWildExpanding
    {
        public void Expanding(WheelViewModel wheel)
        {
            var expandingWild = 0;
            for (int i = 0; i < wheel.reels.Count; i++)
            {
                if (wheel.reels[i].All(s => s.symbol != (int) FuLuShouSymbol.Wild)) continue;

                switch (i)
                {
                    case 1: // reel 2
                        expandingWild = (int)FuLuShouSymbol.R2ExpandingWild;
                        break;
                    case 2: // reel 3
                        expandingWild = (int)FuLuShouSymbol.R3ExpandingWild;
                        break;
                    case 3: // reel 4
                        expandingWild = (int)FuLuShouSymbol.R4ExpandingWild;
                        break;
                }

                wheel.reels[i] = new List<Symbols>() 
                {
                    new Symbols{ symbol = expandingWild, height = 3 },
                    new Symbols{ symbol = -1, height = 1 },
                    new Symbols{ symbol = -1, height = 1 }
                };
            }
        }
    }
}