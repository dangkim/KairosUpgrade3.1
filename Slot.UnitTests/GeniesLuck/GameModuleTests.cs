namespace Slot.UnitTests.GeniesLuck
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.GeniesLuck;
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
            var logger = logFactory.CreateLogger<Games.GeniesLuck.GeniesLuckModule>();
            module = new GeniesLuckModule(logger);
        }

        [TestCase(TestName = "Test Calculate the total bet", ExpectedResult = 40)]
        public decimal TestCalculateTotalBet()
        {
            var user = new UserGameKey(-1, 80);
            var requestContext = new RequestContext<SpinArgs>("unittest", "Genies Luck", PlatformType.None)
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
            var wheel = module.InitialRandomWheel() as GeniesLuckWheel;
            Assert.NotNull(wheel);

            var resuslt = wheel.Reels.Calculate(1);
            Assert.AreEqual(true, resuslt.win == 0);
        }

        [TestCase(TestName = "Test Spin")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 80);
            var requestContext = new RequestContext<SpinArgs>("simulation", "Genies Luck", PlatformType.None)
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
            var result = spin.Value as GeniesLuckResult;

            // Assert

            Assert.AreEqual(1, result.Level);
            Assert.AreNotEqual(result.Wheel, default(GeniesLuckWheel));
            Assert.IsTrue(result.Bet == 40);
            if (result.HasBonus)
            {
                if (result.Bonus.ClientId == 3)
                    Assert.AreEqual(10, result.Bonus.Count);
                else
                    Assert.AreEqual(3, result.Bonus.Count);
            }
        }

        [TestCase(TestName = "Test Create Free Spin Bonus")]
        public void TestCreateFreeSpinBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 80);
            var result = new GeniesLuckResult()
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 3) { Count = 10 }
            };
            // action
            var bonus = module.CreateBonus(result);
            var state = ((GeniesLuckBonus)bonus.Value).State;

            // Assert
            Assert.AreEqual(result.Bonus.Guid, bonus.Value.Guid);
            Assert.AreEqual(true, state is FreeSpin);
            Assert.AreEqual(true, ((FreeSpin)state).Count == 10);
        }

        [TestCase(TestName = "Test Create ReSpin Bonus")]
        public void TestCreateRespinSpinBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 80);
            var result = new GeniesLuckResult()
            {
                Wheel = new GeniesLuckWheel { Reels = new List<int[]> { new[] { 9, 9, 9 }, new[] { 1, 2, 3 }, new[] { 2, 3, 4 }, new[] { 4, 6, 5 }, new[] { 5, 2, 1 } } },
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 1, 0, 0, 0, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 4) { Count = 3 }
            };
            // action
            var bonus = module.CreateBonus(result);
            var state = ((GeniesLuckBonus)bonus.Value).State;

            // Assert
            Assert.AreEqual(result.Bonus.Guid, bonus.Value.Guid);
            Assert.AreEqual(true, state is ReSpin);
            Assert.AreEqual(true, ((ReSpin)state).Count == 3);
        }

        [TestCase(TestName = "Test Bonus Spin")]
        public void TestExecuteBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 80);
            var result = new GeniesLuckResult()
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 3) { Count = 1 }
            };
            result.SpinBet = new SpinBet(user, PlatformType.None)
            {
                Lines = 40,
                LineBet = 1.0m
            };
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var requestContext = new RequestContext<SpinArgs>("simulation", "Genies Luck", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
                Game = new Game { Id = 80 }
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "Genies Luck", PlatformType.None)
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
            var respinResult = module.ExecuteBonus(1, entity, requestBonusContext).Value as GeniesLuckBonusSpinResult;

            // assert
            Assert.NotNull(respinResult);
            Assert.AreEqual(respinResult.SpinResult.HasBonus == false, ((GeniesLuckBonus)respinResult.Bonus).State is Finish);
            Assert.AreEqual(respinResult.GameResultType, GameResultType.FreeSpinResult);
        }
    }
}