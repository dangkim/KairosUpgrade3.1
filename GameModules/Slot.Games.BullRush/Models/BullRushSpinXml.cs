using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using Slot.Model.Utilities;
using Slot.Model.Utility;
using Slot.Model;

namespace Slot.Games.BullRush.Models
{
    [XmlRoot(ElementName = "spin")]
    public class BullRushSpinXml : ResponseXml, IXmlSerializable
    {
        private HashSet<TableWin> tableWins;

        private List<BullRushWinPosition> winPositions;

        public Balance Balance { get; set; }

        public string Code { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public int Scatter { get; set; }

        public decimal Bet { get; set; }

        public int ReelStripId { get; set; }

        public int GameIdXml { get; set; }

        public SpinBet SpinBet { get; set; }

        public SpinBetX sbx { get; set; }

        public HashSet<TableWin> TableWins
        {
            get { return this.tableWins ?? (this.tableWins = new HashSet<TableWin>()); }
            set { this.tableWins = value; }
        }

        public BullRushBonusElement BonusElement { get; set; }

        public long TransactionId { get; set; }

        public string Type { get; set; }

        public Wheel Wheel { get; set; }

        public Wheel WheelOfCoin { get; set; }

        public int Wild { get; set; }

        public Win WinElement { get; set; }

        public List<BullRushWinPosition> WinPositions
        {
            get { return this.winPositions ?? (this.winPositions = new List<BullRushWinPosition>()); }
            set { this.winPositions = value; }
        }

        public List<BullRushWinPosition> WinHistoryPositions
        {
            get { return this.winPositions ?? (this.winPositions = new List<BullRushWinPosition>()); }
            set { this.winPositions = value; }
        }

        public override XmlType XmlType
        {
            get { return XmlType.SpinXml; }
        }

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
            this.ReadWheelsXElement(element);
            this.ReadWinPositionXElement(element, this.WinHistoryPositions);
            this.ReadTableWinXElement(element);
        }

        public XElement ToXElement()
        {
            var element = new XElement("spin");
            this.sbx = this.SpinBet as SpinBetX;

            this.CreateSpinXElement(element);
            this.CreateWinXElement(element);
            this.CreateBalanceXElement(element);
            this.CreateWheelsXElement(element);
            this.CreateWinPositionXElement(element, this.WinHistoryPositions);
            this.CreateTableWinXElement(element, this.TableWins);
            this.CreateBonusXElement(element, this.BonusElement);
            return element;
        }

