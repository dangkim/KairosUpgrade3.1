using NUnit.Framework;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Model;
using System.IO;
using System.Linq;
using System.Xml;
using static Slot.UnitTests.XuanWuBlessing.SpinsHelper;
using SpinXml = Slot.Games.XuanWuBlessing.Models.Xml.SpinXml;

namespace Slot.UnitTests.XuanWuBlessing.Xml
{
    [TestFixture]
    public class SpinXmlTests
    {
        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var responseXml = spinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ReadResponseXmlOfSpinResult")]
        public void EngineShouldReadResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                using (var xmlReader = xElement.CreateReader())
                {
                    var responseXml = new SpinXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-WriteXmlOfSpinResultXml")]
        public void EngineShouldWriteXmlOfSpinResultXml(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                var responseXml = new SpinXml();
                using (var xmlReader = xElement.CreateReader())
                {
                    responseXml.ReadXml(xmlReader);
                }

                using (var memStream = new MemoryStream())
                using (var xmlWriter = XmlWriter.Create(memStream, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Auto }))
                {
                    xmlWriter.WriteStartElement("spin");
                    responseXml.WriteXml(xmlWriter);
                }
            });
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CopySpinValuesFromSpinToXml")]
        public void EngineShouldCopySpinValuesFromSpinToXml(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();
            var responseXml = new SpinXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            Assert.IsNull(responseXml.GetSchema());

            Assert.AreEqual(spinResult.BonusElement.Id, responseXml.BonusElement.Id);
            Assert.AreEqual(spinResult.BonusElement.FreeSpinAwardCount, responseXml.BonusElement.FreeSpinAwardCount);
            Assert.AreEqual(spinResult.BonusElement.AdditionalFreeSpinCount, responseXml.BonusElement.AdditionalFreeSpinCount);
            Assert.AreEqual(spinResult.BonusElement.Count, responseXml.BonusElement.Count);
            Assert.AreEqual(spinResult.BonusElement.NumberOfFreeSpin, responseXml.BonusElement.NumberOfFreeSpin);
            Assert.AreEqual(spinResult.BonusElement.Selection, responseXml.BonusElement.Selection);
            Assert.AreEqual(spinResult.SpinBet.UserGameKey.GameId, responseXml.GameIdXml);
            Assert.AreEqual(spinResult.Type, responseXml.Type);
            Assert.AreEqual(spinResult.Bet, responseXml.Bet);
            Assert.AreEqual(spinResult.Win, responseXml.WinElement.Value);
            Assert.AreEqual(spinResult.XmlType, responseXml.XmlType);
            Assert.AreEqual(spinResult.Multiplier, responseXml.Multiplier);
            Assert.AreEqual(spinResult.TransactionId, responseXml.TransactionId);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldCopyWinPositionsFromSpinToXml")]
        public void EngineShouldCopyWinPositionsFromSpinToXml(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();
            var responseXml = new SpinXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            for (var winPositionIndex = 0; winPositionIndex < spinResult.WinPositions.Count; winPositionIndex++)
            {
                var expectedObject = spinResult.WinPositions[winPositionIndex];
                var actualObject = responseXml.WinPositions[winPositionIndex];

                Assert.AreEqual(expectedObject.Line, actualObject.Line);
                Assert.AreEqual(expectedObject.Multiplier, actualObject.Multiplier);
                Assert.AreEqual(expectedObject.Symbol, actualObject.Symbol);
                Assert.AreEqual(expectedObject.Win, actualObject.Win);
                Assert.AreEqual(string.Join(',', expectedObject.RowPositions), string.Join(',', actualObject.RowPositions));
            }
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldCopyBonusPositionsFromSpinToXml")]
        public void EngineShouldCopyBonusPositionsFromSpinToXml(int level)
        {
            var spinResult = GenerateWithBonusSpinResult(level);
            var xElement = spinResult.ToXElement();
            var responseXml = new SpinXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            for (var bonusPositionIndex = 0; bonusPositionIndex < spinResult.BonusPositions.Count; bonusPositionIndex++)
            {
                var expectedObject = spinResult.BonusPositions[bonusPositionIndex];
                var actualObject = responseXml.BonusPositions[bonusPositionIndex];

                Assert.AreEqual(expectedObject.Symbol, actualObject.Symbol);
                Assert.AreEqual(expectedObject.Count, actualObject.Count);
                Assert.AreEqual(string.Join(',', expectedObject.RowPositions), string.Join(',', actualObject.RowPositions));
            }
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldCopyWheelReelsFromSpinToXml")]
        public void EngineShouldCopyWheelReelsFromSpinToXml(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();
            var responseXml = new SpinXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            var expectedReels = string.Join(',', spinResult.Wheel.Reels.SelectMany(reel => reel));
            var actualReels = string.Join(',', responseXml.Wheel.Reels.SelectMany(reel => reel));

            Assert.AreEqual(expectedReels, actualReels);
        }
    }
}
