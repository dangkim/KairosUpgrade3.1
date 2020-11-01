using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.SweetTreats.Configuration;
using Slot.Games.SweetTreats.Models.Engines;
using Slot.Model;
using System.Linq;

namespace Slot.UnitTests.SweetTreats.Engines
{
    [TestFixture]
    public class MainGameEngineTests
    {
        [TestCase(TestName = "SweetTreats-SpinBet")]
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

        [TestCase(Levels.One, TestName = "SweetTreats-WheelLevel-LevelOne")]
        public void EngineShouldGetCorrectWheelLevel(int level)
        {
            var config = new Configuration();
            var reelWheels = MainGameEngine.GetTargetWheel(level, config);

            foreach(var reelWheel in reelWheels)
            {
                foreach (var reelIndex in reelWheel.Value.Reels.Select((Value, Index) => new { Value, Index }))
                {
                    var referenceReel = config.Wheels[level]
                                                .FirstOrDefault(rw => rw.Key == reelWheel.Key)
                                                .Value[reelIndex.Index];

                    Assert.IsTrue(!reelIndex.Value.Except(referenceReel).Any());
                }
            }
        }

        [TestCase(Levels.One, TestName = "SweetTreats-RandomIndices-LevelOne")]
        public void EngineShouldCreateCorrectRandomIndices(int level)
        {
            var config = new Configuration();
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);
            var topIndices = MainGameEngine.GenerateRandomWheelIndices(targetWheel);

            for (var widthIndex = 0; widthIndex < Game.WheelWidth; widthIndex++)
            {
                for (var heightIndex = 0; heightIndex < Game.WheelHeight; heightIndex++)
                {
                    var index = (Game.WheelHeight * widthIndex) + heightIndex;
                    var topReelIndex = topIndices[index];

                    Assert.AreEqual(true, topReelIndex >= 0 && topReelIndex < targetWheel[index][0].Count);
                }
            }
        }

        [TestCase(Levels.One, TestName = "SweetTreats-MainGameNormalWheel-LevelOne", ExpectedResult = true)]
        public bool EngineShouldCreateCorrectMainGameRandomWheel(int level)
        {
            var areReelsCorrect = true;

            var config = new Configuration();
            var targetWheel = MainGameEngine.GetTargetWheel(level, config);
            var topIndices = MainGameEngine.GenerateRandomWheelIndices(targetWheel);
            var wheel = MainGameEngine.GenerateNormalWheel(level, targetWheel, topIndices);

            for (var widthIndex = 0; widthIndex < Game.WheelWidth; widthIndex++)
            {
                for (var heightIndex = 0; heightIndex < Game.WheelHeight; heightIndex++)
                {
                    var index = (Game.WheelHeight * widthIndex) + heightIndex;
                    var symbol = wheel[widthIndex][heightIndex];

                    if (!targetWheel[index][0].Contains(symbol))
                    {
                        areReelsCorrect = false;
                    }
                }
            }

            return areReelsCorrect;
        }

        [TestCase("9,9,9|9,9,9|9,9,9", Levels.One, TestName = "SweetTreats-WinPositionTest-5600", ExpectedResult = 9)]
        [TestCase("9,9,9|9,8,9|9,9,9", Levels.One, TestName = "SweetTreats-WinPositionTest-3600", ExpectedResult = 9)]
        [TestCase("1,3,4|0,9,5|9,2,9", Levels.One, TestName = "SweetTreats-WinPositionTest-49", ExpectedResult = 4)]
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
                                                1);

            return winPositions.Count;
        }

        [TestCase("9,9,9|9,9,9|9,9,9", Levels.One, TestName = "SweetTreats-WinPositionLineAndMultiplierAndSymbol-5600")]
        [TestCase("9,9,9|9,8,9|9,9,9", Levels.One, TestName = "SweetTreats-WinPositionLineAndMultiplierAndSymbol-3600")]
        [TestCase("1,3,4|0,9,5|9,2,9", Levels.One, TestName = "SweetTreats-WinPositionLineAndMultiplierAndSymbol-49")]
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
                                                1);

            Assert.IsTrue(!winPositions.Any(wp => wp.Line < 0 || wp.Multiplier <= 0 || wp.Symbol < 0));
        }

        [TestCase("9,9,9|9,9,9|9,9,9", Levels.One, TestName = "SweetTreats-RowPositionTest-5600", ExpectedResult = "0,1,0,0,1,0,0,1,0|1,0,0,1,0,0,1,0,0|0,0,1,0,0,1,0,0,1|1,1,1,0,0,0,0,0,0|0,0,0,1,1,1,0,0,0|0,0,0,0,0,0,1,1,1|1,0,0,0,1,0,0,0,1|0,0,1,0,1,0,1,0,0|1,1,1,1,1,1,1,1,1")]
        [TestCase("9,9,9|9,8,9|9,9,9", Levels.One, TestName = "SweetTreats-RowPositionTest-3600", ExpectedResult = "0,1,0,0,1,0,0,1,0|1,0,0,1,0,0,1,0,0|0,0,1,0,0,1,0,0,1|1,1,1,0,0,0,0,0,0|0,0,0,1,1,1,0,0,0|0,0,0,0,0,0,1,1,1|1,0,0,0,1,0,0,0,1|0,0,1,0,1,0,1,0,0|1,1,1,1,0,1,1,1,1")]
        [TestCase("1,3,4|0,9,5|9,2,9", Levels.One, TestName = "SweetTreats-RowPositionTest-49", ExpectedResult = "0,0,0,0,0,0,1,1,1|1,0,0,0,1,0,0,0,1|0,0,1,0,1,0,1,0,0|0,0,0,0,1,0,1,0,1")]
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
                                                1);

            return string.Join('|', winPositions.Select(wp => string.Join(',', wp.RowPositions)));
        }
    }
}
