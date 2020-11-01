using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using Slot.Model;
using System.Linq;

namespace Slot.UnitTests.FrostDragon.Engines
{
    [TestFixture]
    public class MainGameEngineTests
    {
        [TestCase(TestName = "FrostDragon-SpinBet")]
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

        [TestCase(Levels.One, TestName = "FrostDragon-WheelLevel-LevelOne", ExpectedResult = true)]
        public bool EngineShouldGetCorrectWheelLevel(int level)
        {
            var areReelsCorrect = true;
            var config = new Configuration();
            var wheel = MainGameEngine.GetTargetWheel(level, config, false);

            foreach (var reelIndex in wheel.Reels.Select((Value, Index) => new { Value, Index }))
            {
                var referenceReel = config.WeightedReelStripsCollection.FirstOrDefault(rc => rc.Level == level).Wheel[reelIndex.Index];

                if (reelIndex.Value.Except(referenceReel).Any())
                {
                    areReelsCorrect = false;
                }
            }

            return areReelsCorrect;
        }

        [TestCase(Levels.One, TestName = "FrostDragon-RandomIndices-LevelOne")]
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

        [TestCase(Levels.One, TestName = "FrostDragon-MainGameNormalWheel-LevelOne", ExpectedResult = true)]
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

        [TestCase("6,6,1|3,6,5|1,6,6|3,6,1|4,6,1", Levels.One, TestName = "FrostDragon-WinPositionTest-575", ExpectedResult = 5)]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,6,1", Levels.One, TestName = "FrostDragon-WinPositionTest-250", ExpectedResult = 1)]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,8,1", Levels.One, TestName = "FrostDragon-WinPositionTest-250-Wild", ExpectedResult = 1)]
        public int EngineShouldCreateCorrectWinPositionCount(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                1,
                                                Game.Lines,
                                                config.BonusConfig.Collapse.Multipliers.First());

            return winPositions.Count;
        }

        [TestCase("6,6,1|3,6,5|1,6,6|3,6,1|4,6,1", Levels.One, TestName = "FrostDragon-WinPositionLineAndMultiplierAndSymbol-575")]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,6,1", Levels.One, TestName = "FrostDragon-WinPositionLineAndMultiplierAndSymbol-250")]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,8,1", Levels.One, TestName = "FrostDragon-WinPositionLineAndMultiplierAndSymbol-250-Wild")]
        public void EngineShouldCreateCorrectWinPositionLineAndMultiplierAndSymbol(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                1,
                                                Game.Lines,
                                                config.BonusConfig.Collapse.Multipliers.First());

            Assert.IsTrue(!winPositions.Any(wp => wp.Line < 0 || wp.Multiplier <= 0 || wp.Symbol < 0));
        }

        [TestCase("6,6,1|3,6,5|1,6,6|3,6,1|4,6,1", Levels.One, TestName = "FrostDragon-RowPositionTest-575", ExpectedResult = "2,2,2,2,2|1,2,3,2,0|2,2,3,2,2|1,2,2,2,0|1,2,3,0,0")]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,6,1", Levels.One, TestName = "FrostDragon-RowPositionTest-250", ExpectedResult = "2,2,2,2,2")]
        [TestCase("0,6,1|3,6,5|1,6,2|3,6,1|4,8,1", Levels.One, TestName = "FrostDragon-RowPositionTest-250-Wild", ExpectedResult = "2,2,2,2,2")]
        public string EngineShouldCreateCorrectRowPositions(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                1,
                                                Game.Lines,
                                                config.BonusConfig.Collapse.Multipliers.First());

            return string.Join('|', winPositions.Select(wp => string.Join(',', wp.RowPositions)));
        }
    }
}
