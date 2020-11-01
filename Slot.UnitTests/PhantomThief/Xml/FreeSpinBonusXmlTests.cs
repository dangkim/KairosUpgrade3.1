using NUnit.Framework;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using Slot.Games.PhantomThief.Models.Xml;
using Slot.Model;
using System.IO;
using System.Xml;
using static Slot.Games.PhantomThief.Models.Test.SimulationHelper;
using static Slot.UnitTests.PhantomThief.SpinsHelper;

namespace Slot.UnitTests.PhantomThief.Xml
{
    [TestFixture]
    public class FreeSpinBonusXmlTests
    {
        [TestCase(Configuration.Id, Levels.One, TestName = "PhantomThief-CreateBonusXElementOfFreeSpinBonusResult")]
        public void EngineShouldCreateBonusXElementOfFreeSpinBonusResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);
            var freeSpinResult = GenerateFreeSpinResult(level);
            var freeSpinResultAdditionalSpins = FreeSpinBonusEngine.GetAdditionalFreeSpinCount(freeSpinResult, module.Configuration);
            freeSpinBonus.UpdateBonus(freeSpinResult, freeSpinResultAdditionalSpins);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            var xElement = freeSpinBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "PhantomThief-CreateBonusResponseXmlOfFreeSpinBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfFreeSpinBonusResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);
            var freeSpinResult = GenerateFreeSpinResult(level);
            var freeSpinResultAdditionalSpins = FreeSpinBonusEngine.GetAdditionalFreeSpinCount(freeSpinResult, module.Configuration);
            freeSpinBonus.UpdateBonus(freeSpinResult, freeSpinResultAdditionalSpins);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            var responseXml = freeSpinBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "PhantomThief-ReadResponseXmlOfFreeSpinBonusResult")]
        public void EngineShouldReadResponseXmlOfFreeSpinBonusResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);
            var freeSpinResult = GenerateFreeSpinResult(level);
            var freeSpinResultAdditionalSpins = FreeSpinBonusEngine.GetAdditionalFreeSpinCount(freeSpinResult, module.Configuration);
            freeSpinBonus.UpdateBonus(freeSpinResult, freeSpinResultAdditionalSpins);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            var xElement = freeSpinBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                using (var xmlReader = xElement.CreateReader())
                {
                    var responseXml = new FreeSpinBonusXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "PhantomThief-WriteXmlOfFreeSpinBonusResultXml")]
        public void EngineShouldWriteXmlOfFreeSpinBonusResultXml(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);
            var freeSpinResult = GenerateFreeSpinResult(level);
            var freeSpinResultAdditionalSpins = FreeSpinBonusEngine.GetAdditionalFreeSpinCount(freeSpinResult, module.Configuration);
            freeSpinBonus.UpdateBonus(freeSpinResult, freeSpinResultAdditionalSpins);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            var xElement = freeSpinBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                var responseXml = new FreeSpinBonusXml();
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

        [TestCase(Configuration.Id, Levels.One, TestName = "PhantomThief-ShouldCopyBonusValuesFromResultToXml")]
        public void EngineShouldCopyBonusValuesFromResultToXml(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);
            var freeSpinResult = GenerateFreeSpinResult(level);
            var freeSpinResultAdditionalSpins = FreeSpinBonusEngine.GetAdditionalFreeSpinCount(freeSpinResult, module.Configuration);
            freeSpinBonus.UpdateBonus(freeSpinResult, freeSpinResultAdditionalSpins);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            var xElement = freeSpinBonusResult.ToXElement();
            var responseXml = new FreeSpinBonusXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            Assert.IsNull(responseXml.GetSchema());
            Assert.AreEqual(freeSpinBonusResult.TotalWin, responseXml.TotalWin);
            Assert.AreEqual(freeSpinBonusResult.Counter, responseXml.Counter);
            Assert.AreEqual(freeSpinBonusResult.NumberOfFreeSpin, responseXml.NumberOfFreeSpin);
        }
    }
}
