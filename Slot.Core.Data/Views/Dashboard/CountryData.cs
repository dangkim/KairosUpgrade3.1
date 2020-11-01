using System;
using System.Collections.Generic;
using System.Text;

namespace Slot.Core.Data.Views.Dashboard
{
    /// <summary>
    /// Represents <see cref="NominatimData"/> along with the country name from azure data.
    /// </summary>
    public class CountryData : NominatimData
    {
        public CountryData() { }

        public CountryData(string countryName, NominatimData nomData)
        {
            CountryName = countryName;
            PlaceId = nomData.PlaceId;
            License = nomData.License;
            OsmType = nomData.OsmType;
            OsmId = nomData.OsmId;
            BoundingBox = nomData.BoundingBox;
            Latitude = nomData.Latitude;
            Longitude = nomData.Longitude;
            DisplayName = nomData.DisplayName;
            Class = nomData.Class;
            Type = nomData.Type;
            Importance = nomData.Importance;
            Icon = nomData.Icon;
        }

        public string CountryName { get; set; }
    }
}
