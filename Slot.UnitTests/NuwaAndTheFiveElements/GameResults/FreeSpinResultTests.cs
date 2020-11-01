using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Model;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.GameResults
{
    [TestFixture]
    public class FreeSpinResultTests
    {
        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-1935", ExpectedResult = 3870)]
        [TestCase("0,1,2,2,4,5,6,0,6,5,4,3,2,1,0,2,5,6,7,3,1,0,1,4,6,2,2,1,0,4,2,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-850", ExpectedResult = 1700)]
        [TestCase("6,1,2,3,4,5,6,4,6,5,4,8,2,1,0,2,6,6,7,3,1,3,6,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-80", ExpectedResult = 160)]
        [TestCase("1,3,2,2,4,5,6,1,6,5,4,3,2,1,1,2,5,6,7,3,1,3,0,4,6,2,2,1,7,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-60", ExpectedResult = 120)]
        [TestCase("0,1,2,3,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-20", ExpectedResult = 40)]
        [TestCase("0,1,2,1,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-0", ExpectedResult = 0)]
        [TestCase("0,1,2,11,4,5,6,4,1,5,4,11,2,1,0,2,0,6,7,11,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-1", ExpectedResult = 150)]
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
                    Multiplier = 2
                }
            });

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                spinBet.Multiplier);

            var freeSpinResult = new Games.NuwaAndTheFiveElements.Models.GameResults.Spins.FreeSpinResult(level, spinBet, wheel, null, winPositions, null, null);

            return freeSpinResult.Win;
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldFlagSpinResultWithBonusOnScatter")]
        public void FreeSpinResultShouldFlagSpinResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsTrue(spinResult.HasFeatureBonus);
        }

        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,13", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldHaveBombCollapseWhenReelsHaveBombAndWin-True", ExpectedResult = true)]
        [TestCase("0,1,2,4,4,5,6,4,1,5,4,7,2,1,0,2,0,6,7,5,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldHaveBombCollapseWhenReelsHaveBombAndWin-False", ExpectedResult = false)]
        [TestCase("0,1,2,11,4,5,6,4,1,5,4,11,2,1,0,2,0,6,7,11,1,3,0,4,6,2,2,1,0,4,0,6,1,2,13", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldHaveBombCollapseWhenReelsHaveBombAndWin-False-HasFeature", ExpectedResult = false)]
        public bool FreeSpinResultShouldHaveBombCollapseWhenReelsHaveBombAndWin(string wheelString, int level, decimal bet)
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

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                spinBet.Multiplier);
            var bombAndStopperPositions = MainGameEngine.GenerateBombAndStopperPositions(wheel, winPositions);

            var freeSpinResult = new Games.NuwaAndTheFiveElements.Models.GameResults.Spins.FreeSpinResult(level, spinBet, wheel, null, winPositions, null, bombAndStopperPositions);

            return freeSpinResult.HasBomb && freeSpinResult.Collapse;
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void FreeSpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = freeSpinResult.Bonus.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = freeSpinResult.Bonus.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldHaveSameCountersOnBonusElementOnResultUpdate")]
        public void FreeSpinResultShouldHaveSameCountersOnBonusElementOnResultUpdate(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualCurrentFreeSpinCounter = freeSpinResult.Bonus.Count == freeSpinBonus.Counter;
            var isEqualFreeSpinCount = freeSpinResult.Bonus.NumberOfFreeSpin == freeSpinBonus.NumberOfFreeSpin;
            var isEqualAdditionaFreeSpinCount = freeSpinResult.Bonus.AdditionalFreeSpinCount == freeSpinBonus.AdditionalFreeSpinCount;

            Assert.IsTrue(isEqualFreeSpinCount && isEqualCurrentFreeSpinCounter);
        }
    }
}
