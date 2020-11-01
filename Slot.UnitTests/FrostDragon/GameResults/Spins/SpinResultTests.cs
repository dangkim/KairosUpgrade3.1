using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using Slot.Model;
using System.Linq;
using static Slot.UnitTests.FrostDragon.SpinsHelper;
using SpinResult = Slot.Games.FrostDragon.Models.GameResults.Spins.SpinResult;
using SpinXml = Slot.Games.FrostDragon.Models.Xml.SpinXml;

namespace Slot.UnitTests.FrostDragon.GameResults.Spins
{
    [TestFixture]
    public class SpinResultTests
    {
        [TestCase("6,6,1|3,6,5|1,6,6|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-500", ExpectedResult = 575)]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-250", ExpectedResult = 250)]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-250", ExpectedResult = 250)]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,8,1", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-250-Wild", ExpectedResult = 250)]
        [TestCase("0,1,4|3,1,5|6,1,2|3,1,5|4,1,2", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-20", ExpectedResult = 20)]
        [TestCase("0,1,4|3,4,5|6,4,2|3,4,5|3,1,4", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-80", ExpectedResult = 80)]
        [TestCase("0,2,1|3,4,5|1,6,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-0", ExpectedResult = 0)]
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
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                config.BonusConfig.Collapse.Multipliers.First());
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var spinResult = new SpinResult(spinBet, wheel, null, winPositions, bonusPositions, config.BonusConfig.Collapse.Multipliers.First());

            return spinResult.Win;
        }

        [TestCase("0,7,1|3,7,5|1,7,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-ShouldFlagSpinResultWithBonusOnScatterOrCollapse-Scatter-1", ExpectedResult = true)]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-ShouldFlagSpinResultWithBonusOnScatterOrCollapse-Collapse-1", ExpectedResult = true)]
        [TestCase("0,2,1|3,4,5|1,6,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-ShouldFlagSpinResultWithBonusOnScatterOrCollapse-NoWin-1", ExpectedResult = false)]
        public bool SpinResultShouldFlagSpinResultWithBonusOnScatterOrCollapse(string wheelString, int level, decimal bet)
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
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                config.BonusConfig.Collapse.Multipliers.First());
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var spinResult = new SpinResult(spinBet, wheel, null, winPositions, bonusPositions, config.BonusConfig.Collapse.Multipliers.First());

            return spinResult.HasBonus;
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void SpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var spinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            freeSpinBonus.UpdateBonus(spinResult);
            spinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = spinResult.BonusElement.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = spinResult.BonusElement.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var responseXml = spinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ReadResponseXmlOfSpinResult")]
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
