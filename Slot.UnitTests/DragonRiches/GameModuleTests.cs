namespace Slot.UnitTests.DragonRiches
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
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
            var logger = logFactory.CreateLogger<Games.DragonRiches.DragonRichesModule>();
            module = new Games.DragonRiches.DragonRichesModule(logger);
        }

        [TestCase(TestName = "Test Calculate the total bet", ExpectedResult = 30)]
        public decimal TestCalculateTotalBet()
        {
            var user = new UserGameKey(-1, 104);
            var requestContext = new RequestContext<SpinArgs>("unittest", "Dragon Riches", PlatformType.None)
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

        [TestCase(TestName = "Test Create  Free Spin Bonus")]
        public void TestCreateFreeSpinBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 104);
            var result = new Games.DragonRiches.DragonRichesResult();
            var reels = new List<int[]>
            {
                new[] { 0, 6, 10 },
                new[] { 6, 11, 12 },
                new[] { 6, 11, 13 },
                new[] { 9, 11, 12, },
                new[] { 12, 10, 13 }
            };

            result.Bonus = new Stake(Guid.NewGuid(), 3)
            {
                Count = 6
            };
            result.Wheel = new Games.DragonRiches.DragonRichesWheel();
            result.BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } };

            // action
            var bonus = module.CreateBonus(result);
            var state = ((Games.DragonRiches.DragonRichesBonus)bonus.Value).State;

            // Assert
            Assert.AreEqual(result.Bonus.Guid, bonus.Value.Guid);
            Assert.AreEqual(true, state is Games.DragonRiches.FreeSpin);
            Assert.AreEqual(false, state is Games.DragonRiches.HoldSpin);
            Assert.AreEqual(false, state is Games.DragonRiches.HoldFreeSpin);
            Assert.AreEqual(6, ((Games.DragonRiches.FreeSpin)state).State.Count);
        }

        [TestCase(TestName = "Test Create Hold Spin Bonus")]
        public void TestCreateHoldSpinBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 104);
            var result = new Games.DragonRiches.DragonRichesResult();
            var reels = new List<int[]>
            {
                new[] { 1, 6, 11 },
                new[] { 6, 11, 11 },
                new[] { 11, 10, 13 },
                new[] { 9, 11, 12, },
                new[] { 12, 11, 13 }
            };

            result.Bonus = new Stake(Guid.NewGuid(), 4)
            {
                Count = 3
            };
            result.Wheel = new Games.DragonRiches.DragonRichesWheel();

            // action
            var bonus = module.CreateBonus(result);
            var state = ((Games.DragonRiches.DragonRichesBonus)bonus.Value).State;

            // Assert
            Assert.AreEqual(result.Bonus.Guid, bonus.Value.Guid);
            Assert.AreEqual(true, state is Games.DragonRiches.HoldSpin);
            Assert.AreEqual(false, state is Games.DragonRiches.FreeSpin);
            Assert.AreEqual(false, state is Games.DragonRiches.HoldFreeSpin);
            Assert.AreEqual(3, ((Games.DragonRiches.HoldSpin)state).State.Count);
        }

        [TestCase(TestName = "Test Bonus Spin")]
        public void TestExecuteBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 104);
            var spinResult = new Games.DragonRiches.DragonRichesResult();
            var reels = new List<int[]>
            {
                new[] { 0, 6, 10 },
                new[] { 6, 9, 12 },
                new[] { 6, 9, 13 },
                new[] { 9, 11, 12, },
                new[] { 12, 10, 13 }
            };

            spinResult.Bonus = new Stake(Guid.NewGuid(), 3);
            spinResult.Bonus.Count = 6;
            spinResult.Wheel = new Games.DragonRiches.DragonRichesWheel();
            spinResult.BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 1, 0 } } };
            spinResult.SpinBet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
                LineBet = 1.0m
            };
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var requestContext = new RequestContext<SpinArgs>("simulation", "Dragon Riches", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
                Game = new Game { Id = 104 }
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "Dragon Riches", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };
            requestBonusContext.UserSession = userSession;
            requestBonusContext.UserGameKey = user;
            var bonus = module.CreateBonus(spinResult).Value;

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
            var result = module.ExecuteBonus(1, entity, requestBonusContext).Value as Games.DragonRiches.DragonRichesBonusSpinResult;

            // assert
            Assert.NotNull(result);
            //Assert.AreEqual(true, ((DragonRichesBonus)result.Bonus).State.IsFreeSpin);
        }

        [TestCase(TestName = "Test Generate a wheel for getbet")]
        public void TestInitialRandomWheel()
        {
            Assert.NotNull(module.InitialRandomWheel());
        }

        [TestCase(TestName = "Test Spin")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 104);
            var requestContext = new RequestContext<SpinArgs>("simulation", "Dragon Riches", PlatformType.None)
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
            var result = spin.Value as Games.DragonRiches.DragonRichesResult;

            // Assert

            Assert.AreEqual(1, result.Level);
            Assert.AreNotEqual(result.Wheel, default(Games.DragonRiches.DragonRichesWheel));
            Assert.IsTrue(result.Bet == 30);
            if (result.HasBonus)
            {
                if (result.Bonus.ClientId == 3)
                {
                    Assert.AreEqual(6, result.Bonus.Count);
                }
                else
                    Assert.AreEqual(3, result.Bonus.ClientId);
            }
        }
    }
}