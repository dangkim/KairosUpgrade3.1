using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using System;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.Bonuses
{
    [TestFixture]
    public class RevealBonusTests
    {
        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateRevealBonusInstance")]
        public void EngineShouldCreateRevealBonusInstance(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsNotNull(RevealBonusEngine.CreateRevealBonus(spinResult));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateRevealBonusWithValidGuid")]
        public void EngineShouldCreateRevealBonusWithValidGuid(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(revealBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateRevealBonusWithValidGuid")]
        public void EngineShouldCreateRevealBonusWithTransactionId(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);

            Assert.IsTrue(revealBonus.SpinTransactionId == spinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateRevealBonusWithGameResult")]
        public void EngineShouldCreateRevealBonusWithGameResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);

            Assert.AreSame(spinResult, revealBonus.GameResult);
            Assert.AreSame(spinResult, revealBonus.SpinResult);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-StartRevealBonusOnUpdate")]
        public void EngineShouldStartBonusOnCreateRevealBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);

            revealBonus.UpdateBonus(revealItems, 0);

            Assert.IsTrue(revealBonus.IsStarted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CompleteRevealBonusOnUpdateWithSelectedIndex")]
        public void EngineShouldContinueBonusOnWinBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);

            revealBonus.UpdateBonus(revealItems, 1);

            Assert.IsTrue(revealBonus.IsCompleted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-IncompleteRevealBonusOnUpdateWithoutSelectedIndex")]
        public void EngineShouldIncompleteRevealBonusOnUpdateWithoutSelectedIndex(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);

            Assert.IsTrue(!revealBonus.IsCompleted);
        }

        [TestCase(Levels.One, 10, TestName = "NuwaAndTheFiveElements-EngineShouldThrowExceptionOnInvalidRevealSelectionIndex-10")]
        [TestCase(Levels.One, 34, TestName = "NuwaAndTheFiveElements-EngineShouldThrowExceptionOnInvalidRevealSelectionIndex-34")]
        [TestCase(Levels.One, -1, TestName = "NuwaAndTheFiveElements-ShouldThrowExceptionOnInvalidRevealSelectionIndex--1")]
        [TestCase(Levels.One, 5, TestName = "NuwaAndTheFiveElements-ShouldThrowExceptionOnInvalidRevealSelectionIndex-5")]
        public void EngineShouldThrowExceptionOnInvalidRevealSelectionIndex(int level, int revealItem)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var revealBonus = RevealBonusEngine.CreateRevealBonus(spinResult);
            var revealItems = RevealBonusEngine.CreateRevealItems(config.BonusConfig.Reveal.ItemWeights);

            Assert.Throws(typeof(ArgumentException), () => revealBonus.UpdateBonus(revealItems, revealItem));
        }
    }
}
