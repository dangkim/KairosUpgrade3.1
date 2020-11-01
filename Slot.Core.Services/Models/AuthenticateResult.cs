using Newtonsoft.Json;

namespace Slot.Core.Services.Models
{
    public struct AuthenticateResult
    {
        [JsonIgnore]
        public string MemberId { get; set; }
        public string MemberName { get; set; }
        public bool IsTestAccount { get; set; }
        [JsonIgnore]
        public string ExtraInfo { get; set; }
        public string Currency { get; set; }
        public string SessionKey { get; set; }
    }
}
