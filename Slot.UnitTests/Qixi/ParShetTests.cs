namespace Slot.UnitTests.Qixi
{
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    internal class ParShetTests
    {
        [TestCase(TestName = "Create A Wheel")]
        public void TestGetAReel()
        {
            // arrange
            var strip = new List<List<int>>
            {
                new List<int> { 1, 1, 1 },
                new List<int> { 2, 2, 2 },
                new List<int> { 3, 3, 3 },
                new List<int> { 4, 4, 4 },
                new List<int> { 5, 5, 5 }};
            // action
            var wheel = Games.Qixi.ParSheet.CreateWheel(strip);

            //assert
            Assert.AreEqual(wheel.Reels, new List<List<int>> { new List<int> { 1, 1, 1 }, new List<int> { 2, 2, 2 }, new List<int> { 3, 3, 3 }, new List<int> { 4, 4, 4 }, new List<int> { 5, 5, 5 } });
        }
    }
}