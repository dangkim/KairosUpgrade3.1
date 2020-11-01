namespace Slot.UnitTests.GeniesLuck
{
    using NUnit.Framework;
    using Slot.Core;
    using Slot.Games.GeniesLuck;
    using Slot.Model;
    using System.Collections.Generic;

    [TestFixture]
    internal class FeatureTests
    {
        [TestCase(TestName = "Test ReSpin Feature")]
        public void TestReSpinFeatgure()
        {
            // arrange
            var freeSpin = new ReSpin(1, 0, new List<int[]>
            {
                new int[] { 9, 9, 9},
                new int[] { 9, -1, -1, -1},
                new int[] { 9, 9, 9, -1},
                new int[] { -1, 11, -1, -1},
                new int[] { -1, 1, -1},
            });
            var user = new UserGameKey(-1, 80);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 40,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new ReSpinAction(1, PlatformType.None, bet, () =>
            {
                return new List<IReadOnlyList<int>>
                {
                    new int[] { 1,1,1},
                    new int[] { 2,2,2,2},
                    new int[] { 3,3,3,3},
                    new int[] { 4,4,4,4},
                    new int[] { 5,5,5}
                };
            });

            // action
            var geniesLuckState = ReSpinReducer.Dispatch(freeSpin, action);
            var result = geniesLuckState.Result;
            // Assert
            Assert.IsTrue(geniesLuckState.State is Finish);
            Assert.AreEqual(result.Wheel[0], new[] { 9, 9, 9 });
            Assert.AreEqual(result.Wheel[1], new[] { 9, 2, 2, 2 });
            Assert.AreEqual(result.Wheel[2], new[] { 9, 9, 9, 3 });
            Assert.AreEqual(result.Wheel[3], new[] { 4, 11, 4, 4 });
            Assert.AreEqual(result.Wheel[4], new[] { 5, 5, 5 });
        }

        [TestCase(TestName = "Test ReSpin Free Spin Feature")]
        public void TestReSpinFreeSpinFeatgure()
        {
            // arrange
            var freeSpin = new ReSpinFreeSpin(1, 0, 1, new List<int[]>
            {
                new int[] { 9, 9, 9},
                new int[] { 9, -1, -1, -1},
                new int[] { 9, 9, 9, -1},
                new int[] { -1, 11, -1, -1},
                new int[] { -1, 1, -1},
            }, new FreeSpin(1, 0));
            var user = new UserGameKey(-1, 80);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 40,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new ReSpinAction(1, PlatformType.None, bet, () =>
            {
                return new List<IReadOnlyList<int>>
                {
                    new int[] { 1,1,1},
                    new int[] { 2,2,2,2},
                    new int[] { 3,3,3,3},
                    new int[] { 4,4,4,4},
                    new int[] { 5,5,5}
                };
            });

            // action
            var geniesLuckState = ReSpinFreeSpinReducer.Dispatch(freeSpin, action);
            var result = geniesLuckState.Result;

            // Assert
            Assert.IsTrue(geniesLuckState.State is FreeSpin);
            Assert.AreEqual(result.Wheel[0], new[] { 9, 9, 9 });
            Assert.AreEqual(result.Wheel[1], new[] { 9, 2, 2, 2 });
            Assert.AreEqual(result.Wheel[2], new[] { 9, 9, 9, 3 });
            Assert.AreEqual(result.Wheel[3], new[] { 4, 11, 4, 4 });
            Assert.AreEqual(result.Wheel[4], new[] { 5, 5, 5 });
            Assert.IsTrue(result.Win > 0);
        }

        [TestCase(TestName = "Test Free Spin Feature")]
        public void TestFreeSpinFeatgure()
        {
            // arrange
            var freeSpin = new FreeSpin(1, 0);
            var user = new UserGameKey(-1, 30);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 40,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new FreeSpinAction(1, PlatformType.None, bet, () =>
            {
                return new List<IReadOnlyList<int>>
                {
                    new int[] { 1,1,1},
                    new int[] { 2,2,2,2},
                    new int[] { 3,3,3,3},
                    new int[] { 4,4,4,4},
                    new int[] { 5,5,5}
                };
            });

            // action
            var geniesLuckState = FreeSpinReducer.Dispatch(freeSpin, action);
            var result = geniesLuckState.Result;

            // Assert
            Assert.IsTrue(geniesLuckState.State is Finish);
            Assert.AreEqual(result.Wheel[0], new[] { 1, 1, 1 });
            Assert.AreEqual(result.Wheel[1], new[] { 2, 2, 2, 2 });
            Assert.AreEqual(result.Wheel[2], new[] { 3, 3, 3, 3 });
            Assert.AreEqual(result.Wheel[3], new[] { 4, 4, 4, 4 });
            Assert.AreEqual(result.Wheel[4], new[] { 5, 5, 5 });
        }

        [TestCase(TestName = "Test Free Spin Bonus Builder")]
        public void TestBonusBuilder()
        {
            // arrange
            var user = new UserGameKey(-1, 80);
            var wheel1 = new List<int[]>
            {
                new [] { 9,9,9 },
                new [] { 0, 3, 1, 0 },
                new [] { 0, 6, 3, 0 },
                new [] { 0, 3, 2, 0 },
                new [] { 2, 3, 4 }
            };
            var wheel2 = new List<int[]>
            {
                new [] { 0, 2, 1 },
                new [] { 0, 10, 1, 0 },
                new [] { 0, 10, 3, 0 },
                new [] { 7, 10, 2, 0 },
                new [] { 2, 3, 4 }
            };

            // action
            var result1 = BonusInspection.InspectFreeSpin(wheel1);
            var result2 = BonusInspection.InspectReSpin(new[] { 0 }, wheel1);
            var result3 = BonusInspection.InspectFreeSpin(wheel2);
            var result4 = BonusInspection.InspectReSpin(new[] { 0 }, wheel2);

            // Assert

            Assert.IsTrue(result1.None);
            Assert.IsTrue(result2.HasValue);
            Assert.IsTrue(result2.ValueOrDefault().Item.Count == 3);
            Assert.AreEqual(result2.ValueOrDefault().At, new[] { 1, 0, 0, 0, 0 });
            Assert.IsTrue(result3.HasValue);
            Assert.IsTrue(result3.ValueOrDefault().Item.Count == 10);
            Assert.AreEqual(result3.ValueOrDefault().At, new[] { 0, 2, 2, 2, 0 });
            Assert.IsTrue(result4.None);
        }
    }
}