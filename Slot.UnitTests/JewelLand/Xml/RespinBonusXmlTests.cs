using NUnit.Framework;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using Slot.Games.JewelLand.Models.Xml;
using Slot.Model;
using System.IO;
using System.Xml;
using static Slot.Games.JewelLand.Models.Test.SimulationHelper;
using static Slot.UnitTests.JewelLand.SpinsHelper;

namespace Slot.UnitTests.JewelLand.Xml
{
    [TestFixture]
    public class RespinBonusXmlTests
    {
        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-CreateBonusXElementOfrespinBonusResult")]
        public void EngineShouldCreateBonusXElementOfrespinBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);
            var xElement = respinBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-CreateBonusResponseXmlOfrespinBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfrespinBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);
            var responseXml = respinBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ReadResponseXmlOfrespinBonusResult")]
        public void EngineShouldReadResponseXmlOfrespinBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);
            var xElement = respinBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                using (var xmlReader = xElement.CreateReader())
                {
                    var responseXml = new RespinBonusXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-WriteXmlOfrespinBonusResultXml")]
        public void EngineShouldWriteXmlOfrespinBonusResultXml(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);
            var xElement = respinBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                var responseXml = new RespinBonusXml();
                using (var xmlReader = xElement.CreateReader())
                {
                    responseXml.ReadXml(xmlReader);
                }

                using (var memStream = new MemoryStream())
                using (var xmlWriter = XmlWriter.Create(memStream, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Auto }))
                {
                    xmlWriter.WriteStartElement("bonus");
                    responseXml.WriteXml(xmlWriter);
                }
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCopyBonusValuesFromResultToXml")]
        public void EngineShouldCopyBonusValuesFromResultToXml(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);
            var xElement = respinBonusResult.ToXElement();
            var responseXml = new RespinBonusXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            Assert.IsNull(responseXml.GetSchema());
            Assert.AreEqual(respinBonusResult.Win, responseXml.TotalWin);
            Assert.AreEqual(0, responseXml.Counter);
            Assert.AreEqual(1, responseXml.NumberOfFreeSpin);
        }
    }
}
