using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.Bonuses
{
    [TestFixture]
    public class FreeSpinBonusTests
    {
        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateFreeSpinBonusInstance")]
        public void EngineShouldCreateFreeSpinBonusInstance(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsNotNull(FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateFreeSpinBonusWithValidGuid")]
        public void EngineShouldCreateFreeSpinBonusWithValidGuid(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(freeSpinBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateFreeSpinBonusWithValidGuid")]
        public void EngineShouldCreateFreeSpinBonusWithTransactionId(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            Assert.IsTrue(freeSpinBonus.SpinTransactionId == spinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateFreeSpinBonusWithGameResult")]
        public void EngineShouldCreateFreeSpinBonusWithGameResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            Assert.AreSame(spinResult, freeSpinBonus.GameResult);
            Assert.AreSame(spinResult, freeSpinBonus.SpinResult);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-StartFreeSpinBonusOnUpdate")]
        public void EngineShouldStartBonusOnCreateFreeSpinBonusResult(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult);

            Assert.IsTrue(freeSpinBonus.IsStarted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldDeductCounterOnFreeSpin")]
        public void EngineShouldShouldDeductCounterOnFreeSpin(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateNonWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            var initialCounter = freeSpinBonus.Counter;

            freeSpinBonus.UpdateBonus(freeSpinResult);

            Assert.AreEqual(initialCounter - 1, freeSpinBonus.Counter);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldRetainCounterOnCollapsingFreeSpin")]
        public void EngineShouldRetainCounterOnCollapsingFreeSpin(int level)
        {
            var config = new Configuration();
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);
            var freeSpinResult = GenerateWinningNonBonusFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult);

            var initialCounter = freeSpinBonus.Counter;
            var collapsingFreeSpinResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(freeSpinResult, targetWheel, config.SymbolCollapsePairs, config.Payline, config.PayTable);
            freeSpinBonus.UpdateBonus(collapsingFreeSpinResult);

            Assert.AreEqual(initialCounter, freeSpinBonus.Counter);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldIncompleteFreeSpinBonusIfWithCounter")]
        public void EngineShouldIncompleteFreeSpinBonusIfWithCounter(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            var initialCounter = freeSpinBonus.Counter;

            freeSpinBonus.UpdateBonus(freeSpinResult);

            Assert.IsTrue(!freeSpinBonus.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldCompleteFreeSpinBonusWithZeroCounter")]
        public void EngineShouldCompleteFreeSpinBonusWithZeroCounter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            for (var count = 0; count < freeSpinBonus.NumberOfFreeSpin; count++)
            {
                var freeSpinResult = GenerateNonWinningFreeSpinResult(level);
                freeSpinBonus.UpdateBonus(freeSpinResult);
            }

            Assert.IsTrue(freeSpinBonus.IsCompleted);
        }
    }
}
