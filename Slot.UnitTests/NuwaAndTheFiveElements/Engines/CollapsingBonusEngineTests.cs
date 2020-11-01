using NUnit.Framework;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Model;
using System.Collections.Generic;
using System.Linq;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.Engines
{
    [TestFixture]
    public class CollapsingBonusEngineTests
    {
        [TestCase("0,1,2,3,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveWinPositionCount-20")]
        [TestCase("1,3,2,2,4,5,6,1,6,5,4,3,2,1,1,2,5,6,7,3,1,3,0,4,6,2,2,1,7,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveWinPositionCount-60")]
        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveWinPositionCount-1300")]
        public void EngineShouldRemoveCorrectWinPositionCount(string wheelString, int level)
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
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions);

            Assert.AreEqual(winPositions.Count, removedWinPositions.Count);
        }

        [TestCase("0,1,2,3,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveWinPositionLine-20")]
        [TestCase("1,3,2,2,4,5,6,1,6,5,4,3,2,1,1,2,5,6,7,3,1,3,0,4,6,2,2,1,7,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveWinPositionLine-60")]
        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveWinPositionLine-1300")]
        public void EngineShouldRemoveCorrectWinPositions(string wheelString, int level)
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
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions);

            foreach (var winPositionIndex in winPositions.Select((Value, Index) => new { Value, Index }))
            {
                var removedWinPosition = removedWinPositions.ElementAt(winPositionIndex.Index);

                Assert.AreEqual(winPositionIndex.Value.Line, removedWinPosition.Line);
            }
        }

        [TestCase("0,1,2,3,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveRowPositions-20")]
        [TestCase("1,3,2,2,4,5,6,1,6,5,4,3,2,1,1,2,5,6,7,3,1,3,0,4,6,2,2,1,7,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveRowPositions-60")]
        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveRowPositions-1300")]
        public void EngineShouldGetRemoveCorrectRowPositions(string wheelString, int level)
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
            var matchingSymbolPositions = MainGameEngine.GenerateMatchingSymbolPositions(config.SymbolCollapsePairs, winPositions.Select(wp => wp.Symbol).ToList(), wheel);
            var bombAndStopperPositions = MainGameEngine.GenerateBombAndStopperPositions(wheel, winPositions);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions);

            for (var widthIndex = 0; widthIndex < wheel.Width; widthIndex++)
            {
                var removeRowPositions = CollapsingBonusEngine.GetRemoveRowPositions(winPositions, matchingSymbolPositions, bombAndStopperPositions, widthIndex);

                var removeWinPositions = winPositions
                                            .Select(wp => wp.RowPositions[widthIndex] - 1)
                                            .Where(wp => wp != -1)
                                            .Distinct();
                var removeMatchingSymbolPositions = matchingSymbolPositions[widthIndex]
                                                        .Select(position => position - 1)
                                                        .Where(position => position != -1)
                                                        .Distinct();

                var targetRowPositions = removeWinPositions.Union(removeMatchingSymbolPositions);

                Assert.IsTrue(!targetRowPositions.Except(removeRowPositions).Any());
            }
        }

        [TestCase("0,1,2,3,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveItems-20")]
        [TestCase("1,3,2,2,4,5,6,1,6,5,4,3,2,1,1,2,5,6,7,3,1,3,0,4,6,2,2,1,7,4,0,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveItems-60")]
        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,3", Levels.One, TestName = "NuwaAndTheFiveElements-CollapseRemoveItems-1300")]
        public void EngineShouldGetCorrectCollapseRemoveItems(string wheelString, int level)
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
            var matchingSymbolPositions = MainGameEngine.GenerateMatchingSymbolPositions(config.SymbolCollapsePairs, winPositions.Select(wp => wp.Symbol).ToList(), wheel);
            var bombAndStopperPositions = MainGameEngine.GenerateBombAndStopperPositions(wheel, winPositions);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions);
            var collapsingRemoves = CollapsingBonusEngine.GetCollapseRemoveItems(removedWinPositions, matchingSymbolPositions, bombAndStopperPositions);

            for (var widthIndex = 0; widthIndex < wheel.Width; widthIndex++)
            {
                var removeRowPositions = CollapsingBonusEngine.GetRemoveRowPositions(removedWinPositions, matchingSymbolPositions, bombAndStopperPositions, widthIndex);

                Assert.IsTrue(!collapsingRemoves[widthIndex].Except(removeRowPositions).Any());
            }
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CollapseGenerateTopIndices")]
        public void EngineShouldGenerateCorrectNewTopIndices(int level)
        {
            var config = new Configuration();
            var referenceWheel = MainGameEngine.GetTargetWheel(level, config);
            var spinResult = GenerateWinningNonBonusSpinResult(level);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(spinResult.WinPositions);
            var matchingSymbolPositions = MainGameEngine.GenerateMatchingSymbolPositions(config.SymbolCollapsePairs, spinResult.WinPositions.Select(wp => wp.Symbol).ToList(), spinResult.Wheel);
            var bombAndStopperPositions = MainGameEngine.GenerateBombAndStopperPositions(spinResult.Wheel, spinResult.WinPositions);
            var collapsingRemoves = CollapsingBonusEngine.GetCollapseRemoveItems(removedWinPositions, matchingSymbolPositions, bombAndStopperPositions);
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

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CollapseAddItems")]
        public void EngineShouldGenerateCorrectCollapseAddItems(int level)
        {
            var config = new Configuration();
            var referenceWheel = config.Wheels[level];
            var spinResult = GenerateWinningSpinResult(level);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(spinResult.WinPositions);
            var matchingSymbolPositions = MainGameEngine.GenerateMatchingSymbolPositions(config.SymbolCollapsePairs, spinResult.WinPositions.Select(wp => wp.Symbol).ToList(), spinResult.Wheel);
            var bombAndStopperPositions = MainGameEngine.GenerateBombAndStopperPositions(spinResult.Wheel, spinResult.WinPositions);
            var collapsingRemoves = CollapsingBonusEngine.GetCollapseRemoveItems(removedWinPositions, matchingSymbolPositions, bombAndStopperPositions);
            var newTopIndices = CollapsingBonusEngine.GetCollapseTopIndices(referenceWheel, spinResult.TopIndices, collapsingRemoves);
            var collapsingAdds = CollapsingBonusEngine.GetCollapseAddItems(referenceWheel, newTopIndices, collapsingRemoves);

            for (var widthIndex = 0; widthIndex < spinResult.Wheel.Width; widthIndex++)
            {
                var referenceReel = referenceWheel[widthIndex];
                var targetReel = collapsingAdds[widthIndex];

                Assert.IsTrue(!targetReel.Except(referenceReel).Any());
            }
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CollapseWheel")]
        public void EngineShouldGenerateCorrectCollapseWheel(int level)
        {
            var config = new Configuration();
            var referenceWheel = config.Wheels[level];
            var spinResult = GenerateWinningSpinResult(level);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(spinResult.WinPositions);
            var collapsingRemoves = CollapsingBonusEngine.GetCollapseRemoveItems(removedWinPositions, spinResult.MatchingSymbolPositions, spinResult.BombAndStopperPositions);
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
