namespace Slot.UnitTests.FortuneChimes
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.FortuneChimes;
    using Slot.Model;
    using Slot.Model.Entity;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    internal class GameModuleTests
    {
        private static IGameModule module;

        [SetUp]
        public void Settup()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<FortuneChimesModule>();
            module = new FortuneChimesModule(logger);
        }

        [TestCase(TestName = "Test Calculate the total bet", ExpectedResult = 30)]
        public decimal TestCalculateTotalBet()
        {
            var user = new UserGameKey(-1, 109);
            var requestContext = new RequestContext<SpinArgs>("unittest", "FortuneChimes", PlatformType.None)
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
            return module.CalculateTotalBet(new UserGameSpinData(), requestContext);
        }

        [TestCase(TestName = "Test Generate a wheel for getbet")]
        public void TestInitialRandomWheel()
        {
            var wheel = module.InitialRandomWheel() as FortuneChimesWheel;
            Assert.NotNull(wheel);

            var resuslt = wheel.Reels.Calculate(1);
            Assert.AreEqual(true, resuslt.win == 0);
        }

        [TestCase(TestName = "Test Spin")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 109);
            var requestContext = new RequestContext<SpinArgs>("simulation", "FortuneChimes", PlatformType.None)
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

            // Action
            var spin = module.ExecuteSpin(1, new UserGameSpinData(), requestContext);
            var result = spin.Value as FortuneChimesSpinResult;

            // Assert

            Assert.AreEqual(1, result.Level);
            Assert.AreNotEqual(result.Wheel, default(FortuneChimesWheel));
            Assert.IsTrue(result.Bet == 30);
            if (result.HasBonus)
            {
                switch (result.Bonus.ClientId)
                {
                    case 3:
                        Assert.AreEqual(14, result.Bonus.Count);
                        break;

                    case 4:
                        Assert.AreEqual(1, result.Bonus.Count);
                        break;

                    case 5:
                        Assert.AreEqual(1, result.Bonus.Count);
                        break;
                }
            }
        }

        [TestCase(TestName = "Test Create Free Spin Bonus")]
        public void TestCreateFreeGameBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 109);
            var result1 = new FortuneChimesSpinResult(user)
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 0, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 3) { Count = 4 },
                Wheel = new FortuneChimesWheel
                {
                    Reels = new List<int[]>
                    {
                        new int[] { 1, 1, 1 },
                        new int[] { 2, 2, 2 },
                        new int[] { 10, 10, 10 },
                        new int[] { 4, 4, 4 },
                        new int[] { 5, 5, 5 }
                    },
                    Indices = new int[] { 0, 0, 0, 0, 0 }
                }
            };

            // action
            var bonus1 = module.CreateBonus(result1);
            var state1 = ((FortuneChimesBonus)bonus1.Value).State;

            // Assert
            Assert.AreEqual(result1.Bonus.Guid, bonus1.Value.Guid);
            Assert.AreEqual(true, state1 is FreeSpinState);
            Assert.AreEqual(true, ((FreeSpinState)state1).Count == 4);
        }

        [TestCase(TestName = "Test Create Free Spin&ReSpin Bonus")]
        public void TestCreateFreeAndReSpinGameBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 109);
            var wheel = new FortuneChimesWheel
            {
                Reels = new List<int[]>
                {
                    new int[] { 1, 1, 1 },
                    new int[] { 2, 2, 2 },
                    new int[] { 10,10,10 },
                    new int[] { 4, 4, 4 },
                    new int[] { 5, 5, 5 }
                },
                Indices = new int[] { 0, 0, 0, 0, 0 }
            };
            var result1 = new FortuneChimesSpinResult(user)
            {
                Wheel = wheel,
                WinPositions = new List<WinPosition>
                {
                    new WinPosition
                    {
                        Line = 0, Count = 3,  Symbol = 10, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 }
                    },
                    new WinPosition{ Line = 1,Count = 3, RowPositions = new List<int> { 1, 1, 1, 0, 0 } }
                },
                Bonus = new Stake(Guid.NewGuid(), 3) { Count = 4 }
            };

            // action
            var bonus1 = module.CreateBonus(result1);
            var state1 = ((FortuneChimesBonus)bonus1.Value).State;

            // Assert
            Assert.AreEqual(result1.Bonus.Guid, bonus1.Value.Guid);
            Assert.AreEqual(true, state1 is FreeSpinState);
            Assert.AreEqual(true, ((FreeSpinState)state1).Count == 4);
            Assert.AreEqual(true, !state1.IsCompleted);
            Assert.AreEqual(((FreeSpinState)state1).ReSpinCollapse.Indices, wheel.Indices);
            Assert.AreEqual(((FreeSpinState)state1).ReSpinCollapse.ReelsReSpin, new List<int[]>
            {
                new int[0],
                new int[0],
                new int[0],
                new int[0],
                new int[] { 5, 5, 5 }
            });
        }

        [TestCase(TestName = "Test Create Reveal Bonus")]
        public void TestCreateRevealBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 109);
            var wheel = new FortuneChimesWheel
            {
                Reels = new List<int[]>
                {
                new int[] { 1, 1, 1 },
                new int[] { 2, 2, 2 },
                new int[] { 3, 3, 3 },
                new int[] { 4, 4, 4 },
                new int[] { 5, 5, 5 },
                },
                Indices = new int[] { 0, 0, 0, 0, 0 }
            };
            var result1 = new FortuneChimesSpinResult(user)
            {
                Wheel = wheel,
                BonusPositions = new List<BonusPosition>
                {
                new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 1, 2, 2, 2, 0 } },
                },
                Bonus = new Stake(Guid.NewGuid(), 5) { Count = 1 }
            };

            // action
            var bonus1 = module.CreateBonus(result1);
            var state1 = ((FortuneChimesBonus)bonus1.Value).State;

            // Assert
            Assert.AreEqual(result1.Bonus.Guid, bonus1.Value.Guid);
        }

        [TestCase(TestName = "Test Create ReSpin Bonus")]
        public void TestCreateReSpinBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 109);
            var wheel = new FortuneChimesWheel
            {
                Reels = new List<int[]>
                {
                new int[] { 1, 1, 1 },
                new int[] { 2, 2, 2 },
                new int[] { 3, 3, 3 },
                new int[] { 4, 4, 4 },
                new int[] { 5, 5, 5 },
                },
                Indices = new int[] { 0, 0, 0, 0, 0 }
            };
            var result1 = new FortuneChimesSpinResult(user)
            {
                Wheel = wheel,
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 1, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 4) { Count = 1 }
            };

            // action
            var bonus1 = module.CreateBonus(result1);
            var state1 = ((FortuneChimesBonus)bonus1.Value).State;

            // Assert
            Assert.AreEqual(result1.Bonus.Guid, bonus1.Value.Guid);
            Assert.AreEqual(true, state1 is ReSpinState);
            Assert.AreEqual(true, ((ReSpinState)state1).Count == 1);
        }

        [TestCase(TestName = "Test Bonus Spin")]
        public void TestExecuteBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 109);
            var spinBet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
                LineBet = 1.0m
            };
            var wheel = new FortuneChimesWheel
            {
                Reels = new List<int[]>
                {
                new int[] { 1, 1, 1 },
                new int[] { 2, 10, 2 },
                new int[] { 3, 10, 3 },
                new int[] { 4, 10, 4 },
                new int[] { 5, 5, 5 },
                },
                Indices = new int[] { 0, 0, 0, 0, 0 }
            };
            var result1 = new FortuneChimesSpinResult(user)
            {
                Wheel = wheel,
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 3) { Count = 1 }
            };

            result1.SpinBet = spinBet;
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var requestContext = new RequestContext<SpinArgs>("simulation", "Cleopatra", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
                Game = new Game { Id = 109 }
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "Cleopatra", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };
            requestBonusContext.UserSession = userSession;
            requestBonusContext.UserGameKey = user;
            var bonus1 = module.CreateBonus(result1).Value;

            var entity1 = new BonusEntity
            {
                UserId = userSession.UserId,
                GameId = requestContext.Game.Id,
                Guid = bonus1.Guid.ToString("N"),
                Data = Model.Utility.Extension.ToByteArray(bonus1),
                BonusType = bonus1.GetType().Name,
                Version = 3,
                IsOptional = bonus1.IsOptional,
                IsStarted = bonus1.IsStarted,
                RoundId = 1,
                BetReference = ""
            };

            // action
            var freeSpinResult1 = module.ExecuteBonus(1, entity1, requestBonusContext).Value as FortuneChimesBonusSpinResult;

            // assert
            Assert.NotNull(freeSpinResult1);
            Assert.AreEqual(freeSpinResult1.GameResultType, GameResultType.FreeSpinResult);
        }
    }
}