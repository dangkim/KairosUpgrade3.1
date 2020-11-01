using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
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
    public class InstantWinXmlTests
    {
        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusXElementOfInstantWinResult")]
        public void EngineShouldCreateBonusXElementOfInstantWinResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinMultiplier = InstantWinBonusEngine.GetInstantWinMultiplier(module.Configuration.BonusConfig.InstantWin.MultiplierWeights[level]);
            instantWinBonus.UpdateBonus(instantWinMultiplier);

            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);
            var xElement = instantWinBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusResponseXmlOfInstantWinResult")]
        public void EngineShouldCreateBonusResponseXmlOfInstantWinResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinMultiplier = InstantWinBonusEngine.GetInstantWinMultiplier(module.Configuration.BonusConfig.InstantWin.MultiplierWeights[level]);
            instantWinBonus.UpdateBonus(instantWinMultiplier);

            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);
            var responseXml = instantWinBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-ReadResponseXmlOfInstantWinResult")]
        public void EngineShouldReadResponseXmlOfInstantWinResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinMultiplier = InstantWinBonusEngine.GetInstantWinMultiplier(module.Configuration.BonusConfig.InstantWin.MultiplierWeights[level]);
            instantWinBonus.UpdateBonus(instantWinMultiplier);

            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);
            var xElement = instantWinBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                using (var xmlReader = xElement.CreateReader())
                {
                    var responseXml = new InstantWinXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-WriteXmlOfInstantWinResultXml")]
        public void EngineShouldWriteXmlOfInstantWinResultXml(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinMultiplier = InstantWinBonusEngine.GetInstantWinMultiplier(module.Configuration.BonusConfig.InstantWin.MultiplierWeights[level]);
            instantWinBonus.UpdateBonus(instantWinMultiplier);

            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);
            var xElement = instantWinBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                var responseXml = new InstantWinXml();
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
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinMultiplier = InstantWinBonusEngine.GetInstantWinMultiplier(module.Configuration.BonusConfig.InstantWin.MultiplierWeights[level]);
            instantWinBonus.UpdateBonus(instantWinMultiplier);

            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);
            var xElement = instantWinBonusResult.ToXElement();
            var responseXml = new InstantWinXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            Assert.IsNull(responseXml.GetSchema());
            Assert.AreEqual(instantWinBonusResult.Win, responseXml.Win);
        }
    }
}
