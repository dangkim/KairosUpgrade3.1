using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;

namespace Slot.UnitTests.NuwaAndTheFiveElements.Engines
{
    [TestFixture]
    public class InstantWinBonusEngineTests
    {
        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateCorrectScatterMultiplier-1")]
        [TestCase(Levels.Two, TestName = "NuwaAndTheFiveElements-CreateCorrectScatterMultiplier-2")]
        [TestCase(Levels.Three, TestName = "NuwaAndTheFiveElements-CreateCorrectScatterMultiplier-3")]
        public void EngineShouldCreateCorrectScatterMultiplier(int level)
        {
            var config = new Configuration();

            Assert.DoesNotThrow(() => InstantWinBonusEngine.GetInstantWinMultiplier(config.BonusConfig.InstantWin.MultiplierWeights[level]));
        }
    }
}
