using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using Slot.Games.PhantomThief.Models.GameResults.Spins;
using Slot.Model;
using static Slot.UnitTests.PhantomThief.SpinsHelper;

namespace Slot.UnitTests.PhantomThief.GameResults.Spins
{
    [TestFixture]
    public class FreeSpinResultTests
    {
        [TestCase("1,7,2|0,7,1|3,7,1|1,7,2|3,7,2", Levels.One, 1, TestName = "PhantomThief-PayoutTest-LevelOne-Bet1-935", ExpectedResult = 935)]
        [TestCase("5,2,6|0,4,5|5,6,3|6,1,5|4,6,3", Levels.One, 1, TestName = "PhantomThief-PayoutTest-LevelOne-Bet1-390", ExpectedResult = 390)]
        [TestCase("3,0,1|7,8,1|3,8,2|1,7,2|7,2,1", Levels.One, 1, TestName = "PhantomThief-PayoutTest-LevelOne-Bet1-0", ExpectedResult = 0)]
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
            var winPositions = FreeSpinBonusEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.FreeGamePayTable,
                                                    config.FreeGameScatterSymbols,
                                                    wheel,
                                                    spinBet.LineBet,
                                                    spinBet.Lines,
                                                    1);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var freeSpinResult = new FreeSpinResult(spinBet, wheel, null, winPositions, bonusPositions);

            return freeSpinResult.Win;
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldFlagSpinResultWithBonusOnScatter")]
        public void FreeSpinResultShouldFlagSpinResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsTrue(spinResult.HasFeatureBonus);
        }

        [TestCase("1,7,2|0,7,1|3,7,1|1,7,2|3,7,2", Levels.One, 1, TestName = "PhantomThief-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-935", ExpectedResult = true)]
        [TestCase("5,2,6|0,4,5|5,6,3|6,1,5|4,6,3", Levels.One, 1, TestName = "PhantomThief-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-1081", ExpectedResult = true)]
        [TestCase("3,0,1|7,8,1|3,8,2|1,7,2|7,2,1", Levels.One, 1, TestName = "PhantomThief-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-0", ExpectedResult = false)]
        [TestCase("6,8,0|7,8,1|3,8,2|1,7,2|7,2,1", Levels.One, 1, TestName = "PhantomThief-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-0-Bonus", ExpectedResult = true)]
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
            var winPositions = FreeSpinBonusEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.FreeGamePayTable,
                                                    config.FreeGameScatterSymbols,
                                                    wheel,
                                                    spinBet.LineBet,
                                                    spinBet.Lines,
                                                    1);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var freeSpinResult = new FreeSpinResult(spinBet, wheel, null, winPositions, bonusPositions);

            return freeSpinResult.HasBonus;
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void FreeSpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult, 1);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = freeSpinResult.BonusElement.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = freeSpinResult.BonusElement.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldHaveSameCountersOnBonusElementOnResultUpdate")]
        public void FreeSpinResultShouldHaveSameCountersOnBonusElementOnResultUpdate(int level)
        {
            var freeSpinResult = GenerateWinningNonBonusFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult, 0);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualCurrentFreeSpinCounter = freeSpinResult.BonusElement.Count == freeSpinBonus.Counter;
            var isEqualFreeSpinCount = freeSpinResult.BonusElement.AdditionalFreeSpinCount == freeSpinBonus.NumOfFreeSpin;

            Assert.IsTrue(isEqualFreeSpinCount && isEqualCurrentFreeSpinCounter);
        }
    }
}
