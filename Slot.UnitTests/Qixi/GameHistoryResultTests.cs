namespace Slot.UnitTests.Qixi
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.Qixi;
    using Slot.Model;
    using Slot.Model.Entity;
    using Slot.Model.Utility;
    using System;
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
            var logger = logFactory.CreateLogger<QixiModule>();
            module = new QixiModule(logger);
        }

        [TestCase(TestName = "Test Spin Bet History")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 51);
            var requestContext = new RequestContext<SpinArgs>("simulation", "Qixi", PlatformType.None)
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
            var result = module.ExecuteSpin(1, new UserGameSpinData(), requestContext).Value as QixiSpinResult;
            var responseXml = xmlhelper.Serialize(result.ToResponseXml(ResponseXmlFormat.None));
            var element = XElement.Parse(responseXml, LoadOptions.PreserveWhitespace);
            var xelement = result.ToXElement();

            // Assert
            Assert.IsTrue(result.ToString() != null);
            Assert.IsTrue(result.XmlType == XmlType.SpinXml);
            Assert.IsTrue(result.GameResultType == GameResultType.SpinResult);
            Assert.IsNotNull(element);
            Assert.IsNotNull(xelement);
            Assert.IsTrue(element.Element("wheels") != null);
            Assert.IsTrue(element.Element("wheels").Attribute("val") != null);
            Assert.IsTrue(element.Element("wheels").Attribute("val").Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray().Length == 15);
            Assert.IsTrue(element.Element("win") != null);
            Assert.AreEqual(result.Win, decimal.Parse(element.Element("win").Value));
            Assert.IsTrue(element.Element("winposition") != null);
            Assert.AreEqual(null, element.Element("bonus"));
            Assert.AreEqual(null, element.Element("bonusposition"));
        }
    }
}