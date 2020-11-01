using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using Slot.Model;
using System.Linq;
using static Slot.Games.JewelLand.Models.Test.SimulationHelper;
using static Slot.UnitTests.JewelLand.SpinsHelper;
using SpinResult = Slot.Games.JewelLand.Models.GameResults.Spins.SpinResult;

namespace Slot.UnitTests.JewelLand.Engines
{
    [TestFixture]
    public class RespinBonusEngineTests
    {
        [TestCase(Levels.One, TestName = "JewelLand-ShouldCreateCorrectRespinBonus")]
        public void EngineShouldCreateRespinBonus(int level)
        {
            var spinResult = GenerateWithRespinSpinResult(level);

            Assert.DoesNotThrow(() => RespinBonusEngine.CreateRespinBonus(spinResult));
        }

        [TestCase("2,2,3|7,7,8|7,7,7", "5,5,5|7,7,8|7,7,7", Levels.One, TestName = "JewelLand-ShouldCreateCorrectRespinWheel-0-Respin", ExpectedResult = "5,5,5,7,7,8,7,7,7")]
        [TestCase("7,7,8|7,7,7|5,5,5", "7,7,8|7,7,7|8,8,8", Levels.One, TestName = "JewelLand-ShouldCreateCorrectRespinWheel-0-Respin 2", ExpectedResult = "7,7,8,7,7,7,8,8,8")]
        [TestCase("5,5,5|5,5,5|2,2,3", "5,5,5|5,5,5|4,4,4", Levels.One, TestName = "JewelLand-ShouldCreateCorrectRespinWheel-0-Respin-3", ExpectedResult = "5,5,5,5,5,5,4,4,4")]
        public string EngineShouldCreateCorrectRespinWheel(string wheelString, string newWheelString, int level)
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
                }
            });

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, spinBet.LineBet, spinBet.Lines, spinBet.Multiplier);
            var stackedReels = MainGameEngine.GetStackedReels(wheel, config.PayTable);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(stackedReels);
            var spinResult = new SpinResult(spinBet, wheel, winPositions, bonusPositions);
            var newWheel = new Wheel(Game.WheelWidth, Game.WheelHeight, newWheelString.ToFormattedWheelString());
            var generatedWheel = RespinBonusEngine.GenerateRespinWheel(spinResult, newWheel);

            return string.Join(",", generatedWheel.Reels.SelectMany(symbol => symbol));
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateRespinResult")]
        public void EngineShouldCreateRespinResult(int gameId, int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithRespinSpinResult(level);
            var requestContext = GetMockBonusRequestContext(0, gameId);

            Assert.DoesNotThrow(() => RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config));
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ShouldCreateRespinBonusResult")]
        public void EngineShouldCreateRespinBonusResult(int gameId, int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithRespinSpinResult(level);
            var requestContext = GetMockBonusRequestContext(0, gameId);
            var bonus = RespinBonusEngine.CreateRespinBonus(spinResult);
            var respinResult = RespinBonusEngine.CreateRespinResult(spinResult, level, requestContext, config);
            bonus.UpdateBonus(respinResult);


            Assert.DoesNotThrow(() => RespinBonusEngine.CreateRespinBonusResult(bonus, respinResult));
        }
    }
}
