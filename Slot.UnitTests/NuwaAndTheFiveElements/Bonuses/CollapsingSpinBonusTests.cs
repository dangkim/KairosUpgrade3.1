using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.Bonuses
{
    [TestFixture]
    public class CollapsingSpinBonusTests
    {
        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateCollapsingBonusInstance")]
        public void EngineShouldCreateCollapsingSpinBonusInstance(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);

            Assert.IsNotNull(CollapsingBonusEngine.CreateCollapsingSpinBonus(spinResult));
            Assert.IsNotNull(CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateCollapsingBonusWithValidGuid")]
        public void EngineShouldCreateCollapsingSpinBonusWithValidGuid(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);

            var spinResultBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(spinResult);
            var collapsingSpinResultBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(spinResultBonus.Guid.ToString()));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(collapsingSpinResultBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateCollapsingBonusWithValidGuid")]
        public void EngineShouldCreateCollapsingSpinBonusWithTransactionId(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);

            var spinResultBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(spinResult);
            var collapsingSpinResultBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            Assert.IsTrue(spinResultBonus.SpinTransactionId == spinResult.TransactionId);
            Assert.IsTrue(collapsingSpinResultBonus.SpinTransactionId == collapsingSpinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateCollapsingBonusWithGameResult")]
        public void EngineShouldCreateCollapsingSpinBonusWithGameResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);

            var spinResultBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(spinResult);
            var collapsingSpinResultBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            Assert.AreSame(spinResult, spinResultBonus.GameResult);
            Assert.AreSame(spinResult, spinResultBonus.SpinResult);
            Assert.AreSame(collapsingSpinResult, collapsingSpinResultBonus.GameResult);
            Assert.AreSame(collapsingSpinResult, collapsingSpinResultBonus.SpinResult);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-StartCollapsingBonusOnUpdate")]
        public void EngineShouldStartBonusOnCreateCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingBonus.UpdateBonus(collapsingSpinResult);

            Assert.IsTrue(collapsingBonus.IsStarted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ContinueCollapsingBonusOnWin")]
        public void EngineShouldContinueBonusOnWinBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingBonus.UpdateBonus(collapsingSpinResult);
            Assert.IsTrue(!collapsingBonus.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldNotCollapseBonusOnNonWinUpdate")]
        public void EngineShouldNotCollapseBonusOnWinUpdate(int level)
        {
            var collapsingSpinResult = GenerateNonWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingBonus.UpdateBonus(collapsingSpinResult);

            Assert.IsTrue(!collapsingBonus.SpinResult.Collapse);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldCollapseBonusOnWinUpdate")]
        public void EngineShouldCollapseBonusOnWinUpdate(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingBonus.UpdateBonus(collapsingSpinResult);

            Assert.IsTrue(collapsingBonus.SpinResult.Collapse);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CompleteCollapsingBonusOnNonWinUpdate")]
        public void EngineShouldStartBonusOnUpdate(int level)
        {
            var collapsingSpinResult = GenerateNonWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingBonus.UpdateBonus(collapsingSpinResult);

            Assert.IsTrue(collapsingBonus.IsCompleted);
        }
    }
}
