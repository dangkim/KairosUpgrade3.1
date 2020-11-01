namespace Slot.UnitTests.BikiniBeach
{
    using NUnit.Framework;
    using Slot.Games.BikiniBeach;
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

        [TestCase(TestName = "Create A Free Spin Wheel")]
        public void TestCreateAFreeSpinWheel()
        {
            // Arrange
            var strips = new List<IReadOnlyList<int>> {
                new List<int> { 1,1,1},
                new List<int> { 2,2,2,2},
                new List<int> { 3,3,3,3},
                new List<int> { 4,4,4,4},
                new List<int> { 5,5,5}};

            // Action
            var wheel1 = ParSheet.CreateExpandWheel(strips, 1);
            var wheel2 = ParSheet.CreateExpandWheel(strips, 2);
            var wheel3 = ParSheet.CreateExpandWheel(strips, 3);

            //Assert
            Assert.NotNull(wheel1);
            Assert.NotNull(wheel2);
            Assert.NotNull(wheel3);

            Assert.AreEqual(wheel1[0], new[] { 1, 1, 1 });
            Assert.AreEqual(wheel1[1], new[] { 10, 10, 10 });
            Assert.AreEqual(wheel1[2], new[] { 3, 3, 3 });
            Assert.AreEqual(wheel1[3], new[] { 4, 4, 4 });
            Assert.AreEqual(wheel1[4], new[] { 5, 5, 5 });

            Assert.AreEqual(wheel2[0], new[] { 1, 1, 1 });
            Assert.AreEqual(wheel2[1], new[] { 2, 2, 2 });
            Assert.AreEqual(wheel2[2], new[] { 10, 10, 10 });
            Assert.AreEqual(wheel2[3], new[] { 4, 4, 4 });
            Assert.AreEqual(wheel2[4], new[] { 5, 5, 5 });

            Assert.AreEqual(wheel3[0], new[] { 1, 1, 1 });
            Assert.AreEqual(wheel3[1], new[] { 2, 2, 2 });
            Assert.AreEqual(wheel3[2], new[] { 3, 3, 3 });
            Assert.AreEqual(wheel3[3], new[] { 10, 10, 10 });
            Assert.AreEqual(wheel3[4], new[] { 5, 5, 5 });
        }
    }
}