using NUnit.Framework;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Models.Xml;
using Slot.Model;
using System.IO;
using System.Linq;
using System.Xml;
using static Slot.UnitTests.FrostDragon.SpinsHelper;

namespace Slot.UnitTests.FrostDragon.Xml
{
    [TestFixture]
    public class CollapsingSpinXmlTests
    {
        [TestCase(Levels.One, TestName = "FrostDragon-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var xElement = collapsingSpinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var responseXml = collapsingSpinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ReadResponseXmlOfSpinResult")]
        public void EngineShouldReadResponseXmlOfSpinResult(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var xElement = collapsingSpinResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                using (var xmlReader = xElement.CreateReader())
                {
                    var responseXml = new CollapsingSpinXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }

        [TestCase(Levels.One, TestName = "FrostDragon-WriteXmlOfSpinResultXml")]
        public void EngineShouldWriteXmlOfSpinResultXml(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var xElement = collapsingSpinResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                var responseXml = new CollapsingSpinXml();
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

        [TestCase(Levels.One, TestName = "FrostDragon-CopySpinValuesFromSpinToXml")]
        public void EngineShouldCopySpinValuesFromSpinToXml(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var xElement = collapsingSpinResult.ToXElement();
            var responseXml = new CollapsingSpinXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            Assert.IsNull(responseXml.GetSchema());
            Assert.AreEqual(collapsingSpinResult.SpinBet.UserGameKey.GameId, responseXml.GameIdXml);
            Assert.AreEqual(collapsingSpinResult.Type, responseXml.Type);
            Assert.AreEqual(collapsingSpinResult.Bet, responseXml.Bet);
            Assert.AreEqual(collapsingSpinResult.Win, responseXml.WinElement.Value);
            Assert.AreEqual(collapsingSpinResult.AvalancheMultiplier, responseXml.AvalancheMultiplier);
            Assert.AreEqual(collapsingSpinResult.XmlType, responseXml.XmlType);
            Assert.AreEqual(collapsingSpinResult.TransactionId, responseXml.TransactionId);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCopyWinPositionsFromSpinToXml")]
        public void EngineShouldCopyWinPositionsFromSpinToXml(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var xElement = collapsingSpinResult.ToXElement();
            var responseXml = new CollapsingSpinXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            for (var winPositionIndex = 0; winPositionIndex < collapsingSpinResult.WinPositions.Count; winPositionIndex++)
            {
                var expectedObject = collapsingSpinResult.WinPositions[winPositionIndex];
                var actualObject = responseXml.WinPositions[winPositionIndex];

                Assert.AreEqual(expectedObject.Line, actualObject.Line);
                Assert.AreEqual(expectedObject.Multiplier, actualObject.Multiplier);
                Assert.AreEqual(expectedObject.Symbol, actualObject.Symbol);
                Assert.AreEqual(expectedObject.Win, actualObject.Win);
                Assert.AreEqual(string.Join(',', expectedObject.RowPositions), string.Join(',', actualObject.RowPositions));
            }
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCopyBonusPositionsFromSpinToXml")]
        public void EngineShouldCopyBonusPositionsFromSpinToXml(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var xElement = collapsingSpinResult.ToXElement();
            var responseXml = new CollapsingSpinXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            for (var bonusPositionIndex = 0; bonusPositionIndex < collapsingSpinResult.BonusPositions.Count; bonusPositionIndex++)
            {
                var expectedObject = collapsingSpinResult.BonusPositions[bonusPositionIndex];
                var actualObject = responseXml.BonusPositions[bonusPositionIndex];

                Assert.AreEqual(expectedObject.Symbol, actualObject.Symbol);
                Assert.AreEqual(expectedObject.Count, actualObject.Count);
                Assert.AreEqual(string.Join(',', expectedObject.RowPositions), string.Join(',', actualObject.RowPositions));
            }
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCopyWheelReelsFromSpinToXml")]
        public void EngineShouldCopyWheelReelsFromSpinToXml(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var xElement = collapsingSpinResult.ToXElement();
            var responseXml = new CollapsingSpinXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            var expectedReels = string.Join(',', collapsingSpinResult.Wheel.Reels.SelectMany(reel => reel));
            var actualReels = string.Join(',', responseXml.Wheel.Reels.SelectMany(reel => reel));

            Assert.AreEqual(expectedReels, actualReels);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCopyAddedSymbolsFromSpinToXml")]
        public void EngineShouldCopyAddedSymbolsFromSpinToXml(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var xElement = collapsingSpinResult.ToXElement();
            var responseXml = new CollapsingSpinXml();

            using (var xmlReader = xElement.CreateReader())
            {
                responseXml.ReadXml(xmlReader);
            }

            var expectedReels = string.Join(',', collapsingSpinResult.CollapsingAdds.SelectMany(kv => kv.Value));
            var actualReels = string.Join(',', responseXml.CollapsingAdds.SelectMany(kv => kv.Value));

            Assert.AreEqual(expectedReels, actualReels);
        }
    }
}
