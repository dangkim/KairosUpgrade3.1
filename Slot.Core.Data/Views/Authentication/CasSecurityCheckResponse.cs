using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Data.Views.Authentication
{
    public class CasSecurityCheckResponse
    {
        public string User { get; set; }

        [JsonProperty("Op")]
        public string Operator { get; set; }

        public string Code { get; set; }

        public bool IsSuccess { get; private set; }

        public string Message { get; private set; }

        public static CasSecurityCheckResponse Deserialize(string securityCheckResponseString)
        {
            var securityCheck = JsonConvert.DeserializeObject<CasSecurityCheckResponse>(securityCheckResponseString);

            if(securityCheck != null)
            {
                securityCheck.IsSuccess = securityCheck.Code.Contains("200");
                securityCheck.Message = securityCheckResponseString;
            }

            return securityCheck;
        }
    }
}
