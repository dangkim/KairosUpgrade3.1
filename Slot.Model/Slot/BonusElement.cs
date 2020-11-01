using System;

namespace Slot.Model
{
    [Serializable]
    public class BonusElement
    {
        public int Id { get; set; }

        public string TestData { get; set; }

        public string Value { get; set; }

        public int addFSCount { get; set; }

        public int Count { get; set; }

        public int totalCompletedfs { get; set; }

        public int? expandingSymbol { get; set; }
    }

    [Serializable]
    public class AddBonusElement
    {
        public int Id { get; set; }

        public BonusPosition Position { get; set; }
    }

    [Serializable]
    public class BonusElementExt : BonusElement
    {
        public int Step { get; set; }
        public int Mode { get; set; }
        public int? Count { get; set; }
        public decimal? Summary { get; set; }
        public decimal? FsTotalWin { get; set; }
        public decimal? BonusTotalWin { get; set; }
        public decimal? FeatureTotalWin { get; set; }
    }
}