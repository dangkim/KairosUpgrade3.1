using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Games.BullRush.Models
{
    [Serializable]
    public class BullRushBonusElement
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public int Count { get; set; }

        public int AddFSCount { get; set; }
    }
}
