using Newtonsoft.Json;
using Slot.Model;
using System;

namespace Slot.Core.Modules.Infrastructure.Models
{
    [Serializable]
    public abstract class SpinResult : GameResult
    {
        protected SpinResult() : this(true, true) { }

        protected SpinResult(bool isReport, bool isHistory)
        {
            IsHistory = isHistory;
            IsReport = isReport;
        }

        public virtual bool HasBonus { get; protected set; }

        public virtual string Type => "s";

        [JsonIgnore]
        public override GameResultType GameResultType => GameResultType.SpinResult;

        [JsonIgnore]
        public override XmlType XmlType => XmlType.None;
    }
}