using System;



namespace Slot.Model
{
    [Serializable]
    public class FeatureElement
    {
        public int Reel { get; set; }

        public int Row { get; set; }

        public string Type { get; set; }
    }
}