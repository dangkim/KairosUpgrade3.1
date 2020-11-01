using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using Slot.Model.Utility;
using Slot.Model;

namespace Slot.Games.BullRush.Models
{
    [XmlRoot(ElementName = "bonus")]
    public class MegaMoneyBonusXml : ResponseXml, IXmlSerializable
    {
        public MegaMoneyBonusXml()
        {
            this.Attributes = new Dictionary<string, string>();
            this.BonusPosition = new List<BonusPosition>();
        }

        public Dictionary<string, string> Attributes { get; set; }

        public Balance Balance { get; set; }

        public List<BonusPosition> BonusPosition { get; set; }

        public BullRushBonusElement BonusElement { get; set; }

        public string Code { get; set; }

        public XElement Data { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public long TransactionId { get; set; }

        public string Type { get; set; }

        public decimal Win { get; set; }

        public decimal WinSum { get; set; }

        public bool IsFreeGame { get; set; }

        public bool FRExpired { get; set; }

        public decimal FRWinLose { get; set; }

        public BonusElementExt BonusElementExt { get; set; }

        public List<BonusPosition> BonusPositions { get; set; }

        public override XmlType XmlType
        {
            get { return XmlType.BonusXml; }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var element = XElement.Load(reader);
            this.ReadBonusXElement(element);
            this.ReadBonusPositionXElement(element);
            this.ReadWinXElement(element);
            this.ReadBalanceXElement(element);
            this.ReadDataXElement(element);
        }

        public XElement ToXElement()
        {
            var element = new XElement("bonus");

            this.CreateBonusXElement(element);
            this.CreateBonusPositionXElement(element);
            this.CreateWinXElement(element);
            this.CreateBalanceXElement(element);
            this.CreateDataXElement(element);
            this.CreateAddBonusXElement(element);

            return element;
        }

        public void WriteXml(XmlWriter writer)
        {
            this.WriteBonusElement(writer);

            var xElement = this.ToXElement();
            foreach (var element in xElement.Elements())
            {
                element.WriteTo(writer);
            }
        }

        private void CreateBalanceXElement(XElement element)
        {
            this.CreateBalanceXElement(element, this.Balance);
        }

        private void CreateBonusXElement(XElement element)
        {
            if (!string.IsNullOrEmpty(this.Type))
            {
                element.SetAttributeValue("type", Convert.ToString(this.Type));
            }

            if (this.TransactionId != 0)
            {
                element.SetAttributeValue("tid", Convert.ToString(this.TransactionId));
            }

            if ((this.Attributes != null) && this.Attributes.Any())
            {
                foreach (var attribute in this.Attributes)
                {
                    element.SetAttributeValue(attribute.Key, attribute.Value);
                }
            }

            element.SetAttributeValue("ts", Convert.ToString(this.DateTimeUtc.ToUnixTimeStamp()));
            element.SetAttributeValue("freeround", IsFreeGame == false ? "0" : "1");
            if (IsFreeGame) element.SetAttributeValue("expired", FRExpired == false ? "0" : "1");
            if (IsFreeGame) element.SetAttributeValue("frsum", (FRWinLose + Win).ToCustomString());
        }

        private void CreateBonusPositionXElement(XElement element)
        {
            if (this.BonusPosition == null || this.BonusPosition.Count == 0) return;

            var bpelement = new XElement("bonusposition");

            foreach (BonusPosition bp in this.BonusPosition)
            {
                XElement elem = new XElement("item");
                elem.SetAttributeValue("line", bp.Line.ToString());
                elem.SetAttributeValue("win", bp.Win.ToString("F2"));
                elem.SetAttributeValue("mul", bp.Multiplier.ToString());
                elem.Value = bp.RowPositions.ToCommaDelimitedString();
                bpelement.Add(elem);
            }

            element.Add(bpelement);
        }

        private void CreateAddBonusXElement(XElement element)
        {
            if (this.BonusElement != null)
            {
                var bonusElement = new XElement("cont");
                bonusElement.SetAttributeValue("id", Convert.ToString(BonusElement.Id));

                if (BonusElement.AddFSCount > 0)
                    bonusElement.SetAttributeValue("addFSCount", Convert.ToString(BonusElement.AddFSCount));

                if (BonusElement.Count > 0)
                    bonusElement.SetAttributeValue("count", Convert.ToString(BonusElement.Count));

                bonusElement.Value = this.BonusElement.Value;
                element.Add(bonusElement);
                return;
            }

            if (this.BonusElementExt != null)
            {
                var bonusElement = new XElement("cont");
                bonusElement.SetAttributeValue("id", Convert.ToString(this.BonusElementExt.Id));

                if (this.BonusElementExt.Count.HasValue)
                    bonusElement.SetAttributeValue("count", Convert.ToString(this.BonusElementExt.Count));

                if (this.BonusElementExt.Step > 0)
                    bonusElement.SetAttributeValue("step", Convert.ToString(this.BonusElementExt.Step));

                if (this.BonusElementExt.Mode > 0)
                    bonusElement.SetAttributeValue("mode", Convert.ToString(this.BonusElementExt.Mode));

                if (this.BonusElementExt.Summary.HasValue)
                    bonusElement.SetAttributeValue("summ", Convert.ToString(this.BonusElementExt.Summary));

                if (this.BonusElementExt.FsTotalWin.HasValue)
                    bonusElement.SetAttributeValue("fssumm", Convert.ToString(this.BonusElementExt.FsTotalWin));

                if (this.BonusElementExt.BonusTotalWin.HasValue)
                    bonusElement.SetAttributeValue("bsumm", Convert.ToString(this.BonusElementExt.BonusTotalWin));

                if (this.BonusElementExt.FeatureTotalWin.HasValue)
                    bonusElement.SetAttributeValue("fsumm", Convert.ToString(this.BonusElementExt.FeatureTotalWin));

                bonusElement.Value = this.BonusElementExt.Value;
                element.Add(bonusElement);
            }
        }

        private void CreateDataXElement(XElement element)
        {
            var data = this.Data;
            if (data != null)
            {
                element.Add(data);
            }
        }

        private void CreateWinXElement(XElement element)
        {
            var winSumm = this.WinSum != 0 ? new XAttribute("summ", this.WinSum.ToCustomString()) : null;

            var childElement = new XElement("win", winSumm, this.Win.ToCustomString());

            element.Add(childElement);
        }

        private void ReadBalanceXElement(XElement element)
        {
            var balanceElement = element.Element("balance");

            if (balanceElement == null)
            {
                return;
            }

            this.Balance = new Balance
            {
                Credit = balanceElement.AttributeValue("credit").ToDecimal(),
                Conversion = balanceElement.AttributeValue("conv").ToDecimal(),
                Value = balanceElement.Value.ToDecimal()
            };
        }

        private void ReadBonusXElement(XElement element)
        {
            foreach (var attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "tid":
                        this.TransactionId = attribute.Value.ToLong();
                        continue;

                    case "type":
                        this.Type = attribute.Value;
                        continue;

                    default:
                        this.Attributes[attribute.Name.LocalName] = attribute.Value;
                        continue;
                }
            }
        }

