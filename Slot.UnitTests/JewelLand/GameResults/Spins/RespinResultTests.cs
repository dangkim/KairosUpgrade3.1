using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using Slot.Games.JewelLand.Models.GameResults.Spins;
using Slot.Model;
using static Slot.UnitTests.JewelLand.SpinsHelper;
using SpinXml = Slot.Games.JewelLand.Models.Xml.SpinXml;

namespace Slot.UnitTests.JewelLand.GameResults.Spins
{
    [TestFixture]
    public class RespinResultTests
    {
        [TestCase("2,3,3|3,3,3|3,8,4", Levels.One, 1, TestName = "JewelLand-PayoutTest-LevelOne-Bet1-14", ExpectedResult = 14)]
        [TestCase("1,2,2|1,1,1|1,2,2", Levels.One, 1, TestName = "JewelLand-PayoutTest-LevelOne-Bet1-20", ExpectedResult = 20)]
        [TestCase("1,1,1|1,1,1|3,3,8", Levels.One, 1, TestName = "JewelLand-PayoutTest-LevelOne-Bet1-40", ExpectedResult = 40)]
        [TestCase("2,2,3|7,7,8|7,7,7", Levels.One, 1, TestName = "JewelLand-PayoutTest-LevelOne-Bet1-0-Respin", ExpectedResult = 0)]
        [TestCase("7,7,8|7,7,7|5,5,5", Levels.One, 1, TestName = "JewelLand-PayoutTest-LevelOne-Bet1-0-Respin 2", ExpectedResult = 0)]
        [TestCase("5,5,5|5,5,5|2,2,3", Levels.One, 1, TestName = "JewelLand-PayoutTest-LevelOne-Bet1-0-Respin 3", ExpectedResult = 0)]
        [TestCase("8,8,8|8,8,8|8,8,8", Levels.One, 1, TestName = "JewelLand-PayoutTest-LevelOne-Bet1-0-RespinMultiplier", ExpectedResult = 0)]
        [TestCase("3,3,8|7,7,7|8,8,8", Levels.One, 1, TestName = "JewelLand-PayoutTest-LevelOne-Bet1-4", ExpectedResult = 4)]
        [TestCase("3,3,8|7,7,8|8,7,8", Levels.One, 1, TestName = "JewelLand-PayoutTest-LevelOne-Bet1-82", ExpectedResult = 82)]
        public decimal RespinResultShouldCreateCorrectPayout(string wheelString, int level, decimal bet)
        {
            var config = new Configuration();
            var spinBet = MainGameEngine.GenerateSpinBet(new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = bet,
                    Multiplier = 1
                }
            });

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, spinBet.LineBet, spinBet.Lines, spinBet.Multiplier);
            var stackedReels = MainGameEngine.GetStackedReels(wheel, config.PayTable);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(stackedReels);
            var respinResult = new RespinResult(spinBet, wheel, winPositions, bonusPositions);

            return respinResult.Win;
        }

        [TestCase("2,3,3|3,3,3|3,8,4", Levels.One, 1, TestName = "JewelLand-FlagForRespin-14", ExpectedResult = false)]
        [TestCase("1,1,1|1,1,1|3,3,8", Levels.One, 1, TestName = "JewelLand-FlagForRespin-40", ExpectedResult = false)]
        [TestCase("2,2,3|7,7,8|7,7,7", Levels.One, 1, TestName = "JewelLand-FlagForRespin-0-Respin", ExpectedResult = false)]
        [TestCase("7,7,8|7,7,7|5,5,5", Levels.One, 1, TestName = "JewelLand-FlagForRespin-0-Respin 2", ExpectedResult = false)]
        [TestCase("5,5,5|5,5,5|2,2,3", Levels.One, 1, TestName = "JewelLand-FlagForRespin-0-Respin-3", ExpectedResult = false)]
        [TestCase("8,8,8|8,8,8|8,8,8", Levels.One, 1, TestName = "JewelLand-FlagForRespin-0-RespinMultiplier", ExpectedResult = false)]
        public bool RespinResultShouldCorrectlyFlagForRespin(string wheelString, int level, decimal bet)
        {
            var config = new Configuration();
            var spinBet = MainGameEngine.GenerateSpinBet(new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = bet,
                    Multiplier = 1
                }
            });

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, spinBet.LineBet, spinBet.Lines, spinBet.Multiplier);
            var stackedReels = MainGameEngine.GetStackedReels(wheel, config.PayTable);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(stackedReels);
            var respinResult = new RespinResult(spinBet, wheel, winPositions, bonusPositions);

            return respinResult.HasRespinBonus;
        }

        [TestCase("2,3,3|3,3,3|3,8,4", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-14", ExpectedResult = false)]
        [TestCase("1,1,1|1,1,1|3,3,8", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-40", ExpectedResult = false)]
        [TestCase("2,2,3|7,7,8|7,7,7", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-0-Respin", ExpectedResult = false)]
        [TestCase("7,7,8|7,7,7|5,5,5", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-0-Respin 2", ExpectedResult = false)]
        [TestCase("5,5,5|5,5,5|2,2,3", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-0-Respin 3", ExpectedResult = false)]
        [TestCase("8,8,8|8,8,8|8,8,8", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-0-RespinMultiplier", ExpectedResult = true)]
        public bool RespinResultShouldCorrectlyFlagForMultiplier(string wheelString, int level, decimal bet)
        {
            var config = new Configuration();
            var spinBet = MainGameEngine.GenerateSpinBet(new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = bet,
                    Multiplier = 1
                }
            });

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, spinBet.LineBet, spinBet.Lines, spinBet.Multiplier);
            var stackedReels = MainGameEngine.GetStackedReels(wheel, config.PayTable);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(stackedReels);
            var RespinResult = new RespinResult(spinBet, wheel, winPositions, bonusPositions);

            return RespinResult.HasMultiplierBonus;
        }

        [TestCase(Levels.One, TestName = "JewelLand-ShouldFlagRespinResultWithBonusOnScatter")]
        public void RespinResultShouldFlagRespinResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var respinResult = GenerateWithBonusSpinResult(level);

            Assert.IsTrue(respinResult.IsBonus);
        }

        [TestCase(Levels.One, TestName = "JewelLand-ShouldHaveSameRespinBonusDetailsOnResultUpdate")]
        public void RespinResultShouldHaveSameRespinBonusDetailsOnResultUpdate(int level)
        {
            var respinResult = GenerateWithRespinSpinResult(level);
            var respinBonus = RespinBonusEngine.CreateRespinBonus(respinResult);

            respinBonus.UpdateBonus(respinResult);
            respinResult.UpdateBonus(respinBonus);

            var isEqualBonusId = respinResult.BonusElement.Id == respinBonus.Id;
            var isEqualBonusGuid = respinResult.BonusElement.Value == respinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "JewelLand-CreateBonusXElementOfRespinResult")]
        public void EngineShouldCreateBonusXElementOfRespinResult(int level)
        {
            var respinResult = GenerateWinningSpinResult(level);
            var xElement = respinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "JewelLand-CreateBonusResponseXmlOfRespinResult")]
        public void EngineShouldCreateBonusResponseXmlOfRespinResult(int level)
        {
            var RespinResult = GenerateWinningSpinResult(level);
            var responseXml = RespinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "JewelLand-ReadResponseXmlOfRespinResult")]
        public void EngineShouldReadResponseXmlOfRespinResult(int level)
        {
            var respinResult = GenerateWinningSpinResult(level);
            var xElement = respinResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                using (var xmlReader = xElement.CreateReader())
                {
                    var responseXml = new SpinXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }
    }
}
