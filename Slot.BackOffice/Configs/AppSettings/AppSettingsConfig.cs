using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.BackOffice.Configs.AppSettings
{
    public class AppSettingsConfig
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public AzureMetrics AzureMetrics { get; set; }

        public List<GameService> GameServices { get; set; }

        public HealthCheckServices HealthCheckServices { get; set; }
    }
}
