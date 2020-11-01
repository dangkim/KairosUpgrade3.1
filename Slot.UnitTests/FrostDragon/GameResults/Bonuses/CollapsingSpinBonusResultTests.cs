using NUnit.Framework;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using Slot.Model;
using static Slot.UnitTests.FrostDragon.SpinsHelper;

namespace Slot.UnitTests.FrostDragon.GameResults.Bonuses
{
    [TestFixture]
    public class CollapsingBonusResultTests
    {
        [TestCase(Levels.One, TestName = "FrostDragon-WithSameGameResultAndBonus")]
        public void EngineShouldCreateCollapsingBonusResultWithSameGameResultAndBonus(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.AreSame(collapsingBonusResult.Bonus, collapsingBonus);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CollapsingBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateCollapsingBonusResultWithSameSpinTransactionId(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(collapsingBonusResult.SpinTransactionId == collapsingSpinResult.SpinTransactionId);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CompleteCollapsingBonusResultOnLoseSpin")]
        public void EngineShouldCompleteCollapsingBonusResultOnLoseSpin(int level)
        {
            var collapsingSpinResult = GenerateNonWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(collapsingBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ContinueCollapsingBonusResultOnWinSpin")]
        public void EngineShouldContinueCollapsingBonusResultOnWinSpin(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(!collapsingBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CreateBonusXElementOfCollapsingBonusResult")]
        public void EngineShouldCreateBonusXElementOfCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            var xElement = collapsingBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CreateBonusResponseXmlOfCollapsingBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            var responseXml = collapsingBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ReadResponseXmlOfCollapsingBonusResult")]
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
