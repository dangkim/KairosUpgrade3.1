using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Model;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.BonusResults
{
    [TestFixture]
    public class CollapsingBonusResultTests
    {
        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-WithSameGameResultAndBonus")]
        public void EngineShouldCreateCollapsingBonusResultWithSameGameResultAndBonus(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.AreSame(collapsingBonusResult.Bonus, collapsingBonus);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CollapsingBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateCollapsingBonusResultWithSameSpinTransactionId(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(collapsingBonusResult.SpinTransactionId == collapsingSpinResult.SpinTransactionId);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CompleteCollapsingBonusResultOnLoseSpin")]
        public void EngineShouldCompleteCollapsingBonusResultOnLoseSpin(int level)
        {
            var collapsingSpinResult = GenerateNonWinningCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(collapsingBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ContinueCollapsingBonusResultOnWinSpin")]
        public void EngineShouldContinueCollapsingBonusResultOnWinSpin(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            Assert.IsTrue(!collapsingBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusXElementOfCollapsingBonusResult")]
        public void EngineShouldCreateBonusXElementOfCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            var xElement = collapsingBonusResult.ToXElement();
            
            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusResponseXmlOfCollapsingBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfCollapsingBonusResult(int level)
        {
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);
            var collapsingBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);
            collapsingBonus.UpdateBonus(collapsingSpinResult);
            var collapsingBonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingBonus, collapsingSpinResult);

            var responseXml = collapsingBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ReadResponseXmlOfCollapsingBonusResult")]
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
