namespace Slot.UnitTests.CaptainRabbit
{
    using NUnit.Framework;
    using Slot.Games.CaptainRabbit;

    [TestFixture]
    internal class ParShetTests
    {
        [TestCase(TestName = "Wheel For Main Game")]
        public void TestWheelForMainGame()
        {
            // arrange

            var reel1 = new[] { 1, 1, 1 };
            var reel2 = new[] { 2, 2, 2 };
            var reel3 = new[] { 3, 3, 3 };
            var reel4 = new[] { 4, 4, 4 };
            var reel5 = new[] { 5, 5, 5 };
            var reels = new int[][] { reel1, reel2, reel3, reel4, reel5 };

            // action
            var wheel = ParSheet.wheelForMainGame(reels);

            //assert
            Assert.AreEqual(string.Join(',', wheel.Reels[0]), "1,1,1");
            Assert.AreEqual(string.Join(',', wheel.Reels[1]), "2,2,2");
            Assert.AreEqual(string.Join(',', wheel.Reels[2]), "3,3,3");
            Assert.AreEqual(string.Join(',', wheel.Reels[3]), "4,4,4");
            Assert.AreEqual(string.Join(',', wheel.Reels[4]), "5,5,5");
        }

        [TestCase(TestName = "Wheel For Free Game")]
        public void TestWheelForFreeGame()
        {
            // arrange

            var reel1 = new[] { 1, 1, 1 };
            var reel2 = new[] { 2, 2, 2 };
            var reel3 = new[] { 3, 3, 3 };
            var reel4 = new[] { 4, 4, 4 };
            var reel5 = new[] { 5, 5, 5 };
            var reels = new int[][] { reel1, reel2, reel3, reel4, reel5 };

            // action
            var wheel = ParSheet.wheelForFreeGame(reels);

            //assert
            Assert.AreEqual(string.Join(',', wheel.Reels[0]), "1,1,1");
            Assert.AreEqual(string.Join(',', wheel.Reels[1]), "2,2,2");
            Assert.AreEqual(string.Join(',', wheel.Reels[2]), "3,3,3");
            Assert.AreEqual(string.Join(',', wheel.Reels[3]), "4,4,4");
            Assert.AreEqual(string.Join(',', wheel.Reels[4]), "5,5,5");
        }

        [TestCase(0.0069, TestName = "Main Game A1")]
        [TestCase(0.0619, TestName = "Main Game A2")]
        [TestCase(0.1219, TestName = "Main Game A3")]
        [TestCase(0.2919, TestName = "Main Game A4")]
        [TestCase(0.3600, TestName = "Main Game A5")]
        [TestCase(0.5200, TestName = "Main Game A6")]
        [TestCase(0.6800, TestName = "Main Game A7")]
        [TestCase(0.8400, TestName = "Main Game A8")]
        [TestCase(1.0000, TestName = "Main Game A9")]
        public void TestMainGameReelStripsSelection(double ratio)
        {
            Assert.NotNull(MainGame.getStripsSet(1, ratio));
        }

        [TestCase(0.374, TestName = "Feature A1")]
        [TestCase(0.624, TestName = "Feature A2")]
        [TestCase(1.000, TestName = "Feature A3")]
        public void TestFreeGameReelStripsSelection(double ratio)
        {
            Assert.NotNull(FreeGameParSheet.getStripsSet(1, ratio));
        }
    }
}