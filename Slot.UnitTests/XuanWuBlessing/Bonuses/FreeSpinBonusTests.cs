using NUnit.Framework;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Configuration.Bonuses;
using Slot.Games.XuanWuBlessing.Engines;
using static Slot.UnitTests.XuanWuBlessing.SpinsHelper;

namespace Slot.UnitTests.XuanWuBlessing.Bonuses
{
    class FreeSpinBonusTests
    {
        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateFreeSpinBonusInstance")]
        public void EngineShouldCreateFreeSpinBonusInstance(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            Assert.IsNotNull(FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus));
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateFreeSpinBonusWithValidGuid")]
        public void EngineShouldCreateFreeSpinBonusWithValidGuid(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(freeSpinBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateFreeSpinBonusWithValidGuid")]
        public void EngineShouldCreateFreeSpinBonusWithTransactionId(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);

            Assert.IsTrue(freeSpinBonus.SpinTransactionId == spinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateFreeSpinBonusWithGameResult")]
        public void EngineShouldCreateFreeSpinBonusWithGameResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);

            Assert.AreSame(spinResult, freeSpinBonus.GameResult);
            Assert.AreSame(spinResult, freeSpinBonus.SpinResult);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-StartFreeSpinBonusOnUpdate")]
        public void EngineShouldStartBonusOnCreateFreeSpinBonusResult(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);

            freeSpinBonus.UpdateBonus(freeSpinResult);

            Assert.IsTrue(freeSpinBonus.IsStarted);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldDeductCounterOnFreeSpin")]
        public void EngineShouldShouldDeductCounterOnFreeSpin(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateNonWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            var initialCounter = freeSpinBonus.Counter;

            freeSpinBonus.UpdateBonus(freeSpinResult);

            Assert.AreEqual(initialCounter - 1, freeSpinBonus.Counter);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldIncompleteFreeSpinBonusIfWithCounter")]
        public void EngineShouldIncompleteFreeSpinBonusIfWithCounter(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            var initialCounter = freeSpinBonus.Counter;

            freeSpinBonus.UpdateBonus(freeSpinResult);

            Assert.IsTrue(!freeSpinBonus.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldCompleteFreeSpinBonusWithZeroCounter")]
        public void EngineShouldCompleteFreeSpinBonusWithZeroCounter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);

            for (var count = 0; count < freeSpinBonus.NumberOfFreeSpin; count++)
            {
                var freeSpinResult = GenerateNonWinningNonBonusFreeSpinResult(level);
                freeSpinBonus.UpdateBonus(freeSpinResult);
            }

            Assert.IsTrue(freeSpinBonus.IsCompleted);
        }
    }
}
