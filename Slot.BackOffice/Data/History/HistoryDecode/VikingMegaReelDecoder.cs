using Slot.BackOffice.Models.Xml;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Utility;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GameId = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.History.HistoryDecode
{
    [HistoryInfo(GameId.VikingsMegaReels)]
    public class VikingMegaReelDecoder : IGameHistory
    {
        private readonly Payline _mainGamePayLine;
        private readonly Payline _freeGamePayLine;
        private readonly XmlHelper _xmlHelper;
        private const int Width = 6;
        private const int Height = 7;

        public PaylineRepository paylineRepository;
        public string ViewNavigation { get => "HistorySpinResult"; }

        public VikingMegaReelDecoder(PaylineRepository paylineRepository)
        {
            this.paylineRepository = paylineRepository;

            _mainGamePayLine = paylineRepository.Get("local345677_250");
            _freeGamePayLine = paylineRepository.Get("local776543_250");
            _xmlHelper = new XmlHelper();
        }

        private List<byte[,]> GetWinTable(Payline payLine, IEnumerable<WinPosition> winPositions, int[] wheelPattern)
        {
            var winTable = new List<byte[,]>();

            foreach (var wp in winPositions)
            {
                var lineTable = new byte[Width, Height];

                for (var i = 0; i < wheelPattern.Length; i++)
                {
                    for (var j = wheelPattern[i]; j < Height; j++)
                    {
                        lineTable[i, j] = (byte)PaylinePos.Empty;
                    }
                }

                if (wp.Line == 0)
                {
                    for (var i = 0; i < wp.RowPositions.Count; i++)
                    {
                        if (wp.RowPositions[i] > 0)
                            lineTable[i, wp.RowPositions[i] - 1] = (byte)PaylinePos.Hit;
                    }
                }
                else
                {
                    payLine.Lines[wp.Line].ForEach(r =>
                    {
                        lineTable[r.Reel, r.Position] = wp.RowPositions[r.Reel] > 0 ? (byte)PaylinePos.Hit : (byte)PaylinePos.NotHit;
                    });
                }

                winTable.Add(lineTable);
            }

            return winTable;
        }

        private WheelViewModel CreateWheelViewModel(Wheel wheel)
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

                for (var r = wheel.Rows[c]; r < wheel.Height; r++)
                {
                    var symbols = new Symbols(true) { symbol = -1, height = 1 };
                    wheelVm.reels[c].Add(symbols);
                }
            }

            return wheelVm;
        }

        protected void MainGameDecode(MemberHistoryResult model, string historyXml)
        {
            var regex = new Regex(@"<wheels[^>]+>");
            var xml = _xmlHelper.Deserialize<SpinXml>(historyXml);
            var match = regex.Match(historyXml);
            var element = XElement.Parse(match.Value);
            var value = element.AttributeValue("val");
            xml.Wheel = new Wheel(new List<int> { 3, 4, 5, 6, 7, 7 }, value);
            model.SpinXml = xml;
            model.Wheel = CreateWheelViewModel(xml.Wheel);
            model.PayLine = _mainGamePayLine.Lines;
            model.WinTable = GetWinTable(_freeGamePayLine, model.SpinXml.WinPositions, new[] { 3, 4, 5, 6, 7, 7 });
        }

        protected void FreeGameDecode(MemberHistoryResult model, string historyXml)
        {
            var regex = new Regex(@"<wheels[^>]+>");
            var bonusXml = _xmlHelper.Deserialize<BonusXml>(historyXml);
            var xml = _xmlHelper.Deserialize<SpinXml>(bonusXml.Data.Element("spin").ToString());
            var match = regex.Match(historyXml);
            var element = XElement.Parse(match.Value);
            var value = element.AttributeValue("val");
            xml.Wheel = new Wheel(new List<int> { 7, 7, 6, 5, 4, 3 }, value);
            model.BonusXml = bonusXml;
            model.SpinXml = xml;
            model.PayLine = _freeGamePayLine.Lines;
            model.Wheel = CreateWheelViewModel(xml.Wheel);
            model.WinTable = GetWinTable(_freeGamePayLine, model.SpinXml.WinPositions, new[] { 7, 7, 6, 5, 4, 3 });
        }

        public void AmendSpinHistory(MemberHistoryResult model, GameHistory history)
        {
            if (history.GameResultType == GameResultType.SpinResult)
                MainGameDecode(model, history.HistoryXml);
            else FreeGameDecode(model, history.HistoryXml);
        }
    }
}