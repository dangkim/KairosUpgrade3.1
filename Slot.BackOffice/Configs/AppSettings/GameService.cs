using System;

namespace Slot.BackOffice.Configs.AppSettings
{
    /// <summary>
    /// Represents the Game Service found in the gameservices.json file.
    /// </summary>
    public class GameService
    {
        public string Name { get; set; }

        public string Url { get; set; }
    }

    public class HealthCheckServices
    {
        public string Name { get; set; }

        public string Url { get; set; }
    }
}
