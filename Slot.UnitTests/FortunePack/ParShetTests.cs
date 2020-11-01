namespace Slot.UnitTests.FortunePack
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
            var strip = new List<List<int>> { new List<int> { 1, 1, 1 }, new List<int> { 2, 2, 2 }, new List<int> { 3, 3, 3 } };
            // action
            var wheel = Games.FortunePack.ParSheet.CreateWheel(strip);

            //assert
            Assert.AreEqual(wheel.Reels, new List<List<int>> { new List<int> { 1, 1, 1 }, new List<int> { 2, 2, 2 }, new List<int> { 3, 3, 3 } });
        }
    }
}