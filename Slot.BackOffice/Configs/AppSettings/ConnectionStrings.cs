using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.BackOffice.Configs.AppSettings
{
    public class ConnectionStrings
    {
        public string WritableDatabase { get; set; }

        public string ReadOnlyDatabase { get; set; }

        public string Redis { get; set; }
    }
}
