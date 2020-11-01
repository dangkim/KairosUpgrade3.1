using NUnit.Framework;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using static Slot.Games.JewelLand.Models.Test.SimulationHelper;
using static Slot.UnitTests.JewelLand.SpinsHelper;

namespace Slot.UnitTests.JewelLand.Bonuses
{
    [TestFixture]
    public class RespinBonusTests
    {
        [TestCase(Levels.One, TestName = "JewelLand-CreateRespinBonusInstance")]
        public void EngineShouldCreateRespinBonusInstance(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithRespinSpinResult(level);

            Assert.IsNotNull(RespinBonusEngine.CreateRespinBonus(spinResult));
        }

        [TestCase(Levels.One, TestName = "JewelLand-CreateRespinBonusWithValidGuid")]
        public void EngineShouldCreateRespinBonusWithValidGuid(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinBonus = RespinBonusEngine.CreateRespinBonus(spinResult);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(respinBonus.Guid.ToString()));
        }

        [TestCase(Levels.One, TestName = "JewelLand-CreateRespinBonusWithValidGuid")]
        public void EngineShouldCreateRespinBonusWithTransactionId(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var respinBonus = RespinBonusEngine.CreateRespinBonus(spinResult);

            Assert.IsTrue(respinBonus.SpinTransactionId == spinResult.TransactionId);
        }

        [TestCase(Levels.One, TestName = "JewelLand-CreateRespinBonusWithGameResult")]
        public void EngineShouldCreateRespinBonusWithGameResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var respinBonus = RespinBonusEngine.CreateRespinBonus(spinResult);

            Assert.AreSame(spinResult, respinBonus.GameResult);
            Assert.AreSame(spinResult, respinBonus.SpinResult);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-StartRespinBonusOnUpdate")]
        public void EngineShouldStartBonusOnCreateRespinBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithRespinSpinResult(level);
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var respinBonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);

            respinBonus.UpdateBonus(spinResult);

            Assert.IsTrue(respinBonus.IsStarted);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCompleteRespinBonusWithZeroCounter")]
        public void EngineShouldCompleteRespinBonusWithZeroCounter(int gameId, int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithRespinSpinResult(level);
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var respinBonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);

            respinBonus.UpdateBonus(spinResult);

            Assert.IsTrue(respinBonus.IsCompleted);
        }
    }
}
