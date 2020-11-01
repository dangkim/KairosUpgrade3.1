namespace Slot.UnitTests.Cleopatra
{
    using Microsoft.AspNetCore.Http.Internal;
    using NUnit.Framework;
    using Slot.Core;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.Cleopatra;
    using Slot.Model;
    using Slot.Model.Entity;
    using System.Collections.Generic;

    [TestFixture]
    internal class FeatureTests
    {
        [TestCase(TestName = "Test Sarchophagus Feature")]
        public void TestSarchophagusFeature()
        {
            // arrange
            var user = new UserGameKey(-1, 51);
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 50,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "Cleopatra", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };
            requestBonusContext.UserSession = userSession;
            requestBonusContext.UserGameKey = user;
            var bonusContext = new BonusStateContext(1, new CleopatraBonus { SpinBet = bet });
            var state = new SarchophagusState(new PreSpin(1, 0));
            //Action
            var reponse = state.Handle(requestBonusContext, bonusContext);

            // Assert
            Assert.IsTrue(reponse.nextState is FreeSpinState);
            Assert.AreEqual(reponse.result.Win >= 50 * 2, true);
        }

        [TestCase(TestName = "Test Free Spin Feature")]
        public void TestFreeSpinFeature()
        {
            // arrange
            var user = new UserGameKey(-1, 51);
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 50,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "Cleopatra", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };
            requestBonusContext.UserSession = userSession;
            requestBonusContext.UserGameKey = user;
            var bonusContext = new BonusStateContext(1, new CleopatraBonus { SpinBet = bet });
            var state = new FreeSpinState(1, 0);

            //Action
            var reponse = state.Handle(requestBonusContext, bonusContext);

            // Assert
            Assert.AreEqual(!reponse.result.SpinResult.HasBonus, reponse.nextState.IsCompleted);
            Assert.AreEqual(!reponse.result.SpinResult.HasBonus, reponse.nextState is FreeSpinState);
            Assert.AreEqual(reponse.result.SpinResult.HasBonus, reponse.nextState is SarchophagusState);
        }

        [TestCase(TestName = "Test Bonus Builder")]
        public void TestBonusBuilder()
        {
            // arrange
            var user = new UserGameKey(-1, 51);
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
                new [] { 0, 10, 1 },
                new [] { 0, 10, 3},
                new [] { 7, 8, 2},
                new [] { 2, 3, 4 }
            };

            var wheel3 = new List<int[]>
            {
                new [] { 0, 2, 1 },
                new [] { 0, 9, 1 },
                new [] { 0, 9, 3},
                new [] { 7, 9, 2},
                new [] { 2, 3, 4 }
            };

            // action
            var result1 = BonusInspection.InspectFreeSpin(wheel1);
            var result2 = BonusInspection.InspectFreeSpin(wheel2);
            var result3 = BonusInspection.InspectSarchophagus(wheel3);

            // Assert

            Assert.IsTrue(result1.HasValue);
            Assert.IsTrue(result1.ValueOrDefault().Item.Count == 15);

            Assert.IsTrue(result2.None);

            Assert.IsTrue(result3.HasValue);
            Assert.IsTrue(result3.ValueOrDefault().Item.Count == 1);
        }
    }
}