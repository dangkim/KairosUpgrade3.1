using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using Slot.Games.FrostDragon.Models.GameResults.Spins;
using Slot.Model;
using System;
using System.Linq;
using static Slot.UnitTests.FrostDragon.SpinsHelper;

namespace Slot.UnitTests.FrostDragon.GameResults.Spins
{
    [TestFixture]
    public class FreeSpinCollapsingResultTests
    {
        [TestCase(Levels.One, TestName = "FrostDragon-FreeSpinCollapsingResultSameRoundId")]
        public void EngineShouldCreateCollapsingResultSameRoundId(int level)
        {
            var config = new Configuration();

            var spinResult = GenerateWinningFreeSpinResult(level);
            var referenceWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var freeSpinCollapsingResult = GenerateWinningFreeSpinCollapsingResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinCollapsingResult);
            freeSpinBonus.UpdateBonus(freeSpinCollapsingResult);

            var spinResultCollapsingResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, referenceWheel, config.BonusConfig.FreeSpin.Multipliers, config.Payline, config.PayTable);
            var freeSpinCollapsingResultCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinCollapsingResult);

            Assert.IsTrue(spinResult.RoundId == spinResultCollapsingResult.RoundId);
            Assert.IsTrue(freeSpinCollapsingResultCollapsingResult.RoundId == freeSpinCollapsingResult.RoundId);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CollapseFreeSpinCollapsingResultOnWin")]
        public void EngineShouldCollapseCollapsingResultOnWin(int level)
        {
            var config = new Configuration();
            var freeSpinCollapsingResult = GenerateWinningNonBonusFreeSpinCollapsingResult(level);

            Assert.IsTrue(freeSpinCollapsingResult.Collapse);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-DoNotCollapseFreeSpinCollapsingResultOnLose")]
        public void EngineShouldNotCollapseCollapsingResultOnLose(int level)
        {
            var config = new Configuration();
            var freeSpinCollapsingResult = GenerateNonWinningCollapsingSpinResult(level);

            Assert.IsTrue(!freeSpinCollapsingResult.Collapse);
        }

        [TestCase("6,0,5|4,0,2|1,0,6|8,0,3|2,0,3", "2,3,3,12,6", Levels.One, TestName = "FrostDragon-CreateCorrectCollapseReels-1", ExpectedResult = "6,6,5|4,4,2|1,1,6|8,8,3|2,2,3")]
        [TestCase("4,1,3|4,1,7|2,1,3|3,5,2|6,3,4", "26,7,11,24,12", Levels.One, TestName = "FrostDragon-CreateCorrectCollapseReels-2", ExpectedResult = "4,4,3|4,4,7|2,2,3|3,5,2|6,3,4")]
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

            var targetWheel = config.WeightedReelStripsCollection.FirstOrDefault().Wheel;
            var topIndices = Array.ConvertAll(indicesString.Split(','), Convert.ToInt32).ToList();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, spinBet.LineBet, spinBet.Lines, config.BonusConfig.FreeSpin.Multipliers.First());
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var freeSpinResult = new FreeSpinResult(spinBet, wheel, topIndices, winPositions, bonusPositions, config.BonusConfig.FreeSpin.Multipliers.First());
            var freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(freeSpinResult, targetWheel, config.BonusConfig.FreeSpin.Multipliers, config.Payline, config.PayTable);

            return string.Join('|', freeSpinCollapsingResult.Wheel.Reels.Select(symbols => string.Join(',', symbols)));
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldFlagCollapsingResultWithBonusOnScatter")]
        public void EngineShouldFlagCollapsingResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var freeSpinCollapsingResult = GenerateWithBonusFreeSpinCollapsingResult(level);

            Assert.IsTrue(freeSpinCollapsingResult.HasFeatureBonus);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void FreeSpinCollapsingResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var freeSpinCollapsingResult = GenerateWinningFreeSpinCollapsingResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinCollapsingResult);

            freeSpinBonus.UpdateBonus(freeSpinCollapsingResult);
            freeSpinCollapsingResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = freeSpinCollapsingResult.BonusElement.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = freeSpinCollapsingResult.BonusElement.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldHaveSameCountersOnBonusElementOnResultUpdate")]
        public void FreeSpinCollapsingShouldHaveSameCountersOnBonusElementOnResultUpdate(int level)
        {
            var freeSpinCollapsingResult = GenerateWinningFreeSpinCollapsingResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinCollapsingResult);

            freeSpinBonus.UpdateBonus(freeSpinCollapsingResult);
            freeSpinCollapsingResult.UpdateBonus(freeSpinBonus);

            var isEqualCurrentFreeSpinCounter = freeSpinCollapsingResult.BonusElement.Count == freeSpinBonus.Counter;
            var isEqualFreeSpinCount = freeSpinCollapsingResult.BonusElement.AdditionalFreeSpinCount == freeSpinBonus.NumOfFreeSpin;

            Assert.IsTrue(isEqualFreeSpinCount && isEqualCurrentFreeSpinCounter);
        }

        [TestCase(Levels.One, 0, TestName = "FrostDragon-ShouldRetrieveCorrectFreeSpinCollapseMultiplier-0", ExpectedResult = 3)]
        [TestCase(Levels.One, 1, TestName = "FrostDragon-ShouldRetrieveCorrectFreeSpinCollapseMultiplier-1", ExpectedResult = 6)]
        [TestCase(Levels.One, 2, TestName = "FrostDragon-ShouldRetrieveCorrectFreeSpinCollapseMultiplier-2", ExpectedResult = 9)]
        [TestCase(Levels.One, 3, TestName = "FrostDragon-ShouldRetrieveCorrectFreeSpinCollapseMultiplier-3", ExpectedResult = 15)]
        [TestCase(Levels.One, 4, TestName = "FrostDragon-ShouldRetrieveCorrectFreeSpinCollapseMultiplier-4", ExpectedResult = 15)]
        [TestCase(Levels.One, 6, TestName = "FrostDragon-ShouldRetrieveCorrectFreeSpinCollapseMultiplier-7", ExpectedResult = 15)]
        public int EngineShouldRetrieveCorrectFreeSpinCollapseMultiplier(int level, int iterations)
        {
            var config = new Configuration();

            var targetMultiplier = CollapsingBonusEngine.GetCollapseMultiplier(config.BonusConfig.FreeSpin.Multipliers, iterations);
            return targetMultiplier;
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldIncreaseWinPositionMultiplierOnFreeSpinSpinCollapse")]
        public void EngineShouldIncreaseWinPositionMultiplierOnFreeSpinSpinCollapse(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateWinningFreeSpinResult(level) as Games.FrostDragon.Models.GameResults.Spins.SpinResult;
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, freeSpinResult.Wheel.ReelStripsId);

            FreeSpinCollapsingResult freeSpinCollapsingResult = null;

            do
            {
                freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(freeSpinResult, targetWheel, config.BonusConfig.FreeSpin.Multipliers, config.Payline, config.PayTable);

                if (freeSpinCollapsingResult.Win > 0)
                    freeSpinResult = freeSpinCollapsingResult;

            } while (freeSpinCollapsingResult.Win > 0);

            var targetMultiplier = freeSpinResult.AvalancheMultiplier;
            var targetWinPosition = freeSpinResult.WinPositions.FirstOrDefault();
            var symbolMultiplier = config.PayTable.GetOdds(targetWinPosition.Symbol, targetWinPosition.Count);

            Assert.IsTrue(symbolMultiplier * targetMultiplier == targetWinPosition.Multiplier);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCreateCollapsingFreeSpinResultWithCorrectMultiplier")]
        public void EngineShouldCreateCollapsingFreeSpinResultWithCorrectMultiplier(int level)
        {
            var config = new Configuration();
            var freeSpinResult = GenerateWinningFreeSpinResult(level) as Games.FrostDragon.Models.GameResults.Spins.SpinResult;
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, freeSpinResult.Wheel.ReelStripsId);

            FreeSpinCollapsingResult freeSpinCollapsingResult = null;

            do
            {
                freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(freeSpinResult, targetWheel, config.BonusConfig.FreeSpin.Multipliers, config.Payline, config.PayTable);

                if (freeSpinCollapsingResult.Win > 0)
                    freeSpinResult = freeSpinCollapsingResult;

            } while (freeSpinCollapsingResult.Win > 0);

            var targetMultiplier = CollapsingBonusEngine.GetCollapseMultiplier(config.BonusConfig.FreeSpin.Multipliers, freeSpinCollapsingResult.CollapsingSpinCount - 1);

            Assert.IsTrue(freeSpinResult.AvalancheMultiplier == targetMultiplier);
        }
    }
}
