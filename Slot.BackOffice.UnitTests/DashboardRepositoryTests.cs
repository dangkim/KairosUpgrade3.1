using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Slot.BackOffice.Configs.AppSettings;
using Slot.BackOffice.Data.Repositories;
using Slot.BackOffice.HttpClients;
using System.Net.Http;

namespace Slot.BackOffice.UnitTests
{
    [TestFixture(TestName = "Dashboard Repository Tests")]
    public class DashboardRepositoryTests
    {
        private DashboardRepository repository;

        public void Setup()
        {
            var httpClient = new Mock<HttpClient>();
            var appSettingsConfig = new Mock<IOptions<AppSettingsConfig>>();
            appSettingsConfig.Setup(x => x.Value).Returns(new AppSettingsConfig() { AzureMetrics = new AzureMetrics() { } });

            var nominatimClient = new Mock<NominatimClient>(httpClient.Object, appSettingsConfig.Object);
            repository = new DashboardRepository(nominatimClient.Object, appSettingsConfig.Object);
        }

        [TestCase("140m", TestName = "[Determine Valid Format] Valid Minute Format", ExpectedResult = true)]
        [TestCase("36h", TestName = "[Determine Valid Format] Valid Hour Format", ExpectedResult = true)]
        [TestCase("30d", TestName = "[Determine Valid Format] Valid Day Format", ExpectedResult = true)]
        [TestCase("-68d", TestName = "[Determine Valid Format] Negative Value Daily Format", ExpectedResult = false)]
        [TestCase("30x", TestName = "[Determine Valid Format] Invalid Format", ExpectedResult = false)]
        [TestCase("asdf", TestName = "[Determine Valid Format] No Valid Value", ExpectedResult = false)]
        public bool ParseDateShouldDetermineValidFormat(string input)
        {
            Setup();
            var result = repository.ParsePeriodDate(input);

            return result.IsValid;
        }

        [TestCase("140m", TestName = "[Get Valid Grouping] Valid Minute Format", ExpectedResult = "35m")]
        [TestCase("36h", TestName = "[Get Valid Grouping] Valid Hour Format", ExpectedResult = "540m")]
        [TestCase("1d", TestName = "[Get Valid Grouping] Valid 1 Day Hourly Format", ExpectedResult = "1h")]
        [TestCase("3d", TestName = "[Get Valid Grouping] Valid 3 Days Format", ExpectedResult = "1d")]
        [TestCase("7d", TestName = "[Get Valid Grouping] Valid 7 Days Format", ExpectedResult = "1d")]
        [TestCase("8d", TestName = "[Get Valid Grouping] Valid 8 Days Format", ExpectedResult = "1d")]
        [TestCase("30d", TestName = "[Get Valid Grouping] Valid Day Format", ExpectedResult = "1d")]
        [TestCase("-68d", TestName = "[Get Valid Grouping] Negative Value Daily Format", ExpectedResult = "1d")]
        [TestCase("30x", TestName = "[Get Valid Grouping] Invalid Format", ExpectedResult = "1d")]
        [TestCase("asdf", TestName = "[Get Valid Grouping] No Valid Value", ExpectedResult = "1d")]
        public string PeriodSummaryGroupingShouldReturnCorrectAmount(string input)
        {
            Setup();
            var result = repository.GetPeriodSummaryGrouping(input);

            return result;
        }
    }
}