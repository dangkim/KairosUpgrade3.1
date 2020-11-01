namespace Slot.UnitTests.SevenWonders
{
    using Games.SevenWonders;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    internal class ParShetTests
    {
        [TestCase(TestName = "Get A Reel Strips")]
        public void TestGetAReelStrip()
        {
            // action
            var reelStrips = ParSheet.GetStrips(1);

            //assert
            Assert.NotNull(reelStrips);
        }

        [TestCase(TestName = "Do Wild Expand")]
        public void TestWildxpand()
        {
            // arrange
            var wheel = new List<int[]>{
                new[] { 0,2,3},
                new[] { 0,2,3},
                new[] { 1,2,3},
                new[] { 0,2,3},
                new[] { 0,2,3}
            };
            // action
            var wheel1 = ParSheet.Expand(new[] { false, false, true, false, false }, wheel);
            var wheel2 = ParSheet.Expand(new[] { false, true, true, false, false }, wheel);
            var wheel3 = ParSheet.Expand(new[] { false, true, true, true, false }, wheel);

            //assert
            Assert.AreEqual(wheel1[0], wheel[0]);
            Assert.AreEqual(wheel2[0], wheel[0]);
            Assert.AreEqual(wheel3[0], wheel[0]);

            Assert.AreEqual(wheel1[1], wheel[1]);
            Assert.AreEqual(wheel2[1], new[] { 7, 7, 7 });
            Assert.AreEqual(wheel3[1], new[] { 7, 7, 7 });

            Assert.AreEqual(wheel1[2], new[] { 7, 7, 7 });
            Assert.AreEqual(wheel2[2], new[] { 7, 7, 7 });
            Assert.AreEqual(wheel3[2], new[] { 7, 7, 7 });

            Assert.AreEqual(wheel1[3], wheel[3]);
            Assert.AreEqual(wheel2[3], wheel[3]);
            Assert.AreEqual(wheel3[3], new[] { 7, 7, 7 });

            Assert.AreEqual(wheel1[4], wheel[4]);
            Assert.AreEqual(wheel2[4], wheel[4]);
            Assert.AreEqual(wheel3[4], wheel[4]);
        }
    }
}