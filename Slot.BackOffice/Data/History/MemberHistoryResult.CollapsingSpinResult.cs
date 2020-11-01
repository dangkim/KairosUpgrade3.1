using Slot.BackOffice.Models.Xml;
using Slot.Model.Slot.Xml;
using Slot.Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Slot.Model;
using GameId = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.History
{
    public partial class MemberHistoryResult
    {
        private void DisplayCollapseSpinHistory(XElement xml)
        {
            var xmlHelper = new XmlHelper();
            var multiplier = xml.Attribute("mp");
            var collapseXml = xml.Element("data").Element("spin");
            CollapseXml = xmlHelper.Deserialize<CollapseXml>(collapseXml.ToString());
            CollapseXml.Multiplier = null != multiplier ? Convert.ToInt32(multiplier.Value) : 1;
            PayLine = paylineRepository.Get((GameId)GameId).Lines;
            var fsReplacementSymbol = new Dictionary<int, int>();
            if (!FreeSpinReplacement.TryGetValue(GameId, out fsReplacementSymbol))
                fsReplacementSymbol = new Dictionary<int, int>();

            Wheel = new WheelViewModel
            {
                reels = new List<List<Symbols>>()
            };

            var wheel = CollapseXml.Wheel;

            for (var c = 0; c < wheel.Reels.Count; c++)
            {
                Wheel.reels.Add(new List<Symbols>());

                for (var r = 0; r < wheel.Reels[c].Count; r++)
                {
                    var sym = wheel.Reels[c][r];
                    var symbols = new Symbols { symbol = (fsReplacementSymbol.ContainsKey(sym) ? fsReplacementSymbol[sym] : sym), height = 1, width = 1 };
                    Wheel.reels[c].Add(symbols);
                }
            }

            WinTable = gamePayoutEngine.PayoutWays(PayLine, CollapseXml);
        }

        private void DisplayBonusCollapsingSpin(XElement historyXml)
        {
            var xmlHelper = new XmlHelper();
            var xml = xmlHelper.Deserialize<SpinXml>(historyXml.ToString());
            SpinXml = xml;

            Wheel = new WheelViewModel
            {
                reels = new List<List<Symbols>>()
            };

            var wheel = SpinXml.Wheel;

            for (var c = 0; c < wheel.Reels.Count; c++)
            {
                Wheel.reels.Add(new List<Symbols>());

                for (var r = 0; r < wheel.Reels[c].Count; r++)
                {
                    var symbols = new Symbols { symbol = wheel.Reels[c][r], height = 1, width = 1 };
                    Wheel.reels[c].Add(symbols);
                }

                if (!(wheel.Rows?.Any() ?? false))
                    continue;

                for (var r = wheel.Rows[c]; r < wheel.Height; r++)
                {
                    var symbols = new Symbols(true) { symbol = -1, height = 1 };
                    Wheel.reels[c].Add(symbols);
                }
            }

            PayLine = paylineRepository.Get((GameId)GameId).Lines;
            WinTable = GetWinTable((GameId)GameId, PayLine, xml);
        }
    }
}
