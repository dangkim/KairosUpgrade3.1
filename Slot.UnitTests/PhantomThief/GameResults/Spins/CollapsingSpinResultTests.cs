using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using Slot.Model;
using System;
using System.Linq;
using static Slot.UnitTests.PhantomThief.SpinsHelper;
using SpinResult = Slot.Games.PhantomThief.Models.GameResults.Spins.SpinResult;

namespace Slot.UnitTests.PhantomThief.GameResults.Spins
{
    [TestFixture]
    public class CollapsingSpinResultTests
    {
        [TestCase(Levels.One, TestName = "PhantomThief-CollapsingResultSameRoundId")]
        public void EngineShouldCreateCollapsingResultSameRoundId(int level)
        {
            var config = new Configuration();

            var spinResult = GenerateWinningSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, targetWheel, config.Payline, config.MainGamePayTable);

            Assert.IsTrue(spinResult.RoundId == collapsingSpinResult.RoundId);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CollapseCollapsingResultOnWin")]
        public void EngineShouldCollapseCollapsingResultOnWin(int level)
        {
            var config = new Configuration();
            var collapsingSpinResult = GenerateWinningNonBonusCollapsingSpinResult(level);

            Assert.IsTrue(collapsingSpinResult.Collapse);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-DoNotCollapseCollapsingResultOnLose")]
        public void EngineShouldNotCollapseCollapsingResultOnLose(int level)
        {
            var config = new Configuration();
            var collapsingSpinResult = GenerateNonWinningNonBonusCollapsingSpinResult(level);

            Assert.IsTrue(!collapsingSpinResult.Collapse);
        }

        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", "22,32,27,30,32", Levels.One, TestName = "PhantomThief-CreateCorrectCollapseReels-900", ExpectedResult = "2,2,3|0,0,1|8,8,2|0,0,4|4,4,5")]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", "22,11,12,8,5", Levels.One, TestName = "PhantomThief-CreateCorrectCollapseReels-450", ExpectedResult = "1,5,2|3,2,5|1,1,0|2,2,0|2,1,4")]
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

            var targetWheel = config.WeightedReelStripsCollection.ElementAt(1).Wheel;
            var topIndices = Array.ConvertAll(indicesString.Split(','), Convert.ToInt32).ToList();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.MainGamePayTable, wheel, spinBet.LineBet, spinBet.Lines, spinBet.Multiplier);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var spinResult = new SpinResult(spinBet, wheel, topIndices, winPositions, bonusPositions);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, targetWheel, config.Payline, config.MainGamePayTable);

            return string.Join('|', collapsingSpinResult.Wheel.Reels.Select(symbols => string.Join(',', symbols)));
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldFlagCollapsingResultWithBonusOnScatter")]
        public void EngineShouldFlagCollapsingResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusCollapsingSpinResult(level);

            Assert.IsTrue(spinResult.HasFeatureBonus);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void CollapsingSpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var collapsingSpinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(collapsingSpinResult);

            freeSpinBonus.UpdateBonus(collapsingSpinResult, 1);
            collapsingSpinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = collapsingSpinResult.BonusElement.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = collapsingSpinResult.BonusElement.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }
    }
}
