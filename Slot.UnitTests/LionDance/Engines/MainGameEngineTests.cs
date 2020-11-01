using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.LionDance.Configuration;
using Slot.Games.LionDance.Engines;
using Slot.Model;
using System.Linq;

namespace Slot.UnitTests.LionDance.Engines
{
    [TestFixture]
    public class MainGameEngineTests
    {
        [TestCase(TestName = "LionDance-SpinBet")]
        public void EngineShouldCreateCorrectSpinBet()
        {
            var config = new Configuration();
            var requestContext = new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = 1,
                    Multiplier = 1
                }
            };

            var spinBet = MainGameEngine.GenerateSpinBet(requestContext);

            var isCurrencyEqual = spinBet.CurrencyId == requestContext.Currency.Id;
            var isGameSettingGroupEqual = spinBet.GameSettingGroupId == requestContext.GameSetting.GameSettingGroupId;
            var isLineBetEqual = spinBet.LineBet == requestContext.Parameters.LineBet;
            var isCreditsEqual = spinBet.Credits == Game.Credits;
            var isAutoSpinEqual = spinBet.IsAutoSpin == requestContext.Parameters.IsAutoSpin;
            var isLinesEqual = spinBet.Lines == Game.Lines;
            var isMultiplierEqual = spinBet.Multiplier == requestContext.Parameters.Multiplier;
            var isFunPlayDisabled = spinBet.FunPlayDemoKey == 0;

            Assert.IsTrue(isCurrencyEqual && isGameSettingGroupEqual && isLineBetEqual && isCreditsEqual && isAutoSpinEqual && isLinesEqual && isMultiplierEqual && isFunPlayDisabled);
        }

        [TestCase(Levels.One, TestName = "LionDance-WheelLevel-LevelOne", ExpectedResult = true)]
        public bool EngineShouldGetCorrectWheelLevel(int level)
        {
            var areReelsCorrect = true;
            var config = new Configuration();
            var wheel = MainGameEngine.GetTargetWheel(level, config);

            foreach (var reelIndex in wheel.Reels.Select((Value, Index) => new { Value, Index }))
            {
                var referenceReel = config.Wheels[level][reelIndex.Index];

                if (reelIndex.Value.Except(referenceReel).Any())
                {
                    areReelsCorrect = false;
                }
            }

            return areReelsCorrect;
        }

        [TestCase(Levels.One, TestName = "LionDance-RandomIndices-LevelOne")]
        public void EngineShouldCreateCorrectRandomIndices(int level)
        {
            var config = new Configuration();
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);
            var topIndices = MainGameEngine.GenerateRandomWheelIndices(targetWheel);

            foreach (var reelIndex in targetWheel.Reels.Select((Value, Index) => new { Value, Index }))
            {
                var topReelIndex = topIndices[reelIndex.Index];

                Assert.AreEqual(true, topReelIndex >= 0 && topReelIndex < targetWheel.Reels[reelIndex.Index].Count);
            }
        }

        [TestCase(Levels.One, TestName = "LionDance-MainGameNormalWheel-LevelOne", ExpectedResult = true)]
        public bool EngineShouldCreateCorrectMainGameRandomWheel(int level)
        {
            var areReelsCorrect = true;

            var config = new Configuration();
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);
            var topIndices = MainGameEngine.GenerateRandomWheelIndices(targetWheel);
            var wheel = MainGameEngine.GenerateNormalWheel(level, targetWheel, topIndices);

            foreach (var reelIndex in targetWheel.Reels.Select((Value, Index) => new { Value, Index }))
            {
                var reelReference = wheel[reelIndex.Index];

                if (reelReference.Except(reelIndex.Value).Any())
                {
                    areReelsCorrect = false;
                }
            }

            return areReelsCorrect;
        }

        [TestCase("1,8,5|0,1,3|2,0,8", Levels.One, TestName = "LionDance-WinPositionTest-29", ExpectedResult = 7)]
        [TestCase("6,1,4|4,5,1|7,4,1", Levels.One, TestName = "LionDance-WinPositionTest-16", ExpectedResult = 2)]
        [TestCase("2,6,3|1,3,0|0,2,1", Levels.One, TestName = "LionDance-WinPositionTest-0", ExpectedResult = 0)]
        public int EngineShouldCreateCorrectWinPositionCount(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, 1, 1);

            return winPositions.Count;
        }

        [TestCase("1,8,5|0,1,3|2,0,8", Levels.One, TestName = "LionDance-WinPositionLineAndMultiplierAndSymbol-29")]
        [TestCase("6,1,4|4,5,1|7,4,1", Levels.One, TestName = "LionDance-WinPositionLineAndMultiplierAndSymbol-16")]
        [TestCase("2,6,3|1,3,0|0,2,1", Levels.One, TestName = "LionDance-WinPositionLineAndMultiplierAndSymbol-0")]
        public void EngineShouldCreateCorrectWinPositionLineAndMultiplierAndSymbol(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, 1, 1);

            Assert.IsTrue(!winPositions.Any(wp => wp.Line < 0 || wp.Multiplier <= 0 || wp.Symbol < 0));
        }

        [TestCase("1,8,5|0,1,3|2,0,8", Levels.One, TestName = "LionDance-RowPositionTest-29", ExpectedResult = "1,0,0,0,1,0,0,0,1|0,1,0,0,0,0,0,0,0|0,1,0,0,0,0,0,0,0|0,1,0,1,0,0,0,1,0|0,1,0,1,0,0,0,0,1|0,1,0,0,1,0,0,0,1|0,1,0,0,0,1,0,0,1")]
        [TestCase("6,1,4|4,5,1|7,4,1", Levels.One, TestName = "LionDance-RowPositionTest-16", ExpectedResult = "0,1,0,0,0,1,0,0,1|0,0,1,1,0,0,0,1,0")]
        [TestCase("2,6,3|1,3,0|0,2,1", Levels.One, TestName = "LionDance-RowPositionTest-0", ExpectedResult = "")]
        public string EngineShouldCreateCorrectRowPositions(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, 1, 1);

            return string.Join('|', winPositions.Select(wp => string.Join(',', wp.RowPositions)));
        }
    }
}
