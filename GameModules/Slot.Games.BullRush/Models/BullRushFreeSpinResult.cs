using Slot.Model;
using Slot.Model.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Slot.Games.BullRush.Models
{
    [Serializable]
    public class BullRushFreeSpinResult : BonusResult
    {
        public BullRushFreeSpinResult(UserGameKey userGameKey) : base(userGameKey)
        {
            this.Type = "fs";
            TransactionType = GameTransactionType.FreeSpin;
        }
        
        public int RandomMultiplier { get; set; }

        [Category("Bonus Attributes")]
        [DisplayName(@"counter")]
        public int Counter { get; set; }

        [Category("Bonus Attributes")]
        [DisplayName(@"nextstep")]
        public int? NextStep { get; set; }

        public decimal CumulativeWin { get; set; }

        [Category("Bonus Attributes")]
        [DisplayName(@"gamewin")]
        public decimal GameWin { get; set; }

        [Category("Bonus Attributes")]
        [DisplayName(@"mp")]
        public int Multiplier { get; set; }

        [Category("Data")]
        [DisplayName(@"Slot.Model.SpinXml")]
        public BullRushSpinResult SpinResult { get; set; }

        [Category("Bonus Attributes")]
        [DisplayName(@"all")]
        public int TotalSpin { get; set; }

        public BonusFeatureWin BonusFeatureWinInfo { get; set; }

        public override GameResultType GameResultType
        {
            get { return GameResultType.FreeSpinResult; }
        }

        public override XmlType XmlType
        {
            get { return XmlType.BonusXml; }
        }

        protected override void CreateBonusXElement(BonusXml bonusXml)
        {
            this.CreateBonusElementAttribute(bonusXml);
        }

        protected override void CreateDataXElement(BonusXml bonusXml)
        {
            this.CreateDataElement(bonusXml);
        }

        protected override ResponseXml ToXml(ResponseXmlFormat format)
        {
            var bonusXml = (BonusXml)base.ToXml(format);

            this.CreateBonusElementAttribute(bonusXml);
            this.CreateDataElement(bonusXml);

            return bonusXml;
        }

        private void CreateBonusElementAttribute(BonusXml bonusXml)
        {
            bonusXml.Attributes.Add("counter", Convert.ToString(this.Counter));

            if (this.NextStep.HasValue)
                bonusXml.Attributes.Add("nextstep", Convert.ToString(this.NextStep));
            bonusXml.Attributes.Add("mp", Convert.ToString(this.Multiplier));
            bonusXml.Attributes.Add("gamewin", this.GameWin.ToCustomString());
            bonusXml.Attributes.Add("all", Convert.ToString(this.TotalSpin));
        }

        private void CreateDataElement(BonusXml bonusXml)
        {
            if (this.SpinResult == null) return;

            var dataSummAttribute = new XAttribute("summ", this.CumulativeWin);

            bonusXml.Data = new XElement("data", dataSummAttribute, this.SpinResult.ToXElement());

            BonusFeatureWinInfo?.AddAttributes(bonusXml.Data);
        }
    }
}
