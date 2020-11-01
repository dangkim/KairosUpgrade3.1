using Newtonsoft.Json;

namespace Slot.Core.Data.Views.Dashboard
{
    /// <summary>
    /// Represents the return data of Nominatim API.
    /// </summary>
    public class NominatimData
    {
        [JsonProperty("place_id")]
        public int PlaceId { get; set; }

        [JsonProperty("licence")]
        public string License { get; set; }

        [JsonProperty("osm_type")]
        public string OsmType { get; set; }

        [JsonProperty("osm_id")]
        public string OsmId { get; set; }

        [JsonProperty("boundingbox")]
        public decimal[] BoundingBox { get; set; }

        [JsonProperty("lat")]
        public decimal Latitude { get; set; }

        [JsonProperty("lon")]
        public decimal Longitude { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("importance")]
        public decimal Importance { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}
