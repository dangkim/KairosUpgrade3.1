using NUnit.Framework;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using Slot.Model;
using static Slot.UnitTests.PhantomThief.SpinsHelper;

namespace Slot.UnitTests.PhantomThief.GameResults.Bonuses
{
    [TestFixture]
    public class CollapsingBonusResultTests
    {
        [TestCase(Levels.One, TestName = "PhantomThief-WithSameGameResultAndBonus")]
        public void EngineShouldCreateCollapsingBonusResultWithSameGameResultAndBonus(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.AreSame(collapsingBonusResult.Bonus, collapsingBonus);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CollapsingBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateCollapsingBonusResultWithSameSpinTransactionId(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(collapsingBonusResult.SpinTransactionId == collapsingSpinResult.SpinTransactionId);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CompleteCollapsingBonusResultOnLoseSpin")]
        public void EngineShouldCompleteCollapsingBonusResultOnLoseSpin(int level)
        {
            var collapsingSpinResult = GenerateNonWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(collapsingBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ContinueCollapsingBonusResultOnWinSpin")]
        public void EngineShouldContinueCollapsingBonusResultOnWinSpin(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(!collapsingBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CreateBonusXElementOfCollapsingBonusResult")]
        public void EngineShouldCreateBonusXElementOfCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            var xElement = collapsingBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CreateBonusResponseXmlOfCollapsingBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            var responseXml = collapsingBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ReadResponseXmlOfCollapsingBonusResult")]
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
