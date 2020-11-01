using Slot.Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Slot.Model.Slot.Xml
{
    [XmlRoot(ElementName = "spin")]
    public class CollapseXml : ResponseXml, IXmlSerializable, IConvertibleToGameResult
    {
        public CollapseXml()
        {
            WinPositions = new List<WinPosition>();
            TableWins = new HashSet<TableWin>();
            CollapsePosition1 = new List<CollapsePosition>();
            ReelCollapses = new List<ReelCollapse>();
        }

        public SpinBet SpinBet { get; set; }

        [IgnoreDataMember]
        public int Multiplier { get; set; }

        public long TransactionId { get; set; }

        public string Type { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public Wheel Wheel { get; set; }

        public List<WinPosition> WinPositions { get; set; }

        public HashSet<TableWin> TableWins { get; set; }

        public List<CollapsePosition> CollapsePosition
        {
            get { return CollapsePosition1; }
            set { CollapsePosition1 = value; }
        }

        public List<ReelCollapse> ReelCollapses { get; set; }

        public override XmlType XmlType => XmlType.CollapseXml;

        public List<CollapsePosition> CollapsePosition1 { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var element = XElement.Load(reader);

            this.ReadSpinXElement(element);
            this.ReadWinXElement(element);
            this.ReadBalanceXElement(element);
            this.ReadBonusWheelsXElement(element);
            this.ReadWheelsXElement(element);
            this.ReadWinPositionXElement(element);
            this.ReadTableWinXElement(element);
            this.ReadJackpotXElement(element);
            this.ReadBonusPositionXElement(element);
        }

        public GameResult ToGameResult()
        {
            throw new NotImplementedException();
        }

        public XElement ToXElement()
        {
            var element = new XElement("spin");
            CreateSpinXElement(element);
            CreateWheelsXElement(element);
            CreateWinPositionXElement(element);
            CreateTableWinXElement(element);

            CreateCollapsePositionXElement(element);
            return element;
        }

        public void WriteXml(XmlWriter writer)
        {
            WriteSpinElement(writer);

            var xElement = ToXElement();

            foreach (var element in xElement.Elements())
            {
                element.WriteTo(writer);
            }
        }

        private void WriteSpinElement(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(Type))
            {
                writer.WriteAttributeString("type", Type);
            }

            if (TransactionId != 0)
            {
                writer.WriteAttributeString("tid", Convert.ToString(TransactionId));
            }

            if (null != CollapsePosition && !CollapsePosition.Any())
                writer.WriteAttributeString("finish", "true");

            writer.WriteAttributeString(
                 "bet",
                 SpinBet == null ? 0m.ToCustomString() : SpinBet.LineBet.ToCustomString());
            writer.WriteAttributeString("lines", SpinBet == null ? "0" : Convert.ToString(SpinBet.Lines));
            writer.WriteAttributeString(
                "multiplier",
                SpinBet == null ? "0" : Convert.ToString(SpinBet.Multiplier));
            writer.WriteAttributeString(
                "totalbet",
                SpinBet == null ? 0m.ToCustomString() : SpinBet.TotalBet.ToCustomString());
            writer.WriteAttributeString("ts", Convert.ToString(DateTimeUtc.ToUnixTimeStamp()));
        }

        private void CreateSpinXElement(XElement element)
        {
            if (!string.IsNullOrEmpty(Type))
            {
                element.SetAttributeValue("type", Type);
            }

            if (TransactionId != 0)
            {
                element.SetAttributeValue("tid", Convert.ToString(TransactionId));
            }

            if (null != CollapsePosition && !CollapsePosition.Any())
                element.SetAttributeValue("finish", "true");

            element.SetAttributeValue(
                "bet",
                SpinBet == null ? 0m.ToCustomString() : SpinBet.LineBet.ToCustomString());
            element.SetAttributeValue("lines", SpinBet == null ? "0" : Convert.ToString(SpinBet.Lines));
            element.SetAttributeValue(
                "multiplier",
                SpinBet == null ? "0" : Convert.ToString(SpinBet.Multiplier));
            element.SetAttributeValue(
                "totalbet",
                SpinBet == null ? 0m.ToCustomString() : SpinBet.TotalBet.ToCustomString());
            element.SetAttributeValue("ts", DateTimeUtc.ToUnixTimeStamp());
        }

        private void CreateWheelsXElement(XElement element)
        {
            var wheel = this.Wheel;
            var childElement = new XElement("wheels");

            childElement.SetAttributeValue("type", wheel.Type.ToString().ToLower());
            childElement.SetAttributeValue("width", wheel.Width);
            childElement.SetAttributeValue("height", wheel.Height);
            var wheelElement = new List<int>();
            for (var i = 0; i < wheel.Width; i++)
            {
                wheelElement.AddRange(wheel[i]);
            }
            childElement.SetAttributeValue("val", wheelElement.ToCommaDelimitedString());

            element.Add(childElement);
        }

        private void CreateWinPositionXElement(XElement element)
        {
            var childElement = new XElement("winposition");

            foreach (var winPosition in this.WinPositions)
            {
                if (winPosition is WinPositionType)
                {
                    var wp = winPosition as WinPositionType;

                    childElement.Add(
                        new XElement("item",
                            new XAttribute("line", wp.Line),
                            new XAttribute("win", wp.Win),
                            new XAttribute("mul", wp.Multiplier),
                            !String.IsNullOrEmpty(wp.Type) ? new XAttribute("type", wp.Type) : null,
                            winPosition.RowPositions.ToCommaDelimitedString()));
                }
                else
                {
                    childElement.Add(
                        new XElement("item",
                            new XAttribute("line", winPosition.Line),
                            new XAttribute("win", winPosition.Win),
                            new XAttribute("mul", winPosition.Multiplier),
                            winPosition.RowPositions.ToCommaDelimitedString()));
                }
            }

            element.Add(childElement);
        }

        private void CreateTableWinXElement(XElement element)
        {
            var childElement = new XElement("tablewin");

            foreach (var item in this.TableWins)
            {
                childElement.Add(
                    new XElement(
                        "item",
                        new XAttribute("card", item.Card),
                        new XAttribute("count", item.Count),
                        new XAttribute("wild", item.Wild)));
            }

            element.Add(childElement);
        }

        private void CreateCollapsePositionXElement(XElement element)
        {
            var childElement = new XElement("collapseposition");
            if (null != ReelCollapses)
                foreach (var clp in ReelCollapses)
                {
                    if (clp.Symbols.Any())
                        childElement.Add(
                            new XElement(
                                "item",
                                new XAttribute("reel", clp.Reel),
                                new XAttribute("symbols", clp.Symbols.ToCommaDelimitedString())));
                }

            element.Add(childElement);
        }

        private void ReadBalanceXElement(XElement element)
        {

        }

        private void ReadBonusPositionXElement(XElement element)
        {
        }

        private void ReadIndependentWheelXElement(XElement wheelElement)
        {
            foreach (var indepedentWheel in wheelElement.Elements())
            {
                int id = indepedentWheel.AttributeValue("id").ToInt() / this.Wheel.Height;

                this.Wheel[id].Add(indepedentWheel.Value.ToInt());
            }
        }

        private void ReadJackpotXElement(XElement element)
        {
        }

        private void ReadNewWheelXElement(XElement element)
        {
            var value = element.AttributeValue("val").Split(',');

            for (var reel = 0; reel < this.Wheel.Width; reel++)
            {
                for (var row = 0; row < this.Wheel.GetReelHeight(reel); row++)
                {
                    this.Wheel[reel].Add(value[this.Wheel.GetPosition(reel, row)].ToInt());
                }
            }
        }

        private void ReadNormalWheelXElement(XElement wheelElement)
        {
            foreach (var wheel in wheelElement.Elements())
            {
                int id = wheel.AttributeValue("id").ToInt();

                this.Wheel[id] = wheel.Value.Split(',').Select(int.Parse).ToList();
            }
        }

        private void ReadSpinXElement(XElement element)
        {
            this.Type = element.AttributeValue("type");

            this.TransactionId = element.AttributeValue("tid").ToLong();

            this.SpinBet = Activator.CreateInstance<SpinBet>();
            this.SpinBet.LineBet = element.AttributeValue("bet").ToDecimal();
            this.SpinBet.Lines = element.AttributeValue("lines").ToInt();
            this.SpinBet.Multiplier = element.AttributeValue("multiplier").ToInt();
        }

        private void ReadTableWinXElement(XElement element)
        {
        }

        private void ReadBonusWheelsXElement(XElement element)
        {

        }

        private void ReadWheelsXElement(XElement element)
        {
            var wheelElement = element.Element("wheels");

            if (wheelElement == null) return;

            var width = wheelElement.AttributeValue("width").ToInt();
            var height = wheelElement.AttributeValue("height").ToInt();
            var rows = wheelElement.Attribute("rows") != null ? wheelElement.AttributeValue("rows").Split(',').Select(int.Parse).ToList() : new List<int>();
            var value = wheelElement.AttributeValue("val");

            this.Wheel = rows.Any() ? new Wheel(rows, value) : new Wheel(width, height, value);

            WheelType wheelType;
            if (Enum.TryParse(wheelElement.AttributeValue("type"), true, out wheelType))
            {
                this.Wheel.Type = wheelType;
            }
        }

        private void ReadWinPositionXElement(XElement element)
        {
            var winposition = element.Element("winposition");

            if (winposition == null)
            {
                return;
            }

            foreach (var item in winposition.Elements())
            {
                this.WinPositions.Add(
                    new WinPosition
                    {
                        Line = item.AttributeValue("line").ToInt(),
                        Win = item.AttributeValue("win").ToDecimal(),
                        Multiplier = item.AttributeValue("mul").ToInt(),
                        RowPositions = item.Value.Split(',').Select(int.Parse).ToList()
                    });
            }
        }

        private void ReadWinXElement(XElement element)
        {
            var winElement = element.Element("win");

            if (winElement == null)
            {
                return;
            }
        }

    }
}
