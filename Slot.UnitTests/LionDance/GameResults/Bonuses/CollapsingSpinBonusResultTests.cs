using NUnit.Framework;
using Slot.Games.LionDance.Configuration;
using Slot.Games.LionDance.Engines;
using Slot.Model;
using static Slot.UnitTests.LionDance.SpinsHelper;

namespace Slot.UnitTests.LionDance.GameResults.Bonuses
{
    [TestFixture]
    public class CollapsingBonusResultTests
    {
        [TestCase(Levels.One, TestName = "LionDance-WithSameGameResultAndBonus")]
        public void EngineShouldCreateCollapsingBonusResultWithSameGameResultAndBonus(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.AreSame(collapsingBonusResult.Bonus, collapsingBonus);
        }

        [TestCase(Levels.One, TestName = "LionDance-CollapsingBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateCollapsingBonusResultWithSameSpinTransactionId(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(collapsingBonusResult.SpinTransactionId == collapsingSpinResult.SpinTransactionId);
        }

        [TestCase(Levels.One, TestName = "LionDance-CompleteCollapsingBonusResultOnLoseSpin")]
        public void EngineShouldCompleteCollapsingBonusResultOnLoseSpin(int level)
        {
            var collapsingSpinResult = GenerateNonWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(collapsingBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "LionDance-ContinueCollapsingBonusResultOnWinSpin")]
        public void EngineShouldContinueCollapsingBonusResultOnWinSpin(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(!collapsingBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "LionDance-CreateBonusXElementOfCollapsingBonusResult")]
        public void EngineShouldCreateBonusXElementOfCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            var xElement = collapsingBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "LionDance-CreateBonusResponseXmlOfCollapsingBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            var responseXml = collapsingBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "LionDance-ReadResponseXmlOfCollapsingBonusResult")]
        public void EngineShouldReadResponseXmlOfCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            var xElement = collapsingBonusResult.ToXElement();

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
