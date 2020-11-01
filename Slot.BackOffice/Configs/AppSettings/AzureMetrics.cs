using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.BackOffice.Configs.AppSettings
{
    public class AzureMetrics
    {
        public string Url { get; set; }

        public string AppId { get; set; }

        public string Key { get; set; }

        public string NominatimUrl { get; set; }

        public decimal Multiplier { get; set; }

        public string RequestUrl { get => $"{Url}/v1/apps/{AppId}/query?query=";  }
}
}
