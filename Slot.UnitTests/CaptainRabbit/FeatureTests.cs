namespace Slot.UnitTests.CaptainRabbit
{
    using Microsoft.FSharp.Collections;
    using NUnit.Framework;
    using Slot.Games.CaptainRabbit;
    using Slot.Model;
    using System.Collections.Generic;
    using static Slot.Games.CaptainRabbit.Domain;
    using static Slot.Games.CaptainRabbit.Domain.BonusState;
    using static Slot.Games.CaptainRabbit.Global;

    [TestFixture]
    internal class FeatureTests
    {
        [TestCase(TestName = "Test Honey Feature")]
        public void TestHoneyFeatgure()
        {
            // arrange
            var honeyItem = new HoneyItem(4, 1, new Prize(3, 0), new Prize(1, 0), 0);
            var user = new UserGameKey(-1, 103);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 25,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new Domain.Action(1, user, bet, 1, PlatformType.None);

            // action
            var state = HoneyReducer.dispatch(action, honeyItem);
            var honeyState = state.BonusState as Honey;
            // Assert

            Assert.AreEqual(3, honeyState.Item.Count);
            Assert.AreEqual(1, honeyState.Item.CurrentStep);
            Assert.AreEqual(2, honeyState.Item.Pot.Count);
            Assert.AreEqual(1, honeyState.Item.Pot.CurrentStep);
            Assert.AreEqual(1, honeyState.Item.BeeHive.Count);
            Assert.AreEqual(0, honeyState.Item.BeeHive.CurrentStep);
            Assert.AreEqual(false, honeyState.Item.IsCompleted);
            Assert.IsTrue(state.Result.Win > 0);
        }

        [TestCase(TestName = "Test Free  Spin Feature")]
        public void TestFreeSpinFeature()
        {
            // arrange
            var freeSpinItem = new BonusItem(3, 3, new Prize(15, 0), 0);
            var user = new UserGameKey(-1, 103);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 25,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new Domain.Action(1, user, bet, 1, PlatformType.None);

            // action
            var state = FreeSpinReducer.dispatch(action, freeSpinItem);

            // Assert
            Assert.AreEqual(false, state.BonusState.IsHoney);
            if (state.BonusState.IsFreeSpin)
            {
                var freeSpinState = state.BonusState as FreeSpin;
                Assert.AreEqual(3, freeSpinState.Item.Id);
                Assert.AreEqual(3, freeSpinState.Item.Multiplier);
                Assert.AreEqual(true, freeSpinState.Item.IsStarted);
                Assert.AreEqual(14, freeSpinState.Item.Prize.Count);
                Assert.AreEqual(1, freeSpinState.Item.Prize.CurrentStep);
                Assert.AreEqual(false, freeSpinState.Item.IsCompleted);
            }

            if (state.BonusState.IsHoneyFreeSpin)
            {
                var honeyFreeSpinState = state.BonusState as HoneyFreeSpin;
                Assert.AreEqual(4, honeyFreeSpinState.Item.Honey.Id);
                Assert.AreEqual(3, honeyFreeSpinState.Item.Honey.Multiplier);
                Assert.AreEqual(state.Result.Bonus.Count, honeyFreeSpinState.Item.Honey.Pot.Count);
                Assert.AreEqual(true, honeyFreeSpinState.Item.Honey.Pot.Count > 0 && honeyFreeSpinState.Item.Honey.Pot.Count < 6);
                Assert.AreEqual(0, honeyFreeSpinState.Item.Honey.Pot.CurrentStep);
                Assert.AreEqual(1, honeyFreeSpinState.Item.Honey.BeeHive.Count);
                Assert.AreEqual(0, honeyFreeSpinState.Item.Honey.BeeHive.CurrentStep);
                Assert.AreEqual(state.Result.Bonus.Count + 1, honeyFreeSpinState.Item.Honey.TotalSpin);
                Assert.AreEqual(14, honeyFreeSpinState.Item.FreeSpin.Prize.Count);
                Assert.AreEqual(1, honeyFreeSpinState.Item.FreeSpin.Prize.CurrentStep);
                Assert.AreEqual(3, honeyFreeSpinState.Item.FreeSpin.Multiplier);
                Assert.AreEqual(false, honeyFreeSpinState.Item.IsCompleted);
            }
        }

        [TestCase(TestName = "Test Honey Free Spin Feature")]
        public void TestHoneyFreeSpinFeature()
        {
            // arrange
            var honeyItem = new HoneyItem(4, 3, new Prize(0, 0), new Prize(1, 0), 0);
            var freeSpinItem = new BonusItem(3, 3, new Prize(2, 0), 1);
            var honeyFreeSpinItem = new BearHoneyFreeSpin(honeyItem, freeSpinItem);
            var user = new UserGameKey(-1, 103);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 25,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new Domain.Action(1, user, bet, 1, PlatformType.None);

            // action
            var state = HoneyFreeSpinReducer.dispatch(action, honeyFreeSpinItem);

            // Assert
            Assert.AreEqual(false, state.BonusState.IsHoney);
            if (state.BonusState.IsHoneyFreeSpin)
            {
                var honeyFreeSpinState = state.BonusState as HoneyFreeSpin;
                var honey = honeyFreeSpinState.Item.Honey;
                var freeSpin = honeyFreeSpinState.Item.FreeSpin;
                Assert.AreEqual(4, honey.Id);
                Assert.AreEqual(3, honey.Multiplier);
                Assert.AreEqual(0, honey.Pot.Count);
                Assert.AreEqual(1, honey.Pot.CurrentStep);
                Assert.AreEqual(1, honey.BeeHive.Count);
                Assert.AreEqual(0, honey.BeeHive.CurrentStep);
                Assert.AreEqual(false, honey.IsCompleted);

                Assert.AreEqual(3, freeSpin.Id);
                Assert.AreEqual(3, freeSpin.Multiplier);
                Assert.AreEqual(2, freeSpin.Prize.Count);
                Assert.AreEqual(0, freeSpin.Prize.CurrentStep);
                Assert.AreEqual(false, freeSpin.IsCompleted);
            }

            if (state.BonusState.IsFreeSpin)
            {
                var freeSpin = state.BonusState as FreeSpin;
                Assert.AreEqual(3, freeSpin.Item.Id);
                Assert.AreEqual(3, freeSpin.Item.Multiplier);
                Assert.AreEqual(2, freeSpin.Item.Prize.Count);
                Assert.AreEqual(0, freeSpin.Item.Prize.CurrentStep);
                Assert.AreEqual(false, freeSpin.Item.IsCompleted);
                Assert.AreEqual(state.Result.Win + freeSpinItem.CumulativeWin, freeSpin.Item.CumulativeWin);
            }
        }

        [TestCase(TestName = "Test Free Spin Bonus Builder")]
        public void TestBonusBuilder()
        {
            // Arrange
            var bearResult = new BearResult("s");
            var reels = new List<int[]>
            {
                new[] { 0, 6, 10 },
                new[] { 6, 11, 12 },
                new[] { 6, 11, 13 },
                new[] { 9, 11, 12, },
                new[] { 12, 10, 13 }
            };
            bearResult.Wheel = new BearWheel(1, ArrayModule.OfSeq(reels));

            // action
            var bonus = BonusInspection.inspect(HoneyPot.Mode.Primary, bearResult);

            // assert
            Assert.NotNull(bonus);
            Assert.AreEqual(bonus.Value.At[0].RowPositions, new List<int> { 0, 2, 2, 2, 0 });
            Assert.AreEqual(bonus.Value.Item.ClientId, 3);
            Assert.AreEqual(bonus.Value.Item.Count, 15);
        }
    }
}