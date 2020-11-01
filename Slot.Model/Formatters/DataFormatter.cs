using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Slot.Model.Formatters
{
    /// <summary>
    /// Formats the data when json is serialized.
    /// </summary>
    public class DataFormatter : JsonConverter
    {
        /// <summary>
        /// Gets or sets the display format for the field value.
        /// </summary>
        public string DataFormatString { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="DataFormatter"/> class.
        /// </summary>
        /// <param name="dataFormatString">Display format for the field value.</param>
        public DataFormatter(string dataFormatString)
        {
            DataFormatString = dataFormatString;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                var dateValue = JsonConvert.SerializeObject(value, Formatting.None, new IsoDateTimeConverter()
                {
                    DateTimeFormat = string.IsNullOrWhiteSpace(DataFormatString) ? "yyyy-MM-dd hh:mm:ss tt" : DataFormatString
                });

                writer.WriteRawValue(dateValue);
            }
            else
            {
                if(!string.IsNullOrWhiteSpace(DataFormatString))
                {
                    var formattedValue = string.Format(CultureInfo.InvariantCulture, DataFormatString, value);
                    writer.WriteValue(formattedValue);
                }
                else
                {
                    writer.WriteRawValue(Convert.ToString(value));
                }
                
            }
        }
    }
}
