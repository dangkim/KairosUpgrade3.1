using System;

using Newtonsoft.Json;


namespace Slot.Model
{
    [Serializable]
    public class Dice
    {
        public int Side { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}