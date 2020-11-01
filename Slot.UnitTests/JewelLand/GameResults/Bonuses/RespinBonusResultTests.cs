using NUnit.Framework;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using Slot.Model;
using static Slot.Games.JewelLand.Models.Test.SimulationHelper;
using static Slot.UnitTests.JewelLand.SpinsHelper;

namespace Slot.UnitTests.JewelLand.GameResults.Bonuses
{
    [TestFixture]
    public class RespinBonusResultTests
    {
        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateRespinBonusResultWithSameGameResultAndBonus")]
        public void EngineShouldCreateRespinBonusResultWithSameGameResultAndBonus(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);

            Assert.AreSame(respinBonusResult.Bonus, bonus);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateRespinBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateRespinBonusResultWithSameSpinTransactionId(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);

            Assert.IsTrue(respinBonusResult.SpinTransactionId == bonus.SpinTransactionId);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCompleteRespinBonusResultOnBonusCompletion")]
        public void EngineShouldCompleteRespinBonusResultOnBonusCompletion(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);

            Assert.IsTrue(respinBonusResult.IsCompleted);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateBonusXElementOfRespinBonusResult")]
        public void EngineShouldCreateBonusXElementOfRespinBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);

            var xElement = respinBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateBonusResponseXmlOfRespinBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfRespinBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);

            var responseXml = respinBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldReadResponseXmlOfRespinBonusResult")]
        public void EngineShouldReadResponseXmlOfRespinBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            bonus.UpdateBonus(respinResult);

            var respinBonusResult = RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult);

            var xElement = respinBonusResult.ToXElement();

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
