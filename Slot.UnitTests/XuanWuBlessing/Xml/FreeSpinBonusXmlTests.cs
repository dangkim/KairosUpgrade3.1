using NUnit.Framework;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Configuration.Bonuses;
using Slot.Games.XuanWuBlessing.Engines;
using Slot.Games.XuanWuBlessing.Models.Xml;
using Slot.Model;
using System.IO;
using System.Xml;
using static Slot.Games.XuanWuBlessing.Models.Test.SimulationHelper;
using static Slot.UnitTests.XuanWuBlessing.SpinsHelper;

namespace Slot.UnitTests.XuanWuBlessing.Xml
{
    [TestFixture]
    public class FreeSpinBonusXmlTests
    {
        [TestCase(Configuration.Id, Levels.One, TestName = "XuanWuBlessing-CreateBonusXElementOfFreeSpinBonusResult")]
        public void EngineShouldCreateBonusXElementOfFreeSpinBonusResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);

            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            var freeSpinResult = GenerateFreeSpinResult(level);
            freeSpinBonus.UpdateBonus(freeSpinResult);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            var xElement = freeSpinBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "XuanWuBlessing-CreateBonusResponseXmlOfFreeSpinBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfFreeSpinBonusResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);

            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            var freeSpinResult = GenerateFreeSpinResult(level);
            freeSpinBonus.UpdateBonus(freeSpinResult);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            var responseXml = freeSpinBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "XuanWuBlessing-ReadResponseXmlOfFreeSpinBonusResult")]
        public void EngineShouldReadResponseXmlOfFreeSpinBonusResult(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);

            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            var freeSpinResult = GenerateFreeSpinResult(level);
            freeSpinBonus.UpdateBonus(freeSpinResult);

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

        [TestCase(Configuration.Id, Levels.One, TestName = "XuanWuBlessing-WriteXmlOfFreeSpinBonusResultXml")]
        public void EngineShouldWriteXmlOfFreeSpinBonusResultXml(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);

            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            var freeSpinResult = GenerateFreeSpinResult(level);
            freeSpinBonus.UpdateBonus(freeSpinResult);

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

        [TestCase(Configuration.Id, Levels.One, TestName = "XuanWuBlessing-ShouldCopyBonusValuesFromResultToXml")]
        public void EngineShouldCopyBonusValuesFromResultToXml(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinResult = GenerateWithBonusSpinResult(level);

            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            var freeSpinResult = GenerateFreeSpinResult(level);
            freeSpinBonus.UpdateBonus(freeSpinResult);

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
