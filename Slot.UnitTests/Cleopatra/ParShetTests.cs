namespace Slot.UnitTests.Cleopatra
{
    using NUnit.Framework;
    using Slot.Games.Cleopatra;
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
    }
}