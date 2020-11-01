namespace Slot.UnitTests.SevenWonders
{
    using NUnit.Framework;
    using Slot.Games.SevenWonders;
    using Slot.Model;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    internal class FeatureTests
    {
        [TestCase(TestName = "Test ReSpin with Wild Expand All Reel Feature")]
        public void TestReSpinWithWildExpendAllReelFeatgure()
        {
            // arrange
            var freeSpin = new FreeSpin(new[] { 0, 1, 2, 3, 0 });
            var user = new UserGameKey(-1, 30);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 20,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new FreeSpinAction(1, PlatformType.None, bet);

            // action
            var sevenWondersState = FreeSpinReducer.Dispatch(freeSpin, action);

            // Assert
            Assert.AreEqual(sevenWondersState.State is Finish, true);
        }

        [TestCase(TestName = "Test ReSpin with Wild Expand At Second Reel Feature")]
        public void TestReSpinWithWildExpendAtSecondReelFeatgure()
        {
            // arrange
            var freeSpin = new FreeSpin(new[] { 0, 1, 0, 0, 0 });
            var user = new UserGameKey(-1, 30);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 20,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new FreeSpinAction(1, PlatformType.None, bet);

            // action
            var sevenWondersState = FreeSpinReducer.Dispatch(freeSpin, action);
            var result = sevenWondersState.Result;
            var wheel = result.Wheel.Reels;

            // Assert
            Assert.AreEqual(sevenWondersState.State is Finish, wheel[2].All(ele => ele != 7) && wheel[3].All(ele => ele != 7));
            Assert.AreEqual(sevenWondersState.State is FreeSpin, wheel[2].Any(ele => ele == 7) || wheel[3].Any(ele => ele == 7));
        }

        [TestCase(TestName = "Test Free Spin Bonus Builder")]
        public void TestBonusBuilder()
        {
            // arrange
            var user = new UserGameKey(-1, 30);
            var stickyWilds1 = new[] { 0, 1, 0, 0, 0 };
            var stickyWilds2 = new[] { 0, 0, 0, 0, 0 };
            var stickyWilds3 = new[] { 0, 1, 2, 3, 0 };

            var wheel1 = new List<int[]> {
                 new []{ 0,2,1},
                 new []{ 0,0,1},
                 new []{ 0,7,3},
                 new []{ 0,1,2},
                 new []{ 2,3,4}
            };
            var wheel2 = new List<int[]> {
                new []{ 0,2,1},
                 new []{ 0,0,1},
                 new []{ 0,7,3},
                 new []{ 7,1,2},
                 new []{ 2,3,4}};

            var wheel3 = new List<int[]> {
                new []{ 0,2,1},
                 new []{ 0,7,1},
                 new []{ 0,7,3},
                 new []{ 0,1,7},
                 new []{ 2,3,4}
            };

            // action
            var respin1 = BonusInspection.InspectFreeSpin(stickyWilds1, wheel1);
            var respin2 = BonusInspection.InspectFreeSpin(stickyWilds1, wheel2);
            var respin3 = BonusInspection.InspectFreeSpin(stickyWilds2, wheel3);
            var respin4 = BonusInspection.InspectFreeSpin(stickyWilds3, wheel3);

            // assert
            Assert.IsTrue(respin1.HasValue);
            Assert.IsTrue(respin2.HasValue);
            Assert.IsTrue(respin3.HasValue);
            Assert.IsTrue(respin4.None);
        }
    }
}