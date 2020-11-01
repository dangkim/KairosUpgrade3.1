using NUnit.Framework;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using static Slot.UnitTests.FrostDragon.SpinsHelper;

namespace Slot.UnitTests.FrostDragon.Bonuses
{
    [TestFixture]
    public class FreeSpinBonusTests
    {
        [TestCase(Levels.One, TestName = "FrostDragon-CreateFreeSpinBonusInstance")]
        public void EngineShouldCreateFreeSpinBonusInstance(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsNotNull(FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult));
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CreateFreeSpinBonusWithValidGuid")]
        public void EngineShouldCreateFreeSpinBonusWithValidGuid(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(freeSpinBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CreateFreeSpinBonusWithValidGuid")]
        public void EngineShouldCreateFreeSpinBonusWithTransactionId(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            Assert.IsTrue(freeSpinBonus.SpinTransactionId == spinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CreateFreeSpinBonusWithGameResult")]
        public void EngineShouldCreateFreeSpinBonusWithGameResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            Assert.AreSame(spinResult, freeSpinBonus.GameResult);
            Assert.AreSame(spinResult, freeSpinBonus.SpinResult);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-StartFreeSpinBonusOnUpdate")]
        public void EngineShouldStartBonusOnCreateFreeSpinBonusResult(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult);

            Assert.IsTrue(freeSpinBonus.IsStarted);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldDeductCounterOnFreeSpin")]
        public void EngineShouldShouldDeductCounterOnFreeSpin(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateNonWinningNonBonusFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            var initialCounter = freeSpinBonus.Counter;

            freeSpinBonus.UpdateBonus(freeSpinResult);

            Assert.AreEqual(initialCounter - 1, freeSpinBonus.Counter);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldRetainCounterOnCollapsingFreeSpin")]
        public void EngineShouldRetainCounterOnCollapsingFreeSpin(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateWinningNonBonusFreeSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, freeSpinResult.Wheel.ReelStripsId);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult);

            var initialCounter = freeSpinBonus.Counter;
            var collapsingFreeSpinResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(freeSpinResult, targetWheel, config.BonusConfig.FreeSpin.Multipliers, config.Payline, config.PayTable);
            freeSpinBonus.UpdateBonus(collapsingFreeSpinResult);

            Assert.AreEqual(initialCounter, freeSpinBonus.Counter);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldIncompleteFreeSpinBonusIfWithCounter")]
        public void EngineShouldIncompleteFreeSpinBonusIfWithCounter(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            var initialCounter = freeSpinBonus.Counter;

            freeSpinBonus.UpdateBonus(freeSpinResult);

            Assert.IsTrue(!freeSpinBonus.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCompleteFreeSpinBonusWithZeroCounter")]
        public void EngineShouldCompleteFreeSpinBonusWithZeroCounter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            for (var count = 0; count < freeSpinBonus.NumOfFreeSpin; count++)
            {
                var freeSpinResult = GenerateNonWinningNonBonusFreeSpinResult(level);
                freeSpinBonus.UpdateBonus(freeSpinResult);
            }

            Assert.IsTrue(freeSpinBonus.IsCompleted);
        }
    }
}
