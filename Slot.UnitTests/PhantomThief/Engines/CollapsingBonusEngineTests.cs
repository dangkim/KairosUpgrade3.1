using NUnit.Framework;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using Slot.Model;
using System.Collections.Generic;
using System.Linq;
using static Slot.UnitTests.PhantomThief.SpinsHelper;

namespace Slot.UnitTests.PhantomThief.Engines
{
    [TestFixture]
    public class CollapsingBonusEngineTests
    {
        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", Levels.One, TestName = "PhantomThief-CollapseRemoveWinPositionCount-900")]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", Levels.One, TestName = "PhantomThief-CollapseRemoveWinPositionCount-450")]
        [TestCase("2,0,6|4,5,1|8,7,2|0,7,4|5,7,2", Levels.One, TestName = "PhantomThief-CollapseRemoveWinPositionCount-0")]
        public void EngineShouldRemoveCorrectWinPositionCount(string wheelString, int level)
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
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions, bonusPositions);

            Assert.AreEqual(winPositions.Count + bonusPositions.Count, removedWinPositions.Count);
        }

        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", Levels.One, TestName = "PhantomThief-CollapseRemoveWinPositionLine-900")]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", Levels.One, TestName = "PhantomThief-CollapseRemoveWinPositionLine-450")]
        [TestCase("2,0,6|4,5,1|8,7,2|0,7,4|5,7,2", Levels.One, TestName = "PhantomThief-CollapseRemoveWinPositionLine-0")]
        public void EngineShouldRemoveCorrectWinPositions(string wheelString, int level)
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
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions, bonusPositions);

            foreach (var winPositionIndex in winPositions.Select((Value, Index) => new { Value, Index }))
            {
                var removedWinPosition = removedWinPositions.ElementAt(winPositionIndex.Index);

                Assert.AreEqual(winPositionIndex.Value.Line, removedWinPosition.Line);
            }

            foreach (var bonusPositionIndex in bonusPositions.Select((Value, Index) => new { Value, Index }))
            {
                Assert.AreEqual(bonusPositionIndex.Value.Line, 0);
            }
        }

        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", Levels.One, TestName = "PhantomThief-CollapseRemoveRowPositions-900")]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", Levels.One, TestName = "PhantomThief-CollapseRemoveRowPositions-450")]
        [TestCase("2,0,6|4,5,1|8,7,2|0,7,4|5,7,2", Levels.One, TestName = "PhantomThief-CollapseRemoveRowPositions-0")]
        public void EngineShouldGetRemoveCorrectRowPositions(string wheelString, int level)
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
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions, bonusPositions);

            for (var widthIndex = 0; widthIndex < wheel.Width; widthIndex++)
            {
                var removeRowPositions = CollapsingBonusEngine.GetRemoveRowPositions(winPositions, widthIndex);

                var removeWinPositions = winPositions
                                            .Select(wp => wp.RowPositions[widthIndex] - 1)
                                            .Where(wp => wp != -1)
                                            .Distinct();

                var targetRowPositions = removeWinPositions;

                Assert.IsTrue(!targetRowPositions.Except(removeRowPositions).Any());
            }
        }

        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", Levels.One, TestName = "PhantomThief-CollapseRemoveItems-900")]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", Levels.One, TestName = "PhantomThief-CollapseRemoveItems-450")]
        [TestCase("2,0,6|4,5,1|8,7,2|0,7,4|5,7,2", Levels.One, TestName = "PhantomThief-CollapseRemoveItems-0")]
        public void EngineShouldGetCorrectCollapseRemoveItems(string wheelString, int level)
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
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions, bonusPositions);
            var collapsingRemoves = CollapsingBonusEngine.GetCollapseRemoveItems(removedWinPositions);

            for (var widthIndex = 0; widthIndex < wheel.Width; widthIndex++)
            {
                var removeRowPositions = CollapsingBonusEngine.GetRemoveRowPositions(removedWinPositions, widthIndex);

                Assert.IsTrue(!collapsingRemoves[widthIndex].Except(removeRowPositions).Any());
            }
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CollapseGenerateTopIndices")]
        public void EngineShouldGenerateCorrectNewTopIndices(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningNonBonusSpinResult(level);
            var referenceWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(spinResult.WinPositions, spinResult.BonusPositions);
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

        [TestCase(Levels.One, TestName = "PhantomThief-CollapseAddItems")]
        public void EngineShouldGenerateCorrectCollapseAddItems(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);
            var referenceWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(spinResult.WinPositions, spinResult.BonusPositions);
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

        [TestCase(Levels.One, TestName = "PhantomThief-CollapseWheel")]
        public void EngineShouldGenerateCorrectCollapseWheel(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWinningSpinResult(level);
            var referenceWheel = MainGameEngine.GetTargetWheel(level, config, spinResult.Wheel.ReelStripsId);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(spinResult.WinPositions, spinResult.BonusPositions);
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
