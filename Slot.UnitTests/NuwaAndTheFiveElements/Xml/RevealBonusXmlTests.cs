using NUnit.Framework;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Configuration.Bonuses;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Games.NuwaAndTheFiveElements.Models.Xml;
using Slot.Model;
using System.IO;
using System.Xml;
using static Slot.Games.NuwaAndTheFiveElements.Models.Test.SimulationHelper;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.Xml
{
    [TestFixture]
    public class RevealBonusXmlTests
    {
        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusXElementOfRevealBonusResult")]
        public void EngineShouldCreateBonusXElementOfRevealBonusResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(module.Configuration.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            var revealItem = RandomNumberEngine.Next(Reveal.RandomWeightMinRange, Reveal.RandomWeightMaxRange);
            revealBonus.UpdateBonus(revealItems, revealItem);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);
            var xElement = revealBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusResponseXmlOfRevealBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfRevealBonusResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(module.Configuration.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            var revealItem = RandomNumberEngine.Next(Reveal.RandomWeightMinRange, Reveal.RandomWeightMaxRange);
            revealBonus.UpdateBonus(revealItems, revealItem);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);
            var responseXml = revealBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-ReadResponseXmlOfRevealBonusResult")]
        public void EngineShouldReadResponseXmlOfRevealBonusResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(module.Configuration.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            var revealItem = RandomNumberEngine.Next(Reveal.RandomWeightMinRange, Reveal.RandomWeightMaxRange);
            revealBonus.UpdateBonus(revealItems, revealItem);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);
            var xElement = revealBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                using (var xmlReader = xElement.CreateReader())
                {
                    var responseXml = new RevealBonusXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-WriteXmlOfRevealBonusResultXml")]
        public void EngineShouldWriteXmlOfRevealBonusResultXml(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(module.Configuration.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            var revealItem = RandomNumberEngine.Next(Reveal.RandomWeightMinRange, Reveal.RandomWeightMaxRange);
            revealBonus.UpdateBonus(revealItems, revealItem);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);
            var xElement = revealBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                var responseXml = new RevealBonusXml();
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

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-ShouldCopyBonusValuesFromResultToXml")]
        public void EngineShouldCopyBonusValuesFromResultToXml(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(module.Configuration.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            var revealItem = RandomNumberEngine.Next(Reveal.RandomWeightMinRange, Reveal.RandomWeightMaxRange);
            revealBonus.UpdateBonus(revealItems, revealItem);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);
            var xElement = revealBonusResult.ToXElement();
            var responseXml = new RevealBonusXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            Assert.IsNull(responseXml.GetSchema());
            Assert.AreEqual(revealBonusResult.Win, responseXml.Win);
            Assert.AreEqual(revealBonusResult.Multiplier, responseXml.Multiplier);
            Assert.AreEqual(revealBonusResult.RevealItem, responseXml.RevealItem);
            Assert.AreEqual(string.Join(',', revealBonusResult.RevealItems), string.Join(',', responseXml.RevealItems));
        }
    }
}
