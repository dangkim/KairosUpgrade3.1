namespace Slot.UnitTests.DragonRiches
{
    using NUnit.Framework;
    using Slot.Core;
    using Slot.Games.DragonRiches;
    using Slot.Model;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    internal class FeatureTests
    {
        [TestCase(TestName = "Test Hold Spin Feature")]
        public void TestHoldSpinFeatgure()
        {
            // arrange
            var coins = new DragonRichesWheel.CoinCollapse();
            coins.Add(new DragonRichesWheel.CoinUnfilled(new[] { new Coin(), new Coin(1), new Coin(2) }));
            coins.Add(new DragonRichesWheel.CoinUnfilled(new[] { new Coin(), new Coin(1), new Coin(2) }));
            coins.Add(new DragonRichesWheel.CoinUnfilled(new[] { new Coin(), new Coin(1), new Coin(2) }));
            coins.Add(new DragonRichesWheel.CoinUnfilled(new[] { new Coin(), new Coin(1), new Coin(2) }));
            coins.Add(new DragonRichesWheel.CoinUnfilled(new[] { new Coin(), new Coin(), new Coin() }));

            coins.TotalItems = 8;

            var holdSpinItem = new HoldSpin(new State(3, 0), new List<int[]>
            {
                new int[3],
                new int[3],
                new int[3],
                new int[3],
                new int[3]
            }, coins);
            var user = new UserGameKey(-1, 104);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new HoldSpinAction(1, PlatformType.None, bet);

            // action
            var dragonState = HoldSpinReducer.Dispatch(holdSpinItem, action);
            var holdSpinState = dragonState.State as HoldSpin;
            var wheel = dragonState.Result.Wheel;

            // Assert
            Assert.AreEqual(true, holdSpinState.State.Count >= 2);
            Assert.AreEqual(1, holdSpinState.State.CurrentStep);
            Assert.AreEqual(false, holdSpinState.IsCompleted);
            Assert.AreEqual(wheel.CoinCollapsing.TotalItems, wheel.Coins.Sum(reward => reward.Count(item => item > 0)));
            Assert.IsTrue(dragonState.Result.Win == 0);
        }

        [TestCase(TestName = "Test Free Spin Feature")]
        public void TestFreeSpinFeature()
        {
            // arrange
            var wheel = new DragonRichesWheel
            {
                Reels = new List<int[]>(),
                CoinCollapsing = new DragonRichesWheel.CoinCollapse()
            };
            var holdSpinItem = new FreeSpin(new State(6, 0), 0);
            var user = new UserGameKey(-1, 104);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new FreeSpinAction(1, PlatformType.None, bet, () => new List<IReadOnlyList<int>>
            {
                new List<int> { 1, 1, 1 },
                new List<int> { 2, 2, 2 },
                new List<int> { 3, 3, 3 },
                new List<int> { 4, 4, 4 },
                new List<int> { 5, 5, 5, }
            });

            // action
            var dragonState = FreeSpinReducer.Dispatch(holdSpinItem, action);

            // Assert
            if (dragonState.State is FreeSpin)
            {
                var freeSpinState = dragonState.State as FreeSpin;
                var scatters = dragonState.Result.Wheel.Reels.Sum(ele => ele.Count(item => item == 9));
                Assert.AreEqual(freeSpinState.State.Count >= 5, true);
                Assert.AreEqual(false, freeSpinState.IsCompleted);
                Assert.AreEqual(scatters >= 3, freeSpinState.State.Count >= 12);
            }
            if (dragonState.State is HoldFreeSpin)
            {
                var holdFreeSpinState = dragonState.State as HoldFreeSpin;
                Assert.AreEqual(3, holdFreeSpinState.HoldSpin.State.Count == 3);
                Assert.AreEqual(3, holdFreeSpinState.FreeSpin.State.Count == 5);
                Assert.AreEqual(true, dragonState.Result.Wheel.Reels.Sum(ele => ele.Count(item => item == 12)) >= 6);
                Assert.AreEqual(false, holdFreeSpinState.IsCompleted);
            }
        }

        [TestCase(TestName = "Test Hold Free Spin Feature")]
        public void TestHoldFreeSpinFeature()
        {
            // arrange
            var coins = new DragonRichesWheel.CoinCollapse();
            coins.Add(new DragonRichesWheel.CoinUnfilled(new[] { new Coin(), new Coin(1), new Coin(5) }));
            coins.Add(new DragonRichesWheel.CoinUnfilled(new[] { new Coin(), new Coin(1), new Coin(5) }));
            coins.Add(new DragonRichesWheel.CoinUnfilled(new[] { new Coin(), new Coin(1), new Coin(5) }));
            coins.Add(new DragonRichesWheel.CoinUnfilled(new[] { new Coin(), new Coin(1), new Coin(5) }));
            coins.Add(new DragonRichesWheel.CoinUnfilled(new[] { new Coin(), new Coin(), new Coin() }));
            coins.TotalItems = 8;
            var user = new UserGameKey(-1, 104);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new HoldFreeSpinAction(1, PlatformType.None, bet);

            var holdSpinItem = new HoldFreeSpin(
                holdSpin: new HoldSpin(
                    new State(3, 0),
                    new List<int[]>
                    {
                        new int[3],
                        new int[3],
                        new int[3],
                        new int[3],
                        new int[3]
                    }, coins),
                freeSpin: new FreeSpin(new State(5, 1), 1)
            );

            // action
            var dragonState = HoldFreeSpinReducer.Dispatch(holdSpinItem, action);
            var holdSpinState = dragonState.State as HoldFreeSpin;
            var wheel = dragonState.Result.Wheel;

            // Assert
            Assert.NotNull(holdSpinState);
            Assert.AreEqual(holdSpinState.HoldSpin.State.Count >= 2, true);

            Assert.AreEqual(5, holdSpinState.FreeSpin.State.Count);
            Assert.AreEqual(wheel.CoinCollapsing.TotalItems, wheel.Coins.Sum(reward => reward.Count(item => item > 0)));
            Assert.IsTrue(dragonState.Result.Win == 0);
            Assert.AreEqual(false, holdSpinState.IsCompleted);
        }

        [TestCase(TestName = "Test Bonus Builder")]
        public void TestBonusBuilder()
        {
            var user = new UserGameKey(-1, 104);
            var result = new Games.DragonRiches.DragonRichesResult();
            var reelsHoldSpin = new List<int[]>
            {
                new [] { 1, 6, 11 },
                new [] { 6, 11, 11 },
                new [] { 11, 10, 11 },
                new [] { 9, 11, 11, },
                new [] { 11, 11, 11 }
            };

            var reelsFreeSpin = new List<int[]>
            {
                new [] { 1, 6, 12 },
                new [] { 6, 12, 12 },
                new [] { 12, 9, 13 },
                new [] { 9, 12, 12, },
                new [] { 12, 12, 9 }
            };

            // action
            var holdSpinBonus = BonusInspection.InspectHoldSpin(reelsHoldSpin);
            var freeSpinBonus = BonusInspection.InspectFreeSpin(reelsFreeSpin);

            // assert
            Assert.NotNull(holdSpinBonus);
            Assert.NotNull(freeSpinBonus);
            Assert.IsTrue(holdSpinBonus.HasValue);
            Assert.IsTrue(freeSpinBonus.HasValue);
            Assert.AreEqual(holdSpinBonus.ValueOrDefault().At, new List<int[]> { new[] { 0, 0, 3 }, new[] { 0, 2, 3 }, new[] { 1, 0, 3 }, new[] { 0, 2, 3 }, new[] { 1, 2, 3 } });
            Assert.AreEqual(freeSpinBonus.ValueOrDefault().At, new List<int[]> { new[] { 0 }, new[] { 0 }, new[] { 2 }, new[] { 1 }, new[] { 3 } });
        }
    }
}