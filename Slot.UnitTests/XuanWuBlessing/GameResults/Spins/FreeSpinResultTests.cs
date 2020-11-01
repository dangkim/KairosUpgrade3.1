using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Configuration.Bonuses;
using Slot.Games.XuanWuBlessing.Engines;
using Slot.Games.XuanWuBlessing.Models.GameResults.Spins;
using Slot.Model;
using System.Linq;
using static Slot.UnitTests.XuanWuBlessing.SpinsHelper;

namespace Slot.UnitTests.XuanWuBlessing.GameResults.Spins
{
    [TestFixture]
    public class FreeSpinResultTests
    {
        [TestCase("6,0,8|12,12,5|6,8,11|13,13,13|13,13,13", 6, 1, 1, Levels.One, 1, TestName = "XuanWuBlessing-PayoutTest-LevelOne-Bet1-155", ExpectedResult = 155)]
        [TestCase("2,7,5|2,2,8|2,5,6|2,1,11|2,6,1", 2, 1, 2, Levels.One, 1, TestName = "XuanWuBlessing-PayoutTest-LevelOne-Bet1-55", ExpectedResult = 55)]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,10", 2, 1, 3, Levels.One, 1, TestName = "XuanWuBlessing-PayoutTest-LevelOne-Bet1-255", ExpectedResult = 255)]
        [TestCase("5,3,9|12,1,3|3,5,12|8,10,3|6,2,10", 2, 1, 4, Levels.One, 1, TestName = "XuanWuBlessing-PayoutTest-LevelOne-Bet1-40", ExpectedResult = 40)]
        public decimal FreeSpinResultShouldCreateCorrectPayout(string wheelString, int replacementSymbol, int freeSpinSelection, int freeSpinMultiplier, int level, decimal bet)
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

            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);

            FreeSpinResult freeSpinResult = null;
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var stackedReels = MainGameEngine.GetStackedSymbols(wheel);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var hasWildStackedReels = stackedReels.Any(sr => sr.Symbol == Symbols.Wild);
            var multiplier = hasWildStackedReels ? freeSpinMultiplier : spinBet.Multiplier;

            if (wheel.CountDistinct(Symbols.Mystery) > 0)
            {
                var replacedMysterySymbolWheel = MainGameEngine.GenerateReplacedMysterySymbolWheel(config, wheel, replacementSymbol);
                var winPositions = MainGameEngine.GenerateWinPositions(
                                    config.Payline,
                                    config.PayTable,
                                    replacedMysterySymbolWheel,
                                    spinBet.LineBet,
                                    spinBet.Lines,
                                    multiplier);

                freeSpinResult = new FreeSpinResult(level, spinBet, wheel, winPositions, bonusPositions, multiplier, replacementSymbol);
            }
            else /// Calculate wins with initial wheel
            {
                var winPositions = MainGameEngine.GenerateWinPositions(
                                    config.Payline,
                                    config.PayTable,
                                    wheel,
                                    spinBet.LineBet,
                                    spinBet.Lines,
                                    multiplier);

                freeSpinResult = new FreeSpinResult(level, spinBet, wheel, winPositions, bonusPositions, multiplier);
            }

            return freeSpinResult.Win;
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldFlagSpinResultWithBonusOnScatter")]
        public void FreeSpinResultShouldFlagSpinResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsTrue(spinResult.HasBonus);
        }

        [TestCase("6,0,8|12,12,5|6,8,11|13,13,13|13,13,13", 6, 1, 1, Levels.One, 1, TestName = "XuanWuBlessing-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-155", ExpectedResult = false)]
        [TestCase("2,7,5|2,2,11|2,5,6|2,1,11|2,6,11", 2, 1, 2, Levels.One, 1, TestName = "XuanWuBlessing-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-55", ExpectedResult = true)]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,10", 2, 1, 3, Levels.One, 1, TestName = "XuanWuBlessing-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-255", ExpectedResult = false)]
        [TestCase("5,3,9|12,1,3|3,5,12|8,10,3|6,2,10", 2, 1, 4, Levels.One, 1, TestName = "XuanWuBlessing-ShouldFlagFreeSpinResultWithBonusOnScatterOrCollapse-40", ExpectedResult = false)]
        public bool FreeSpinResultShouldFlagSpinResultWithBonusOnScatterOrCollapse(string wheelString, int replacementSymbol, int freeSpinSelection, int freeSpinMultiplier, int level, decimal bet)
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
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);

            FreeSpinResult freeSpinResult = null;
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var stackedReels = MainGameEngine.GetStackedSymbols(wheel);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var hasWildStackedReels = stackedReels.Any(sr => sr.Symbol == Symbols.Wild);
            var multiplier = hasWildStackedReels ? freeSpinMultiplier : spinBet.Multiplier;

            if (wheel.CountDistinct(Symbols.Mystery) > 0)
            {
                var replacedMysterySymbolWheel = MainGameEngine.GenerateReplacedMysterySymbolWheel(config, wheel, replacementSymbol);
                var winPositions = MainGameEngine.GenerateWinPositions(
                                    config.Payline,
                                    config.PayTable,
                                    replacedMysterySymbolWheel,
                                    spinBet.LineBet,
                                    spinBet.Lines,
                                    multiplier);

                freeSpinResult = new FreeSpinResult(level, spinBet, wheel, winPositions, bonusPositions, multiplier, replacementSymbol);
            }
            else /// Calculate wins with initial wheel
            {
                var winPositions = MainGameEngine.GenerateWinPositions(
                                    config.Payline,
                                    config.PayTable,
                                    wheel,
                                    spinBet.LineBet,
                                    spinBet.Lines,
                                    multiplier);

                freeSpinResult = new FreeSpinResult(level, spinBet, wheel, winPositions, bonusPositions, multiplier);
            }

            return freeSpinResult.HasBonus;
        }


        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void FreeSpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = freeSpinResult.BonusElement.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = freeSpinResult.BonusElement.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldHaveSameCountersOnBonusElementOnResultUpdate")]
        public void FreeSpinResultShouldHaveSameCountersOnBonusElementOnResultUpdate(int level)
        {
            var freeSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(freeSpinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            freeSpinBonus.UpdateBonus(freeSpinResult);
            freeSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualCurrentFreeSpinCounter = freeSpinResult.BonusElement.Count == freeSpinBonus.Counter;
            var isEqualFreeSpinCount = freeSpinResult.BonusElement.NumberOfFreeSpin == freeSpinBonus.NumberOfFreeSpin;

            Assert.IsTrue(isEqualFreeSpinCount && isEqualCurrentFreeSpinCounter);
        }
    }
}
