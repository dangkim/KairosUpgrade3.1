using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Slot.Games.MagicPaper.Models.GameResults;
using static Slot.Games.MagicPaper.Configuration.Configuration;

namespace Slot.UnitTests.MagicPaper
{
    [TestFixture]
    public class ExtensionTests
    {
        [TestCase(1, 1, TestName = "MagicPaper-WheelTest-ShouldCreateWheel-1,1")]
        [TestCase(2, 2, TestName = "MagicPaper-WheelTest-ShouldCreateWheel-2,2")]
        [TestCase(3, 3, TestName = "MagicPaper-WheelTest-ShouldCreateWheel-3,3")]
        public void EngineShouldCreateWheel(int wheelWidth, int wheelHeight)
        {
            var spinResult = new MagicPaperSpinResult()
                .CreateWheel(wheelWidth, wheelHeight);

            Assert.IsNotNull(spinResult.Wheel);
        }

        [TestCase(1, 1, TestName = "MagicPaper-WheelTest-ShouldCreateCorrectWheelDimension-1,1")]
        [TestCase(2, 2, TestName = "MagicPaper-WheelTest-ShouldCreateCorrectWheelDimension-2,2")]
        [TestCase(3, 3, TestName = "MagicPaper-WheelTest-ShouldCreateCorrectWheelDimension-3,3")]
        public void EngineShouldCreateCorrectWheelDimensions(int wheelWidth, int wheelHeight)
        {
            var spinResult = new MagicPaperSpinResult()
                .CreateWheel(wheelWidth, wheelHeight);

            Assert.AreEqual(wheelWidth, spinResult.Wheel.Width);
            Assert.AreEqual(wheelHeight, spinResult.Wheel.Height);
        }

