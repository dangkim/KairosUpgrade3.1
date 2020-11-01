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
    public class FreeSpinBonusSelectionResultTests
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

            var freeSpinSelectionBonusResult = FreeSpinBonusEngine.CreateFreeSpinSelectionBonusResult(freeSpinBonus);

            Assert.AreSame(freeSpinSelectionBonusResult.Bonus, freeSpinBonus);
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

            var freeSpinSelectionBonusResult = FreeSpinBonusEngine.CreateFreeSpinSelectionBonusResult(freeSpinBonus);

            Assert.IsTrue(freeSpinSelectionBonusResult.SpinTransactionId == freeSpinBonus.SpinTransactionId);
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

            var freeSpinSelectionBonusResult = FreeSpinBonusEngine.CreateFreeSpinSelectionBonusResult(freeSpinBonus);

            Assert.IsTrue(!freeSpinSelectionBonusResult.IsCompleted);
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

            var freeSpinSelectionBonusResult = FreeSpinBonusEngine.CreateFreeSpinSelectionBonusResult(freeSpinBonus);

            var xElement = freeSpinSelectionBonusResult.ToXElement();

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

            var freeSpinSelectionBonusResult = FreeSpinBonusEngine.CreateFreeSpinSelectionBonusResult(freeSpinBonus);

            var responseXml = freeSpinSelectionBonusResult.ToResponseXml(ResponseXmlFormat.History);

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

            var freeSpinSelectionBonusResult = FreeSpinBonusEngine.CreateFreeSpinSelectionBonusResult(freeSpinBonus);

            var xElement = freeSpinSelectionBonusResult.ToXElement();

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
