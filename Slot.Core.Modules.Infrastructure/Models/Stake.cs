using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Slot.Core.Modules.Infrastructure.Models
{
    [Serializable]
    public class Stake
    {
        [JsonIgnore]
        public int ReelStripId { get; set; }    
        
        [JsonProperty("id")]
        public int ClientId { get; }

        [JsonIgnore]
        public  Guid Guid { get; }

        public string Value => Guid.ToString();

        public int Count { get; set; }

        public List<StakePosition> TriggerPositions { get; set; }

        public Stake(Guid guid, int clientId)
        {
            Guid = guid;
            ClientId = clientId;
        }
    }
}
