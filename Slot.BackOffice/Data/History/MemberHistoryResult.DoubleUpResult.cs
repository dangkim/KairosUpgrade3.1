using Newtonsoft.Json;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Utility;
using System.Collections.Generic;
using System.Linq;
using GameIdEnum = Slot.BackOffice.Data.Enums.GameId;

namespace Slot.BackOffice.Data.History
{
    public partial class MemberHistoryResult
    {
        [JsonIgnore]
        public readonly Dictionary<int, Dictionary<string, string>> DoubleUpDescription = new Dictionary<int, Dictionary<string, string>>
        {
            {(int) GameIdEnum.DeepBlue, new Dictionary<string, string> {{"0", "Collect"}, {"1", "Escape"}, {"2", "Capture"}}},
            {(int) GameIdEnum.GoldenEggs,new Dictionary<string, string> {{"0", "Collect"}, {"1", "Normal Egg"}, {"2", "Golden Egg"}}},
            {(int) GameIdEnum.LittleMonsters, new Dictionary<string, string> {{"0", "Collect"}, {"1", "Odd"}, {"2", "Even"}}},
            {(int) GameIdEnum.Zeus, new Dictionary<string, string> {{"0", "Collect"}, {"1", "Head"}, {"2", "Tail"}}},
            {(int) GameIdEnum.CasinoRoyale, new Dictionary<string, string> {{"0", "Collect"}, {"1", "King"}, {"2", "Queen"}}},
            {(int) GameIdEnum.CasinoRoyalePro,new Dictionary<string, string> {{"0", "Collect"}, {"1", "King"}, {"2", "Queen"}}},
            {(int) GameIdEnum.RomanEmpire, new Dictionary<string, string> {{"0", "Collect"}, {"1", "Head"}, {"2", "Tail"}}},
            {(int) GameIdEnum.RomanEmpirePro,new Dictionary<string, string> {{"0", "Collect"}, {"1", "Head"}, {"2", "Tail"}}},
            {(int) GameIdEnum.GodOfGamblers, new Dictionary<string, string> {{"0", "Collect"}, {"1", "Small"}, {"2", "Big"}}},
            {(int) GameIdEnum.Boxing,new Dictionary<string, string> {{"0", "Collect"}, {"1", "DoubleHalf"}, {"2", "Double"}}},
            {(int) GameIdEnum.BoxingPro,new Dictionary<string, string> {{"0", "Collect"}, {"1", "DoubleHalf"}, {"2", "Double"}}},
            {(int) GameIdEnum.UnderwaterWorld,new Dictionary<string, string> {{"0", "Collect"}, {"1", "DoubleHalf"}, {"2", "Double"}}},
            {(int) GameIdEnum.UnderwaterWorldPro,new Dictionary<string, string> {{"0", "Collect"}, {"1", "DoubleHalf"}, {"2", "Double"}}},
            {(int) GameIdEnum.RedChamber, new Dictionary<string, string> {{"0", "Collect"}, {"1", "Red"}, {"2", "White"}}},
            {(int) GameIdEnum.ForbiddenChamber,new Dictionary<string, string> {{"0", "Collect"}, {"1", "Left"}, {"2", "Right"}}},
            {(int) GameIdEnum.TrickOrTreat,new Dictionary<string, string> {{"0", "Collect"}, {"1", "Right"}, {"2", "Left"}}}
        };

        private void DisplayDoubleUpHistory(GameHistory history)
        {
            var xmlHelper = new XmlHelper();
            BonusXml = xmlHelper.Deserialize<BonusXml>(history.HistoryXml);
            HistoryType = HistoryType.DoubleUp;
            History = new List<History>();

            if (BonusXml.Data.Element("history") != null)
            {
                var steps = BonusXml.Data.Element("history").Elements("step").ToArray();
                if (steps != null)
                {
                    foreach (var step in steps)
                    {
                        History.Add(new History
                        {
                            selected = DoubleUpDescription[history.Game.Id][step.Attribute("selected").Value],
                            result = DoubleUpDescription[history.Game.Id][step.Attribute("result").Value],
                            value = step.Attribute("value").Value
                        });
                    }
                }
            }
        }
    }
}
