using Newtonsoft.Json;
using Slot.Model.Entity;
using Slot.Model.Formatters;
using System;

namespace Slot.Core.Data.Views.Members
{
    public class Member
    {
        public int OperatorId { get; set; }

        public string OperatorTag { get; set; }

        public int MemberId { get; set; }

        public string MemberName { get; set; }

        public string ExternalId { get; set; }

        public int CurrencyId { get; set; }

        public string Currency { get; set; }

        public bool IsDemoAccount { get; set; }

        [JsonConverter(typeof(DataFormatter), Formats.DateTime)]
        public DateTime? LastLoginUtc { get; set; }

        [JsonConverter(typeof(DataFormatter), Formats.DateTime)]
        public DateTime CreatedOnUtc { get; set; }

        public Operator Operator { get; set; }
    }
}
