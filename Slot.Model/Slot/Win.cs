using System;
using System.Xml.Linq;

namespace Slot.Model
{
    [Serializable]
    public class Win
    {
        public decimal Credit { get; set; }

        public decimal Jackpot { get; set; }

        public decimal Value { get; set; }
    }

    [Serializable]
    public class BonusFeatureWin
    {
        public decimal? TotalWin { get; set; }
        public decimal? FsTotalWin { get; set; }
        public decimal? BonusTotalWin { get; set; }
        public decimal? FeatureTotalWin { get; set; }

        public void AddAttributes(XElement element)
        {
            if (this.TotalWin.HasValue)
                element.SetAttributeValue("summ", Convert.ToString(this.TotalWin));

            if (this.FsTotalWin.HasValue)
                element.SetAttributeValue("fssumm", Convert.ToString(this.FsTotalWin));

            if (this.BonusTotalWin.HasValue)
                element.SetAttributeValue("bsumm", Convert.ToString(this.BonusTotalWin));

            if (this.FeatureTotalWin.HasValue)
                element.SetAttributeValue("fsumm", Convert.ToString(this.FeatureTotalWin));
        }
    }

}