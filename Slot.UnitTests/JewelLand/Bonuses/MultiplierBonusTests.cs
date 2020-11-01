using NUnit.Framework;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using static Slot.Games.JewelLand.Models.Test.SimulationHelper;
using static Slot.UnitTests.JewelLand.SpinsHelper;

namespace Slot.UnitTests.JewelLand.Bonuses
{
    [TestFixture]
    public class MultiplierBonusTests
    {
        [TestCase(Levels.One, TestName = "JewelLand-CreateMultiplierBonusInstance")]
        public void EngineShouldCreateMultiplierBonusInstance(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithMultiplierSpinResult(level);

            Assert.IsNotNull(MultiplierBonusEngine.CreateMultiplierBonus(spinResult));
        }

        [TestCase(Levels.One, TestName = "JewelLand-CreateMultiplierBonusWithValidGuid")]
        public void EngineShouldCreateMultiplierBonusWithValidGuid(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var multiplierBonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(multiplierBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "JewelLand-CreateMultiplierBonusWithValidGuid")]
        public void EngineShouldCreateMultiplierBonusWithTransactionId(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var multiplierBonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);

            Assert.IsTrue(multiplierBonus.SpinTransactionId == spinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "JewelLand-CreateMultiplierBonusWithGameResult")]
        public void EngineShouldCreateMultiplierBonusWithGameResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var multiplierBonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);

            Assert.AreSame(spinResult, multiplierBonus.GameResult);
            Assert.AreSame(spinResult, multiplierBonus.SpinResult);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-StartMultiplierBonusOnUpdate")]
        public void EngineShouldStartBonusOnCreateMultiplierBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var multiplierBonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);

            multiplierBonus.UpdateBonus(multiplier);

            Assert.IsTrue(multiplierBonus.IsStarted);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCompleteMultiplierBonusWithZeroCounter")]
        public void EngineShouldCompleteMultiplierBonusWithZeroCounter(int gameId, int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithMultiplierSpinResult(level);
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var multiplierBonus = MultiplierBonusEngine.CreateMultiplierBonus(spinResult);
            var multiplier = MultiplierBonusEngine.GetMultiplier(config);

            multiplierBonus.UpdateBonus(multiplier);

            Assert.IsTrue(multiplierBonus.IsCompleted);
        }
    }
}
