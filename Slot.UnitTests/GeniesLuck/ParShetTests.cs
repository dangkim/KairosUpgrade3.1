namespace Slot.UnitTests.GeniesLuck
{
    using NUnit.Framework;
    using Slot.Games.GeniesLuck;
    using System.Collections.Generic;

    [TestFixture]
    internal class ParShetTests
    {
        [TestCase(TestName = "Create A Wheel")]
        public void TestCreateAWheel()
        {
            // Arrange
            var strips = new List<IReadOnlyList<int>> {
                new List<int> { 1,1,1},
                new List<int> { 2,2,2,2},
                new List<int> { 12,12,12,12},
                new List<int> { 4,4,4,4},
                new List<int> { 5,5,5}};

            // Action
            var wheel = ParSheet.CreateWheel(strips, -1);

            //Assert
            Assert.NotNull(wheel);
            Assert.AreEqual(wheel[0], new[] { 1, 1, 1 });
            Assert.AreEqual(wheel[1], new[] { 2, 2, 2, 2 });
            Assert.AreEqual(wheel[2], new[] { -1, -1, -1, -1 });
            Assert.AreEqual(wheel[3], new[] { 4, 4, 4, 4 });
            Assert.AreEqual(wheel[4], new[] { 5, 5, 5 });
        }

        [TestCase(TestName = "Create A ReSpin Wheel")]
        public void TestCreateAReSpinWheel()
        {
            // Arrange
            var strips = new List<IReadOnlyList<int>> {
                new List<int> { 1,1,1},
                new List<int> { 2,2,2,2},
                new List<int> { 3,3,3,3},
                new List<int> { 4,4,4,4},
                new List<int> { 11, 11, 11}};

            // Action
            var wheel = ParSheet.CreateReSpinWheel(strips, new List<int[]> {
                new int[] { 9, 9, 9},
                new int[] { 9, -1, -1, -1},
                new int[] { 9, 9, 9, -1},
                new int[] { -1, 11, -1, -1},
                new int[] { -1, 1, -1},});

            //Assert
            Assert.NotNull(wheel);
            Assert.AreEqual(wheel[0], new[] { 9, 9, 9 });
            Assert.AreEqual(wheel[1], new[] { 9, 2, 2, 2 });
            Assert.AreEqual(wheel[2], new[] { 9, 9, 9, 3 });
            Assert.AreEqual(wheel[3], new[] { 4, 11, 4, 4 });
            Assert.AreEqual(wheel[4], new[] { 11, 11, 11 });
        }
    }
}