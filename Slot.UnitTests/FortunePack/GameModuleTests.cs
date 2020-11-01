namespace Slot.UnitTests.FortunePack
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.FortunePack;
    using Slot.Model;
    using Slot.Model.Entity;

    [TestFixture]
    internal class GameModuleTests
    {
        private static IGameModule module;

        /// <summary>
        /// Dependency resolve and simulation setting stuff
        /// </summary>
        [SetUp]
        public void Settup()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<FortunePackModule>();
            module = new FortunePackModule(logger);
        }

        [TestCase(TestName = "Test Calculate the total bet", ExpectedResult = 8)]
        public decimal TestCalculateTotalBet()
        {
            var user = new UserGameKey(-1, 104);
            var requestContext = new RequestContext<SpinArgs>("unittest", "God of Fortune", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };

            var userSession = new UserSession
            {
                SessionKey = "unittest"
            };
            var spinArgs = new SpinArgs
            {
                LineBet = 1,
                Multiplier = 1
            };
            requestContext.Parameters = spinArgs;
            requestContext.UserSession = userSession;
            return module.CalculateTotalBet(new UserGameSpinData(), requestContext);
        }

        [TestCase(TestName = "Test Spin")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 14);
            var requestContext = new RequestContext<SpinArgs>("simulation", "God of Fortune", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };

            var userSession = new UserSession
            {
                SessionKey = "unittest"
            };
            var spinArgs = new SpinArgs
            {
                LineBet = 1,
                Multiplier = 1
            };
            requestContext.Parameters = spinArgs;
            requestContext.UserSession = userSession;

            // Action
            var spin = module.ExecuteSpin(1, new UserGameSpinData(), requestContext);
            var result = spin.Value as Games.FortunePack.FortunePackResult;

            // Assert

            Assert.AreEqual(1, result.Level);
            Assert.AreNotEqual(result.Wheel, default(Wheel));
            Assert.IsTrue(result.Bet == 8);
        }
    }
}