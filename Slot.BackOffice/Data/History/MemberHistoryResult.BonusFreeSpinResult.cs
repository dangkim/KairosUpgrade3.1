using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Slot.BackOffice.Data.History
{
    public partial class MemberHistoryResult
    {
        private void DisplayBonusFreeSpinHistory(GameHistory history)
        {
            var xmlHelper = new XmlHelper();
            BonusXml = xmlHelper.Deserialize<BonusXml>(history.HistoryXml);
            HistoryType = HistoryType.BFS;
            History = new List<History>();

            if (BonusXml.Data.Element("history") != null)
            {
                var steps = BonusXml.Data.Element("history").Elements("step").ToArray();
                if (steps != null)
                {
                    foreach (var step in steps)
                    {
                        string rname = "Multiplier";

                        switch (step.Attribute("typew").Value)
                        {
                            case "FS": rname = "Free Spin"; break;
                            case "mode": rname = "Mode"; break;
                            case "wmul": rname = "Wild Multiplier"; break;
                        }

                        History.Add(new History
                        {
                            result = rname,
                            value = step.Attribute("value").Value
                        });
                    }
                }
            }
        }
    }
}