        public void WriteXml(XmlWriter writer)
        {
            this.WriteSpinElement(writer);

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

        private void CreateNormalWheelsXElement(XElement element)
        {
            if (this.Wheel != null)
            {
                for (int i = 0; i < this.Wheel.Width; i++)
                {
                    element.Add(new XElement("item", new XAttribute("id", i), this.Wheel[i].ToCommaDelimitedString()));
                }
            }
        }

        private void CreateSpinXElement(XElement element)
        {
            if (!string.IsNullOrEmpty(this.Type))
            {
                element.SetAttributeValue("type", this.Type);
            }

            if (this.TransactionId != 0)
            {
                element.SetAttributeValue("tid", Convert.ToString(this.TransactionId));
            }

            element.SetAttributeValue(
                "bet",
                this.SpinBet == null ? 0m.ToCustomString() : this.SpinBet.LineBet.ToCustomString());
            if ((this.SpinBet?.Credits ?? 0) > 0) element.SetAttributeValue("credits", Convert.ToString(this.SpinBet?.Credits ?? 0));
            element.SetAttributeValue("lines", this.SpinBet == null ? "0" : Convert.ToString(this.SpinBet.Lines));
            element.SetAttributeValue(
                "multiplier",
                this.SpinBet == null ? "0" : Convert.ToString(this.SpinBet.Multiplier));
            element.SetAttributeValue(
                "totalbet",
                this.SpinBet == null ? 0m.ToCustomString() : this.SpinBet.TotalBet.ToCustomString());

            element.SetAttributeValue(
                "sidebet",
                this.sbx == null || this.sbx.IsSideBet == false ? 0m.ToCustomString() : this.SpinBet.TotalBet.ToCustomString());

            if (this.SpinBet == null || this.SpinBet.UserGameKey.IsFreeGame)
            {
                element.SetAttributeValue(
                    "expired",
                    this.SpinBet == null || this.SpinBet.UserGameKey.FRExpired == false ? "0" : "1");

                var win = WinElement != null ? WinElement.Value : 0;
                element.SetAttributeValue(
                    "frsum",
                    this.SpinBet == null ? "0" : (this.SpinBet.UserGameKey.FRWinLose + win).ToCustomString());
            }

            element.SetAttributeValue("wild", Convert.ToString(this.Wild));
            element.SetAttributeValue("scatter", Convert.ToString(this.Scatter));

            element.SetAttributeValue("ts", this.DateTimeUtc.ToUnixTimeStamp());
        }

        private void CreateTableWinXElement(XElement element, HashSet<TableWin> tableWinList)
        {
            var childElement = new XElement("tablewin");

            foreach (var item in tableWinList)
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

        private void CreateWheelsXElement(XElement element)
        {
            var childElement = new XElement("wheels");

            if (this.Wheel != null)
            {
                childElement.SetAttributeValue("type", this.Wheel.Type.ToString().ToLower());
                childElement.SetAttributeValue("width", this.Wheel.Width);
                childElement.SetAttributeValue("height", this.Wheel.Height);

                if (this.Wheel.Rows?.Any() ?? false)
                    childElement.SetAttributeValue("rows", this.Wheel.Rows.ToCommaDelimitedString());

                childElement.SetAttributeValue("val", this.Wheel.ToList().ToCommaDelimitedString());
            }

            if (this.WheelOfCoin != null)
            {
                childElement.SetAttributeValue("coinwheel", this.WheelOfCoin.ToList().ToCommaDelimitedString());
            }

            element.Add(childElement);
        }

        private void CreateWinPositionXElement(XElement element, List<BullRushWinPosition> winPositionList)
        {
            var childElement = new XElement("winposition");

            foreach (var winPosition in winPositionList)
            {
                childElement.Add(
                    new XElement("item",
                        new XAttribute("line", winPosition.Line),
                        new XAttribute("win", winPosition.Win),
                        new XAttribute("symbol", winPosition.Symbol),
                        new XAttribute("count", winPosition.Count),
                        new XAttribute("mul", winPosition.Multiplier),
                        new XAttribute("wildmul", winPosition.RandomMultiplier),
                        winPosition.RowPositions.ToCommaDelimitedString()));
            }

            element.Add(childElement);
        }

        private void CreateWinXElement(XElement xElement)
        {
            string jackpot = 0m.ToCustomString();
            string credit = 0m.ToCustomString();
            string value = 0m.ToCustomString();

            var win = this.WinElement;
            if (win != null)
            {
                jackpot = win.Jackpot.ToCustomString();
                credit = win.Credit.ToCustomString();
                value = win.Value.ToCustomString();
            }

            var element = new XElement("win", new XAttribute("jackpot", jackpot), value);

            xElement.Add(element);
        }

        private void CreateBonusXElement(XElement element, BullRushBonusElement bonus)
        {
            var childElement = new XElement("bonus");

            if (bonus != null)
            {
                childElement = new XElement("bonus", bonus.Value);

                if (bonus.Id != 0)
                    childElement.SetAttributeValue("id", bonus.Id);

                if (bonus.AddFSCount > 0)
                    childElement.SetAttributeValue("addFSCount", bonus.AddFSCount);
            }

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

        private void ReadWinPositionXElement(XElement element, List<BullRushWinPosition> winPositionList)
        {
            var winposition = element.Element("winposition");

            if (winposition == null)
            {
                return;
            }

            foreach (var item in winposition.Elements())
            {
                var winPosition = new BullRushWinPosition
                {
                    Line = item.AttributeValue("line").ToInt(),
                    Win = item.AttributeValue("win").ToDecimal(),
                    Symbol = item.AttributeValue("symbol").ToInt(),
                    Count = item.AttributeValue("count").ToInt(),
                    Multiplier = item.AttributeValue("mul").ToInt(),
                    RandomMultiplier = item.AttributeValue("wildmul").ToInt(),
                    RowPositions = item.Value.Split(',').Select(int.Parse).ToList()
                };

                winPositionList.Add(winPosition);
            }
        }

        private void ReadWinXElement(XElement element)
        {
            var winElement = element.Element("win");

            if (winElement == null)
            {
                return;
            }

            this.WinElement = new Win
            {
                Credit = winElement.AttributeValue("credit").ToDecimal(),
                Value = winElement.Value.ToDecimal()
            };
        }

        private void WriteSpinElement(XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Type))
            {
                writer.WriteAttributeString("type", this.Type);
            }

            if (this.TransactionId != 0)
            {
                writer.WriteAttributeString("tid", Convert.ToString(this.TransactionId));
            }

            writer.WriteAttributeString(
                "bet",
                this.SpinBet == null ? 0m.ToCustomString() : this.SpinBet.LineBet.ToCustomString());
            if ((this.SpinBet?.Credits ?? 0) > 0) writer.WriteAttributeString("credits", Convert.ToString(this.SpinBet?.Credits ?? 0));
            writer.WriteAttributeString("lines", this.SpinBet == null ? "0" : Convert.ToString(this.SpinBet.Lines));
            writer.WriteAttributeString(
                "multiplier",
                this.SpinBet == null ? "0" : Convert.ToString(this.SpinBet.Multiplier));
            writer.WriteAttributeString(
                "totalbet",
                this.SpinBet == null ? 0m.ToCustomString() : this.SpinBet.TotalBet.ToCustomString());

            writer.WriteAttributeString("sidebet", this.sbx == null || this.sbx.IsSideBet == false ? "0" : Convert.ToString(this.SpinBet.TotalBet));

            if (this.SpinBet == null || this.SpinBet.UserGameKey.IsFreeGame)
            {
                writer.WriteAttributeString("expired", this.SpinBet == null || this.SpinBet.UserGameKey.FRExpired == false ? "0" : "1");

                var win = WinElement != null ? WinElement.Value : 0;
                writer.WriteAttributeString(
                    "frsum",
                    this.SpinBet == null ? "0" : (this.SpinBet.UserGameKey.FRWinLose + win).ToCustomString());
            }

            writer.WriteAttributeString("wild", Convert.ToString(this.Wild));
            writer.WriteAttributeString("scatter", Convert.ToString(this.Scatter));
            writer.WriteAttributeString("ts", Convert.ToString(this.DateTimeUtc.ToUnixTimeStamp()));
        }
    }
}