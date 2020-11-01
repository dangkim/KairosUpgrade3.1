namespace Slot.UnitTests.Cleopatra
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.Cleopatra;
    using Slot.Model;
    using Slot.Model.Entity;
    using Slot.Model.Utility;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    [TestFixture]
    internal class GameHistoryResultTests
    {
        private static IGameModule module;
        private static readonly XmlHelper xmlhelper = new XmlHelper();

        [SetUp]
        public void Settup()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<CleopatraModule>();
            module = new CleopatraModule(logger);
        }

        [TestCase(TestName = "Test Sarchophagus Bet History")]
        public void TestSarchophagusFeature()
        {
            // arrange
            var user = new UserGameKey(-1, 51);
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 50,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "Cleopatra", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };
            requestBonusContext.UserSession = userSession;
            requestBonusContext.UserGameKey = user;
            var bonusContext = new BonusStateContext(1, new CleopatraBonus { SpinBet = bet });
            var state = new SarchophagusState(new PreSpin(1, 0));

            //Action
            var response = state.Handle(requestBonusContext, bonusContext);
            var result = response.result;
            var responseXml = xmlhelper.Serialize(result.ToResponseXml(ResponseXmlFormat.None));
            var element = XElement.Parse(responseXml, LoadOptions.PreserveWhitespace);

            // Assert
            Assert.IsNotNull(element);
            Assert.AreEqual(result.Win > 0, element.Element("win") != null);
            Assert.AreEqual(result.Win, decimal.Parse(element.Element("win").Value));
            Assert.AreEqual(result.Win > 0, element.Element("data").Element("spin").Element("winposition") != null);
            Assert.AreEqual("", element.Element("data").Element("spin").Element("winposition").Value);
        }

        [TestCase(TestName = "Test Free Spin Bet History")]
        public void TestFreeSpinFeature()
        {
            // arrange
            var user = new UserGameKey(-1, 51);
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 50,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "Cleopatra", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };
            requestBonusContext.UserSession = userSession;
            requestBonusContext.UserGameKey = user;
            var bonusContext = new BonusStateContext(1, new CleopatraBonus { SpinBet = bet });
            var state = new FreeSpinState(1, 0);

            //Action
            var response = state.Handle(requestBonusContext, bonusContext);
            var result = response.result;
            var responseXml = xmlhelper.Serialize(result.ToResponseXml(ResponseXmlFormat.None));
            var element = XElement.Parse(responseXml, LoadOptions.PreserveWhitespace);

            // Assert
            Assert.IsNotNull(element);
            Assert.IsTrue(element.Element("data").Element("spin").Element("wheels") != null);
            Assert.AreEqual(result.Win, decimal.Parse(element.Element("win").Value));
            Assert.AreEqual(result.Win > 0, element.Element("data").Element("spin").Element("winposition").Descendants().Count() > 0);
            Assert.AreEqual(result.SpinResult.HasBonus , !string.IsNullOrEmpty(element.Element("data").Element("spin").Element("bonus").Value)); 
            Assert.AreEqual(result.SpinResult.HasBonus , element.Element("data").Element("spin").Element("bonusposition").Descendants().Count() > 0);            
        }

        [TestCase(TestName = "Test Spin Bet History")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 51);
            var requestContext = new RequestContext<SpinArgs>("simulation", "Cleopatra", PlatformType.None)
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
            var wheel = new CleopatraWheel
            {
                Reels = new List<int[]> {
                    new [] {0,1,2 },
                    new [] { 0, 10, 1 },
                    new [] { 0, 10, 3 },
                    new [] { 0, 10, 2 },
                    new [] { 2, 3, 4 }}
            };

            // Action
            var result = GameReduce.DoSpin(1, requestContext, wheel);
            var responseXml = xmlhelper.Serialize(result.ToResponseXml(ResponseXmlFormat.None));
            var element = XElement.Parse(responseXml, LoadOptions.PreserveWhitespace);

            // Assert
            Assert.IsNotNull(element);
            Assert.IsTrue(element.Element("wheels") != null);
            Assert.AreEqual(wheel.Reels.Select(ele => string.Join(',', ele)).Aggregate((s1, s2) => string.Join(',', s1, s2)), element.Element("wheels").Attribute("val").Value);
            Assert.AreEqual(result.Win > 0, element.Element("win") != null);
            Assert.AreEqual(result.Win, decimal.Parse(element.Element("win").Value));
            Assert.AreEqual(result.Win > 0, element.Element("winposition") != null);
            Assert.AreEqual(result.HasBonus, element.Element("bonus") != null);
            Assert.AreEqual(result.HasBonus, element.Element("bonusposition") != null);
        }
    }
}