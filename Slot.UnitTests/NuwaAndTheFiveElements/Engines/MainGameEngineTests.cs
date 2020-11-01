using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Model;
using System.Linq;

namespace Slot.UnitTests.NuwaAndTheFiveElements.Engines
{
    [TestFixture]
    public class MainGameEngineTests
    {
        [TestCase(TestName = "NuwaAndTheFiveElements-SpinBet")]
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

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-WheelLevel-LevelOne", ExpectedResult = true)]
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

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-RandomIndices-LevelOne")]
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

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-MainGameNormalWheel-LevelOne", ExpectedResult = true)]
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

        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionTest-1935", ExpectedResult = 6)]
        [TestCase("0,1,2,2,4,5,6,0,6,5,4,3,2,1,0,2,5,6,7,3,1,0,1,4,6,2,2,1,0,4,2,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionTest-850", ExpectedResult = 2)]
        [TestCase("6,1,2,3,4,5,6,4,6,5,4,8,2,1,0,2,6,6,7,3,1,3,6,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionTest-80", ExpectedResult = 2)]
        [TestCase("1,3,2,2,4,5,6,1,6,5,4,3,2,1,1,2,5,6,7,3,1,3,0,4,6,2,2,1,7,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionTest-60", ExpectedResult = 2)]
        [TestCase("0,1,2,3,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionTest-20", ExpectedResult = 1)]
        [TestCase("0,1,2,1,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionTest-0", ExpectedResult = 0)]
        [TestCase("0,1,2,11,4,5,6,4,1,5,4,11,2,1,0,2,0,6,7,11,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionTest-1", ExpectedResult = 1)]
        public int EngineShouldCreateCorrectWinPositionCount(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                1,
                                                Game.Lines,
                                                1);

            return winPositions.Count;
        }

        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionLineAndMultiplierAndSymbol-1935")]
        [TestCase("0,1,2,2,4,5,6,0,6,5,4,3,2,1,0,2,5,6,7,3,1,0,1,4,6,2,2,1,0,4,2,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionLineAndMultiplierAndSymbol-850")]
        [TestCase("6,1,2,3,4,5,6,4,6,5,4,8,2,1,0,2,6,6,7,3,1,3,6,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionLineAndMultiplierAndSymbol-80")]
        [TestCase("1,3,2,2,4,5,6,1,6,5,4,3,2,1,1,2,5,6,7,3,1,3,0,4,6,2,2,1,7,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionLineAndMultiplierAndSymbol-60")]
        [TestCase("0,1,2,3,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionLineAndMultiplierAndSymbol-20")]
        [TestCase("0,1,2,11,4,5,6,4,1,5,4,11,2,1,0,2,0,6,7,11,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-WinPositionLineAndMultiplierAndSymbol-1")]
        public void EngineShouldCreateCorrectWinPositionLineAndMultiplierAndSymbol(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                1,
                                                Game.Lines,
                                                1);

            Assert.IsTrue(!winPositions.Any(wp => wp.Line < 0 || wp.Multiplier <= 0 || wp.Symbol < 0));
        }

        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-RowPositionTest-1935", ExpectedResult = "1,1,1,1,1|2,1,1,1,0|1,1,2,0,0|1,2,1,0,0|1,1,1,2,0|1,1,2,1,1")]
        [TestCase("0,1,2,2,4,5,6,0,6,5,4,3,2,1,0,2,5,6,7,3,1,0,1,4,6,2,2,1,0,4,2,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-RowPositionTest-850", ExpectedResult = "1,1,1,1,1|1,1,1,0,0")]
        [TestCase("6,1,2,3,4,5,6,4,6,5,4,8,2,1,0,2,6,6,7,3,1,3,6,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-RowPositionTest-80", ExpectedResult = "1,2,3,4,0|1,2,3,2,0")]
        [TestCase("1,3,2,2,4,5,6,1,6,5,4,3,2,1,1,2,5,6,7,3,1,3,0,4,6,2,2,1,7,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-RowPositionTest-60", ExpectedResult = "1,1,1,0,0|1,1,1,0,0")]
        [TestCase("0,1,2,3,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-RowPositionTest-20", ExpectedResult = "4,5,6,0,0")]
        [TestCase("0,1,2,1,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-RowPositionTest-0", ExpectedResult = "")]
        [TestCase("0,1,2,11,4,5,6,4,1,5,4,11,2,1,0,2,0,6,7,11,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-RowPositionTest-1", ExpectedResult = "4,5,6,0,0")]
        public string EngineShouldCreateCorrectRowPositions(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                1,
                                                Game.Lines,
                                                1);

            return string.Join('|', winPositions.Select(wp => string.Join(',', wp.RowPositions)));
        }
    }
}
