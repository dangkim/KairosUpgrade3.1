using NUnit.Framework;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using Slot.Model;
using System.Linq;
using static Slot.Games.JewelLand.Models.Test.SimulationHelper;
using static Slot.UnitTests.JewelLand.SpinsHelper;

namespace Slot.UnitTests.JewelLand.GameResults.Bonuses
{
    [TestFixture]
    public class MultiplierBonusResultTests
    {
        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateMultiplierBonusResultWithSameGameResultAndBonus")]
        public void EngineShouldCreateMultiplierBonusResultWithSameGameResultAndBonus(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var bonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);
            var totalWin = MultiplierBonusEngine.CalculateWin(spinResult.BonusPositions.First().Symbol, 1, multiplier, config);
            bonus.UpdateBonus(multiplier);

            var multiplierBonusResult = MultiplierBonusEngine.CreateMultiplierBonusResult(bonus, totalWin);

            Assert.AreSame(multiplierBonusResult.Bonus, bonus);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateMultiplierBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateMultiplierBonusResultWithSameSpinTransactionId(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var bonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);
            var totalWin = MultiplierBonusEngine.CalculateWin(spinResult.BonusPositions.First().Symbol, 1, multiplier, config);
            bonus.UpdateBonus(multiplier);

            var multiplierBonusResult = MultiplierBonusEngine.CreateMultiplierBonusResult(bonus, totalWin);

            Assert.IsTrue(multiplierBonusResult.SpinTransactionId == bonus.SpinTransactionId);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCompleteMultiplierBonusResultOnBonusCompletion")]
        public void EngineShouldCompleteMultiplierBonusResultOnBonusCompletion(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var bonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);
            var totalWin = MultiplierBonusEngine.CalculateWin(spinResult.BonusPositions.First().Symbol, 1, multiplier, config);
            bonus.UpdateBonus(multiplier);

            var multiplierBonusResult = MultiplierBonusEngine.CreateMultiplierBonusResult(bonus, totalWin);

            Assert.IsTrue(multiplierBonusResult.IsCompleted);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateBonusXElementOfMultiplierBonusResult")]
        public void EngineShouldCreateBonusXElementOfMultiplierBonusResult(int gameId, int level)
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

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateBonusResponseXmlOfMultiplierBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfMultiplierBonusResult(int gameId, int level)
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

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldReadResponseXmlOfMultiplierBonusResult")]
        public void EngineShouldReadResponseXmlOfMultiplierBonusResult(int gameId, int level)
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
                    var responseXml = new BonusXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }
    }
}
