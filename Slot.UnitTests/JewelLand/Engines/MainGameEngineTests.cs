using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using Slot.Model;
using System.Linq;

namespace Slot.UnitTests.JewelLand.Engines
{
    [TestFixture]
    public class MainGameEngineTests
    {
        [TestCase(TestName = "JewelLand-SpinBet")]
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

        [TestCase(Levels.One, TestName = "JewelLand-WheelLevel-LevelOne", ExpectedResult = true)]
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

        [TestCase(Levels.One, TestName = "JewelLand-RandomIndices-LevelOne")]
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

        [TestCase(Levels.One, TestName = "JewelLand-MainGameNormalWheel-LevelOne", ExpectedResult = true)]
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

        [TestCase("2,3,3|3,3,3|3,8,4", Levels.One, TestName = "JewelLand-WinPositionTest-14", ExpectedResult = 2)]
        [TestCase("3,3,8|7,7,8|8,7,8", Levels.One, TestName = "JewelLand-WinPositionTest-82", ExpectedResult = 2)]
        [TestCase("3,3,8|7,7,7|8,8,8", Levels.One, TestName = "JewelLand-WinPositionTest-4", ExpectedResult = 2)]
        public int EngineShouldCreateCorrectWinPositionCount(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, 1, Game.Lines, 1);

            return winPositions.Count;
        }

        [TestCase("2,3,3|3,3,3|3,8,4", Levels.One, TestName = "JewelLand-WinPositionLineAndMultiplierAndSymbol-14")]
        [TestCase("3,3,8|7,7,8|8,7,8", Levels.One, TestName = "JewelLand-WinPositionLineAndMultiplierAndSymbol-82")]
        [TestCase("3,3,8|7,7,7|8,8,8", Levels.One, TestName = "JewelLand-WinPositionLineAndMultiplierAndSymbol-4")]
        public void EngineShouldCreateCorrectWinPositionLineAndMultiplierAndSymbol(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, 1, Game.Lines, 1);

            Assert.IsTrue(!winPositions.Any(wp => wp.Line < 0 || wp.Multiplier <= 0 || wp.Symbol < 0));
        }

        [TestCase("2,3,3|3,3,3|3,8,4", Levels.One, TestName = "JewelLand-RowPositionTest-14", ExpectedResult = "2,2,2|3,2,1")]
        [TestCase("3,3,8|7,7,8|8,7,8", Levels.One, TestName = "JewelLand-RowPositionTest-82", ExpectedResult = "3,3,3|3,2,1")]
        [TestCase("3,3,8|7,7,7|8,8,8", Levels.One, TestName = "JewelLand-RowPositionTest-4", ExpectedResult = "3,3,3|3,2,1")]
        public string EngineShouldCreateCorrectRowPositions(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, 1, Game.Lines, 1);

            return string.Join('|', winPositions.Select(wp => string.Join(',', wp.RowPositions)));
        }

        [TestCase("2,3,3|3,3,3|3,8,4", TestName = "JewelLand-ShouldGetCorrectStackedReels-14", ExpectedResult = 1)]
        [TestCase("3,3,8|7,7,8|8,7,8", TestName = "JewelLand-ShouldGetCorrectStackedReels-82", ExpectedResult = 3)]
        [TestCase("3,3,8|7,7,7|8,8,8", TestName = "JewelLand-ShouldGetCorrectStackedReels-4", ExpectedResult = 3)]
        public int EngineShouldGetCorrectStackedReels(string wheelString)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var stackedReels = MainGameEngine.GetStackedReels(wheel, config.PayTable);

            return stackedReels.Count;
        }

        [TestCase("2,3,3|3,3,3|3,8,4", TestName = "JewelLand-ShouldCreateCorrectBonusPositionsCount-14", ExpectedResult = 0)]
        [TestCase("3,3,8|7,7,8|8,7,8", TestName = "JewelLand-ShouldCreateCorrectBonusPositionsCount-82", ExpectedResult = 1)]
        [TestCase("3,3,8|7,7,7|8,8,8", TestName = "JewelLand-ShouldCreateCorrectBonusPositionsCount-4", ExpectedResult = 1)]
        public int EngineShouldCreateCorrectBonusPositionsCount(string wheelString)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var stackedReels = MainGameEngine.GetStackedReels(wheel, config.PayTable);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(stackedReels);

            return bonusPositions.Count;
        }

        [TestCase("2,3,3|3,3,3|3,8,4", TestName = "JewelLand-ShouldCreateCorrectBonusPositionsRowPositions-14", ExpectedResult = null)]
        [TestCase("3,3,8|7,7,8|8,7,8", TestName = "JewelLand-ShouldCreateCorrectBonusPositionsRowPositions-82", ExpectedResult = "0,1,1")]
        [TestCase("3,3,8|7,7,7|8,8,8", TestName = "JewelLand-ShouldCreateCorrectBonusPositionsRowPositions-4", ExpectedResult = "1,0,1")]
        public string EngineShouldCreateCorrectBonusPositionsRowPositions(string wheelString)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var stackedReels = MainGameEngine.GetStackedReels(wheel, config.PayTable);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(stackedReels);
            var targetBonusPosition = bonusPositions.FirstOrDefault();

            return targetBonusPosition != null ? string.Join(",", targetBonusPosition.RowPositions.Select(rp => rp)) : null;
        }
    }
}
