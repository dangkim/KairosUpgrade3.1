using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using Slot.Games.FrostDragon.Models.GameResults.Spins;
using Slot.Model;
using System;
using System.Linq;
using static Slot.UnitTests.FrostDragon.SpinsHelper;
using SpinResult = Slot.Games.FrostDragon.Models.GameResults.Spins.SpinResult;

namespace Slot.UnitTests.FrostDragon.GameResults.Spins
{
    [TestFixture]
    public class CollapsingSpinResultTests
    {
        [TestCase(Levels.One, TestName = "FrostDragon-CollapsingResultSameRoundId")]
        public void EngineShouldCreateCollapsingResultSameRoundId(int level)
        {
            var config = new Configuration();

            var spinResult = GenerateWinningSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, targetWheel, config.BonusConfig.Collapse.Multipliers, config.Payline, config.PayTable);

            Assert.IsTrue(spinResult.RoundId == collapsingSpinResult.RoundId);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CollapseCollapsingResultOnWin")]
        public void EngineShouldCollapseCollapsingResultOnWin(int level)
        {
            var config = new Configuration();
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);

            Assert.IsTrue(collapsingSpinResult.Collapse);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-DoNotCollapseCollapsingResultOnLose")]
        public void EngineShouldNotCollapseCollapsingResultOnLose(int level)
        {
            var config = new Configuration();
            var collapsingSpinResult = GenerateNonWinningNonBonusCollapsingSpinResult(level);

            Assert.IsTrue(!collapsingSpinResult.Collapse);
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
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, spinBet.LineBet, spinBet.Lines, config.BonusConfig.Collapse.Multipliers.First());
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var spinResult = new SpinResult(spinBet, wheel, topIndices, winPositions, bonusPositions, config.BonusConfig.Collapse.Multipliers.First());
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, targetWheel, config.BonusConfig.Collapse.Multipliers, config.Payline, config.PayTable);

            return string.Join('|', collapsingSpinResult.Wheel.Reels.Select(symbols => string.Join(',', symbols)));
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldFlagCollapsingResultWithBonusOnScatter")]
        public void EngineShouldFlagCollapsingResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusCollapsingSpinResult(level);

            Assert.IsTrue(spinResult.HasFeatureBonus);
        }

        [TestCase(Levels.One, 0, TestName = "FrostDragon-ShouldRetrieveCorrectCollapseMultiplier-0", ExpectedResult = 1)]
        [TestCase(Levels.One, 1, TestName = "FrostDragon-ShouldRetrieveCorrectCollapseMultiplier-1", ExpectedResult = 2)]
        [TestCase(Levels.One, 2, TestName = "FrostDragon-ShouldRetrieveCorrectCollapseMultiplier-2", ExpectedResult = 3)]
        [TestCase(Levels.One, 3, TestName = "FrostDragon-ShouldRetrieveCorrectCollapseMultiplier-3", ExpectedResult = 5)]
        [TestCase(Levels.One, 4, TestName = "FrostDragon-ShouldRetrieveCorrectCollapseMultiplier-4", ExpectedResult = 5)]
        [TestCase(Levels.One, 6, TestName = "FrostDragon-ShouldRetrieveCorrectCollapseMultiplier-7", ExpectedResult = 5)]
        public int EngineShouldRetrieveCorrectCollapseMultiplier(int level, int iterations)
        {
            var config = new Configuration();
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, false);

            var targetMultiplier = CollapsingBonusEngine.GetCollapseMultiplier(config.BonusConfig.Collapse.Multipliers, iterations);
            return targetMultiplier;
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldIncreaseWinPositionMultiplierOnSpinCollapse")]
        public void EngineShouldIncreaseWinPositionMultiplierOnSpinCollapse(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);

            CollapsingSpinResult collapsingSpinResult = null;

            do
            {
                collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, targetWheel, config.BonusConfig.Collapse.Multipliers, config.Payline, config.PayTable);

                if (collapsingSpinResult.Win > 0)
                    spinResult = collapsingSpinResult;

            } while (collapsingSpinResult.Win > 0);

            var targetMultiplier = spinResult.AvalancheMultiplier;
            var targetWinPosition = spinResult.WinPositions.FirstOrDefault();
            var symbolMultiplier = config.PayTable.GetOdds(targetWinPosition.Symbol, targetWinPosition.Count);

            Assert.IsTrue(symbolMultiplier * targetMultiplier == targetWinPosition.Multiplier);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCreateCollapsingSpinResultWithCorrectMultiplier")]
        public void EngineShouldCreateCollapsingSpinResultWithCorrectMultiplier(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);

            CollapsingSpinResult collapsingSpinResult = null;

            do
            {
                collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, targetWheel, config.BonusConfig.Collapse.Multipliers, config.Payline, config.PayTable);

                if (collapsingSpinResult.Win > 0)
                    spinResult = collapsingSpinResult;

            } while (collapsingSpinResult.Win > 0);

            var targetMultiplier = CollapsingBonusEngine.GetCollapseMultiplier(config.BonusConfig.Collapse.Multipliers, collapsingSpinResult.CollapsingSpinCount - 1);

            Assert.IsTrue(spinResult.AvalancheMultiplier == targetMultiplier);
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void CollapsingSpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var collapsingSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(collapsingSpinResult);

            freeSpinBonus.UpdateBonus(collapsingSpinResult);
            collapsingSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = collapsingSpinResult.BonusElement.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = collapsingSpinResult.BonusElement.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }
    }
}
