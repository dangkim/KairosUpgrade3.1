using System.Collections.Generic;


namespace Slot.Model
{
    public class ExtWildSymbolInfo : WildSymbolInfo
    {
        public ExtWildSymbolInfo()
        {
            WildNeighborhood = - 1;
        }
        public int WildNeighborhood { get; set; }
    }

    public class WildSymbolInfo
    {
        public int Id { get; set; }
        
        public int Multiplier { get; set; }

        public bool IsExpand { get; set; }

        public List<bool> ExpandReels { get; set; }

        public WildSymbolInfo()
        {
            this.Id = -1;
            this.IsExpand = false;
        }
    }
    public class SpecialWildSymbolInfo
    {
        public int Id { get; set; }

        public int Multiplier { get; set; }

        public bool IsExpand { get; set; }

        public WildExpandType ExpandType { get; set; }
        public List<int> ExpandReels { get; set; }
        public SpecialWildSymbolInfo()
        {
            Id = -1;
            IsExpand = false;
            ExpandReels = new List<int>();
            ExpandType = WildExpandType.None;
        }
    }
    public enum WildExpandType
    {
        None = 0,
        Diagonal = 1,
        Square = 2,
        Horizontal = 3,
        Vertical = 4
    }
}
