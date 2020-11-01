using NUnit.Framework;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Configuration.Bonuses;
using Slot.Games.XuanWuBlessing.Engines;
using Slot.Model;
using static Slot.UnitTests.XuanWuBlessing.SpinsHelper;

namespace Slot.UnitTests.XuanWuBlessing.GameResults.Bonuses
{
    [TestFixture]
    public class FreeSpinBonusResultTests
    {
        [TestCase(Levels.One, TestName = "XuanWuBlessing-WithSameGameResultAndBonus")]
        public void EngineShouldCreateFreeSpinBonusResultWithSameGameResultAndBonus(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            Assert.AreSame(freeSpinBonusResult.Bonus, freeSpinBonus);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-FreeSpinBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateFreeSpinBonusResultWithSameSpinTransactionId(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            Assert.IsTrue(freeSpinBonusResult.SpinTransactionId == freeSpinBonus.SpinTransactionId);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CompleteFreeSpinBonusResultOnBonusCompletion")]
        public void EngineShouldCompleteFreeSpinBonusResultOnBonusCompletion(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            while (!freeSpinBonus.IsCompleted)
            {
                freeSpinResult = GenerateNonWinningFreeSpinResult(level);
                freeSpinBonus.UpdateBonus(freeSpinResult);
                freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            }

            Assert.IsTrue(freeSpinBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ContinueFreeSpinBonusResultOnWinSpin")]
        public void EngineShouldContinueFreeSpinBonusResultOnWinSpin(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            Assert.IsTrue(!freeSpinBonusResult.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateBonusXElementOfFreeSpinBonusResult")]
        public void EngineShouldCreateBonusXElementOfFreeSpinBonusResult(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            var xElement = freeSpinBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateBonusResponseXmlOfFreeSpinBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfFreeSpinBonusResult(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);

            var responseXml = freeSpinBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-EngineShouldReadResponseXmlOfFreeSpinBonusResult")]
        public void EngineShouldReadResponseXmlOfFreeSpinBonusResult(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

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
