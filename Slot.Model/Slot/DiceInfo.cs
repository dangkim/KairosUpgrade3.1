using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Slot.Model
{
    [Serializable]
    [XmlRoot(ElementName = "dice")]
    public class DiceInfo
    {
        public DiceInfo()
        {
            Dices = new List<Dice>();
        }

        public List<Dice> Dices { get; set; }
    }
}