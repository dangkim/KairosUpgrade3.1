using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Engines;
using Slot.Model;
using System.Linq;

namespace Slot.UnitTests.XuanWuBlessing.Engines
{
    [TestFixture]
    public class MainGameEngineTests
    {
        [TestCase(TestName = "XuanWuBlessing-SpinBet")]
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

        [TestCase(Levels.One, TestName = "XuanWuBlessing-WheelLevel-LevelOne", ExpectedResult = true)]
        public bool EngineShouldGetCorrectWheelLevel(int level)
        {
            var areReelsCorrect = true;
            var config = new Configuration();
            var wheel = MainGameEngine.GetTargetWheel(level, config);

            foreach (var reelIndex in wheel.Reels.Select((Value, Index) => new { Value, Index }))
            {
                var referenceReel = config.MainGameWeightedReelStrips.FirstOrDefault(reelStrip => reelStrip.Level == level).ReelStrips[reelIndex.Index];

                if (reelIndex.Value.Except(referenceReel).Any())
                {
                    areReelsCorrect = false;
                }
            }

            return areReelsCorrect;
        }

        [TestCase("6,0,8|12,12,5|6,8,11|13,13,13|13,13,13", 6, Levels.One, TestName = "XuanWuBlessing-ShouldGetCorrectReplacedMysterySymbolsWheel-310", ExpectedResult = "6,0,8|12,12,5|6,8,11|6,6,6|6,6,6")]
        [TestCase("2,7,5|2,2,8|2,5,6|2,1,11|2,6,1", 2, Levels.One, TestName = "XuanWuBlessing-ShouldGetCorrectReplacedMysterySymbolsWheel-55", ExpectedResult = "2,7,5|2,2,8|2,5,6|2,1,11|2,6,1")]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,10", 2, Levels.One, TestName = "XuanWuBlessing-ShouldGetCorrectReplacedMysterySymbolsWheel-170", ExpectedResult = "5,3,9|12,12,3|3,5,12|8,10,3|6,2,10")]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,13", 2, Levels.One, TestName = "XuanWuBlessing-ShouldGetCorrectReplacedMysterySymbolsWheel-170-2", ExpectedResult = "5,3,9|12,12,3|3,5,12|8,10,3|6,2,2")]
        public string EngineShouldGetCorrectReplacedMysterySymbolsWheel(string wheelString, int replacementSymbol, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());

            if (wheel.CountDistinct(Symbols.Mystery) > 0) /// Calculate wins with replaced mystery symbols
            {
                var replacedMysterySymbolWheel = MainGameEngine.GenerateReplacedMysterySymbolWheel(config, wheel, replacementSymbol);

                return string.Join("|", replacedMysterySymbolWheel.Reels.Select(reel => string.Join(",", reel)));
            }
            else
            {
                return string.Join("|", wheel.Reels.Select(reel => string.Join(",", reel)));
            }
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-RandomIndices-LevelOne")]
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

        [TestCase(Levels.One, TestName = "XuanWuBlessing-MainGameNormalWheel-LevelOne", ExpectedResult = true)]
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

        [TestCase("6,0,8|12,12,5|6,8,11|13,13,13|13,13,13", 6, Levels.One, TestName = "XuanWuBlessing-WinPositionTest-310", ExpectedResult = 3)]
        [TestCase("2,7,5|2,2,8|2,5,6|2,1,11|2,6,1", 2, Levels.One, TestName = "XuanWuBlessing-WinPositionTest-55", ExpectedResult = 2)]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,10", 2, Levels.One, TestName = "XuanWuBlessing-WinPositionTest-170", ExpectedResult = 9)]
        public int EngineShouldCreateCorrectWinPositionCount(string wheelString, int replacementSymbol, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());

            if (wheel.CountDistinct(Symbols.Mystery) > 0) /// Calculate wins with replaced mystery symbols
            {
                var replacedMysterySymbolWheel = MainGameEngine.GenerateReplacedMysterySymbolWheel(config, wheel, replacementSymbol);
                var winPositions = MainGameEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.PayTable,
                                                    replacedMysterySymbolWheel,
                                                    1,
                                                    Game.Lines,
                                                    1);
                return winPositions.Count;
            }
            else
            {
                var winPositions = MainGameEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.PayTable,
                                                    wheel,
                                                    1,
                                                    Game.Lines,
                                                    1);
                return winPositions.Count;
            }
        }

        [TestCase("6,0,8|12,12,5|6,8,11|13,13,13|13,13,13", 6, Levels.One, TestName = "XuanWuBlessing-WinPositionLineAndMultiplierAndSymbol-310")]
        [TestCase("2,7,5|2,2,8|2,5,6|2,1,11|2,6,1", 2, Levels.One, TestName = "XuanWuBlessing-WinPositionLineAndMultiplierAndSymbol-55")]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,10", 2, Levels.One, TestName = "XuanWuBlessing-WinPositionLineAndMultiplierAndSymbol-170")]
        public void EngineShouldCreateCorrectWinPositionLineAndMultiplierAndSymbol(string wheelString, int replacementSymbol, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());

            if (wheel.CountDistinct(Symbols.Mystery) > 0) /// Calculate wins with replaced mystery symbols
            {
                var replacedMysterySymbolWheel = MainGameEngine.GenerateReplacedMysterySymbolWheel(config, wheel, replacementSymbol);
                var winPositions = MainGameEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.PayTable,
                                                    replacedMysterySymbolWheel,
                                                    1,
                                                    Game.Lines,
                                                    1);

                Assert.IsTrue(!winPositions.Any(wp => wp.Line < 0 || wp.Multiplier <= 0 || wp.Symbol < 0));
            }
            else
            {
                var winPositions = MainGameEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.PayTable,
                                                    wheel,
                                                    1,
                                                    Game.Lines,
                                                    1);
                Assert.IsTrue(!winPositions.Any(wp => wp.Line < 0 || wp.Multiplier <= 0 || wp.Symbol < 0));
            }
        }

        [TestCase("6,0,8|12,12,5|6,8,11|13,13,13|13,13,13", 6, Levels.One, TestName = "XuanWuBlessing-RowPositionTest-310", ExpectedResult = "1,1,1,1,1|3,2,2,0,0|1,2,1,2,1")]
        [TestCase("2,7,5|2,2,8|2,5,6|2,1,11|2,6,1", 2, Levels.One, TestName = "XuanWuBlessing-RowPositionTest-55", ExpectedResult = "1,1,1,1,1|1,2,1,0,0")]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,10", 2, Levels.One, TestName = "XuanWuBlessing-RowPositionTest-170", ExpectedResult = "1,2,3,0,0|2,1,1,0,0|2,3,3,3,0|1,1,2,0,0|1,2,2,0,0|3,2,3,0,0|2,2,1,0,0|2,2,3,0,0|1,1,3,0,0")]
        public string EngineShouldCreateCorrectRowPositions(string wheelString, int replacementSymbol, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());

            if (wheel.CountDistinct(Symbols.Mystery) > 0) /// Calculate wins with replaced mystery symbols
            {
                var replacedMysterySymbolWheel = MainGameEngine.GenerateReplacedMysterySymbolWheel(config, wheel, replacementSymbol);
                var winPositions = MainGameEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.PayTable,
                                                    replacedMysterySymbolWheel,
                                                    1,
                                                    Game.Lines,
                                                    1);

                return string.Join('|', winPositions.Select(wp => string.Join(',', wp.RowPositions)));
            }
            else
            {
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

        [TestCase("6,0,8|12,12,5|6,8,11|13,13,13|13,13,13", TestName = "XuanWuBlessing-ShouldGetCorrectStackedSymbols-310", ExpectedResult = 3)]
        [TestCase("2,7,5|2,2,8|2,5,6|2,1,11|2,6,1", TestName = "XuanWuBlessing-ShouldGetCorrectStackedSymbols-55", ExpectedResult = 1)]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,10", TestName = "XuanWuBlessing-ShouldGetCorrectStackedSymbols-170", ExpectedResult = 1)]
        public int EngineShouldGetCorrectStackedSymbols(string wheelString)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var stackedSymbols = MainGameEngine.GetStackedSymbols(wheel);

            return stackedSymbols.Count;
        }

        [TestCase("6,0,8|12,12,5|6,8,11|13,13,13|13,13,13", TestName = "XuanWuBlessing-ShouldCreateCorrectBonusPositionsCount-310", ExpectedResult = 0)]
        [TestCase("2,7,5|2,2,11|2,11,6|2,1,11|2,6,1", TestName = "XuanWuBlessing-ShouldCreateCorrectBonusPositionsCount-55", ExpectedResult = 1)]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,10", TestName = "XuanWuBlessing-ShouldCreateCorrectBonusPositionsCount-170", ExpectedResult = 0)]
        public int EngineShouldCreateCorrectBonusPositionsCount(string wheelString)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            return bonusPositions.Count;
        }

        [TestCase("6,0,8|12,12,5|6,8,11|13,13,13|13,13,13", TestName = "XuanWuBlessing-ShouldCreateCorrectBonusPositionsRowPositions-310", ExpectedResult = null)]
        [TestCase("2,7,5|2,2,11|2,11,6|2,1,11|2,6,1", TestName = "XuanWuBlessing-ShouldCreateCorrectBonusPositionsRowPositions-55", ExpectedResult = "0,3,2,3,0")]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,10", TestName = "XuanWuBlessing-ShouldCreateCorrectBonusPositionsRowPositions-170", ExpectedResult = null)]
        public string EngineShouldCreateCorrectBonusPositionsRowPositions(string wheelString)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var targetBonusPosition = bonusPositions.FirstOrDefault();

            return targetBonusPosition != null ? string.Join(",", targetBonusPosition.RowPositions.Select(rp => rp)) : null;
        }
    }
}
