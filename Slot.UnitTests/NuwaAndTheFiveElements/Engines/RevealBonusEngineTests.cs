using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;

namespace Slot.UnitTests.NuwaAndTheFiveElements.Engines
{
    [TestFixture]
    public class RevealBonusEngineTests
    {
        [TestCase(TestName = "NuwaAndTheFiveElements-ShouldCreateCorrectRevealItemIndex")]
        public void EngineShouldCreateCorrectRevealItemIndex()
        {
            var config = new Configuration();

            Assert.DoesNotThrow(() => RevealBonusEngine.GetRevealItem(config.BonusConfig.Reveal.ItemWeights));
        }
    }
}
