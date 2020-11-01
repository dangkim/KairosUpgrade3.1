using NUnit.Framework;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using Slot.Model;
using static Slot.UnitTests.FrostDragon.SpinsHelper;

namespace Slot.UnitTests.FrostDragon.GameResults.Bonuses
{
    [TestFixture]
    public class FreeSpinBonusResultTests
    {
        [TestCase(Levels.One, TestName = "FrostDragon-WithSameGameResultAndBonus")]
        public void EngineShouldCreateFreeSpinBonusResultWithSameGameResultAndBonus(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            Assert.AreSame(freeSpinBonusResult.Bonus, freeSpinBonus);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-FreeSpinBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateFreeSpinBonusResultWithSameSpinTransactionId(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            Assert.IsTrue(freeSpinBonusResult.SpinTransactionId == freeSpinBonus.SpinTransactionId);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CompleteFreeSpinBonusResultOnBonusCompletion")]
        public void EngineShouldCompleteFreeSpinBonusResultOnBonusCompletion(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            while (!freeSpinBonus.IsCompleted)
            {
                freeSpinResult = GenerateNonWinningFreeSpinResult(level);
                freeSpinBonus.UpdateBonus(freeSpinResult);
                freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            }

            Assert.IsTrue(freeSpinBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ContinueFreeSpinBonusResultOnWinSpin")]
        public void EngineShouldContinueFreeSpinBonusResultOnWinSpin(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            Assert.IsTrue(!freeSpinBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CreateBonusXElementOfFreeSpinBonusResult")]
        public void EngineShouldCreateBonusXElementOfFreeSpinBonusResult(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            var xElement = freeSpinBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CreateBonusResponseXmlOfFreeSpinBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfFreeSpinBonusResult(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            var responseXml = freeSpinBonusResult.ToResponseXml(Model.ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-EngineShouldReadResponseXmlOfFreeSpinBonusResult")]
        public void EngineShouldReadResponseXmlOfFreeSpinBonusResult(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            var xElement = freeSpinBonusResult.ToXElement();

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
