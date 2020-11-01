namespace Slot.UnitTests.Cleopatra
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.Cleopatra;
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
            var logger = logFactory.CreateLogger<CleopatraModule>();
            module = new CleopatraModule(logger);
        }

        [TestCase(TestName = "Test Calculate the total bet", ExpectedResult = 50)]
        public decimal TestCalculateTotalBet()
        {
            var user = new UserGameKey(-1, 3);
            var requestContext = new RequestContext<SpinArgs>("unittest", "Cleopatra", PlatformType.None)
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
            var wheel = module.InitialRandomWheel() as CleopatraWheel;
            Assert.NotNull(wheel);

            var resuslt = wheel.Reels.Calculate(1);
            Assert.AreEqual(true, resuslt.win == 0);
        }

        [TestCase(TestName = "Test Spin")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 3);
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

            // Action
            var spin = module.ExecuteSpin(1, new UserGameSpinData(), requestContext);
            var result = spin.Value as CleopatraSpinResult;

            // Assert

            Assert.AreEqual(1, result.Level);
            Assert.AreNotEqual(result.Wheel, default(CleopatraWheel));
            Assert.IsTrue(result.Bet == 50);
            if (result.HasBonus)
            {
                switch (result.Bonus.ClientId)
                {
                    case 2:
                        Assert.AreEqual(15, result.Bonus.Count);
                        break;

                    case 3:
                        Assert.AreEqual(1, result.Bonus.Count);
                        break;
                }
            }
        }

        [TestCase(TestName = "Test Create Free Spin Bonus")]
        public void TestCreateFreeGameBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 3);
            var result1 = new CleopatraSpinResult(user)
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 2) { Count = 4 }
            };

            // action
            var bonus1 = module.CreateBonus(result1);
            var state1 = ((CleopatraBonus)bonus1.Value).State;

            // Assert
            Assert.AreEqual(result1.Bonus.Guid, bonus1.Value.Guid);
            Assert.AreEqual(true, state1 is FreeSpinState);
            Assert.AreEqual(true, ((FreeSpinState)state1).Count == 4);
        }

        [TestCase(TestName = "Test Bonus Spin")]
        public void TestExecuteBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 3);
            var spinBet = new SpinBet(user, PlatformType.None)
            {
                Lines = 50,
                LineBet = 1.0m
            };
            var result1 = new CleopatraSpinResult(user)
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 2) { Count = 1 }
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
                Game = new Game { Id = 51 }
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
            var freeSpinResult1 = module.ExecuteBonus(1, entity1, requestBonusContext).Value as CleopatraBonusSpinResult;

            // assert
            Assert.NotNull(freeSpinResult1);
            Assert.AreEqual(freeSpinResult1.SpinResult.HasBonus == false, ((CleopatraBonus)freeSpinResult1.Bonus).State is FreeSpinState);
            Assert.AreEqual(freeSpinResult1.GameResultType, GameResultType.FreeSpinResult);
        }
    }
}