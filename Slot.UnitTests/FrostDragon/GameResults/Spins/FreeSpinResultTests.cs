using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using Slot.Games.FrostDragon.Models.GameResults.Spins;
using Slot.Model;
using System.Linq;
using static Slot.UnitTests.FrostDragon.SpinsHelper;

namespace Slot.UnitTests.FrostDragon.GameResults.Spins
{
    [TestFixture]
    public class FreeSpinResultTests
    {
        [TestCase("6,6,1|3,6,5|1,6,6|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-500", ExpectedResult = 1725)]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-250", ExpectedResult = 750)]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,8,1", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-250-Wild", ExpectedResult = 750)]
        [TestCase("0,1,4|3,1,5|6,1,2|3,1,5|4,1,2", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-20", ExpectedResult = 60)]
        [TestCase("0,1,4|3,4,5|6,4,2|3,4,5|3,1,4", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-80", ExpectedResult = 240)]
        [TestCase("0,2,1|3,4,5|1,6,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-PayoutTest-LevelOne-Bet1-0", ExpectedResult = 0)]
        public decimal FreeSpinResultShouldCreateCorrectPayout(string wheelString, int level, decimal bet)
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
                                                config.BonusConfig.FreeSpin.Multipliers.First());
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var freeSpinResult = new FreeSpinResult(spinBet, wheel, null, winPositions, bonusPositions, config.BonusConfig.FreeSpin.Multipliers.First());

            return freeSpinResult.Win;
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldFlagSpinResultWithBonusOnScatter")]
        public void FreeSpinResultShouldFlagSpinResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsTrue(spinResult.HasFeatureBonus);
        }

        [TestCase("0,7,1|3,7,5|1,7,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-Scatter-1", ExpectedResult = true)]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-Collapse-1", ExpectedResult = true)]
        [TestCase("0,2,1|3,4,5|1,6,2|3,6,1|4,6,1", Levels.One, 1, TestName = "FrostDragon-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-NoWin-1", ExpectedResult = false)]
        public bool FreeSpinResultShouldFlagSpinResultWithBonusOnScatterOrCollapse(string wheelString, int level, decimal bet)
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
                                                config.BonusConfig.FreeSpin.Multipliers.First());
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var freeSpinResult = new FreeSpinResult(spinBet, wheel, null, winPositions, bonusPositions, config.BonusConfig.Collapse.Multipliers.First());

            return freeSpinResult.HasBonus;
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void FreeSpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = freeSpinResult.BonusElement.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = freeSpinResult.BonusElement.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldHaveSameCountersOnBonusElementOnResultUpdate")]
        public void FreeSpinResultShouldHaveSameCountersOnBonusElementOnResultUpdate(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualCurrentFreeSpinCounter = freeSpinResult.BonusElement.Count == freeSpinBonus.Counter;
            var isEqualFreeSpinCount = freeSpinResult.BonusElement.AdditionalFreeSpinCount == freeSpinBonus.NumOfFreeSpin;

            Assert.IsTrue(isEqualFreeSpinCount && isEqualCurrentFreeSpinCounter);
        }
    }
}
