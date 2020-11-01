using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Games.BullRush.Models
{
    [Serializable]
    public struct BonusStruct
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public int Count { get; set; }
    };
}
