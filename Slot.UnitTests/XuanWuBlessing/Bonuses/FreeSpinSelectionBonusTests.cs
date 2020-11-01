using NUnit.Framework;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Configuration.Bonuses;
using Slot.Games.XuanWuBlessing.Engines;
using static Slot.UnitTests.XuanWuBlessing.SpinsHelper;

namespace Slot.UnitTests.XuanWuBlessing.Bonuses
{
    [TestFixture]
    public class FreeSpinSelectionBonusTests
    {
        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateFreeSpinSelectionBonusInstance")]
        public void EngineShouldCreateFreeSpinSelectionBonusInstance(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsNotNull(FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult));
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateFreeSpinSelectionBonusWithValidGuid")]
        public void EngineShouldCreateFreeSpinSelectionBonusWithValidGuid(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(freeSpinSelectionBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateFreeSpinSelectionBonusWithValidGuid")]
        public void EngineShouldCreateFreeSpinSelectionBonusWithTransactionId(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);

            Assert.IsTrue(freeSpinSelectionBonus.SpinTransactionId == spinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateFreeSpinSelectionBonusWithGameResult")]
        public void EngineShouldCreateFreeSpinSelectionBonusWithGameResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);

            Assert.AreSame(spinResult, freeSpinSelectionBonus.GameResult);
            Assert.AreSame(spinResult, freeSpinSelectionBonus.SpinResult);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-StartFreeSpinSelectionBonusOnUpdate")]
        public void EngineShouldStartAndCompleteBonusOnCreateFreeSpinSelectionBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);

            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            Assert.IsTrue(freeSpinSelectionBonus.IsStarted);
            Assert.IsTrue(freeSpinSelectionBonus.IsCompleted);
        }
    }
}
