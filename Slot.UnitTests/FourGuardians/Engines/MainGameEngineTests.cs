using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FourGuardians.Configuration;
using Slot.Games.FourGuardians.Engines;
using Slot.Model;
using System.Linq;

namespace Slot.UnitTests.FourGuardians.Engines
{
    [TestFixture]
    public class MainGameEngineTests
    {
        [TestCase(TestName = "FourGuardians-SpinBet")]
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

        [TestCase(Levels.One, TestName = "FourGuardians-WheelLevel-LevelOne", ExpectedResult = true)]
        public bool EngineShouldGetCorrectWheelLevel(int level)
        {
            var areReelsCorrect = true;
            var config = new Configuration();
            var wheel = MainGameEngine.GetTargetWheel(level, config);

            foreach (var reelIndex in wheel.Reels.Select((Value, Index) => new { Value, Index }))
            {
                var referenceReel = config.ReelStrips[level][reelIndex.Index];

                if (reelIndex.Value.Except(referenceReel).Any())
                {
                    areReelsCorrect = false;
                }
            }

            return areReelsCorrect;
        }

        [TestCase(Levels.One, TestName = "FourGuardians-RandomIndices-LevelOne")]
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

        [TestCase(Levels.One, TestName = "FourGuardians-MainGameNormalWheel-LevelOne", ExpectedResult = true)]
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

        [TestCase("7,8,5|7,8,10|0,11,0|7,8,10|7,8,6", Levels.One, TestName = "FourGuardians-WinPositionTest-2415", ExpectedResult = 20)]
        [TestCase("0,0,3|0,4,0|0,1,3|0,7,3|0,1,0", Levels.One, TestName = "FourGuardians-WinPositionTest-31", ExpectedResult = 7)]
        [TestCase("5,7,6|5,6,3|6,4,2|6,4,3|6,6,6", Levels.One, TestName = "FourGuardians-WinPositionTest-110", ExpectedResult = 2)]
        public int EngineShouldCreateCorrectWinPositionCount(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var expandedWheel = ExpandingWildsEngine.GenerateWheelWithExpandedWilds(wheel, config);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                expandedWheel,
                                                1,
                                                Game.Lines,
                                                1);

            return winPositions.Count;
        }

        [TestCase("7,8,5|7,8,10|0,11,0|7,8,10|7,8,6", Levels.One, TestName = "FourGuardians-WinPositionLineAndMultiplierAndSymbol-1750")]
        [TestCase("0,0,3|0,4,0|0,1,3|0,7,3|0,1,0", Levels.One, TestName = "FourGuardians-WinPositionLineAndMultiplierAndSymbol-31")]
        [TestCase("5,7,6|5,6,3|6,4,2|6,4,3|6,6,6", Levels.One, TestName = "FourGuardians-WinPositionLineAndMultiplierAndSymbol-110")]
        public void EngineShouldCreateCorrectWinPositionLineAndMultiplierAndSymbol(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var expandedWheel = ExpandingWildsEngine.GenerateWheelWithExpandedWilds(wheel, config);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                expandedWheel,
                                                1,
                                                Game.Lines,
                                                1);

            Assert.IsTrue(!winPositions.Any(wp => wp.Line < 0 || wp.Multiplier <= 0 || wp.Symbol < 0));
        }

        [TestCase("7,8,5|7,8,10|0,11,0|7,8,10|7,8,6", Levels.One, TestName = "FourGuardians-RowPositionTest-2415", ExpectedResult = "2,2,2,2,2|1,1,1,1,1|3,3,3,3,0|1,2,3,2,1|3,2,1,2,0|1,1,2,1,1|3,3,2,3,0|2,3,3,3,2|2,1,1,1,2|2,1,2,1,2|2,3,2,3,2|1,2,1,2,1|3,2,3,2,0|2,2,1,2,2|2,2,3,2,2|1,2,2,2,1|3,2,2,2,0|1,2,3,3,0|3,2,1,1,0|1,3,1,3,1")]
        [TestCase("0,0,3|0,4,0|0,1,3|0,7,3|0,1,0", Levels.One, TestName = "FourGuardians-RowPositionTest-31", ExpectedResult = "1,1,1,1,1|1,1,0,0,0|2,3,0,0,0|2,1,1,1,0|2,1,0,0,0|2,3,0,0,0|1,3,1,0,0")]
        [TestCase("5,7,6|5,6,3|6,4,2|6,4,3|6,6,6", Levels.One, TestName = "FourGuardians-RowPositionTest-110", ExpectedResult = "3,2,1,0,0|3,2,1,1,1")]
        public string EngineShouldCreateCorrectRowPositions(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var expandedWheel = ExpandingWildsEngine.GenerateWheelWithExpandedWilds(wheel, config);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                expandedWheel,
                                                1,
                                                Game.Lines,
                                                1);

            return string.Join('|', winPositions.Select(wp => string.Join(',', wp.RowPositions)));
        }
    }
}
