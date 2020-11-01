using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Model;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.BonusResults
{
    [TestFixture]
    public class FreeSpinBonusResultTests
    {
        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-WithSameGameResultAndBonus")]
        public void EngineShouldCreateFreeSpinBonusResultWithSameGameResultAndBonus(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            Assert.AreSame(freeSpinBonusResult.Bonus, freeSpinBonus);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-FreeSpinBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateFreeSpinBonusResultWithSameSpinTransactionId(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            Assert.IsTrue(freeSpinBonusResult.SpinTransactionId == freeSpinBonus.SpinTransactionId);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CompleteFreeSpinBonusResultOnBonusCompletion")]
        public void EngineShouldCompleteFreeSpinBonusResultOnBonusCompletion(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            while(!freeSpinBonus.IsCompleted)
            {
                freeSpinResult = GenerateNonWinningFreeSpinResult(level);
                freeSpinBonus.UpdateBonus(freeSpinResult);
                freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            }

            Assert.IsTrue(freeSpinBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ContinueFreeSpinBonusResultOnWinSpin")]
        public void EngineShouldContinueFreeSpinBonusResultOnWinSpin(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            Assert.IsTrue(!freeSpinBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusXElementOfFreeSpinBonusResult")]
        public void EngineShouldCreateBonusXElementOfFreeSpinBonusResult(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            var xElement = freeSpinBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusResponseXmlOfFreeSpinBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfFreeSpinBonusResult(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            var responseXml = freeSpinBonusResult.ToResponseXml(Model.ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-EngineShouldReadResponseXmlOfFreeSpinBonusResult")]
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
