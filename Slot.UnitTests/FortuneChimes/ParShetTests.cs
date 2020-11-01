namespace Slot.UnitTests.FortuneChimes
{
    using NUnit.Framework;
    using Slot.Games.FortuneChimes;
    using Slot.Model;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="ParShetTests"/>
    /// </summary>
    [TestFixture]
    internal class ParShetTests
    {
        [TestCase(TestName = "Get A Reel")]
        public void TestGetAReel()
        {
            // Arrange
            var strips = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            // Action
            var reel = ParSheet.CascadeReel(strips, 2, 3);

            //Assert
            Assert.NotNull(reel);
            Assert.AreEqual(reel.strip, new[] { 0, 1, 2 });
            Assert.AreEqual(reel.indexSelected, 9);
        }

        /// <summary>
        /// The TestCreateAWheel
        /// </summary>
        [TestCase(TestName = "Create A Wheel")]
        public void TestCreateAWheel()
        {
            // Arrange
            var strips = new List<IReadOnlyList<int>> {
                new List<int> { 1,1,1},
                new List<int> { 2,2,2},
                new List<int> { 3,3,3},
                new List<int> { 4,4,4},
                new List<int> { 5,5,5}};

            // Action
            var wheel = ParSheet.CreateWheel(strips);

            //Assert
            Assert.NotNull(wheel);
            Assert.AreEqual(wheel[0], new[] { 1, 1, 1 });
            Assert.AreEqual(wheel[1], new[] { 2, 2, 2 });
            Assert.AreEqual(wheel[2], new[] { 3, 3, 3 });
            Assert.AreEqual(wheel[3], new[] { 4, 4, 4 });
            Assert.AreEqual(wheel[4], new[] { 5, 5, 5 });
        }

        /// <summary>
        /// The TestCreateAReSpinWheel
        /// </summary>
        [TestCase(TestName = "Create A ReSpin Wheel")]
        public void TestCreateAReSpinWheel()
        {
            // Arrange
            var strips = new List<IReadOnlyList<int>> {
                new List<int> { 1,1,1},
                new List<int> { -2,-2,-2},
                new List<int> { 3,3,3},
                new List<int> { -1,-1,-1},
                new List<int> { 5,5,5}};

            // Action
            var wheel = ParSheet.CreateReSpinWheel(strips, new int[] { 0, 0, 0, 0, 0 }, new List<int[]> { new int[] { 1, 2, 3 }, new int[0], new int[] { 4, 5, 6 }, new int[0], new int[] { 7, 8, 9 } });

            //Assert
            Assert.NotNull(wheel);
            Assert.AreEqual(wheel[0], new[] { 1, 2, 3 });
            Assert.AreEqual(wheel[1], new[] { -2, -2, -2 });
            Assert.AreEqual(wheel[2], new[] { 4, 5, 6 });
            Assert.AreEqual(wheel[3], new[] { -1, -1, -1 });
            Assert.AreEqual(wheel[4], new[] { 7, 8, 9 });
        }

        /// <summary>
        /// The TestReelsExploding
        /// </summary>
        [TestCase(TestName = "Reels Exploding")]
        public void TestReelsExploding()
        {
            // Arrange
            var wheel = new List<int[]> {
                new int[]{1,1,1 },
                new int[]{2,2,2 },
                new int[]{3,3,3 },
                new int[]{4,4,4 },
                new int[]{5,5,5 },
            };
            var winPositions1 = new List<WinPosition>
            {
                new WinPosition { Line = 0,Symbol = 10, Count =3, RowPositions= new List<int>{0, 1,2,3,0 } },
            };

            var winPositions2 = new List<WinPosition> {
                new WinPosition { Line = 1, Count = 4,RowPositions= new List<int>{1,2,3,4,0 } }
            };

            var winPositions3 = new List<WinPosition> {
                new WinPosition { Line = 0, Count = 3, Symbol = 10, RowPositions= new List<int>{0, 1,2,3,0 } },
                new WinPosition { Line = 1, Count = 3, RowPositions= new List<int>{1,2,3,0,0 } }};

            var winPositions4 = new List<WinPosition> {
                new WinPosition { Line = 0, Count = 3, Symbol = 10, RowPositions= new List<int>{0, 1,2,3,0 } },
                new WinPosition { Line = 1, Count = 5, RowPositions= new List<int>{1,2,3,1,2 } }};

            // Action
            var result1 = ParSheet.ExplodeReels(winPositions1, wheel);
            var result2 = ParSheet.ExplodeReels(winPositions2, wheel);
            var result3 = ParSheet.ExplodeReels(winPositions3, wheel);
            var result4 = ParSheet.ExplodeReels(winPositions4, wheel);

            //Assert
            Assert.NotNull(result1); Assert.AreEqual(result1, new List<int[]> { new int[] { 1, 1, 1 }, new int[0], new int[0], new int[0], new int[] { 5, 5, 5 } });
            Assert.NotNull(result2); Assert.AreEqual(result2, new List<int[]> { new int[0], new int[0], new int[0], new int[0], new int[] { 5, 5, 5 } });
            Assert.NotNull(result3); Assert.AreEqual(result3, new List<int[]> { new int[0], new int[0], new int[0], new int[0], new int[] { 5, 5, 5 } });
            Assert.NotNull(result4); Assert.AreEqual(result4, new List<int[]> { new int[0], new int[0], new int[0], new int[0], new int[0] });
        }
    }
}