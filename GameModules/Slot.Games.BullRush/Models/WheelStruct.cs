using Slot.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Games.BullRush.Models
{
    [Serializable]
    public struct WheelStruct : IWheel
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public List<List<int>> Reels { get; set; }

        public List<List<int>> Symbols { get; set; }

        public List<int> FallDownIndices { get; set; }
    };
}
