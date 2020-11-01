using Newtonsoft.Json;
using Slot.Model.Formatters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Data.Views.Tournament
{
    public class GlobalTournament
    {
        public long No { get; set; }

        public int Id { get; set; }

        public string Operators { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Owner { get; set; }

        public string Status { get; set; }

        public string Platforms { get; set; }

        [JsonConverter(typeof(DataFormatter), Formats.Currency)]
        public int Total { get; set; }
    }
}
