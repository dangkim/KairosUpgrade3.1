using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Model;
using System;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.BonusResults
{
    [TestFixture]
    public class RevealBonusResultTests
    {
        [TestCase(Levels.One, 0, TestName = "NuwaAndTheFiveElements-WithSameGameResultAndBonus")]
        public void EngineShouldCreateRevealBonusResultWithSameGameResultAndBonus(int level, int revealItem)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);
            revealBonus.UpdateBonus(revealItems, revealItem);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);

            Assert.AreSame(revealBonusResult.Bonus, revealBonus);
        }

        [TestCase(Levels.One, 0, TestName = "NuwaAndTheFiveElements-ShouldThrowExceptionOnInvalidRevealSelectionIndex-0")]
        [TestCase(Levels.One, 2, TestName = "NuwaAndTheFiveElements-ShouldThrowExceptionOnInvalidRevealSelectionIndex-2")]
        [TestCase(Levels.One, 4, TestName = "NuwaAndTheFiveElements-ShouldThrowExceptionOnInvalidRevealSelectionIndex-4")]

        public void EngineShouldNotThrowExceptionOnValidRevealSelectionIndex(int level, int revealItem)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);
            revealBonus.UpdateBonus(revealItems, revealItem);

            Assert.DoesNotThrow(() => RevealBonusEngine.CreateRevealBonusResult(revealBonus));
        }

        [TestCase(Levels.One, 0, TestName = "NuwaAndTheFiveElements-RevealBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateRevealBonusResultWithSameSpinTransactionId(int level, int revealItem)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);
            revealBonus.UpdateBonus(revealItems, revealItem);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);

            Assert.IsTrue(revealBonusResult.SpinTransactionId == revealBonus.SpinTransactionId);
        }

        [TestCase(Levels.One, 0, TestName = "NuwaAndTheFiveElements-CompleteRevealBonusResultOnCreate")]
        public void EngineShouldCompleteRevealBonusResultOnCreate(int level, int revealItem)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);
            revealBonus.UpdateBonus(revealItems, revealItem);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);

            Assert.IsTrue(revealBonusResult.IsCompleted);
        }

        [TestCase("1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2", 1, Levels.One, TestName = "NuwaAndTheFiveElements-PayoutTest-1", ExpectedResult = 375)]
        [TestCase("3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2", 3, Levels.One, TestName = "NuwaAndTheFiveElements-PayoutTest-3", ExpectedResult = 675)]
        [TestCase("4,4,4,4,4,4,4,4,4,2,2,2,2,2,2,2,2,2,2,4,4,4,4,2,2,2,2,2,2,2,2,2,2,2,2", 4, Levels.One, TestName = "NuwaAndTheFiveElements-PayoutTest-4", ExpectedResult = 975)]
        public decimal EngineShouldCreateCorrectPayout(string revealItemString, int revealItem, int level)
        {
            var config = new Configuration();
            var spinBet = MainGameEngine.GenerateSpinBet(new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = 1,
                    Multiplier = 1
                },
            });

            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = Array.ConvertAll(revealItemString.Split(','), Convert.ToInt32);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            revealBonus.UpdateBonus(revealItems, revealItem);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);

            return revealBonusResult.Win;
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusXElementOfRevealBonusResult")]
        public void EngineShouldCreateBonusXElementOfRevealBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            revealBonus.UpdateBonus(revealItems, 0);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);

            var xElement = revealBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusResponseXmlOfRevealBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfRevealBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            revealBonus.UpdateBonus(revealItems, 0);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);

            var responseXml = revealBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ReadResponseXmlOfRevealBonusResult")]
        public void EngineShouldReadResponseXmlOfRevealBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            revealBonus.UpdateBonus(revealItems, 0);

            var revealBonusResult = RevealBonusEngine.CreateRevealBonusResult(revealBonus);

            var xElement = revealBonusResult.ToXElement();

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
