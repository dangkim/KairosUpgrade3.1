using Microsoft.AspNetCore.Http.Internal;
using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FortuneChimes;
using Slot.Model;
using Slot.Model.Entity;
using System.Collections.Generic;

namespace Slot.UnitTests.FortuneChimes

{
    [TestFixture]
    internal class FeatureTests
    {
        [TestCase(TestName = "Test Exploding Feature")]
        public void TestExplodingFeature()
        {
            // arrange
            var user = new UserGameKey(-1, 109);
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
                LineBet = 1.0m,
                Multiplier = 1
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "FortuneChimes", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };
            requestBonusContext.UserSession = userSession;
            requestBonusContext.UserGameKey = user;
            var bonusContext = new BonusStateContext(1, new FortuneChimesBonus { SpinBet = bet });
            var bonusPositions = new List<BonusPosition> { new BonusPosition { RowPositions = new List<int> { 1, 1, 1, 0, 0 } } };
            var wheel = new FortuneChimesWheel
            {
                Reels = new List<int[]> { new[] { 1, -1, -1 }, new[] { 1, -1, -1 }, new[] { 1, -1, -1 }, new[] { -1, -1, -1 }, new[] { -1, -1, -1 } },
                Indices = new int[] { 1, 2, 3, 4, 5 }
            };
            var reSpinCollapse = new ReSpinCollapse(bonusPositions, wheel, new List<int[]> { new int[0], new int[0], new int[0], new int[] { -1, -2, -3 }, new int[] { -1, -2, -3 } });
            var state = new ReSpinState(reSpinCollapse);

            //Action
            var response = state.Handle(requestBonusContext, bonusContext);

            // Assert
            Assert.AreEqual(response.result.Win > 0, response.nextState is ReSpinState);
            Assert.AreEqual(response.result.Win > 0, !response.nextState.IsCompleted);
            Assert.AreEqual(response.result.Win == 0, response.nextState is Finish);
        }

        [TestCase(TestName = "Test Free Spin Feature")]
        public void TestFreeSpinFeature()
        {
            // arrange
            var user = new UserGameKey(-1, 109);
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
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
            var bonusPositions = new List<BonusPosition> { new BonusPosition { RowPositions = new List<int> { 1, 1, 1, 0, 0 } } };
            var wheel = new FortuneChimesWheel
            {
                Reels = new List<int[]> { new[] { 1, -1, -1 }, new[] { 1, -1, -1 }, new[] { 1, -1, -1 }, new[] { -1, -1, -1 }, new[] { -1, -1, -1 } },
                Indices = new int[] { 1, 2, 3, 4, 5 }
            };
            var bonusContext = new BonusStateContext(1, new FortuneChimesBonus { SpinBet = bet });
            var state = new FreeSpinState((1, 0), new ReSpinCollapse(bonusPositions, wheel, new List<int[]> { new int[0], new int[0], new int[0], new int[] { 1, 2, 4 }, new int[] { 2, 5, 7 } }));

            //Action
            var response = state.Handle(requestBonusContext, bonusContext);

            // Assert
            Assert.AreEqual(response.result.Win == 0, response.nextState is ReSpinState);
            Assert.AreEqual(response.result.Win > 0, response.nextState is FreeSpinReSpinState);
            Assert.IsFalse(response.nextState.IsCompleted);
        }

        [TestCase(TestName = "Test Free Spin Exploding Feature")]
        public void TestFreeSpinExplodingFeature()
        {
            // arrange
            var user = new UserGameKey(-1, 109);
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var bet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
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
            var bonusContext = new BonusStateContext(1, new FortuneChimesBonus { SpinBet = bet });
            var bonusPositions = new List<BonusPosition> { new BonusPosition { RowPositions = new List<int> { 1, 1, 1, 0, 0 } } };
            var wheel = new FortuneChimesWheel
            {
                Reels = new List<int[]> { new[] { 1, -1, -1 }, new[] { 1, -1, -1 }, new[] { 1, -1, -1 }, new[] { -1, -1, -1 }, new[] { -1, -1, -1 } },
                Indices = new int[] { 1, 2, 3, 4, 5 }
            };
            var previousFreeSpin =
                new PreviousFreeSpin((1, 0, new List<int[]> { new[] { 1, -1, -1 }, new[] { 1, -1, -1 }, new[] { 1, -1, -1 }, new[] { -1, -1, -1 }, new[] { -1, -1, -1 } }),
                new ReSpinCollapse(bonusPositions, wheel, new List<int[]> { new int[0], new int[0], new int[0], new int[] { 1, 2, 4 }, new int[] { 2, 5, 7 } }));
            var state = new FreeSpinReSpinState(previousFreeSpin, new ReSpinCollapse(bonusPositions, wheel, new List<int[]> { new int[0], new int[0], new int[0], new int[] { 1, 2, 4 }, new int[] { 2, 5, 7 } }));

            //Action
            var response = state.Handle(requestBonusContext, bonusContext);

            // Assert
            Assert.AreEqual(response.result.Win == 0, response.nextState is FreeSpinState);
            Assert.AreEqual(response.result.Win > 0, response.nextState is FreeSpinReSpinState);
            Assert.IsTrue(!response.nextState.IsCompleted);
        }

        [TestCase(TestName = "Test Bonus Builder")]
        public void TestBonusBuilder()
        {
            // arrange
            var user = new UserGameKey(-1, 109);
            var requestContext = new RequestContext<SpinArgs>("simulation", "Cleopatra", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };

            var userSession = new UserSession
            {
                SessionKey = "unittest"
            };
            var spinArgs = new SpinArgs
            {
                LineBet = 1,
                Multiplier = 1
            };
            requestContext.Parameters = spinArgs;
            requestContext.UserSession = userSession;
            var freeSpinWheel = new FortuneChimesWheel
            {
                Reels = new List<int[]> {
                    new [] {0,1,2 },
                    new [] { 0, 10, 1 },
                    new [] { 0, 10, 3 },
                    new [] { 0, 10, 2 },
                    new [] { 2, 3, 4 }}
            };

            var explodingWheel = new FortuneChimesWheel
            {
                Reels = new List<int[]> {
                    new [] {0,1,2 },
                    new [] { 0, 4, 1 },
                    new [] { 0, 10, 3 },
                    new [] { 0, 10, 2 },
                    new [] { 2, 3, 4 }}
            };

            // Action
            var freeSpin = GameReduce.DoSpin(1, requestContext, freeSpinWheel);
            var expoding = GameReduce.DoSpin(1, requestContext, explodingWheel);

            // Assert
            Assert.IsTrue(freeSpin.HasBonus);
            Assert.IsTrue(freeSpin.Bonus.ClientId == 3);
            Assert.IsTrue(freeSpin.Bonus.Count == 14);

            Assert.IsTrue(expoding.HasBonus);
            Assert.IsTrue(expoding.Bonus.ClientId == 4);
            Assert.IsTrue(expoding.Bonus.Count == 1);
        }
    }
}