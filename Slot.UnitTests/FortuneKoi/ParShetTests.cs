namespace Slot.UnitTests.FortuneKoi
{
    using NUnit.Framework;
    using Slot.Games.FortuneKoi;
    using System.Collections.Generic;

    [TestFixture]
    internal class ParShetTests
    {
        [TestCase(TestName = "Test Create A Wheel")]
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

        [TestCase(TestName = "Test Any Wild exist In A Reel")]
        public void TestHasWildInReel()
        {
            // Arrange
            var reel1 = new[] { 1, 2, 4 };
            var reel2 = new[] { 1, 7, 4 };

            //Assert
            Assert.IsTrue(!ParSheet.HasWild(reel1));
            Assert.IsTrue(ParSheet.HasWild(reel2));
        }

        [TestCase(TestName = "Create A ReSpin Wheel")]
        public void TestCreateAReSpinWheel()
        {
            // Arrange
            var previousWheel = new List<int[]> {
                new int[] { },
                new int[] { 7,7,7},
                new int[] { },
                new int[] {7,7,7 },
                new int[] { }
            };
            var expandReels = new bool[] { false, true, false, true, false };
            var strips = new List<IReadOnlyList<int>> {
                new List<int> { 1,1,1},
                new List<int> { 2,2,2},
                new List<int> { 3,7,3},
                new List<int> { 4,4,4},
                new List<int> { 5,5,5}};

            var strips2 = new List<IReadOnlyList<int>> {
                new List<int> { 1,1,1},
                new List<int> { 2,2,2},
                new List<int> { 3,3,3},
                new List<int> { 4,4,4},
                new List<int> { 5,5,5}};

            // Action
            var wheel = ParSheet.CreateReSpinWheel(strips, previousWheel, expandReels);
            var wheel1 = ParSheet.CreateReSpinWheel(strips2, previousWheel, expandReels);

            //Assert
            Assert.NotNull(wheel);
            Assert.AreEqual(wheel[0], new[] { 1, 1, 1 });
            Assert.AreEqual(wheel[1], new[] { 7, 7, 7 });
            Assert.AreEqual(wheel[2], new[] { 7, 7, 7 });
            Assert.AreEqual(wheel[3], new[] { 7, 7, 7 });
            Assert.AreEqual(wheel[4], new[] { 5, 5, 5 });
            Assert.AreEqual(wheel.ExpandReels, new bool[] { false, true, true, true, false });

            Assert.NotNull(wheel);
            Assert.AreEqual(wheel1[0], new[] { 1, 1, 1 });
            Assert.AreEqual(wheel1[1], new[] { 7, 7, 7 });
            Assert.AreEqual(wheel1[2], new[] { 3, 3, 3 });
            Assert.AreEqual(wheel1[3], new[] { 7, 7, 7 });
            Assert.AreEqual(wheel1[4], new[] { 5, 5, 5 });
            Assert.AreEqual(wheel1.ExpandReels, new bool[] { false, true, false, true, false });
        }
    }
}