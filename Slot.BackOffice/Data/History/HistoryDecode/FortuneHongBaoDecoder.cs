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
    [HistoryInfo(GameId.FortuneHongBao)]
    public class FortuneHongBaoDecoder : IGameHistory
    {
        private readonly PaylineRepository paylineRepository;
        private readonly XmlHelper _xmlHelper;
        public static List<int[]> Pattern;
        private const int Width = 3;
        private const int Height = 3;

        public FortuneHongBaoDecoder(PaylineRepository paylineRepository)
        {
            this.paylineRepository = paylineRepository;
            _xmlHelper = new XmlHelper();
            Pattern = new List<int[]>
            {
                new []{3,4,5},
                new []{0,1,2},
                new []{6,7,8},
                new []{0,3,6},
                new []{1,4,7},
                new []{2,5,8},
                new []{0,4,8},
                new []{6,4,2}
            };
        }
        private static List<byte[,]> GetWinTable(IEnumerable<WinPosition> winPositions)
        {
            var winTable = new List<byte[,]>();

            foreach (var wp in winPositions)
            {
                var lineTable = new byte[Width, Height];
                var payLine = wp.Line == 0 ? new int[3] : Pattern[wp.Line - 1];
                for (var i = 0; i < wp.RowPositions.Count; i++)
                {
                    var position = wp.RowPositions[i] - 1;

                    if (position >= 0)
                    {
                        lineTable[position % 3, position / 3] = (byte)PaylinePos.Hit;
                    }

                    else if (wp.Line > 0)
                    {
                        lineTable[payLine[i] % 3, payLine[i] / 3] = (byte)PaylinePos.NotHit;
                    }
                }
                winTable.Add(lineTable);
            }

            return winTable;
        }
        private static WheelViewModel CreateWheelViewModel(Wheel wheel)
        {
            var wheelVm = new WheelViewModel
            {
                reels = new List<List<Symbols>>()
            };
            for (var c = 0; c < wheel.Reels.Count; c++)
            {
                wheelVm.reels.Add(new List<Symbols>());
                for (var r = 0; r < wheel.Reels[c].Count; r++)
                {
                    var symbols = new Symbols { symbol = wheel.Reels[c][r], height = 1, width = 1 };
                    wheelVm.reels[c].Add(symbols);
                }
            }

            return wheelVm;
        }


        public string ViewNavigation
        {
            get { return "HistorySpinResult"; }
            set { }
        }

        private void EncodeWheel(MemberHistoryResult model, string historyXml)
        {
            var regex = new Regex(@"<wheels[^>]+>");
            var match = regex.Match(historyXml);
            var element = XElement.Parse(match.Value);
            var value = element.AttributeValue("val");
            if (string.IsNullOrEmpty(value))
                return;
            var wheelPattern = new int[] { 3, 3, 3 };
            var xml = _xmlHelper.Deserialize<SpinXml>(historyXml);
            xml.Wheel = new Wheel(wheelPattern.ToList(), value);
            model.SpinXml = xml;
            model.Wheel = CreateWheelViewModel(xml.Wheel);
            model.WinTable = GetWinTable(xml.WinPositions);
        }

        protected void MainGameDecode(MemberHistoryResult model, string historyXml)
        {
            EncodeWheel(model, historyXml);
        }

        protected void FreeGameDecode(MemberHistoryResult model, string historyXml)
        {
            var bonusXml = _xmlHelper.Deserialize<BonusXml>(historyXml);
            var spinXml = bonusXml.Data.Element("spin").ToString();
            model.BonusXml = bonusXml;
            EncodeWheel(model, spinXml);
        }

        public void AmendSpinHistory(MemberHistoryResult model, GameHistory history)
        {
            if (history.GameResultType == GameResultType.SpinResult)
                MainGameDecode(model, history.HistoryXml);
            else FreeGameDecode(model, history.HistoryXml);


        }
    }
}