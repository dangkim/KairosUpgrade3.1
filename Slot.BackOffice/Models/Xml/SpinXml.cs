using Slot.Model;
using Slot.Model.Utilities;
using Slot.Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using GameId = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Models.Xml
{
    [XmlRoot(ElementName = "spin")]
    public class SpinXml : ResponseXml, IXmlSerializable
    {
        private List<BonusPosition> bonusPositions;

        private HashSet<TableWin> tableWins;

        private List<WinPosition> winPositions;

        public Balance Balance { get; set; }

        public BonusElement BonusElement { get; set; }

        public FreeRoundInfo FreeRoundInfo { get; set; }

        private List<AddBonusElement> addBonusElement;

        public List<AddBonusElement> AddBonusElement
        {
            get { return this.addBonusElement ?? (this.addBonusElement = new List<AddBonusElement>()); }
            set { this.addBonusElement = value; }
        }

        public List<BonusPosition> BonusPositions
        {
            get { return this.bonusPositions ?? (this.bonusPositions = new List<BonusPosition>()); }
            set { this.bonusPositions = value; }
        }

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

        public long TransactionId { get; set; }

        public string Type { get; set; }

        public Wheel Wheel { get; set; }

        public Wheel PostWheel { get; set; }

        public Wheel BonusWheel { get; set; }

        public int Wild { get; set; }

        public Win WinElement { get; set; }

        public BonusElementExt BonusElementExt { get; set; }

        public List<WinPosition> WinPositions
        {
            get { return this.winPositions ?? (this.winPositions = new List<WinPosition>()); }
            set { this.winPositions = value; }
        }

        public DiceInfo DiceInfo { get; set; }

        public int? RespunReel { get; set; }

        public Dictionary<int, decimal> ReelRespinCost { get; set; }

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
            this.ReadFreeRoundXElement(element);
            this.ReadWinXElement(element);
            this.ReadBalanceXElement(element);
            this.ReadBonusWheelsXElement(element);
            this.ReadWheelsXElement(element);
            this.ReadWinPositionXElement(element, this.WinPositions);
            this.ReadTableWinXElement(element);
            this.ReadBonusXElement(element);
            //this.ReadAddBonusXElement(element);
            this.ReadJackpotXElement(element);
            this.ReadBonusPositionXElement(element);
            this.ReadDiceXElement(element);
        }

        //public GameResult ToGameResult()
        //{
        //    var spinResult = new SpinResult(new UserGameKey(0, GameId.None));

        //    spinResult.InjectFrom(this);

        //    return spinResult;
        //}

        public XElement ToXElement()
        {
            var element = new XElement("spin");
            this.sbx = this.SpinBet as SpinBetX;

            this.CreateSpinXElement(element);
            this.CreateFreeRoundXElement(element);
            this.CreateWinXElement(element);
            this.CreateBalanceXElement(element);
            this.CreateBonusWheelsXElement(element);
            this.CreateWheelsXElement(element);
            this.CreateWinPositionXElement(element, this.WinPositions);
            this.CreateTableWinXElement(element, this.TableWins);
            this.CreateBonusXElement(element, this.BonusElement ?? this.BonusElementExt);
            this.CreateAddBonusXElement(element);
            this.CreateJackpotXElement(element);
            this.CreateBonusPositionXElement(element, this.BonusPositions);
            this.CreateDiceXElement(element);
            this.CreateReelRespinXElement(element);
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

        private void CreateBonusPositionXElement(XElement element, List<BonusPosition> bonusPositionList)
        {
            var childElement = new XElement("bonusposition");

            foreach (var bonusPosition in bonusPositionList)
            {
                childElement.Add(
                    new XElement(
                        "item",
                        new XAttribute("line", bonusPosition.Line),
                        new XAttribute("win", bonusPosition.Win),
                        new XAttribute("mul", bonusPosition.Multiplier),
                        bonusPosition.RowPositions.ToCommaDelimitedString()));
            }

            element.Add(childElement);
        }

        private void CreateBonusXElement(XElement element, BonusElement bonus)
        {
            var childElement = new XElement("bonus");

            if (bonus != null)
            {
                childElement = new XElement("bonus", bonus.Value);

                if (bonus.Id != 0)
                    childElement.SetAttributeValue("id", bonus.Id);

                if (bonus.addFSCount > 0)
                    childElement.SetAttributeValue("addFSCount", bonus.addFSCount);

                if (bonus.expandingSymbol != null)
                    childElement.SetAttributeValue("expandingsymbol", Convert.ToString(bonus.expandingSymbol.Value));

                if (bonus.Count > 0)
                    childElement.SetAttributeValue("count", bonus.Count);
            }

            element.Add(childElement);
        }

        private void CreateAddBonusXElement(XElement element)
        {
            var childElement = new XElement("cont");

            if (this.AddBonusElement != null && this.addBonusElement.Count > 0)
            {
                foreach (var item in this.AddBonusElement)
                {
                    var itemElement = new XElement("item");

                    itemElement.SetAttributeValue("id", item.Id);
                    itemElement.Value = item.Position.RowPositions.ToCommaDelimitedString();

                    childElement.Add(itemElement);
                }
                element.Add(childElement);
            }
        }

        private void CreateIndependentWheelsXElement(XElement element)
        {
            if (this.Wheel != null)
            {
                for (int col = 0; col < this.Wheel.Width; col++)
                {
                    for (int row = 0; row < this.Wheel.Height; row++)
                    {
                        int index = (this.Wheel.Height * col) + row;

                        element.Add(new XElement("item", new XAttribute("id", index), this.Wheel[col][row]));
                    }
                }
            }
        }

        private void CreateJackpotXElement(XElement xElement)
        {
            var element = new XElement("jackpot", null);

            xElement.Add(element);
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

            if (this.RespunReel.HasValue)
            {
                element.SetAttributeValue("reel", this.RespunReel.Value + 1);
                element.SetAttributeValue("reelbet", this.Bet.ToString("F"));
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

            element.SetAttributeValue(
                "freeround",
                this.SpinBet == null || this.SpinBet.UserGameKey.IsFreeGame == false ? "0" : "1");

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
            try
            {
                if (!string.IsNullOrEmpty(this.Wheel.ReelStripsId) && this.GameIdXml == (int)GameId.GeniesLuck)
                {
                    string anticipation = "";
                    if (this.Wheel.ReelStripsId == "RS" || this.Wheel.ReelStripsId == "MGB" || this.Wheel.ReelStripsId == "FSB")
                    {
                        anticipation = "1";
                    }
                    else if (this.Wheel.ReelStripsId == "FSC")
                    {
                        anticipation = "2";
                    }
                    else
                    {
                        anticipation = "0";
                    }

                    element.SetAttributeValue("r", anticipation);
                }
            }
            catch
            {
                // hard code issue  if (!string.IsNullOrEmpty(this.Wheel.ReelStripsId) && this.GameIdXml == (int)GameId.GeniesLuck)
            }

            element.SetAttributeValue("ts", this.DateTimeUtc.ToUnixTimeStamp());
        }

        private void CreateFreeRoundXElement(XElement element)
        {
            if (null == FreeRoundInfo) return;

            var freeGame = new XElement("fround",
                new XAttribute("state", FreeRoundInfo.State.ToName()),
                new XAttribute("round", FreeRoundInfo.Round),
                new XAttribute("isfinished", FreeRoundInfo.IsFinished ? 1 : 0));

            element.AddFirst(freeGame);
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

        private void CreateBonusWheelsXElement(XElement element)
        {
            if (this.BonusWheel == null)
                return;

            var childElement = new XElement("bonuswheels");

            childElement.SetAttributeValue("type", this.BonusWheel.Type.ToString().ToLower());
            childElement.SetAttributeValue("width", this.BonusWheel.Width);
            childElement.SetAttributeValue("height", this.BonusWheel.Height);
            childElement.SetAttributeValue("mode", this.BonusWheel.Mode);

            var wheelElement = new List<int>();
            for (var i = 0; i < this.Wheel.Width; i++)
            {
                wheelElement.AddRange(this.BonusWheel[i]);
            }

            childElement.SetAttributeValue("val", wheelElement.ToCommaDelimitedString());

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

                var wheelElement = new List<int>();
                for (var i = 0; i < this.Wheel.Width; i++)
                {
                    wheelElement.AddRange(this.Wheel[i]);
                }

                if (this.Wheel.ReelSets != null)
                {
                    childElement.SetAttributeValue("reelsets", this.Wheel.ReelSets.Select(reelSet => reelSet.Width).ToCommaDelimitedString());
                    CreateReelSetsXElement(childElement);
                }

                childElement.SetAttributeValue("val", this.Wheel.ToList().ToCommaDelimitedString());

                childElement.SetAttributeValue("val", wheelElement.ToCommaDelimitedString());
            }

            element.Add(childElement);
        }

        private void CreateWinPositionXElement(XElement element, List<WinPosition> winPositionList)
        {
            var childElement = new XElement("winposition");

            foreach (var winPosition in winPositionList)
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

        private void CreateReelRespinXElement(XElement element)
        {
            var reelRespinElement = new XElement("reelrespin");

            if (this.ReelRespinCost?.Any() ?? false)
            {
                foreach (var kvp in this.ReelRespinCost)
                {
                    reelRespinElement.Add(
                        new XElement("item",
                            new XAttribute("reel", kvp.Key + 1),
                            new XAttribute("cost", kvp.Value.ToString("F2"))));
                }
            }

            element.Add(reelRespinElement);
        }

        private void CreateReelSetsXElement(XElement element)
        {
            var childElement = new XElement("reelsets");

            this.Wheel.ReelSets.ForEach(reelSet =>
            {
                var reelSetElement = new XElement("item");

                reelSetElement.SetAttributeValue("reelset", reelSet.Id);
                reelSetElement.SetAttributeValue("width", reelSet.Width);
                reelSetElement.SetAttributeValue("height", reelSet.Height);
                reelSetElement.SetAttributeValue("val", reelSet.ToList().ToCommaDelimitedString());

                if (reelSet.Counter.HasValue)
                    reelSetElement.SetAttributeValue("counter", reelSet.Counter);
                if (reelSet.TotalSpin.HasValue)
                    reelSetElement.SetAttributeValue("totalspin", reelSet.TotalSpin);

                CreateWinPositionXElement(reelSetElement, reelSet.WinPositions);
                CreateTableWinXElement(reelSetElement, reelSet.TableWins);
                CreateBonusXElement(reelSetElement, reelSet.BonusElement);
                CreateBonusPositionXElement(reelSetElement, reelSet.BonusPositions);

                childElement.Add(reelSetElement);
            });

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

        private void ReadBonusPositionXElement(XElement element)
        {
        }

        private void ReadBonusXElement(XElement element)
        {
            var bonusElement = element.Element("bonus");

            if (bonusElement == null) return;

            this.BonusElement = new BonusElement
            {
                Id = bonusElement.AttributeValue("id").ToInt(),
                Value = bonusElement.Value,
            };

            if (bonusElement.AttributeValue("count") != null)
                this.BonusElement.Count = bonusElement.AttributeValue("count").ToInt();
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

            this.sbx = Activator.CreateInstance<SpinBetX>();
            this.sbx.IsSideBet = element.AttributeValue("sidebet").ToDecimal() > 0;
        }

        private void ReadFreeRoundXElement(XElement element)
        {
            if (null == this.FreeRoundInfo) return;

            this.FreeRoundInfo.State = (FreeRoundInfo.FreeGameState)element.AttributeValue("state").ToInt();
            this.FreeRoundInfo.Round = element.AttributeValue("round").ToInt();
            this.FreeRoundInfo.Lines = element.AttributeValue("lines").ToInt();
            this.FreeRoundInfo.Bet = element.AttributeValue("bet").ToDecimal();
            this.FreeRoundInfo.Multiplier = element.AttributeValue("mul").ToInt();
        }

        private void ReadTableWinXElement(XElement element)
        {
        }

        private void ReadBonusWheelsXElement(XElement element)
        {
            var bonusWheelElement = element.Element("bonuswheels");

            if (bonusWheelElement == null) return;

            var width = bonusWheelElement.AttributeValue("width").ToInt();
            var height = bonusWheelElement.AttributeValue("height").ToInt();
            var value = bonusWheelElement.AttributeValue("val").Split(',');
            var mode = bonusWheelElement.AttributeValue("mode").ToInt();

            this.BonusWheel = new Wheel(width, height);

            WheelType wheelType;
            if (Enum.TryParse(bonusWheelElement.AttributeValue("type"), true, out wheelType))
                this.BonusWheel.Type = wheelType;

            this.BonusWheel.Mode = mode;

            for (var i = 0; i < width; i++)
            {
                var wheel = new List<int>();
                for (var j = 0; j < height; j++)
                {
                    if (j > height - 1) continue;
                    wheel.Add(value[(i * height) + j].ToInt());
                }
                this.BonusWheel[i] = wheel;
            }
        }

        private void ReadWheelsXElement(XElement element)
        {
            var wheelElement = element.Element("wheels");

            if (wheelElement == null) return;

            var width = wheelElement.AttributeValue("width").ToInt();
            var height = wheelElement.AttributeValue("height").ToInt();
            var rows = wheelElement.Attribute("rows") != null ? wheelElement.AttributeValue("rows").Split(',').Select(int.Parse).ToList() : new List<int>();
            var value = wheelElement.AttributeValue("val");
            var postvalue = wheelElement.AttributeValue("postval");
            var reelSets = !string.IsNullOrEmpty(wheelElement.AttributeValue("reelsets"))
                    ? wheelElement.AttributeValue("reelsets").Split(',').Select(int.Parse).ToList()
                    : new List<int>();

            this.Wheel = rows.Any() ? new Wheel(rows, value) : new Wheel(width, height, value);

            if (!string.IsNullOrEmpty(postvalue))
            {
                this.PostWheel = rows.Any() ? new Wheel(rows, postvalue) : new Wheel(width, height, postvalue);
            }

            WheelType wheelType;
            if (Enum.TryParse(wheelElement.AttributeValue("type"), true, out wheelType))
            {
                this.Wheel.Type = wheelType;
            }

            if (reelSets.Any())
                ReadReelSetsXElement(wheelElement);
        }

        private void ReadWinPositionXElement(XElement element, List<WinPosition> winPositionList)
        {
            var winposition = element.Element("winposition");

            if (winposition == null)
            {
                return;
            }

            foreach (var item in winposition.Elements())
            {
                if (item.Attribute("isexpanded") != null)
                {
                    winPositionList.Add(new WinPositionExpanding
                    {
                        Line = item.AttributeValue("line").ToInt(),
                        Win = item.AttributeValue("win").ToDecimal(),
                        Multiplier = item.AttributeValue("mul").ToInt(),
                        IsExpanded = true,
                        RowPositions = item.Value.Split(',').Select(int.Parse).ToList()
                    });
                }
                else
                {
                    winPositionList.Add(
                    new WinPosition
                    {
                        Line = item.AttributeValue("line").ToInt(),
                        Win = item.AttributeValue("win").ToDecimal(),
                        Multiplier = item.AttributeValue("mul").ToInt(),
                        RowPositions = item.Value.Split(',').Select(int.Parse).ToList()
                    });
                }
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
                Jackpot = winElement.AttributeValue("jackpot").ToDecimal(),
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

            if (this.RespunReel.HasValue)
            {
                writer.WriteAttributeString("reel", (this.RespunReel.Value + 1).ToString());
                writer.WriteAttributeString("reelbet", this.Bet.ToString("F"));
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
            writer.WriteAttributeString("freeround", this.SpinBet == null || this.SpinBet.UserGameKey.IsFreeGame == false ? "0" : "1");
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
            try
            {
                if (!string.IsNullOrEmpty(this.Wheel.ReelStripsId) && this.GameIdXml == (int)GameId.GeniesLuck)
                {
                    string anticipation = "";
                    if (this.Wheel.ReelStripsId == "RS" || this.Wheel.ReelStripsId == "MGB" || this.Wheel.ReelStripsId == "FSB")
                    {
                        anticipation = "1";
                    }
                    else if (this.Wheel.ReelStripsId == "FSC")
                    {
                        anticipation = "2";
                    }
                    else
                    {
                        anticipation = "0";
                    }

                    writer.WriteAttributeString("r", anticipation);
                }
            }
            catch
            {
                // hard code issue if (!string.IsNullOrEmpty(this.Wheel.ReelStripsId) && this.GameIdXml == (int)GameId.GeniesLuck)
            }

            writer.WriteAttributeString("ts", Convert.ToString(this.DateTimeUtc.ToUnixTimeStamp()));
        }

        private void CreateDiceXElement(XElement element)
        {
            if (DiceInfo == null) return;

            var diceInfo = new XElement("dice",
                new XAttribute("side", string.Join(",", DiceInfo.Dices.Select(x => x.Side))));

            element.Add(diceInfo);
        }

        private void ReadDiceXElement(XElement element)
        {
            if (DiceInfo == null)
            {
                DiceInfo = new DiceInfo();
            };

            var dice = element.Element("dice");

            if (dice == null)
            {
                return;
            }
            var sides = dice.AttributeValue("side").Split(',').Select(x => Convert.ToInt32(x)).ToList();
            foreach (var side in sides)
            {
                DiceInfo.Dices.Add(new Dice { Side = side });
            }
        }

        private void ReadReelSetsXElement(XElement element)
        {
            var reelSets = element.Element("reelsets");
            if (reelSets == null)
                return;

            this.Wheel.ReelSets = new List<ReelSet>();

            foreach (var item in reelSets.Elements())
            {
                var reelSetId = item.AttributeValue("reelset").ToInt();
                var width = item.AttributeValue("width").ToInt();
                var height = item.AttributeValue("height").ToInt();
                var value = item.AttributeValue("val").Split(',').Select(int.Parse).ToList();

                var reelSet = new ReelSet(width, height) { Id = reelSetId };

                for (var i = 0; i < reelSet.Width; i++)
                {
                    reelSet[i] = value.GetRange(i * reelSet.Height, reelSet.Height);
                }

                ReadWinPositionXElement(item, reelSet.WinPositions);

                this.Wheel.ReelSets.Add(reelSet);
            }
        }
    }
}