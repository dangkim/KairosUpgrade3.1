using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.BackOffice.Configs.Authentication
{
    public class JwtInformation
    {
        public string Issuer { get; set; }

        public string Key { get; set; }

        public double Duration { get; set; }
    }
}
