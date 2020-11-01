using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Model;
using System;
using System.Linq;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.GameResults
{
    [TestFixture]
    public class CollapsingSpinResultTests
    {
        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CollapsingResultSameRoundId")]
        public void EngineShouldCreateCollapsingResultSameRoundId(int level)
        {
            var config = new Configuration();

            var spinResult = GenerateWinningSpinResult(level);
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);

            var spinResultCollapsingResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, config.Wheels[level], config.SymbolCollapsePairs, config.Payline, config.PayTable);
            var collapsingSpinResultCollapsingResult = CollapsingBonusEngine.CreateCollapsingSpinResult(collapsingSpinResult, config.Wheels[level], config.SymbolCollapsePairs, config.Payline, config.PayTable);

            Assert.IsTrue(spinResult.RoundId == spinResultCollapsingResult.RoundId);
            Assert.IsTrue(collapsingSpinResultCollapsingResult.RoundId == collapsingSpinResult.RoundId);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CollapseCollapsingResultOnWin")]
        public void EngineShouldCollapseCollapsingResultOnWin(int level)
        {
            var config = new Configuration();
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);

            Assert.IsTrue(collapsingSpinResult.Collapse);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-DoNotCollapseCollapsingResultOnLose")]
        public void EngineShouldNotCollapseCollapsingResultOnLose(int level)
        {
            var config = new Configuration();
            var collapsingSpinResult = GenerateNonWinningCollapsingSpinResult(level);

            Assert.IsTrue(!collapsingSpinResult.Collapse);
        }

        [TestCase("9,9,9,9,9,4,4|12,12,12,8,8,8,3|10,12,12,12,11,7,7|0,6,6,6,6,2,2|12,13,12,9,9,9,9", "19,19,19,19,13", Levels.One, TestName = "NuwaAndTheFiveElements-CreateCorrectCollapseReels-1", ExpectedResult = "9,9,9,9,9,4,4|12,12,12,8,8,8,3|10,12,12,12,11,7,7|0,6,6,6,6,2,2|12,13,12,9,9,9,9")]
        [TestCase("4,4,5,5,5,1,1|12,12,12,12,12,12,12|7,2,2,6,6,6,1|1,8,8,8,8,3,3|12,13,12,9,9,9,9", "25,15,25,25,13", Levels.One, TestName = "NuwaAndTheFiveElements-CreateCorrectCollapseReels-2", ExpectedResult = "4,4,5,5,5,1,1|12,12,12,12,12,12,12|7,2,2,6,6,6,1|1,8,8,8,8,3,3|12,13,12,9,9,9,9")]
        [TestCase("8,8,8,3,6,6,6|10,12,12,12,12,12,12|10,12,12,12,11,7,7|12,12,12,5,5,1,6|12,13,12,9,9,9,9", "0,14,19,14,13", Levels.One, TestName = "NuwaAndTheFiveElements-CreateCorrectCollapseReels-3", ExpectedResult = "0,0,8,3,6,6,6|2,9,9,9,9,4,4|10,10,10,10,11,7,7|4,11,10,5,5,1,6|3,3,10,9,9,9,9")]
        [TestCase("5,0,0,8,8,8,3|12,12,12,12,12,12,12|9,4,10,10,10,10,10|12,12,12,5,5,0,6|8,3,3,10,12,13,12", "29,15,11,14,9", Levels.One, TestName = "NuwaAndTheFiveElements-CreateCorrectCollapseReels-4", ExpectedResult = "5,0,0,8,8,8,3|12,12,12,12,12,12,12|9,4,10,10,10,10,10|12,12,12,5,5,0,6|8,3,3,10,12,13,12")]
        public string EngineShouldCreateCorrectCollapseReels(string wheelString, string indicesString, int level)
        {
            wheelString = string.Join(',', wheelString.Split('|'));
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

            var targetWheel = MainGameEngine.GetTargetWheel(level, config);
            var topIndices = Array.ConvertAll(indicesString.Split(','), Convert.ToInt32).ToList();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString);
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, spinBet.LineBet, spinBet.Lines, 1);
            var matchingSymbolPositions = MainGameEngine.GenerateMatchingSymbolPositions(config.SymbolCollapsePairs, winPositions.Select(wp => wp.Symbol).ToList(), wheel);
            var bombAndStopperPositions = MainGameEngine.GenerateBombAndStopperPositions(wheel, winPositions);

            var spinResult = new Games.NuwaAndTheFiveElements.Models.GameResults.Spins.SpinResult(level, spinBet, wheel, topIndices, winPositions, matchingSymbolPositions, bombAndStopperPositions);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, targetWheel, config.SymbolCollapsePairs, config.Payline, config.PayTable);

            return string.Join('|', collapsingSpinResult.Wheel.Reels.Select(symbols => string.Join(',', symbols)));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldFlagCollapsingResultWithBonusOnScatter")]
        public void EngineShouldFlagCollapsingResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusCollapsingSpinResult(level);

            Assert.IsTrue(spinResult.HasFeatureBonus);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void CollapsingSpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var collapsingSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(collapsingSpinResult);

            freeSpinBonus.UpdateBonus(collapsingSpinResult);
            collapsingSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = collapsingSpinResult.Bonus.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = collapsingSpinResult.Bonus.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }
    }
}
