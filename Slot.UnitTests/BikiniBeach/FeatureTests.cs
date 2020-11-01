namespace Slot.UnitTests.BikiniBeach
{
    using NUnit.Framework;
    using Slot.Core;
    using Slot.Games.BikiniBeach;
    using Slot.Model;
    using System.Collections.Generic;

    [TestFixture]
    internal class FeatureTests
    {
        [TestCase(TestName = "Test Swim Wear Feature")]
        public void TestSwimWearFeatgure()
        {
            // arrange
            var swimWear = new SwimWear(1, 0);
            var user = new UserGameKey(-1, 80);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new SwimWearAction(1, PlatformType.None, bet);

            // action
            var bikiniBeachState = SwimWearReducer.Dispatch(swimWear, action);
            var result = bikiniBeachState.Result;
            // Assert
            Assert.IsTrue(bikiniBeachState.State is Finish);
            Assert.IsTrue(result.Win > 0);
        }

        [TestCase(TestName = "Test Body Part Feature")]
        public void TestBodyPartFeatgure()
        {
            // arrange
            var bodyPart = new BodyPart(1, 0, new Item[6]);
            var user = new UserGameKey(-1, 3);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var action = new BodyPartChooseAction(1, PlatformType.None, bet, 1);

            // action
            var bikiniBeachState = BodyPartReducer.Dispatch(bodyPart, action);
            var result = bikiniBeachState.Result;

            // Assert
            Assert.IsTrue(bikiniBeachState.State is Finish);
            Assert.AreEqual(result.Win, result.Wheel.Items[0].Prize);
        }

        [TestCase(TestName = "Test Free Spin Feature")]
        public void TestFreeSpinFeatgure()
        {
            // arrange
            var freeSpin = new FreeSpin(1, 0);
            var user = new UserGameKey(-1, 30);
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
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
            var bikiniBeachState = FreeSpinReducer.Dispatch(freeSpin, action);
            var result = bikiniBeachState.Result;

            // Assert
            Assert.IsTrue(bikiniBeachState.State is Finish);
            Assert.AreEqual(result.Wheel[0], new[] { 1, 1, 1 });
            Assert.AreEqual(result.Wheel[1], new[] { 10, 10, 10 });
            Assert.AreEqual(result.Wheel[2], new[] { 3, 3, 3 });
            Assert.AreEqual(result.Wheel[3], new[] { 4, 4, 4 });
            Assert.AreEqual(result.Wheel[4], new[] { 5, 5, 5 });
        }

        [TestCase(TestName = "Test Bonus Builder")]
        public void TestBonusBuilder()
        {
            // arrange
            var user = new UserGameKey(-1, 30);
            var wheel1 = new List<int[]>
            {
                new [] { 9,9,9 },
                new [] { 0, 10, 1 },
                new [] { 0, 10, 3 },
                new [] { 0, 10, 2 },
                new [] { 2, 3, 4 }
            };
            var wheel2 = new List<int[]>
            {
                new [] { 0, 2, 1 },
                new [] { 0, 8, 1 },
                new [] { 0, 8, 3},
                new [] { 7, 8, 2},
                new [] { 2, 3, 4 }
            };

            var wheel3 = new List<int[]>
            {
                new [] { 0, 2, 1 },
                new [] { 0, 9, 1},
                new [] { 0, 9, 3 },
                new [] { 7, 9, 2 },
                new [] { 2, 3, 4 }
            };

            // action
            var result1 = BonusInspection.InspectFreeSpin(wheel1);
            var result11 = BonusInspection.InspectSwimWear(wheel1);
            var result12 = BonusInspection.InspectBodyPart(wheel1);

            var result2 = BonusInspection.InspectFreeSpin(wheel2);
            var result21 = BonusInspection.InspectSwimWear(wheel2);
            var result22 = BonusInspection.InspectBodyPart(wheel2);

            var result3 = BonusInspection.InspectFreeSpin(wheel3);
            var result31 = BonusInspection.InspectSwimWear(wheel3);
            var result32 = BonusInspection.InspectBodyPart(wheel3);

            // Assert

            Assert.IsTrue(result1.HasValue);
            Assert.IsTrue(result1.ValueOrDefault().Item.Count == 4);
            Assert.IsTrue(result11.None);
            Assert.IsTrue(result12.None);

            Assert.IsTrue(result2.None);
            Assert.IsTrue(result21.HasValue);
            Assert.IsTrue(result21.ValueOrDefault().Item.Count == 1);
            Assert.IsTrue(result22.None);

            Assert.IsTrue(result3.None);
            Assert.IsTrue(result31.None);
            Assert.IsTrue(result32.HasValue);
            Assert.IsTrue(result32.ValueOrDefault().Item.Count == 3);
        }
    }
}