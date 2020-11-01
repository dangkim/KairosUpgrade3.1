using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.LionDance.Configuration;
using Slot.Games.LionDance.Engines;
using Slot.Model;
using System;
using System.Linq;
using static Slot.UnitTests.LionDance.SpinsHelper;
using SpinResult = Slot.Games.LionDance.Models.GameResults.Spins.SpinResult;

namespace Slot.UnitTests.LionDance.GameResults.Spins
{
    [TestFixture]
    public class CollapsingSpinResultTests
    {
        [TestCase(Levels.One, TestName = "LionDance-CollapsingResultSameRoundId")]
        public void EngineShouldCreateCollapsingResultSameRoundId(int level)
        {
            var config = new Configuration();

            var spinResult = GenerateWinningSpinResult(level);
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, targetWheel, config.Payline, config.PayTable);

            Assert.IsTrue(spinResult.RoundId == collapsingSpinResult.RoundId);
        }

        [TestCase(Levels.One, TestName = "LionDance-CollapseCollapsingResultOnWin")]
        public void EngineShouldCollapseCollapsingResultOnWin(int level)
        {
            var config = new Configuration();
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);

            Assert.IsTrue(collapsingSpinResult.Collapse);
        }

        [TestCase(Levels.One, TestName = "LionDance-DoNotCollapseCollapsingResultOnLose")]
        public void EngineShouldNotCollapseCollapsingResultOnLose(int level)
        {
            var config = new Configuration();
            var collapsingSpinResult = GenerateNonWinningCollapsingSpinResult(level);

            Assert.IsTrue(!collapsingSpinResult.Collapse);
        }

        [TestCase("1,8,5|0,1,3|2,0,8", "17,4,41", Levels.One, TestName = "LionDance-CreateCorrectCollapseReels-1", ExpectedResult = "2,1,5|6,5,0|7,2,2")]
        [TestCase("2,1,8|6,0,4|7,4,1", "16,21,21", Levels.One, TestName = "LionDance-CreateCorrectCollapseReels-2", ExpectedResult = "2,2,1|1,6,6|7,7,1")]
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

            var targetWheel = config.Wheels.FirstOrDefault().Value;
            var topIndices = Array.ConvertAll(indicesString.Split(','), Convert.ToInt32).ToList();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, spinBet.LineBet, 1);

            var spinResult = new SpinResult(spinBet, wheel, topIndices, winPositions);
            var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(spinResult, targetWheel, config.Payline, config.PayTable);

            return string.Join('|', collapsingSpinResult.Wheel.Reels.Select(symbols => string.Join(',', symbols)));
        }

        [TestCase(Levels.One, TestName = "LionDance-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void CollapsingSpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var collapsingSpinResult = GenerateWinningCollapsingSpinResult(level);
            var collapsingSpinBonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(collapsingSpinResult);

            collapsingSpinBonus.UpdateBonus(collapsingSpinResult);
            collapsingSpinResult.UpdateBonus(collapsingSpinBonus);

            var isEqualBonusId = collapsingSpinResult.BonusElement.Id == collapsingSpinBonus.Id;
            var isEqualBonusGuid = collapsingSpinResult.BonusElement.Value == collapsingSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }
    }
}
