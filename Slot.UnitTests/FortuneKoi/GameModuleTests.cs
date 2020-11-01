namespace Slot.UnitTests.FortuneKoi
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.FortuneKoi;
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
            var logger = logFactory.CreateLogger<FortuneKoiModule>();
            module = new FortuneKoiModule(logger);
        }

        [TestCase(TestName = "Test Calculate the total bet", ExpectedResult = 10)]
        public decimal TestCalculateTotalBet()
        {
            var user = new UserGameKey(-1, 32);
            var requestContext = new RequestContext<SpinArgs>("unittest", "FortuneKoi", PlatformType.None)
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

        [TestCase(TestName = "Test Spin")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 32);
            var requestContext = new RequestContext<SpinArgs>("simulation", "FortuneKoi", PlatformType.None)
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
            var result = spin.Value as FortuneKoiSpinResult;

            // Assert

            Assert.AreEqual(1, result.Level);
            Assert.AreNotEqual(result.Wheel, default(FortuneKoiWheel));
            Assert.IsTrue(result.Bet == 10);
            Assert.AreEqual(result.GameResultType, GameResultType.SpinResult);
            if (result.HasBonus)
            {
                Assert.NotNull(result.Bonus);
                Assert.AreEqual(1, result.Bonus.Count);
            }
        }

        [TestCase(TestName = "Test Create Free ReSpin Bonus")]
        public void TestCreateFreeGameBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 32);
            var result1 = new FortuneKoiSpinResult(user)
            {
                Wheel = new FortuneKoiWheel
                {
                    Reels = new List<int[]>
                    {
                        new int[] { },
                        new int[] { 7,7,7},
                        new int[] { },
                        new int[] { },
                        new int[] { }
                    }
                },
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 0, 0, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 3) { Count = 1 }
            };

            // action
            var bonus1 = module.CreateBonus(result1);
            var state1 = ((FortuneKoiBonus)bonus1.Value).State;

            // Assert
            Assert.AreEqual(result1.Bonus.Guid, bonus1.Value.Guid);
            Assert.AreEqual(true, state1 is ReSpinState);
            Assert.AreEqual(true, ((ReSpinState)state1).Count == 1);
        }

        [TestCase(TestName = "Test Bonus Spin")]
        public void TestExecuteBonus()
        {
            // Arrange
            var user = new UserGameKey(-1, 3);
            var spinBet = new SpinBet(user, PlatformType.None)
            {
                Lines = 10,
                LineBet = 1.0m
            };
            var result1 = new FortuneKoiSpinResult(user)
            {
                Wheel = new FortuneKoiWheel
                {
                    Reels = new List<int[]>
                    {
                        new int[] { },
                        new int[] { 7,7,7},
                        new int[] { },
                        new int[] { },
                        new int[] { }
                    }
                },
                BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 0, 0, 0 } } },
                Bonus = new Stake(Guid.NewGuid(), 2) { Count = 1 }
            };

            result1.SpinBet = spinBet;
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var requestContext = new RequestContext<SpinArgs>("simulation", "FortuneKoi", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
                Game = new Game { Id = 32 }
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "FortuneKoi", PlatformType.None)
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
            var reSpinResult = module.ExecuteBonus(1, entity1, requestBonusContext).Value as FortuneKoiReSpinResult;

            // Assert
            Assert.NotNull(reSpinResult);
            Assert.AreEqual(reSpinResult.SpinResult.HasBonus, ((FortuneKoiBonus)reSpinResult.Bonus).State is ReSpinState);
            Assert.AreEqual(reSpinResult.SpinResult.HasBonus, reSpinResult.SpinResult.Bonus != null);
            Assert.AreEqual(reSpinResult.SpinResult.HasBonus == false, ((FortuneKoiBonus)reSpinResult.Bonus).State is Finish);
            Assert.AreEqual(reSpinResult.GameResultType, GameResultType.RespinResult);
        }

        [TestCase(TestName = "Test InitialRandomWheel")]
        public void TestInitialRandomWheel()
        {
            // Action
            var wheel = module.InitialRandomWheel() as FortuneKoiWheel;
            var spinResult = Payout.Calculate(wheel.Reels, 1.0m);

            // Assert
            Assert.IsTrue(spinResult.win == 0);
            Assert.IsTrue(spinResult.positions.Count == 0);
        }

        [TestCase(TestName = "Test Get Extra Settings")]
        public void TestGetExtraSettings()
        {
            // Action
            var extraSetting = module.GetExtraSettings(1);

            // Assert
            Assert.IsTrue(extraSetting != null);
            Assert.IsTrue(extraSetting is EmptyExtraGameSettings);
        }
    }
}