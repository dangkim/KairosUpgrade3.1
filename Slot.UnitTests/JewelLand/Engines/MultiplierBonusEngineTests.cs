using NUnit.Framework;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using System.Linq;
using static Slot.Games.JewelLand.Models.Test.SimulationHelper;
using static Slot.UnitTests.JewelLand.SpinsHelper;

namespace Slot.UnitTests.JewelLand.Engines
{
    [TestFixture]
    public class MultiplierBonusEngineTests
    {
        [TestCase(Levels.One, TestName = "JewelLand-ShouldCreateMultiplierBonus")]
        public void EngineShouldCreateMultiplierBonus(int level)
        {
            var spinResult = GenerateWithMultiplierSpinResult(level);

            Assert.DoesNotThrow(() => MultiplierBonusEngine.CreateMultiplierBonus(spinResult));
        }

        [TestCase(TestName = "JewelLand-ShouldGetCorrectMultiplier")]
        public void EngineShouldGetCorrectMultiplier()
        {
            var config = new Configuration();
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);

            Assert.IsTrue(config.BonusConfig.MultiplierSelections.Any(mult => mult == multiplier));
        }

        [TestCase(0, 1, 5, TestName = "JewelLand-ShouldCalculateCorrectWin-125", ExpectedResult = 125)]
        [TestCase(8, 1, 10, TestName = "JewelLand-ShouldCalculateCorrectWin-800", ExpectedResult = 800)]
        [TestCase(7, 1, 2, TestName = "JewelLand-ShouldCalculateCorrectWin-4", ExpectedResult = 4)]
        public decimal EngineShouldCalculateCorrectWin(int symbol, decimal totalBet, int multiplier)
        {
            var config = new Configuration();
            var totalWin = MultiplierBonusEngine.CalculateWin(symbol, totalBet, multiplier, config);

            return totalWin;
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateMultiplierBonusResult")]
        public void EngineShouldCreateMultiplierBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var symbol = spinResult.BonusPositions.First().Symbol;
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var bonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);
            bonus.UpdateBonus(multiplier);
            var totalWin = MultiplierBonusEngine.CalculateWin(symbol, bonus.TotalBet, multiplier, config);

            Assert.DoesNotThrow(() => MultiplierBonusEngine.CreateMultiplierBonusResult(bonus, totalWin));
        }
    }
}
