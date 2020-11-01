using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.Bonuses
{
    [TestFixture]
    public class InstantWinBonusTests
    {
        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateInstantWinBonusInstance")]
        public void EngineShouldCreateInstantWinBonusInstance(int level)
        {
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsNotNull(InstantWinBonusEngine.CreateInstantWinBonus(spinResult));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateInstantWinBonusWithValidGuid")]
        public void EngineShouldCreateInstantWinBonusWithValidGuid(int level)
        {
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(instantWinBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateInstantWinBonusWithValidGuid")]
        public void EngineShouldCreateInstantWinBonusWithTransactionId(int level)
        {
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);

            Assert.IsTrue(instantWinBonus.SpinTransactionId == spinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateInstantWinBonusWithGameResult")]
        public void EngineShouldCreateInstantWinBonusWithGameResult(int level)
        {
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);

            Assert.AreSame(spinResult, instantWinBonus.GameResult);
            Assert.AreSame(spinResult, instantWinBonus.SpinResult);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-StartInstantWinBonusOnUpdate")]
        public void EngineShouldStartBonusOnCreateInstantWinBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);

            instantWinBonus.UpdateBonus(1);

            Assert.IsTrue(instantWinBonus.IsStarted);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CompleteInstantWinBonusOnCreate")]
        public void EngineShouldContinueBonusOnWinBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);

            instantWinBonus.UpdateBonus(1);

            Assert.IsTrue(instantWinBonus.IsCompleted);
        }
    }
}
