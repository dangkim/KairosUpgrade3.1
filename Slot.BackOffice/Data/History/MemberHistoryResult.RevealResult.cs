using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Slot.BackOffice.Data.History
{
    public partial class MemberHistoryResult
    {
        private void DisplayRevealBonusHistory(GameHistory history)
        {
            var xmlHelper = new XmlHelper();
            BonusXml = xmlHelper.Deserialize<BonusXml>(history.HistoryXml);
            HistoryType = HistoryType.Reveal;
            History = new List<History>();

            var steps = BonusXml.Data.Element("history")?.Elements("step")?.ToArray();
            if (steps == null)
                return;

            var bonusWheels = BonusXml.Data.Element("wheels")?.Elements("item")?.ToArray();
            var selectedMultiplier = string.Empty;
            if (bonusWheels != null && bonusWheels.Length == 1)
            {
                var mulElement = bonusWheels.FirstOrDefault();
                if (mulElement != null && mulElement.Attribute("mul") != null)
                {
                    selectedMultiplier = mulElement.Attribute("mul").Value;
                }
            }

            foreach (var step in steps)
            {
                var itemType = step.Attribute("typew")?.Value ?? string.Empty;
                var itemValue = step.Attribute("value")?.Value ?? string.Empty;
                var itemSelected = step.Attribute("selected")?.Value ?? string.Empty;
                var multiplier = step.Attribute("mul")?.Value ?? selectedMultiplier;

                if (!IsBonusHistoryItemForDisplay(itemType, itemValue))
                    continue;

                var typeDesc = string.Empty;
                switch (itemType)
                {
                    case "FS":
                        typeDesc = $"{itemValue} Free Spin";
                        break;
                    case "xwild":
                        typeDesc = $"{itemValue} Extra Wild";
                        break;
                    case "wmul":
                        typeDesc = $"x{itemValue} Wild Multiplier";
                        break;
                    case "dwmul":
                        typeDesc = $"x{itemValue} Default Wild Multiplier";
                        break;
                    case "xpick":
                        typeDesc = $"{itemValue} Extra Pick";
                        break;
                    case "FSPick":
                        typeDesc = $"{itemValue} Free Spin";
                        break;
                }

                History.Add(new History
                {
                    selected = itemSelected,
                    mul = multiplier,
                    value = !string.IsNullOrEmpty(typeDesc) ? typeDesc : itemValue
                });
            }
        }

        private bool IsBonusHistoryItemForDisplay(string typew, string value)
        {
            if (string.IsNullOrEmpty(typew) || typew == "startfs")
                return false;
            if ((typew == "FS" || typew == "wmul" || typew == "xwild") && value.ToInt() <= 0)
                return false;

            return true;
        }
    }
}
