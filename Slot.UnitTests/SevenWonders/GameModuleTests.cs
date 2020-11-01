namespace Slot.UnitTests.SevenWonders
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.SevenWonders;
    using Slot.Model;
    using Slot.Model.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
            var logger = logFactory.CreateLogger<Games.SevenWonders.SevenWondersModule>();
            module = new Games.SevenWonders.SevenWondersModule(logger);
        }

        [TestCase(TestName = "Test Calculate the total bet", ExpectedResult = 20)]
        public decimal TestCalculateTotalBet()
        {
            var user = new UserGameKey(-1, 104);
            var requestContext = new RequestContext<SpinArgs>("unittest", "Seven Wonders", PlatformType.None)
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
            var wheel = module.InitialRandomWheel() as SevenWondersWheel;
            Assert.NotNull(wheel);

            var resuslt = wheel.Reels.Calculate(1);
            Assert.AreEqual(true, resuslt.win == 0);
        }

        [TestCase(TestName = "Test Spin")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 104);
            var requestContext = new RequestContext<SpinArgs>("simulation", "Seven Wonders", PlatformType.None)
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
            var result = spin.Value as SevenWondersResult;

            // Assert

            Assert.AreEqual(1, result.Level);
            Assert.AreNotEqual(result.Wheel, default(SevenWondersWheel));
            Assert.IsTrue(result.Bet == 20);
            if (result.HasBonus)
            {
                if (result.Bonus.ClientId == 3)
                {
                    Assert.AreEqual(1, result.Bonus.Count);
                }
            }
        }

        [TestCase(TestName = "Test Create  Free Spin Bonus")]
        public void TestCreateFreeSpinBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 30);
            var result = new SevenWondersResult()
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 3) { Count = 1 }
            };
            // action
            var bonus = module.CreateBonus(result);
            var state = ((SevenWondersBonus)bonus.Value).State;

            // Assert
            Assert.AreEqual(result.Bonus.Guid, bonus.Value.Guid);
            Assert.AreEqual(true, state is FreeSpin);
        }

        [TestCase(TestName = "Test Bonus Spin")]
        public void TestExecuteBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 30);
            var result = new SevenWondersResult()
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 0, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 3) { Count = 1 }
            };
            result.SpinBet = new SpinBet(user, PlatformType.None)
            {
                Lines = 20,
                LineBet = 1.0m
            };
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var requestContext = new RequestContext<SpinArgs>("simulation", "Seven Wonders", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
                Game = new Game { Id = 104 }
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "Seven Wonders", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };
            requestBonusContext.UserSession = userSession;
            requestBonusContext.UserGameKey = user;
            var bonus = module.CreateBonus(result).Value;

            var entity = new BonusEntity
            {
                UserId = userSession.UserId,
                GameId = requestContext.Game.Id,
                Guid = bonus.Guid.ToString("N"),
                Data = Model.Utility.Extension.ToByteArray(bonus),
                BonusType = bonus.GetType().Name,
                Version = 3,
                IsOptional = bonus.IsOptional,
                IsStarted = bonus.IsStarted,
                RoundId = 1,
                BetReference = ""
            };

            // action
            var respinResult = module.ExecuteBonus(1, entity, requestBonusContext).Value as SevenWondersBonusSpinResult;

            // assert
            Assert.NotNull(respinResult);
            Assert.AreEqual(respinResult.SpinResult.Wheel[3].Any(ele => ele == 7), ((SevenWondersBonus)respinResult.Bonus).State is FreeSpin);
            Assert.AreEqual(respinResult.SpinResult.Wheel[3].All(ele => ele != 7), ((SevenWondersBonus)respinResult.Bonus).State is Finish);
        }
    }
}