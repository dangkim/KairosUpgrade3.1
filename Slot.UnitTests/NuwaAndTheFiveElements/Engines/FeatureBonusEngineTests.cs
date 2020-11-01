using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Models.Bonuses;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using System;
using System.Linq;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.Engines
{
    [TestFixture]
    public class FeatureBonusEngineTests
    {
        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldReturnCorrectFeatureTypeForCollapse")]
        public void EngineShouldReturnCorrectFeatureTypeForCollapsingSpin(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningNonBonusSpinResult(level);
            var featureType = FeatureBonusEngine.GetFeatureType(spinResult, config.BonusConfig.TriggerWeights[level]);

            Assert.AreEqual(Features.Collapse, featureType);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldReturnCorrectFeatureTypeForNonCollapse")]
        public void EngineShouldReturnCorrectFeatureTypeForNonCollapse(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var targetTriggerWeights = config.BonusConfig.TriggerWeights[level];
            var featureType = FeatureBonusEngine.GetFeatureType(spinResult, config.BonusConfig.TriggerWeights[level]);

            Assert.IsTrue(targetTriggerWeights.Keys.Contains(featureType));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldReturnCollapsingSpinBonus")]
        public void EngineShouldReturnCollapsingSpinBonus(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);

            var bonus = FeatureBonusEngine.GetBonus(Features.Collapse, spinResult, config);

            Assert.AreSame(typeof(CollapsingSpinBonus), bonus.GetType());
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldReturnFreeSpinBonus")]
        public void EngineShouldReturnFreeSpinBonus(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);

            var bonus = FeatureBonusEngine.GetBonus(Features.FreeSpins, spinResult, config);

            Assert.AreSame(typeof(FreeSpinBonus), bonus.GetType());
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldReturnRevealBonus")]
        public void EngineShouldReturnRevealBonus(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);

            var bonus = FeatureBonusEngine.GetBonus(Features.Reveal, spinResult, config);

            Assert.AreSame(typeof(RevealBonus), bonus.GetType());
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldReturnInstantWinBonus")]
        public void EngineShouldReturnInstantWinBonus(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);

            var bonus = FeatureBonusEngine.GetBonus(Features.InstantWin, spinResult, config);

            Assert.AreSame(typeof(InstantWinBonus), bonus.GetType());
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldReturnNullOnInvalidBonus")]
        public void EngineShouldReturnNullOnInvalidBonus(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);

            var bonus = FeatureBonusEngine.GetBonus(int.MaxValue, spinResult, config);

            Assert.AreSame(null, bonus);
        }
    }
}
