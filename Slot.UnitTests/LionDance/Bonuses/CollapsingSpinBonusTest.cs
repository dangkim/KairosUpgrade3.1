using NUnit.Framework;
using Slot.Games.LionDance.Configuration;
using Slot.Games.LionDance.Engines;
using static Slot.UnitTests.LionDance.SpinsHelper;

namespace Slot.UnitTests.LionDance.Bonuses
{
    [TestFixture]
    public class CollapsingSpinBonusTests
    {
        [TestCase(Levels.One, TestName = "LionDance-CreateCorrectCollapsingSpinBonusType")]
        public void EngineShouldCreateCorrectCollapsingSpinBonusType(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var bonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(spinResult);

            Assert.IsTrue(bonus.BonusType == Model.BonusType.CollapsingFreeSpin);
        }

        [TestCase(Levels.One, TestName = "LionDance-CreateCollapsingBonusInstance")]
        public void EngineShouldCreateCollapsingSpinBonusInstance(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);

            Assert.IsNotNull(CollapsingBonusEngine.CreateCollapsingSpinBonus(spinResult));
            Assert.IsNotNull(CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult));
        }

        [TestCase(Levels.One, TestName = "LionDance-CreateCollapsingBonusWithValidGuid")]
        public void EngineShouldCreateCollapsingSpinBonusWithValidGuid(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);

            var spinResultBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(spinResult);
            var collapsingSpinResultBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(spinResultBonus.Guid.ToString()));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(collapsingSpinResultBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "LionDance-CreateCollapsingBonusWithValidGuid")]
        public void EngineShouldCreateCollapsingSpinBonusWithTransactionId(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);

            var spinResultBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(spinResult);
            var collapsingSpinResultBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            Assert.IsTrue(spinResultBonus.SpinTransactionId == spinResult.TransactionId);
            Assert.IsTrue(collapsingSpinResultBonus.SpinTransactionId == collapsingSpinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "LionDance-CreateCollapsingBonusWithGameResult")]
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

        [TestCase(Levels.One, TestName = "LionDance-StartCollapsingBonusOnUpdate")]
        public void EngineShouldStartBonusOnCreateCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingBonus.UpdateBonus(collapsingSpinResult);

            Assert.IsTrue(collapsingBonus.IsStarted);
        }

        [TestCase(Levels.One, TestName = "LionDance-ContinueCollapsingBonusOnWin")]
        public void EngineShouldContinueBonusOnWinBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingBonus.UpdateBonus(collapsingSpinResult);
            Assert.IsTrue(!collapsingBonus.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "LionDance-ShouldNotCollapseBonusOnNonWinUpdate")]
        public void EngineShouldNotCollapseBonusOnWinUpdate(int level)
        {
            var collapsingSpinResult = GenerateNonWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingBonus.UpdateBonus(collapsingSpinResult);

            Assert.IsTrue(!collapsingBonus.SpinResult.Collapse);
        }

        [TestCase(Levels.One, TestName = "LionDance-ShouldCollapseBonusOnWinUpdate")]
        public void EngineShouldCollapseBonusOnWinUpdate(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingBonus.UpdateBonus(collapsingSpinResult);

            Assert.IsTrue(collapsingBonus.SpinResult.Collapse);
        }

        [TestCase(Levels.One, TestName = "LionDance-CompleteCollapsingBonusOnNonWinUpdate")]
        public void EngineShouldStartBonusOnUpdate(int level)
        {
            var collapsingSpinResult = GenerateNonWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingBonus.UpdateBonus(collapsingSpinResult);

            Assert.IsTrue(collapsingBonus.IsCompleted);
        }
    }
}
