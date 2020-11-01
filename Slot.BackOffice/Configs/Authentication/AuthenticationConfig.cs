using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.BackOffice.Configs.Authentication
{
    public class AuthenticationConfig
    {
        public JwtInformation Jwt { get; set; }

        public string CasClientId { get; set; }

        public string CasClientPassword { get; set; }

        public string CasSecurityCheckUrl { get; set; }
    }
}