        private void ReadBonusPositionXElement(XElement element)
        {
            var bpelement = element.Element("bonusposition");

            if (bpelement == null) return;

            foreach (XElement elem in bpelement.Elements())
            {
                BonusPosition bp = new BonusPosition();
                bp.Line = elem.AttributeValue("line").ToInt();
                bp.Win = elem.AttributeValue("win").ToDecimal();
                bp.Multiplier = elem.AttributeValue("mul").ToInt();
                bp.RowPositions = elem.Value.Split(',').Select(int.Parse).ToList();
            }
        }

        /// <summary>The read data x element.</summary>
        private void ReadDataXElement(XElement element)
        {
            var dataElement = element.Element("data");

            if (dataElement == null) return;

            this.Data = dataElement;
        }

        private void ReadWinXElement(XElement element)
        {
            var winElement = element.Element("win");

            if (winElement == null) return;

            this.Win = winElement.Value.ToDecimal();
        }

        private void WriteBonusElement(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Type))
            {
                writer.WriteAttributeString("type", Convert.ToString(this.Type));
            }

            if (this.TransactionId != 0)
            {
                writer.WriteAttributeString("tid", Convert.ToString(this.TransactionId));
            }

            if ((this.Attributes != null) && this.Attributes.Any())
            {
                foreach (var attribute in this.Attributes)
                {
                    writer.WriteAttributeString(attribute.Key, attribute.Value);
                }
            }

            writer.WriteAttributeString("ts", Convert.ToString(this.DateTimeUtc.ToUnixTimeStamp()));
            if (IsFreeGame) writer.WriteAttributeString("frsum", (FRWinLose + Win).ToCustomString());
        }
    }
}