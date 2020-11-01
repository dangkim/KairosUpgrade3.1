namespace Slot.UnitTests.BikiniBeach
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.BikiniBeach;
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
            var logger = logFactory.CreateLogger<Games.BikiniBeach.BikiniBeachModule>();
            module = new BikiniBeachModule(logger);
        }

        [TestCase(TestName = "Test Calculate the total bet", ExpectedResult = 30)]
        public decimal TestCalculateTotalBet()
        {
            var user = new UserGameKey(-1, 3);
            var requestContext = new RequestContext<SpinArgs>("unittest", "Bikini Beach", PlatformType.None)
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
            var wheel = module.InitialRandomWheel() as BikiniBeachWheel;
            Assert.NotNull(wheel);

            var resuslt = wheel.Reels.Calculate(-1, 1);
            Assert.AreEqual(true, resuslt.win == 0);
        }

        [TestCase(TestName = "Test Spin")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 3);
            var requestContext = new RequestContext<SpinArgs>("simulation", "Bikini Beach", PlatformType.None)
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
            var result = spin.Value as BikiniBeachResult;

            // Assert

            Assert.AreEqual(1, result.Level);
            Assert.AreNotEqual(result.Wheel, default(BikiniBeachWheel));
            Assert.IsTrue(result.Bet == 30);
            if (result.HasBonus)
            {
                switch (result.Bonus.ClientId)
                {
                    case 2:
                        Assert.AreEqual(4, result.Bonus.Count);
                        break;

                    case 3:
                        Assert.AreEqual(1, result.Bonus.Count);
                        break;

                    case 4:
                        Assert.AreEqual(3, result.Bonus.Count);
                        break;
                }
            }
        }

        [TestCase(TestName = "Test Create Free Spin Bonus")]
        public void TestCreateFreeGameBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 3);
            var result1 = new BikiniBeachResult()
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 2) { Count = 4 }
            };
            var result2 = new BikiniBeachResult()
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 3) { Count = 1 }
            };
            var result3 = new BikiniBeachResult()
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 4) { Count = 3 }
            };

            // action
            var bonus1 = module.CreateBonus(result1);
            var bonus2 = module.CreateBonus(result2);
            var bonus3 = module.CreateBonus(result3);
            var state1 = ((BikiniBeachBonus)bonus1.Value).State;
            var state2 = ((BikiniBeachBonus)bonus2.Value).State;
            var state3 = ((BikiniBeachBonus)bonus3.Value).State;

            // Assert
            Assert.AreEqual(result1.Bonus.Guid, bonus1.Value.Guid);
            Assert.AreEqual(result2.Bonus.Guid, bonus2.Value.Guid);
            Assert.AreEqual(result3.Bonus.Guid, bonus3.Value.Guid);
            Assert.AreEqual(true, state1 is FreeSpin);
            Assert.AreEqual(true, state2 is SwimWear);
            Assert.AreEqual(true, state3 is BodyPart);
            Assert.AreEqual(true, ((FreeSpin)state1).Count == 4);
            Assert.AreEqual(true, ((SwimWear)state2).Count == 1);
            Assert.AreEqual(true, ((BodyPart)state3).Count == 3);
        }

        [TestCase(TestName = "Test Bonus Spin")]
        public void TestExecuteBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 3);
            var spinBet = new SpinBet(user, PlatformType.None)
            {
                Lines = 30,
                LineBet = 1.0m
            };
            var result1 = new BikiniBeachResult()
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 2) { Count = 1 }
            };
            var result2 = new BikiniBeachResult()
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 3) { Count = 1 }
            };
            var result3 = new BikiniBeachResult()
            {
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 4) { Count = 1 }
            };
            result1.SpinBet = spinBet;
            result2.SpinBet = spinBet;
            result3.SpinBet = spinBet;
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var requestContext = new RequestContext<SpinArgs>("simulation", "Bikini Beach", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
                Game = new Game { Id = 3 }
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "Bikini Beach", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };
            requestBonusContext.UserSession = userSession;
            requestBonusContext.UserGameKey = user;
            var bonus1 = module.CreateBonus(result1).Value;
            var bonus2 = module.CreateBonus(result2).Value;
            var bonus3 = module.CreateBonus(result3).Value;

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
            var entity2 = new BonusEntity
            {
                UserId = userSession.UserId,
                GameId = requestContext.Game.Id,
                Guid = bonus2.Guid.ToString("N"),
                Data = Model.Utility.Extension.ToByteArray(bonus2),
                BonusType = bonus2.GetType().Name,
                Version = 3,
                IsOptional = bonus2.IsOptional,
                IsStarted = bonus2.IsStarted,
                RoundId = 1,
                BetReference = ""
            };
            var entity3 = new BonusEntity
            {
                UserId = userSession.UserId,
                GameId = requestContext.Game.Id,
                Guid = bonus3.Guid.ToString("N"),
                Data = Model.Utility.Extension.ToByteArray(bonus3),
                BonusType = bonus3.GetType().Name,
                Version = 3,
                IsOptional = bonus3.IsOptional,
                IsStarted = bonus3.IsStarted,
                RoundId = 1,
                BetReference = ""
            };

            // action
            var freeSpinResult1 = module.ExecuteBonus(1, entity1, requestBonusContext).Value as BikiniBeachBonusSpinResult;
            var freeSpinResult2 = module.ExecuteBonus(1, entity2, requestBonusContext).Value as BikiniBeachBonusSpinResult;
            var freeSpinResult3 = module.ExecuteBonus(1, entity3, requestBonusContext).Value as BikiniBeachBonusSpinResult;

            // assert
            Assert.NotNull(freeSpinResult1);
            Assert.NotNull(freeSpinResult2);
            Assert.NotNull(freeSpinResult3);

            Assert.AreEqual(freeSpinResult1.SpinResult.HasBonus == false, ((BikiniBeachBonus)freeSpinResult1.Bonus).State is Finish);
            Assert.AreEqual(freeSpinResult2.SpinResult.HasBonus == false, ((BikiniBeachBonus)freeSpinResult1.Bonus).State is Finish);
            Assert.AreEqual(freeSpinResult3.SpinResult.HasBonus == false, ((BikiniBeachBonus)freeSpinResult1.Bonus).State is Finish);
            Assert.AreEqual(freeSpinResult1.GameResultType, GameResultType.FreeSpinResult);
            Assert.AreEqual(freeSpinResult2.GameResultType, GameResultType.RevealResult);
            Assert.AreEqual(freeSpinResult3.GameResultType, GameResultType.RevealResult);
        }
    }
}