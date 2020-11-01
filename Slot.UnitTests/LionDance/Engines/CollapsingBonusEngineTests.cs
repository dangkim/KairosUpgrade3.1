using NUnit.Framework;
using Slot.Games.LionDance.Configuration;
using Slot.Games.LionDance.Engines;
using Slot.Model;
using System.Collections.Generic;
using System.Linq;
using static Slot.UnitTests.LionDance.SpinsHelper;

namespace Slot.UnitTests.LionDance.Engines
{
    [TestFixture]
    public class CollapsingBonusEngineTests
    {
        [TestCase("1,8,5|0,1,3|2,0,8", Levels.One, TestName = "LionDance-CollapseRemoveWinPositionCount-29")]
        [TestCase("2,1,8|6,0,4|7,4,1", Levels.One, TestName = "LionDance-CollapseRemoveWinPositionCount-18")]
        public void EngineShouldRemoveCorrectWinPositionCount(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, 1, 1);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions);

            Assert.AreEqual(winPositions.Count, removedWinPositions.Count);
        }

        [TestCase("1,8,5|0,1,3|2,0,8", Levels.One, TestName = "LionDance-CollapseRemoveWinPositionLine-29")]
        [TestCase("2,1,8|6,0,4|7,4,1", Levels.One, TestName = "LionDance-CollapseRemoveWinPositionLine-18")]
        public void EngineShouldRemoveCorrectWinPositions(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, 1, 1);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions);

            foreach (var winPositionIndex in winPositions.Select((Value, Index) => new { Value, Index }))
            {
                var removedWinPosition = removedWinPositions.ElementAt(winPositionIndex.Index);

                Assert.AreEqual(winPositionIndex.Value.Line, removedWinPosition.Line);
            }
        }

        [TestCase("1,8,5|0,1,3|2,0,8", Levels.One, TestName = "LionDance-CollapseRemoveRowPositions-29")]
        [TestCase("2,1,8|6,0,4|7,4,1", Levels.One, TestName = "LionDance-CollapseRemoveRowPositions-18")]
        public void EngineShouldGetRemoveCorrectRowPositions(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, 1, 1);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions);

            for (var widthIndex = 0; widthIndex < wheel.Width; widthIndex++)
            {
                for (var heightIndex = 0; heightIndex < Game.WheelHeight; heightIndex++)
                {
                    var index = (widthIndex * Game.WheelHeight) + heightIndex;
                    var removeRowPositions = CollapsingBonusEngine.GetRemoveRowPositions(winPositions, index);

                    var removeWinPositions = winPositions
                                                .Select(wp => wp.RowPositions[index] > 0 ? index % Game.WheelWidth : -1)
                                                .Where(wp => wp != -1)
                                                .Distinct();

                    var targetRowPositions = removeWinPositions;

                    Assert.IsTrue(!targetRowPositions.Except(removeRowPositions).Any());
                }
            }
        }

        [TestCase("1,8,5|0,1,3|2,0,8", Levels.One, TestName = "LionDance-CollapseRemoveItems-29")]
        [TestCase("2,1,8|6,0,4|7,4,1", Levels.One, TestName = "LionDance-CollapseRemoveItems-18")]
        public void EngineShouldGetCorrectCollapseRemoveItems(string wheelString, int level)
        {
            var config = new Configuration();
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, 1, 1);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions);
            var collapsingRemoves = CollapsingBonusEngine.GetCollapseRemoveItems(removedWinPositions);

            for (var widthIndex = 0; widthIndex < wheel.Width; widthIndex++)
            {
                var removeRowPositionsCollection = new List<int>();

                for (var heightIndex = 0; heightIndex < Game.WheelHeight; heightIndex++)
                {
                    var index = (widthIndex * Game.WheelHeight) + heightIndex;
                    var removeRowPositions = CollapsingBonusEngine.GetRemoveRowPositions(removedWinPositions, index);
                    removeRowPositionsCollection.AddRange(removeRowPositions);
                }

                Assert.IsTrue(!collapsingRemoves[widthIndex].Except(removeRowPositionsCollection).Any());
            }
        }

        [TestCase(Levels.One, TestName = "LionDance-CollapseGenerateTopIndices")]
        public void EngineShouldGenerateCorrectNewTopIndices(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateNonWinningSpinResult(level);
            var referenceWheel = MainGameEngine.GetTargetWheel(level, config);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(spinResult.WinPositions);
            var collapsingRemoves = CollapsingBonusEngine.GetCollapseRemoveItems(removedWinPositions);
            var newTopIndices = CollapsingBonusEngine.GetCollapseTopIndices(referenceWheel, spinResult.TopIndices, collapsingRemoves);

            for (var widthIndex = 0; widthIndex < spinResult.Wheel.Width; widthIndex++)
            {
                var previousTopIndex = spinResult.TopIndices[widthIndex];
                var currentTopIndex = newTopIndices[widthIndex];

                var tempIndex = previousTopIndex - collapsingRemoves[widthIndex].Count;
                var expectedNewTopIndex = tempIndex >= 0 ? tempIndex : referenceWheel[widthIndex].Count + tempIndex;

                Assert.AreEqual(expectedNewTopIndex, currentTopIndex);
            }
        }

        [TestCase(Levels.One, TestName = "LionDance-CollapseAddItems")]
        public void EngineShouldGenerateCorrectCollapseAddItems(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);
            var referenceWheel = MainGameEngine.GetTargetWheel(level, config);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(spinResult.WinPositions);
            var collapsingRemoves = CollapsingBonusEngine.GetCollapseRemoveItems(removedWinPositions);
            var newTopIndices = CollapsingBonusEngine.GetCollapseTopIndices(referenceWheel, spinResult.TopIndices, collapsingRemoves);
            var collapsingAdds = CollapsingBonusEngine.GetCollapseAddItems(referenceWheel, newTopIndices, collapsingRemoves);

            for (var widthIndex = 0; widthIndex < spinResult.Wheel.Width; widthIndex++)
            {
                var referenceReel = referenceWheel[widthIndex];
                var targetReel = collapsingAdds[widthIndex];

                Assert.IsTrue(!targetReel.Except(referenceReel).Any());
            }
        }

        [TestCase(Levels.One, TestName = "LionDance-CollapseWheel")]
        public void EngineShouldGenerateCorrectCollapseWheel(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);
            var referenceWheel = MainGameEngine.GetTargetWheel(level, config);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(spinResult.WinPositions);
            var collapsingRemoves = CollapsingBonusEngine.GetCollapseRemoveItems(removedWinPositions);
            var newTopIndices = CollapsingBonusEngine.GetCollapseTopIndices(referenceWheel, spinResult.TopIndices, collapsingRemoves);
            var collapsingAdds = CollapsingBonusEngine.GetCollapseAddItems(referenceWheel, newTopIndices, collapsingRemoves);
            var collapsingWheel = CollapsingBonusEngine.GenerateCollapsedWheel(spinResult.Wheel, collapsingRemoves, collapsingAdds);
            var newWheel = spinResult.Wheel.Copy();

            foreach (var removeItem in collapsingRemoves)
            {
                var cleanReel = newWheel[removeItem.Key];

                foreach (var index in removeItem.Value.OrderByDescending(val => val))
                {
                    cleanReel.RemoveAt(index);
                }

                newWheel[removeItem.Key] = collapsingAdds[removeItem.Key].Concat(cleanReel).ToList();
            }

            foreach (var reelIndex in collapsingWheel.Reels.Select((Value, Index) => new { Value, Index }))
            {
                var referenceReel = newWheel[reelIndex.Index];

                Assert.IsTrue(!referenceReel.Except(reelIndex.Value).Any());
            }


            newWheel.Equals(collapsingWheel);
        }
    }
}
