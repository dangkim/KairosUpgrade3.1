using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Slot.BackOffice.Configs.AppSettings;
using Slot.BackOffice.Data.Queries.AzureMetrics;
using Slot.BackOffice.HttpClients;
using Slot.Core.Data.Views.Dashboard;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Slot.BackOffice.Data.Repositories
{
    /// <summary>
    /// Provides data and functionality related functions for the dashboard.
    /// </summary>
    public class DashboardRepository
    {
        private static readonly Dictionary<string, CountryData> countryData = new Dictionary<string, CountryData>();
        private static readonly Regex periodDateParserRegex = new Regex(@"^(\d+)(d|h|m)$", RegexOptions.Compiled);
        private readonly NominatimClient nominatimClient;
        private readonly AppSettingsConfig appSettingsConfig;
        private const string hourPeriodGrouping = "h";
        private const string dayPeriodGrouping = "d";
        private const string minutePeriodGrouping = "m";

        public DashboardRepository(NominatimClient nominatimClient, IOptions<AppSettingsConfig> appSettingsConfigOptions)
        {
            this.nominatimClient = nominatimClient;
            this.appSettingsConfig = appSettingsConfigOptions.Value;
        }

        /// <summary>
        /// Get timestamp filter based on the specified period.
        /// </summary>
        /// <param name="period">Period from the front-end.</param>
        /// <returns>Timestamp filter for the query to be sent to azure.</returns>
        public string GetTimestampFilter(string period)
        {
            var translatedPeriod = string.Empty;
            var parsedPeriod = ParsePeriodDate(period);

            switch (parsedPeriod.Format)
            {
                case "h":
                    {
                        translatedPeriod = $"timestamp >= ago({period})";
                        break;
                    }
                case "d":
                    {
                        var startDate = DateTime.UtcNow;
                        var endDate = DateTime.UtcNow;

                        if (parsedPeriod.Value > 1)
                        {
                            startDate = startDate.AddDays(-Math.Abs(parsedPeriod.Value));
                        }

                        translatedPeriod = $"timestamp between(startofday(datetime({startDate.ToString("MM/dd/yyyy")})) .. endofday(datetime({endDate.Date.ToString("MM/dd/yyyy")})))";
                        break;
                    }
                default:
                    throw new ArgumentException("Invalid period.");
            }

            return translatedPeriod;
        }

        /// <summary>
        /// Get query filter for azure based on the selected values.
        /// </summary>
        /// <param name="query">Query values from the front-end.</param>
        /// <returns>Filter string to be added to the azure query.</returns>
        public string GetQueryFilters(AzureMetricsQuery query)
        {
            string filter = string.Empty;

            if (query.Region != null && string.Compare(query.Region, "All", true, CultureInfo.InvariantCulture) != 0)
            {
                filter += $" and client_CountryOrRegion =~ \"{query.Region}\"";
            }

            if (query.Operator != null && string.Compare(query.Operator, "All", true, CultureInfo.InvariantCulture) != 0)
            {
                filter += $" and customDimensions[\"Operator ID\"] =~ \"{query.Operator}\"";
            }

            if (query.Currency != null && string.Compare(query.Currency, "All", true, CultureInfo.InvariantCulture) != 0)
            {
                filter += $" and customDimensions[\"Currency\"] =~ \"{query.Currency}\"";
            }

            return filter;
        }

        /// <summary>
        /// Retrieves the azure data summary multiplier in the application configuration.
        /// </summary>
        /// <returns>The azure data summary multiplier.</returns>
        public decimal GetDataSummaryMultiplier() => appSettingsConfig.AzureMetrics.Multiplier;

        /// <summary>
        /// Utilizes Nominatim API to get the boundaries of the country then caches the result.
        /// Falls back to Latitude and Longitude 0 when api is not available for maps.
        /// </summary>
        /// <param name="regions">Regions to be queried.</param>
        /// <returns>Data in <see cref="List{CountryData}}"/> format.</returns>
        [Obsolete]
        public async Task<List<CountryData>> GetCountriesData(string[] regions)
        {
            var localCountryDataCollection = new List<CountryData>();

            foreach (var region in regions)
            {
                if (!countryData.TryGetValue(region, out var data))
                {
                    try
                    {
                        var content = await nominatimClient.GetCountryData(region);
                        var nominatimData = JsonConvert.DeserializeObject<List<NominatimData>>(content);

                        if (nominatimData?.Count > 0)
                        {
                            data = new CountryData(region, nominatimData[0]);
                            countryData.Add(region, data);
                            localCountryDataCollection.Add(data);
                            /// Nominatim requires request intervals of 1 second
                            /// Refer to https://operations.osmfoundation.org/policies/nominatim/#requirements
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            AddDefaultCountryData(region, localCountryDataCollection);
                        }
                    }
                    catch
                    {
                        AddDefaultCountryData(region, localCountryDataCollection);
                    }
                }
                else
                {
                    localCountryDataCollection.Add(data);
                }
            }

            return localCountryDataCollection;
        }

        /// <summary>
        /// Retrieve the period summary grouping to be used in azure api calls.
        /// </summary>
        /// <param name="period">Period from the front-end.</param>
        /// <returns>Period summary grouping in string format.</returns>
        public string GetPeriodSummaryGrouping(string period)
        {
            var grouping = $"1{dayPeriodGrouping}";

            if (!string.IsNullOrWhiteSpace(period))
            {
                var parseResult = ParsePeriodDate(period);

                if (parseResult.IsValid)
                {
                    var groupingSuffix = GetGroupingSuffix(parseResult.Format, parseResult.Value);
                    var periodGapAmount = GetGroupingGapAmount(parseResult.Format, parseResult.Value);

                    grouping = $"{periodGapAmount}{groupingSuffix}";
                }
            }

            return grouping;
        }

        /// <summary>
        /// Parse the period date from the front-end dashboard.
        /// </summary>
        /// <param name="input">Period date from the front-end.</param>
        /// <returns>Tuple that specifies if the input is valid, the value of the period, and the specified format.</returns>
        public (bool IsValid, int Value, string Format) ParsePeriodDate(string input)
        {
            var match = periodDateParserRegex.Match(input);
            var returnTuple = (false, 0, "");

            if (match.Success)
            {
                var value = int.Parse(match.Groups[1].Value);
                var format = match.Groups[2].Value;

                var isFormatValid = format == dayPeriodGrouping || format == hourPeriodGrouping || format == minutePeriodGrouping;
                var isValidValue = value >= 0;

                if (isFormatValid && isValidValue)
                {
                    returnTuple = (true, value, format);
                }
            }

            return returnTuple;
        }

        /// <summary>
        /// Determines the gap amount for dashboard charts based on the date format and period selected.
        /// </summary>
        /// <param name="format">Front-end date format.</param>
        /// <param name="period">Front-end period value.</param>
        /// <returns>Gap amount in <see cref="double"/>.</returns>
        private double GetGroupingGapAmount(string format, int period)
        {
            var groupingPercentage = 0.25;
            double periodGapAmount = 1d;

            switch(format)
            {
                case minutePeriodGrouping:
                {
                    periodGapAmount = period * groupingPercentage;
                    break;
                }
                case hourPeriodGrouping:
                {
                    periodGapAmount = period * 60 * groupingPercentage;
                    break;
                }
                case dayPeriodGrouping:
                {
                    periodGapAmount = 1;
                    break;
                }
            }

            /// Ensure there is gap within the graphs by forcing 1.
            return Math.Max(Math.Round(periodGapAmount), 1);
        }

        /// <summary>
        /// Retrieve grouping suffix for grouping azure data.
        /// </summary>
        /// <param name="format">Front-end date format.</param>
        /// <param name="period">Front-end period value.</param>
        /// <returns>Valid azure metrics date grouping suffix.</returns>
        private string GetGroupingSuffix(string format, int period)
        {
            var grouping = dayPeriodGrouping;

            switch (format)
            {
                case minutePeriodGrouping:
                case hourPeriodGrouping:
                    grouping = minutePeriodGrouping;
                    break;
                case dayPeriodGrouping:
                    if (period == 1)
                        grouping = hourPeriodGrouping;
                    else
                        grouping = dayPeriodGrouping;
                    break;
            }

            return grouping;
        }

        private void AddDefaultCountryData(string region, List<CountryData> dataCollection)
        {
            dataCollection.Add(new CountryData(region, new NominatimData
            {
                Longitude = 0,
                Latitude = 0
            }));
        }
    }
}
