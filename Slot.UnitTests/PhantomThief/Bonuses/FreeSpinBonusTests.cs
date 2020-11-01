using NUnit.Framework;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using static Slot.UnitTests.PhantomThief.SpinsHelper;

namespace Slot.UnitTests.PhantomThief.Bonuses
{
    [TestFixture]
    public class FreeSpinBonusTests
    {
        [TestCase(Levels.One, TestName = "PhantomThief-CreateFreeSpinBonusInstance")]
        public void EngineShouldCreateFreeSpinBonusInstance(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsNotNull(FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult));
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CreateFreeSpinBonusWithValidGuid")]
        public void EngineShouldCreateFreeSpinBonusWithValidGuid(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(freeSpinBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CreateFreeSpinBonusWithValidGuid")]
        public void EngineShouldCreateFreeSpinBonusWithTransactionId(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            Assert.IsTrue(freeSpinBonus.SpinTransactionId == spinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CreateFreeSpinBonusWithGameResult")]
        public void EngineShouldCreateFreeSpinBonusWithGameResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            Assert.AreSame(spinResult, freeSpinBonus.GameResult);
            Assert.AreSame(spinResult, freeSpinBonus.SpinResult);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-StartFreeSpinBonusOnUpdate")]
        public void EngineShouldStartBonusOnCreateFreeSpinBonusResult(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult, 0);

            Assert.IsTrue(freeSpinBonus.IsStarted);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldDeductCounterOnFreeSpin")]
        public void EngineShouldShouldDeductCounterOnFreeSpin(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateNonWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            var initialCounter = freeSpinBonus.Counter;

            freeSpinBonus.UpdateBonus(freeSpinResult, 0);

            Assert.AreEqual(initialCounter - 1, freeSpinBonus.Counter);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldRetainCounterOnCollapsingFreeSpin")]
        public void EngineShouldRetainCounterOnCollapsingFreeSpin(int level)
        {
            var config = new Configuration();
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, true);
            var freeSpinResult = GenerateWinningNonBonusFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult, 0);

            var initialCounter = freeSpinBonus.Counter;
            var collapsingFreeSpinResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(freeSpinResult, targetWheel, config.Payline, config.FreeGamePayTable, config.FreeGameScatterSymbols);
            freeSpinBonus.UpdateBonus(collapsingFreeSpinResult, 0);

            Assert.AreEqual(initialCounter, freeSpinBonus.Counter);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldIncompleteFreeSpinBonusIfWithCounter")]
        public void EngineShouldIncompleteFreeSpinBonusIfWithCounter(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            var initialCounter = freeSpinBonus.Counter;

            freeSpinBonus.UpdateBonus(freeSpinResult, 0);

            Assert.IsTrue(!freeSpinBonus.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldCompleteFreeSpinBonusWithZeroCounter")]
        public void EngineShouldCompleteFreeSpinBonusWithZeroCounter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            for (var count = 0; count < freeSpinBonus.NumOfFreeSpin; count++)
            {
                var freeSpinResult = GenerateNonWinningNonBonusFreeSpinResult(level);
                freeSpinBonus.UpdateBonus(freeSpinResult, 0);
            }

            Assert.IsTrue(freeSpinBonus.IsCompleted);
        }
    }
}
