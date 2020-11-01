using NUnit.Framework;
using Slot.Games.FourGuardians.Configuration;
using Slot.Games.FourGuardians.Engines;
using Slot.Model;
using System.Linq;

namespace Slot.UnitTests.FourGuardians.Engines
{
    [TestFixture]
    public class ExpandingWildsEngineTests
    {
        [TestCase(0, 2, TestName = "FourGuardians-DetermineIfCoordinatesAreWithinWheelBounds-1", ExpectedResult = true)]
        [TestCase(5, 2, TestName = "FourGuardians-DetermineIfCoordinatesAreWithinWheelBounds-2", ExpectedResult = false)]
        [TestCase(2, 2, TestName = "FourGuardians-DetermineIfCoordinatesAreWithinWheelBounds-3", ExpectedResult = true)]
        [TestCase(5, 3, TestName = "FourGuardians-DetermineIfCoordinatesAreWithinWheelBounds-4", ExpectedResult = false)]
        [TestCase(1, 0, TestName = "FourGuardians-DetermineIfCoordinatesAreWithinWheelBounds-5", ExpectedResult = true)]
        [TestCase(4, 5, TestName = "FourGuardians-DetermineIfCoordinatesAreWithinWheelBounds-6", ExpectedResult = false)]
        [TestCase(4, 2, TestName = "FourGuardians-DetermineIfCoordinatesAreWithinWheelBounds-7", ExpectedResult = true)]
        [TestCase(0, 5, TestName = "FourGuardians-DetermineIfCoordinatesAreWithinWheelBounds-8", ExpectedResult = false)]
        [TestCase(-1, 1, TestName = "FourGuardians-DetermineIfCoordinatesAreWithinWheelBounds-9", ExpectedResult = false)]
        public bool EngineShouldDetermineIfCoordinatesAreWithinWheelBounds(int widthIndex, int heightIndex)
        {
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight);
            return wheel.IsInBounds(widthIndex, heightIndex);
        }

        [TestCase(0, 2, TestName = "FourGuardians-ExpandCrossWildSymbolCorrectly-1", ExpectedResult = "0,2")]
        [TestCase(4, 0, TestName = "FourGuardians-ExpandCrossWildSymbolCorrectly-2", ExpectedResult = "4,0")]
        [TestCase(1, 1, TestName = "FourGuardians-ExpandCrossWildSymbolCorrectly-3", ExpectedResult = "0,0|0,2|1,1|2,0|2,2")]
        public string EngineShouldExpandCrossWildSymbolCorrectly(int widthIndex, int heightIndex)
        {
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight);

            for (var columnIndex = 0; columnIndex < Game.WheelWidth; columnIndex++)
                wheel.Reels[columnIndex].AddRange(Enumerable.Repeat(0, Game.WheelHeight));

            wheel.Reels[widthIndex][heightIndex] = Symbols.WildCross;

            ExpandingWildsEngine.ExpandWildCrossSymbol(wheel, widthIndex, heightIndex);
            SpinsHelper.DisplayWheelOnOutput(wheel);

            var expandedCoordinates = SpinsHelper.GetWildCoordinates(wheel, Symbols.Wild);

            return string.Join('|', expandedCoordinates);
        }

        [TestCase(0, 2, TestName = "FourGuardians-ExpandSquareWildSymbolCorrectly-1", ExpectedResult = "0,1|0,2|1,1|1,2")]
        [TestCase(4, 0, TestName = "FourGuardians-ExpandSquareWildSymbolCorrectly-2", ExpectedResult = "3,0|3,1|4,0|4,1")]
        [TestCase(1, 1, TestName = "FourGuardians-ExpandSquareWildSymbolCorrectly-3", ExpectedResult = "0,0|0,1|0,2|1,0|1,1|1,2|2,0|2,1|2,2")]
        [TestCase(2, 2, TestName = "FourGuardians-ExpandSquareWildSymbolCorrectly-4", ExpectedResult = "1,1|1,2|2,1|2,2|3,1|3,2")]
        public string EngineShouldExpandSquareWildSymbolCorrectly(int widthIndex, int heightIndex)
        {
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight);

            for (var columnIndex = 0; columnIndex < Game.WheelWidth; columnIndex++)
                wheel.Reels[columnIndex].AddRange(Enumerable.Repeat(0, Game.WheelHeight));

            wheel.Reels[widthIndex][heightIndex] = Symbols.WildSquare;

            ExpandingWildsEngine.ExpandWildSquareSymbol(wheel, widthIndex, heightIndex);
            SpinsHelper.DisplayWheelOnOutput(wheel);

            var expandedCoordinates = SpinsHelper.GetWildCoordinates(wheel, Symbols.Wild);

            return string.Join('|', expandedCoordinates);
        }

        [TestCase(0, 2, TestName = "FourGuardians-ExpandHorizontalWildSymbolCorrectly-1", ExpectedResult = "0,2|1,2")]
        [TestCase(4, 0, TestName = "FourGuardians-ExpandHorizontalWildSymbolCorrectly-2", ExpectedResult = "3,0|4,0")]
        [TestCase(1, 1, TestName = "FourGuardians-ExpandHorizontalWildSymbolCorrectly-3", ExpectedResult = "0,1|1,1|2,1")]
        [TestCase(2, 2, TestName = "FourGuardians-ExpandHorizontalWildSymbolCorrectly-4", ExpectedResult = "1,2|2,2|3,2")]
        public string EngineShouldExpandHorizontalWildSymbolCorrectly(int widthIndex, int heightIndex)
        {
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight);

            for (var columnIndex = 0; columnIndex < Game.WheelWidth; columnIndex++)
                wheel.Reels[columnIndex].AddRange(Enumerable.Repeat(0, Game.WheelHeight));

            wheel.Reels[widthIndex][heightIndex] = Symbols.WildHorizontal;

            ExpandingWildsEngine.ExpandWildHorizontalSymbol(wheel, widthIndex, heightIndex);
            SpinsHelper.DisplayWheelOnOutput(wheel);

            var expandedCoordinates = SpinsHelper.GetWildCoordinates(wheel, Symbols.Wild);

            return string.Join('|', expandedCoordinates);
        }

        [TestCase(0, 2, TestName = "FourGuardians-ExpandVerticalWildSymbolCorrectly-1", ExpectedResult = "0,0|0,1|0,2")]
        [TestCase(4, 0, TestName = "FourGuardians-ExpandVerticalWildSymbolCorrectly-2", ExpectedResult = "4,0|4,1|4,2")]
        [TestCase(1, 1, TestName = "FourGuardians-ExpandVerticalWildSymbolCorrectly-3", ExpectedResult = "1,0|1,1|1,2")]
        [TestCase(2, 2, TestName = "FourGuardians-ExpandVerticalWildSymbolCorrectly-4", ExpectedResult = "2,0|2,1|2,2")]
        public string EngineShouldExpandVerticalWildSymbolCorrectly(int widthIndex, int heightIndex)
        {
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight);

            for (var columnIndex = 0; columnIndex < Game.WheelWidth; columnIndex++)
                wheel.Reels[columnIndex].AddRange(Enumerable.Repeat(0, Game.WheelHeight));

            wheel.Reels[widthIndex][heightIndex] = Symbols.WildVertical;

            ExpandingWildsEngine.ExpandWildVerticalSymbol(wheel, widthIndex, heightIndex);
            SpinsHelper.DisplayWheelOnOutput(wheel);

            var expandedCoordinates = SpinsHelper.GetWildCoordinates(wheel, Symbols.Wild);

            return string.Join('|', expandedCoordinates);
        }
    }
}
