using NUnit.Framework;
using Slot.Games.LionDance.Configuration;
using Slot.Model;
using System.IO;
using System.Linq;
using System.Xml;
using static Slot.UnitTests.LionDance.SpinsHelper;
using SpinXml = Slot.Games.LionDance.Models.Xml.SpinXml;

namespace Slot.UnitTests.LionDance.Xml
{
    [TestFixture]
    public class SpinXmlTests
    {
        [TestCase(Levels.One, TestName = "LionDance-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "LionDance-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var responseXml = spinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "LionDance-ReadResponseXmlOfSpinResult")]
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

        [TestCase(Levels.One, TestName = "LionDance-WriteXmlOfSpinResultXml")]
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

        [TestCase(Levels.One, TestName = "LionDance-CopySpinValuesFromSpinToXml")]
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
            Assert.AreEqual(spinResult.SpinBet.UserGameKey.GameId, responseXml.GameIdXml);
            Assert.AreEqual(spinResult.Type, responseXml.Type);
            Assert.AreEqual(spinResult.Bet, responseXml.Bet);
            Assert.AreEqual(spinResult.Win, responseXml.WinElement.Value);
            Assert.AreEqual(spinResult.XmlType, responseXml.XmlType);
            Assert.AreEqual(spinResult.TransactionId, responseXml.TransactionId);
        }

        [TestCase(Levels.One, TestName = "LionDance-ShouldCopyWinPositionsFromSpinToXml")]
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

        [TestCase(Levels.One, TestName = "LionDance-ShouldCopyWheelReelsFromSpinToXml")]
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
