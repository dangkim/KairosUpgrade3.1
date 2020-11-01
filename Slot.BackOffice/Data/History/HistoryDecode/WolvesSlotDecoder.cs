using Slot.BackOffice.Models.Xml;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GameId = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.History.HistoryDecode
{
    [HistoryInfo(GameId.WolvesSlot)]
    public class WolvesSlotDecoder : IGameHistory
    {
        private readonly PaylineRepository paylineRepository;
        private readonly XmlHelper _xmlHelper;
        private const int Width = 5;
        private const int Height = 3;

        public WolvesSlotDecoder(PaylineRepository paylineRepository)
        {
            this.paylineRepository = paylineRepository;
            _xmlHelper = new XmlHelper();
        }

        public string ViewNavigation
        {
            get { return "HistorySpinResult"; }
            set { }
        }

        public void AmendSpinHistory(MemberHistoryResult model, GameHistory history)
        {
            if (history.GameResultType == GameResultType.SpinResult)
                MainGameDecode(model, history.HistoryXml);
            else FreeGameDecode(model, history.HistoryXml);
        }

        protected void MainGameDecode(MemberHistoryResult model, string historyXml)
        {
            DecodeWheel(model, historyXml);
        }

        protected void FreeGameDecode(MemberHistoryResult model, string historyXml)
        {
            var bonusXml = _xmlHelper.Deserialize<BonusXml>(historyXml);
            var spinXml = bonusXml.Data.Element("spin").ToString();
            var multiplier = XElement.Parse(historyXml).Attribute("mp");
            model.BonusXml = bonusXml;
            DecodeWheel(model, spinXml, null != multiplier ? multiplier.Value : "1");
        }

        private static List<byte[,]> GetWinTable(IEnumerable<WinPosition> winPositions)
        {
            var winTable = new List<byte[,]>();

            foreach (var wp in winPositions)
            {
                var lineTable = new byte[Width, Height];
                for (var i = 0; i < wp.RowPositions.Count; i++)
                {
                    if (wp.RowPositions[i] > 0)
                        lineTable[i, wp.RowPositions[i] - 1] = (byte)PaylinePos.Hit;
                }
                winTable.Add(lineTable);
            }

            return winTable;
        }

        private static WheelViewModel CreateWheelViewModel(Wheel wheel, bool isSmashingWild)
        {
            var reelStackWilds = new[] { 1, 2, 3 };
            var wheelVm = new WheelViewModel
            {
                reels = new List<List<Symbols>>()
            };
            for (var c = 0; c < wheel.Reels.Count; c++)
            {
                wheelVm.reels.Add(new List<Symbols>());
                var isBreak = false;
                for (var r = 0; r < wheel.Reels[c].Count; r++)
                {
                    Symbols symbol = null;
                    if (isSmashingWild && wheel.Reels[c][r] == 10)
                    {
                        symbol = new Symbols { symbol = 11, height = 1, width = 1 };
                        isBreak = true;
                    }
                    else
                        symbol = new Symbols { symbol = wheel.Reels[c][r], height = 1, width = 1 };

                    wheelVm.reels[c].Add(symbol);
                    if (isBreak)
                        break;
                }

                for (var r = wheel.Rows[c]; r < wheel.Height; r++)
                {
                    var symbols = new Symbols(true) { symbol = -1, height = 1 };
                    wheelVm.reels[c].Add(symbols);
                }
            }

            return wheelVm;
        }

        private void DecodeWheel(MemberHistoryResult model, string historyXml, string multiplier = "1")
        {
            var regex = new Regex(@"<wheels[^>]+>");
            var wheelPattern = new int[] { 3, 3, 3, 3, 3 };
            var xml = _xmlHelper.Deserialize<SpinXml>(historyXml);
            var match = regex.Match(historyXml);
            var element = XElement.Parse(match.Value);
            var value = element.AttributeValue("val");
            var whelType = element.AttributeValue("type");
            xml.Wheel = new Wheel(wheelPattern.ToList(), value);
            xml.SpinBet.Multiplier = int.Parse(multiplier);
            model.SpinXml = xml;
            model.Wheel = CreateWheelViewModel(xml.Wheel, whelType == "guaranteed");
            model.WinTable = GetWinTable(xml.WinPositions);
        }
    }
}
