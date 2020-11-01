using NUnit.Framework;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using Slot.Games.JewelLand.Models.Xml;
using Slot.Model;
using System.IO;
using System.Linq;
using System.Xml;
using static Slot.Games.JewelLand.Models.Test.SimulationHelper;
using static Slot.UnitTests.JewelLand.SpinsHelper;

namespace Slot.UnitTests.JewelLand.Xml
{
    public class MultiplierBonusXmlTests
    {
        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-CreateBonusXElementOfRevealBonusResult")]
        public void EngineShouldCreateBonusXElementOfRevealBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var bonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);
            var totalWin = MultiplierBonusEngine.CalculateWin(spinResult.BonusPositions.First().Symbol, 1, multiplier, config);
            bonus.UpdateBonus(multiplier);

            var multiplierBonusResult = MultiplierBonusEngine.CreateMultiplierBonusResult(bonus, totalWin);
            var xElement = multiplierBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-CreateBonusResponseXmlOfRevealBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfRevealBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var bonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);
            var totalWin = MultiplierBonusEngine.CalculateWin(spinResult.BonusPositions.First().Symbol, 1, multiplier, config);
            bonus.UpdateBonus(multiplier);

            var multiplierBonusResult = MultiplierBonusEngine.CreateMultiplierBonusResult(bonus, totalWin);
            var responseXml = multiplierBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ReadResponseXmlOfRevealBonusResult")]
        public void EngineShouldReadResponseXmlOfRevealBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var bonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);
            var totalWin = MultiplierBonusEngine.CalculateWin(spinResult.BonusPositions.First().Symbol, 1, multiplier, config);
            bonus.UpdateBonus(multiplier);

            var multiplierBonusResult = MultiplierBonusEngine.CreateMultiplierBonusResult(bonus, totalWin);
            var xElement = multiplierBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                using (var xmlReader = xElement.CreateReader())
                {
                    var responseXml = new MultiplierBonusXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-WriteXmlOfRevealBonusResultXml")]
        public void EngineShouldWriteXmlOfRevealBonusResultXml(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var bonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);
            var totalWin = MultiplierBonusEngine.CalculateWin(spinResult.BonusPositions.First().Symbol, 1, multiplier, config);
            bonus.UpdateBonus(multiplier);

            var multiplierBonusResult = MultiplierBonusEngine.CreateMultiplierBonusResult(bonus, totalWin);
            var xElement = multiplierBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                var responseXml = new MultiplierBonusXml();
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
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var bonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);
            var totalWin = MultiplierBonusEngine.CalculateWin(spinResult.BonusPositions.First().Symbol, 1, multiplier, config);
            bonus.UpdateBonus(multiplier);

            var multiplierBonusResult = MultiplierBonusEngine.CreateMultiplierBonusResult(bonus, totalWin);
            var xElement = multiplierBonusResult.ToXElement();
            var responseXml = new MultiplierBonusXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            Assert.IsNull(responseXml.GetSchema());
            Assert.AreEqual(multiplierBonusResult.Win, responseXml.Win);
            Assert.AreEqual(multiplierBonusResult.Multiplier, responseXml.Multiplier);
        }
    }
}