        [TestCase("1,1,1,1,1,1,1,1,1", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CreateOnWin-1")]
        [TestCase("2,2,2,2,2,2,2,2,2", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CreateOnWin-2")]
        [TestCase("0,1,2,3,0,1,2,1,3", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CreateOnWin-3")]
        public void EngineShouldCreateWinPositionsOnWin(string wheelString, int level, decimal bet, int betLevel)
        {
            var wheelValues = Array.ConvertAll(wheelString.Split(','), Convert.ToInt32);
            var encodedWheel = EncodeWheel(Wheel.Width, Wheel.Height, wheelValues);

            var spinResult = new MagicPaperSpinResult()
            {
                Wheel = encodedWheel
            }.CalculateWin(bet, betLevel);

            Assert.IsNotNull(spinResult.WinPositions.FirstOrDefault());
        }

        [TestCase("1,0,1,1,1,1,1,3,1", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-NotCreateOnLose-1")]
        [TestCase("2,1,2,2,2,2,2,3,2", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-NotCreateOnLose-2")]
        [TestCase("0,1,2,3,2,1,2,3,3", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-NotCreateOnLose-3")]
        public void EngineShouldNotCreateWinPositionsOnLose(string wheelString, int level, decimal bet, int betLevel)
        {
            var wheelValues = Array.ConvertAll(wheelString.Split(','), Convert.ToInt32);
            var encodedWheel = EncodeWheel(Wheel.Width, Wheel.Height, wheelValues);

            var spinResult = new MagicPaperSpinResult()
            {
                Wheel = encodedWheel
            }.CalculateWin(bet, betLevel);

            Assert.IsNull(spinResult.WinPositions.FirstOrDefault());
        }

        [TestCase("0,0,0,0,0,0,0,0,0", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CorrectWinPositionWinAmount-1", ExpectedResult = 3)]
        [TestCase("1,1,1,1,1,1,1,1,1", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CorrectWinPositionWinAmount-2", ExpectedResult = 10)]
        [TestCase("2,2,2,2,2,2,2,2,2", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CorrectWinPositionWinAmount-3", ExpectedResult = 25)]
        [TestCase("0,1,2,3,0,1,2,1,3", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CorrectWinPositionWinAmount-4", ExpectedResult = 2)]
        public decimal EngineShouldCreateCorrectWinPositionsWin(string wheelString, int level, decimal bet, int betLevel)
        {
            var wheelValues = Array.ConvertAll(wheelString.Split(','), Convert.ToInt32);
            var encodedWheel = EncodeWheel(Wheel.Width, Wheel.Height, wheelValues);

            var spinResult = new MagicPaperSpinResult()
                {
                    Wheel = encodedWheel
                }
                .CalculateWin(bet, betLevel);

            return spinResult.WinPositions.FirstOrDefault().Win;
        }

        [TestCase("1,1,1,1,1,1,1,1,1", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CorrectWinPositionLineNumber-1", ExpectedResult = Lines)]
        [TestCase("2,2,2,2,2,2,2,2,2", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CorrectWinPositionLineNumber-2", ExpectedResult = Lines)]
        [TestCase("0,1,2,3,0,1,2,1,3", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CorrectWinPositionLineNumber-3", ExpectedResult = Lines)]
        public int EngineShouldCreateWinPositionLineNumber(string wheelString, int level, decimal bet, int betLevel)
        {
            var wheelValues = Array.ConvertAll(wheelString.Split(','), Convert.ToInt32);
            var encodedWheel = EncodeWheel(Wheel.Width, Wheel.Height, wheelValues);

            var spinResult = new MagicPaperSpinResult()
            {
                Wheel = encodedWheel
            }.CalculateWin(bet, betLevel);

            return spinResult.WinPositions.FirstOrDefault().Line;
        }

        [TestCase("1,1,1,1,1,1,1,1,1", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CreateCorrectRowPosition-1", ExpectedResult = "0,1,0,0,1,0,0,1,0")]
        [TestCase("2,2,2,2,2,2,2,2,2", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CreateCorrectRowPosition-2", ExpectedResult = "0,1,0,0,1,0,0,1,0")]
        [TestCase("0,1,2,3,0,1,2,1,3", LevelOne, 1, 1, TestName = "MagicPaper-WinPositionTest-CreateCorrectRowPosition-3", ExpectedResult = "0,1,0,0,1,0,0,1,0")]
        public string EngineShouldCreateCorrectRowPositionsForWinPositions(string wheelString, int level, decimal bet, int betLevel)
        {
            var wheelValues = Array.ConvertAll(wheelString.Split(','), Convert.ToInt32);
            var encodedWheel = EncodeWheel(Wheel.Width, Wheel.Height, wheelValues);

            var spinResult = new MagicPaperSpinResult()
            {
                Wheel = encodedWheel
            }.CalculateWin(bet, betLevel);

            return string.Join(",", spinResult.WinPositions.FirstOrDefault().RowPositions);
        }

        [TestCase("0,1,2,3,0,1,2,3,0", LevelOne, 1, 1, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel1-0", ExpectedResult = 0)]
        [TestCase("0,0,0,0,0,0,0,0,0", LevelOne, 1, 1, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel1-3", ExpectedResult = 3)]
        [TestCase("1,1,1,1,1,1,1,1,1", LevelOne, 1, 1, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel1-10", ExpectedResult = 10)]
        [TestCase("2,2,2,2,2,2,2,2,2", LevelOne, 1, 1, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel1-25", ExpectedResult = 25)]
        [TestCase("0,1,2,3,0,1,2,1,3", LevelOne, 1, 1, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel1-2", ExpectedResult = 2)]

        [TestCase("0,1,2,3,0,1,2,3,0", LevelOne, 3, 1, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel1-0", ExpectedResult = 0)]
        [TestCase("0,0,0,0,0,0,0,0,0", LevelOne, 3, 1, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel1-3", ExpectedResult = 9)]
        [TestCase("1,1,1,1,1,1,1,1,1", LevelOne, 3, 1, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel1-10", ExpectedResult = 30)]
        [TestCase("2,2,2,2,2,2,2,2,2", LevelOne, 3, 1, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel1-25", ExpectedResult = 75)]
        [TestCase("0,1,2,3,0,1,2,1,3", LevelOne, 3, 1, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel1-2", ExpectedResult = 6)]

        [TestCase("0,1,2,3,0,1,2,3,0", LevelOne, 1, 2, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel2-0", ExpectedResult = 0)]
        [TestCase("0,0,0,0,0,0,0,0,0", LevelOne, 1, 2, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel2-3", ExpectedResult = 6)]
        [TestCase("1,1,1,1,1,1,1,1,1", LevelOne, 1, 2, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel2-10", ExpectedResult = 20)]
        [TestCase("2,2,2,2,2,2,2,2,2", LevelOne, 1, 2, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel2-25", ExpectedResult = 50)]
        [TestCase("0,1,2,3,0,1,2,1,3", LevelOne, 1, 2, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel2-2", ExpectedResult = 4)]

        [TestCase("0,1,2,3,0,1,2,3,0", LevelOne, 3, 2, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel2-0", ExpectedResult = 0)]
        [TestCase("0,0,0,0,0,0,0,0,0", LevelOne, 3, 2, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel2-3", ExpectedResult = 18)]
        [TestCase("1,1,1,1,1,1,1,1,1", LevelOne, 3, 2, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel2-10", ExpectedResult = 60)]
        [TestCase("2,2,2,2,2,2,2,2,2", LevelOne, 3, 2, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel2-25", ExpectedResult = 150)]
        [TestCase("0,1,2,3,0,1,2,1,3", LevelOne, 3, 2, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel2-2", ExpectedResult = 12)]

        [TestCase("0,1,2,3,0,1,2,3,0", LevelOne, 1, 3, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel3-0", ExpectedResult = 0)]
        [TestCase("0,0,0,0,0,0,0,0,0", LevelOne, 1, 3, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel3-3", ExpectedResult = 9)]
        [TestCase("1,1,1,1,1,1,1,1,1", LevelOne, 1, 3, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel3-10", ExpectedResult = 30)]
        [TestCase("2,2,2,2,2,2,2,2,2", LevelOne, 1, 3, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel3-25", ExpectedResult = 85)]
        [TestCase("0,1,2,3,0,1,2,1,3", LevelOne, 1, 3, TestName = "MagicPaper-PayoutTest-1Bet-BetLevel3-2", ExpectedResult = 6)]

        [TestCase("0,1,2,3,0,1,2,3,0", LevelOne, 3, 3, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel3-0", ExpectedResult = 0)]
        [TestCase("0,0,0,0,0,0,0,0,0", LevelOne, 3, 3, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel3-3", ExpectedResult = 27)]
        [TestCase("1,1,1,1,1,1,1,1,1", LevelOne, 3, 3, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel3-10", ExpectedResult = 90)]
        [TestCase("2,2,2,2,2,2,2,2,2", LevelOne, 3, 3, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel3-25", ExpectedResult = 255)]
        [TestCase("0,1,2,3,0,1,2,1,3", LevelOne, 3, 3, TestName = "MagicPaper-PayoutTest-3Bet-BetLevel3-2", ExpectedResult = 18)]
        public decimal EngineShouldReturnCorrectPayout(string wheelString, int level, decimal bet, int betLevel)
        {
            var wheelValues = Array.ConvertAll(wheelString.Split(','), Convert.ToInt32);
            var encodedWheel = EncodeWheel(Wheel.Width, Wheel.Height, wheelValues);

            var spinResult = new MagicPaperSpinResult()
                {
                    Wheel = encodedWheel
                }
                .CalculateWin(bet, betLevel);

            return spinResult.Win;
        }

        private Model.Wheel EncodeWheel(int width, int height, int[] wheelValues)
        {
            var currentIndex = 0;
            var wheel = new Model.Wheel(new List<int>() { 3, 3, 3 });

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < wheel.Rows[i]; ++j)
                {
                    if (i > 0)
                    {
                        wheel[i].Add(wheelValues[currentIndex + j]);
                    }
                    else
                    {
                        wheel[i].Add(wheelValues[j]);
                    }
                }

                currentIndex = currentIndex + wheel.Rows[i];
            }

            return wheel;
        }
    }
}