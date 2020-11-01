namespace Slot.UnitTests.DragonRiches
{
    using Games.DragonRiches;
    using Games.DragonRiches.Configuration;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    internal class ParShetTests
    {
        [TestCase(TestName = "Get A Reel")]
        public void TestGetAReel()
        {
            // arrange
            var strip = new List<int> { 1, 2, 3, 4, 5 };
            // action
            var reel = ParSheet.GetReel(strip);

            //assert
            Assert.AreEqual(reel.Length, 3);
        }

        [TestCase(TestName = "Get A Element at special position")]
        public void TestGetCardAtPosition()
        {
            // arrange
            var strip = new List<int> { 1, 2, 3, 4, 5 };
            // action
            var card = ParSheet.GetCard(strip);

            //assert
            Assert.AreEqual(card > 0 && card < 6, true);
        }

        [TestCase(TestName = "Wheel For Main Game")]
        public void TestWheelForMainGame()
        {
            // arrange

            var reel1 = new List<int> { 1, 1, 1 };
            var reel2 = new List<int> { 2, 2, 2 };
            var reel3 = new List<int> { 3, 3, 3 };
            var reel4 = new List<int> { 4, 4, 4 };
            var reel5 = new List<int> { 5, 5, 5 };
            var reels = new List<IReadOnlyList<int>> { reel1, reel2, reel3, reel4, reel5 };

            // action
            var wheel = ParSheet.CreateWheel(reels, Config.MainGameCoinWeighted);

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

            var reel1 = new List<int> { 1, 1, 1 };
            var reel2 = new List<int> { 2, 2, 2 };
            var reel3 = new List<int> { 3, 3, 3 };
            var reel4 = new List<int> { 4, 4, 4 };
            var reel5 = new List<int> { 5, 5, 5 };
            var reels = new List<IReadOnlyList<int>> { reel1, reel2, reel3, reel4, reel5 };

            // action
            var wheel = ParSheet.CreateWheel(reels, Config.FreeGameCoinWeighted);

            //assert
            Assert.AreEqual(string.Join(',', wheel.Reels[0]), "1,1,1");
            Assert.AreEqual(string.Join(',', wheel.Reels[1]), "2,2,2");
            Assert.AreEqual(string.Join(',', wheel.Reels[2]), "3,3,3");
            Assert.AreEqual(string.Join(',', wheel.Reels[3]), "4,4,4");
            Assert.AreEqual(string.Join(',', wheel.Reels[4]), "5,5,5");
        }
    }
}