using NUnit.Framework;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using Slot.Model;
using System.Collections.Generic;
using System.Linq;
using static Slot.UnitTests.FrostDragon.SpinsHelper;

namespace Slot.UnitTests.FrostDragon.Engines
{
    [TestFixture]
    public class CollapsingBonusEngineTests
    {
        [TestCase("4,1,3|4,1,7|2,1,3|3,5,2|6,3,4", Levels.One, TestName = "FrostDragon-CollapseRemoveWinPositionCount-3")]
        [TestCase("6,0,5|4,0,2|1,0,6|8,0,3|2,0,3", Levels.One, TestName = "FrostDragon-CollapseRemoveWinPositionCount-20")]
        [TestCase("2,7,4|1,7,0|7,2,1|8,0,3|2,0,3", Levels.One, TestName = "FrostDragon-CollapseRemoveWinPositionCount-Bonus")]
        public void EngineShouldRemoveCorrectWinPositionCount(string wheelString, int level)
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
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions, bonusPositions);

            Assert.AreEqual(winPositions.Count + bonusPositions.Count, removedWinPositions.Count);
        }

        [TestCase("4,1,3|4,1,7|2,1,3|3,5,2|6,3,4", Levels.One, TestName = "FrostDragon-CollapseRemoveWinPositionLine-3")]
        [TestCase("6,0,5|4,0,2|1,0,6|8,0,3|2,0,3", Levels.One, TestName = "FrostDragon-CollapseRemoveWinPositionLine-20")]
        [TestCase("2,7,4|1,7,0|7,2,1|8,0,3|2,0,3", Levels.One, TestName = "FrostDragon-CollapseRemoveWinPositionLine-Bonus")]
        public void EngineShouldRemoveCorrectWinPositions(string wheelString, int level)
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

        [TestCase("4,1,3|4,1,7|2,1,3|3,5,2|6,3,4", Levels.One, TestName = "FrostDragon-CollapseRemoveRowPositions-3")]
        [TestCase("6,0,5|4,0,2|1,0,6|8,0,3|2,0,3", Levels.One, TestName = "FrostDragon-CollapseRemoveRowPositions-20")]
        [TestCase("2,7,4|1,7,0|7,2,1|8,0,3|2,0,3", Levels.One, TestName = "FrostDragon-CollapseRemoveRowPositions-Bonus")]
        public void EngineShouldGetRemoveCorrectRowPositions(string wheelString, int level)
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

        [TestCase("4,1,3|4,1,7|2,1,3|3,5,2|6,3,4", Levels.One, TestName = "FrostDragon-CollapseRemoveItems-3")]
        [TestCase("6,0,5|4,0,2|1,0,6|8,0,3|2,0,3", Levels.One, TestName = "FrostDragon-CollapseRemoveItems-20")]
        [TestCase("2,7,4|1,7,0|7,2,1|8,0,3|2,0,3", Levels.One, TestName = "FrostDragon-CollapseRemoveItems-Bonus")]
        public void EngineShouldGetCorrectCollapseRemoveItems(string wheelString, int level)
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
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var removedWinPositions = CollapsingBonusEngine.GetRemoveWinPositions(winPositions, bonusPositions);
            var collapsingRemoves = CollapsingBonusEngine.GetCollapseRemoveItems(removedWinPositions);

            for (var widthIndex = 0; widthIndex < wheel.Width; widthIndex++)
            {
                var removeRowPositions = CollapsingBonusEngine.GetRemoveRowPositions(removedWinPositions, widthIndex);

                Assert.IsTrue(!collapsingRemoves[widthIndex].Except(removeRowPositions).Any());
            }
        }

        [TestCase(Levels.One, TestName = "FrostDragon-CollapseGenerateTopIndices")]
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

        [TestCase(Levels.One, TestName = "FrostDragon-CollapseAddItems")]
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

        [TestCase(Levels.One, TestName = "FrostDragon-CollapseWheel")]
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
