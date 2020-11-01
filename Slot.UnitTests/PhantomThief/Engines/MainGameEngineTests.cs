using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using Slot.Model;
using System.Linq;

namespace Slot.UnitTests.PhantomThief.Engines
{
    [TestFixture]
    public class MainGameEngineTests
    {
        [TestCase(TestName = "PhantomThief-SpinBet")]
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

        [TestCase(Levels.One, TestName = "PhantomThief-WheelLevel-LevelOne")]
        public void EngineShouldGetCorrectWheelLevel(int level)
        {
            var config = new Configuration();
            var wheel = MainGameEngine.GetTargetWheel(level, config, false);

            foreach (var reelIndex in wheel.Reels.Select((Value, Index) => new { Value, Index }))
            {
                var referenceReel = config.WeightedReelStripsCollection
                                            .FirstOrDefault(rc => rc.Name == wheel.ReelStripsId && rc.Level == level)
                                            .Wheel[reelIndex.Index];

                Assert.IsTrue(!reelIndex.Value.Except(referenceReel).Any());
            }
        }

        [TestCase(Levels.One, TestName = "PhantomThief-RandomIndices-LevelOne")]
        public void EngineShouldCreateCorrectRandomIndices(int level)
        {
            var config = new Configuration();
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, false);
            var topIndices = MainGameEngine.GenerateRandomWheelIndices(targetWheel);

            foreach (var reelIndex in targetWheel.Reels.Select((Value, Index) => new { Value, Index }))
            {
                var topReelIndex = topIndices[reelIndex.Index];

                Assert.AreEqual(true, topReelIndex >= 0 && topReelIndex < targetWheel.Reels[reelIndex.Index].Count);
            }
        }

        [TestCase(Levels.One, TestName = "PhantomThief-MainGameNormalWheel-LevelOne", ExpectedResult = true)]
        public bool EngineShouldCreateCorrectMainGameRandomWheel(int level)
        {
            var areReelsCorrect = true;

            var config = new Configuration();
            var targetWheel = MainGameEngine.GetTargetWheel(level, config, false);
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

        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", Levels.One, TestName = "PhantomThief-WinPositionTest-900", ExpectedResult = 3)]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", Levels.One, TestName = "PhantomThief-WinPositionTest-450", ExpectedResult = 6)]
        [TestCase("2,0,6|4,5,1|8,7,2|0,7,4|5,7,2", Levels.One, TestName = "PhantomThief-WinPositionTest-0", ExpectedResult = 0)]
        public int EngineShouldCreateCorrectWinPositionCount(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.MainGamePayTable,
                                                wheel,
                                                1,
                                                Game.Lines,
                                                1);

            return winPositions.Count;
        }

        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", Levels.One, TestName = "PhantomThief-WinPositionLineAndMultiplierAndSymbol-900")]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", Levels.One, TestName = "PhantomThief-WinPositionLineAndMultiplierAndSymbol-450")]
        [TestCase("2,0,6|4,5,1|8,7,2|0,7,4|5,7,2", Levels.One, TestName = "PhantomThief-WinPositionLineAndMultiplierAndSymbol-0")]
        public void EngineShouldCreateCorrectWinPositionLineAndMultiplierAndSymbol(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.MainGamePayTable,
                                                wheel,
                                                1,
                                                Game.Lines,
                                                1);

            Assert.IsTrue(!winPositions.Any(wp => wp.Line < 0 || wp.Multiplier <= 0 || wp.Symbol < 0));
        }

        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", Levels.One, TestName = "PhantomThief-RowPositionTest-900", ExpectedResult = "2,2,2,2,2|2,2,2,0,0|2,2,2,0,0")]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", Levels.One, TestName = "PhantomThief-RowPositionTest-450", ExpectedResult = "2,2,2,2,0|1,1,2,2,0|1,2,2,0,0|3,2,2,0,0|2,2,2,0,0|2,2,2,0,0")]
        [TestCase("2,0,6|4,5,1|8,7,2|0,7,4|5,7,2", Levels.One, TestName = "PhantomThief-RowPositionTest-0", ExpectedResult = "")]
        public string EngineShouldCreateCorrectRowPositions(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.MainGamePayTable,
                                                wheel,
                                                1,
                                                Game.Lines,
                                                1);

            return string.Join('|', winPositions.Select(wp => string.Join(',', wp.RowPositions)));
        }
    }
}
