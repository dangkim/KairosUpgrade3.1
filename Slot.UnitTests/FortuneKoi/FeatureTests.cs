namespace Slot.UnitTests.FortuneKoi
{
    using Microsoft.AspNetCore.Http.Internal;
    using NUnit.Framework;
    using Slot.Core;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.FortuneKoi;
    using Slot.Model;
    using Slot.Model.Entity;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    internal class FeatureTests
    {
        [TestCase(TestName = "Test Re Spin Feature")]
        public void TestReSpinFeature()
        {
            // arrange
            var user = new UserGameKey(-1, 32);
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 10,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "FortuneKoi", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };
            requestBonusContext.UserSession = userSession;
            requestBonusContext.UserGameKey = user;
            var bonusContext = new BonusStateContext(1, new FortuneKoiBonus { SpinBet = bet });
            var state = new ReSpinState(new List<int[]> {
                new [] { 0,1,2 },
                new [] { 0, 7, 1 },
                new [] { 4,5,6},
                new [] { 1,2,3 },
                new [] { 6,5,4}  },
                new bool[] { false, true, false, false, false });

            //Action
            var response = state.Handle(requestBonusContext, bonusContext);
            var spinResult = response.result.SpinResult;
            var wheel = spinResult.Wheel;

            // Assert
            Assert.AreEqual(spinResult.HasBonus, !response.nextState.IsCompleted);
            Assert.AreEqual(wheel[2].Any(item => item == 7) || wheel[3].Any(item => item == 7), response.nextState is ReSpinState);
            Assert.AreEqual(wheel[2].All(item => item < 7) && wheel[3].All(item => item < 7), response.nextState is Finish);
        }

        [TestCase(TestName = "Test Bonus Inspection")]
        public void TestBonusInspection()
        {
            // arrange
            var expandReels = new bool[] { false, false, false, false, false };
            var user = new UserGameKey(-1, 32);
            var wheel1 = new List<int[]>
            {
                new [] { 0,1,2 },
                new [] { 0, 7, 1 },
                new [] { 4,5,6},
                new [] { 1,2,3 },
                new [] { 6,5,4}
            };
            var wheel2 = new List<int[]>
            {
                new [] { 0,1,2 },
                new [] { 0, 5, 1 },
                new [] { 7,5,6},
                new [] { 1,2,3 },
                new [] { 6,5,4}
            };

            var wheel3 = new List<int[]>
            {
                new [] { 0,1,2 },
                new [] { 0, 2, 1 },
                new [] { 4,5,6},
                new [] { 1,2,7 },
                new [] { 6,5,4}
            };

            var wheel4 = new List<int[]>
            {
                new [] { 0,1,2 },
                new [] { 0, 2, 1 },
                new [] { 4,5,6},
                new [] { 1,2,0 },
                new [] { 6,5,4}
            };

            // Action
            var result1 = BonusInspection.InspectReSpin(wheel1, expandReels);
            var result2 = BonusInspection.InspectReSpin(wheel2, expandReels);
            var result3 = BonusInspection.InspectReSpin(wheel3, expandReels);
            var result4 = BonusInspection.InspectReSpin(wheel4, expandReels);

            // Assert

            Assert.IsTrue(result1.HasValue);
            Assert.IsTrue(result1.ValueOrDefault().Item.Count == 1);

            Assert.IsTrue(result2.HasValue);
            Assert.IsTrue(result2.ValueOrDefault().Item.Count == 1);

            Assert.IsTrue(result3.HasValue);
            Assert.IsTrue(result3.ValueOrDefault().Item.Count == 1);

            Assert.IsTrue(result4.None);
        }
    }
}