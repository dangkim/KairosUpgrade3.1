using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using Slot.Model;
using static Slot.UnitTests.JewelLand.SpinsHelper;
using SpinResult = Slot.Games.JewelLand.Models.GameResults.Spins.SpinResult;
using SpinXml = Slot.Games.JewelLand.Models.Xml.SpinXml;

namespace Slot.UnitTests.JewelLand.GameResults.Spins
{
    [TestFixture]
    public class SpinResultTests
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
        public decimal SpinResultShouldCreateCorrectPayout(string wheelString, int level, decimal bet)
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
            var spinResult = new SpinResult(spinBet, wheel, winPositions, bonusPositions);

            return spinResult.Win;
        }

        [TestCase("2,3,3|3,3,3|3,8,4", Levels.One, 1, TestName = "JewelLand-FlagForRespin-14", ExpectedResult = false)]
        [TestCase("1,1,1|1,1,1|3,3,8", Levels.One, 1, TestName = "JewelLand-FlagForRespin-40", ExpectedResult = false)]
        [TestCase("2,2,3|7,7,8|7,7,7", Levels.One, 1, TestName = "JewelLand-FlagForRespin-0-Respin", ExpectedResult = true)]
        [TestCase("7,7,8|7,7,7|5,5,5", Levels.One, 1, TestName = "JewelLand-FlagForRespin-0-Respin 2", ExpectedResult = true)]
        [TestCase("5,5,5|5,5,5|2,2,3", Levels.One, 1, TestName = "JewelLand-FlagForRespin-0-Respin-3", ExpectedResult = true)]
        [TestCase("8,8,8|8,8,8|8,8,8", Levels.One, 1, TestName = "JewelLand-FlagForRespin-0-RespinMultiplier", ExpectedResult = false)]
        public bool SpinResultShouldCorrectlyFlagForRespin(string wheelString, int level, decimal bet)
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
            var spinResult = new SpinResult(spinBet, wheel, winPositions, bonusPositions);

            return spinResult.HasRespinBonus;
        }

        [TestCase("2,3,3|3,3,3|3,8,4", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-14", ExpectedResult = false)]
        [TestCase("1,1,1|1,1,1|3,3,8", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-40", ExpectedResult = false)]
        [TestCase("2,2,3|7,7,8|7,7,7", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-0-Respin", ExpectedResult = false)]
        [TestCase("7,7,8|7,7,7|5,5,5", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-0-Respin 2", ExpectedResult = false)]
        [TestCase("5,5,5|5,5,5|2,2,3", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-0-Respin 3", ExpectedResult = false)]
        [TestCase("8,8,8|8,8,8|8,8,8", Levels.One, 1, TestName = "JewelLand-FlagForMultiplier-0-RespinMultiplier", ExpectedResult = true)]
        public bool SpinResultShouldCorrectlyFlagForMultiplier(string wheelString, int level, decimal bet)
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
            var spinResult = new SpinResult(spinBet, wheel, winPositions, bonusPositions);

            return spinResult.HasMultiplierBonus;
        }

        [TestCase(Levels.One, TestName = "JewelLand-ShouldFlagSpinResultWithBonusOnScatter")]
        public void SpinResultShouldFlagSpinResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsTrue(spinResult.IsBonus);
        }

        [TestCase(Levels.One, TestName = "JewelLand-ShouldHaveSameRespinBonusDetailsOnResultUpdate")]
        public void SpinResultShouldHaveSameRespinBonusDetailsOnResultUpdate(int level)
        {
            var spinResult = GenerateWithRespinSpinResult(level);
            var respinBonus = RespinBonusEngine.CreateRespinBonus(spinResult);

            respinBonus.UpdateBonus(spinResult);
            spinResult.UpdateBonus(respinBonus);

            var isEqualBonusId = spinResult.BonusElement.Id == respinBonus.Id;
            var isEqualBonusGuid = spinResult.BonusElement.Value == respinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "JewelLand-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "JewelLand-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var responseXml = spinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "JewelLand-ReadResponseXmlOfSpinResult")]
        public void EngineShouldReadResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

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
