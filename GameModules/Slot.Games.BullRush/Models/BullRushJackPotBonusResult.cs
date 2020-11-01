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
    public class BullRushJackpotBonusResult : BonusResult
    {
        public BullRushJackpotBonusResult(UserGameKey userGameKey) : base(userGameKey)
        {
            this.Type = "fs";
            this.TransactionType = GameTransactionType.ReSpin;
        }

        public decimal CumulativeWin { get; set; }

        public decimal TotalWin { get; set; }

        public int Counter { get; set; }

        [Category("Data")]
        [DisplayName(@"Slot.Model.SpinXml")]
        public BullRushSpinResult SpinResult { get; set; }

        public bool IsFreeSpin { get; set; }

        public bool ContinueJackpot { get; set; }

        public int CurrentStep { get; set; }

        public int TotalSpin { get; set; }

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
        }

        private void CreateDataElement(BonusXml bonusXml)
        {
            if (this.SpinResult == null) return;

            var dataSummAttribute = new XAttribute("summ", this.CumulativeWin);

            bonusXml.Data = new XElement("data", dataSummAttribute, this.SpinResult.ToXElement());

            bonusXml.Data.Add(
                    new XElement(
                        "mode",
                        2));
        }
    }
}
