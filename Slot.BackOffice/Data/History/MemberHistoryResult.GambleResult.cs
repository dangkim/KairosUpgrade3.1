using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Slot.BackOffice.Data.History
{
    public partial class MemberHistoryResult
    {
        private void DisplayGambleHistory(GameHistory history)
        {
            var xmlHelper = new XmlHelper();
            BonusXml = xmlHelper.Deserialize<BonusXml>(history.HistoryXml);
            HistoryType = HistoryType.Gamble;
            History = new List<History>();

            if (BonusXml.Data.Element("history") != null)
            {
                var steps = BonusXml.Data.Element("history").Elements("step").ToArray();
                if (steps?.Length > 0)
                {
                    foreach (var step in steps)
                    {
                        History.Add(new History
                        {
                            selected = step.Attribute("selected").Value == "1" ? "Double Half" : "Double",
                            bet = step.Attribute("bet").Value,
                            value = step.Attribute("value").Value,
                            dcard = step.Attribute("dcard").Value,
                            pcard = step.Attribute("pcard").Value
                        });
                    }
                }
            }
        }
    }
}
