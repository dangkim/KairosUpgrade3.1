using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using Slot.Games.PhantomThief.Models.GameResults.Spins;
using Slot.Model;
using System;
using System.Linq;
using static Slot.UnitTests.PhantomThief.SpinsHelper;

namespace Slot.UnitTests.PhantomThief.GameResults.Spins
{
    [TestFixture]
    public class FreeSpinCollapsingResultTests
    {
        [TestCase(Levels.One, TestName = "PhantomThief-FreeSpinCollapsingResultSameRoundId")]
        public void EngineShouldCreateCollapsingResultSameRoundId(int level)
        {
            var config = new Configuration();

            var spinResult = GenerateWinningFreeSpinResult(level);
            var referenceWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var freeSpinCollapsingResult = GenerateWinningFreeSpinCollapsingResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinCollapsingResult);
            freeSpinBonus.UpdateBonus(freeSpinCollapsingResult, 0);

            var spinResultCollapsingResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, referenceWheel, config.Payline, config.FreeGamePayTable);
            var freeSpinCollapsingResultCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinCollapsingResult);

            Assert.IsTrue(spinResult.RoundId == spinResultCollapsingResult.RoundId);
            Assert.IsTrue(freeSpinCollapsingResultCollapsingResult.RoundId == freeSpinCollapsingResult.RoundId);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CollapseFreeSpinCollapsingResultOnWin")]
        public void EngineShouldCollapseCollapsingResultOnWin(int level)
        {
            var config = new Configuration();
            var freeSpinCollapsingResult = GenerateWinningNonBonusFreeSpinCollapsingResult(level);

            Assert.IsTrue(freeSpinCollapsingResult.Collapse);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-DoNotCollapseFreeSpinCollapsingResultOnLose")]
        public void EngineShouldNotCollapseCollapsingResultOnLose(int level)
        {
            var config = new Configuration();
            var freeSpinCollapsingResult = GenerateNonWinningCollapsingSpinResult(level);

            Assert.IsTrue(!freeSpinCollapsingResult.Collapse);
        }

        [TestCase("1,7,2|0,7,1|3,7,1|1,7,2|3,7,2", "8,33,28,1,31", Levels.One, TestName = "PhantomThief-CreateCorrectCollapseReels-935", ExpectedResult = "0,1,2|2,0,0|0,3,3|0,1,2|3,3,2")]
        [TestCase("5,2,6|0,4,5|5,6,3|6,1,5|4,6,3", "25,7,8,6,8", Levels.One, TestName = "PhantomThief-CreateCorrectCollapseReels-390", ExpectedResult = "1,5,2|0,0,4|0,5,3|0,6,1|4,4,3")]
        public string EngineShouldCreateCorrectCollapseReels(string wheelString, string indicesString, int level)
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

            var targetWheel = config.WeightedReelStripsCollection
                                        .Where(reelStripItem => reelStripItem.Name.Contains("freegame", StringComparison.InvariantCultureIgnoreCase))
                                        .ElementAt(1)
                                        .Wheel;
            var topIndices = Array.ConvertAll(indicesString.Split(','), Convert.ToInt32).ToList();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = FreeSpinBonusEngine.GenerateWinPositions(config.Payline, config.FreeGamePayTable, config.FreeGameScatterSymbols, wheel, spinBet.LineBet, spinBet.Lines, 1);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var freeSpinResult = new FreeSpinResult(spinBet, wheel, topIndices, winPositions, bonusPositions);
            var freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(
                                                                                freeSpinResult, 
                                                                                targetWheel, 
                                                                                config.Payline, 
                                                                                config.FreeGamePayTable, 
                                                                                config.FreeGameScatterSymbols);

            return string.Join('|', freeSpinCollapsingResult.Wheel.Reels.Select(symbols => string.Join(',', symbols)));
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldFlagCollapsingResultWithBonusOnScatter")]
        public void EngineShouldFlagCollapsingResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var freeSpinCollapsingResult = GenerateWithBonusFreeSpinCollapsingResult(level);

            Assert.IsTrue(freeSpinCollapsingResult.HasFeatureBonus);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void FreeSpinCollapsingResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var freeSpinCollapsingResult = GenerateWinningFreeSpinCollapsingResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinCollapsingResult);

            freeSpinBonus.UpdateBonus(freeSpinCollapsingResult, 0);
            freeSpinCollapsingResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = freeSpinCollapsingResult.BonusElement.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = freeSpinCollapsingResult.BonusElement.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldHaveSameCountersOnBonusElementOnResultUpdate")]
        public void FreeSpinCollapsingShouldHaveSameCountersOnBonusElementOnResultUpdate(int level)
        {
            var freeSpinCollapsingResult = GenerateWinningFreeSpinCollapsingResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinCollapsingResult);

            freeSpinBonus.UpdateBonus(freeSpinCollapsingResult, 0);
            freeSpinCollapsingResult.UpdateBonus(freeSpinBonus);

            var isEqualCurrentFreeSpinCounter = freeSpinCollapsingResult.BonusElement.Count == freeSpinBonus.Counter;
            var isEqualFreeSpinCount = freeSpinCollapsingResult.BonusElement.AdditionalFreeSpinCount == freeSpinBonus.NumOfFreeSpin;

            Assert.IsTrue(isEqualFreeSpinCount && isEqualCurrentFreeSpinCounter);
        }
    }
}
