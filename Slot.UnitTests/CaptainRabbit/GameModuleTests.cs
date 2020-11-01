namespace Slot.UnitTests.CaptainRabbit
{
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.FSharp.Collections;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.CaptainRabbit;
    using Slot.Model;
    using Slot.Model.Entity;
    using System;
    using System.Collections.Generic;
    using static Slot.Games.CaptainRabbit.Domain;
    using static Slot.Games.CaptainRabbit.Domain.BonusState;
    using static Slot.Games.CaptainRabbit.Global;

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
            var logger = logFactory.CreateLogger<BonusBearModule.Engine>();
            module = new BonusBearModule.Engine(logger);
        }

        [TestCase(TestName = "Test Spin")]
        public void TestSpin()
        {
            // Arrange
            var user = new UserGameKey(-1, 85);
            var requestContext = new RequestContext<SpinArgs>("simulation", "Bonus Bear", PlatformType.None)
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
            var result = spin.Value as BearResult;

            // Assert

            Assert.AreEqual(1, result.Level);
            Assert.AreNotEqual(result.Wheel, default(BearWheel));
            Assert.IsTrue(result.Bet == 25);
            if (result.HasBonus)
            {
                if (result.Bonus.ClientId == 3)
                {
                    Assert.AreEqual(15, result.Bonus.Count);
                }
                else
                    Assert.AreEqual(4, result.Bonus.ClientId);
            }
        }

        [TestCase(TestName = "Test Bonus Spin")]
        public void TestExecuteBonus()
        {
            // Arrange
            var spinResult = new BearResult("s");
            var user = new UserGameKey(-1, 103);
            var reels = new List<int[]>
            {
                new[] { 0, 6, 10 },
                new[] { 6, 11, 12 },
                new[] { 6, 11, 13 },
                new[] { 9, 11, 12, },
                new[] { 12, 10, 13 }
            };

            spinResult.Bonus = new Stake(Guid.NewGuid(), 3);
            spinResult.Bonus.Count = 15;
            spinResult.Wheel = new BearWheel(1, ArrayModule.OfSeq(reels));
            spinResult.BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } };
            spinResult.SpinBet = new SpinBet(user, PlatformType.None)
            {
                Lines = 25,
                LineBet = 1.0m
            };
            var userSession = new UserSession
            {
                SessionKey = "unittest",
                UserId = -1
            };
            var requestContext = new RequestContext<SpinArgs>("simulation", "Bonus Bear", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
                Game = new Game { Id = 103 }
            };
            var requestBonusContext = new RequestContext<BonusArgs>("unittest", "Bonus Bear", PlatformType.None)
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
            var result = module.ExecuteBonus(1, entity, requestBonusContext).Value as BearSeqFreeSpinResult;

            // assert
            Assert.NotNull(result);
            Assert.AreEqual(true, ((Domain.BearBonus)result.Bonus).State.IsFreeSpin);
            Assert.IsTrue(result.Type == "fs");
        }

        [TestCase(TestName = "Test Generate a wheel for getbet")]
        public void TestInitialRandomWheel()
        {
            Assert.NotNull(module.InitialRandomWheel());
        }

        [TestCase(TestName = "Test Calculate the total bet", ExpectedResult = 25)]
        public decimal TestCalculateTotalBet()
        {
            var user = new UserGameKey(-1, 85);
            var requestContext = new RequestContext<SpinArgs>("unittest", "Bonus Bear", PlatformType.None)
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
            var result = new BearResult("s");
            var reels = new List<int[]>
            {
                new[] { 0, 6, 10 },
                new[] { 6, 11, 12 },
                new[] { 6, 11, 13 },
                new[] { 9, 11, 12, },
                new[] { 12, 10, 13 }
            };

            result.Bonus = new Stake(Guid.NewGuid(), 3);
            result.Bonus.Count = 15;
            result.Wheel = new BearWheel(1, ArrayModule.OfSeq(reels));
            result.BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 0, 2, 2, 2, 0 } } };

            // action
            var bonus = module.CreateBonus(result);
            var state = ((BearBonus)bonus.Value).State;

            // Assert
            Assert.AreEqual(result.Bonus.Guid, bonus.Value.Guid);
            Assert.AreEqual(true, state.IsFreeSpin);
            Assert.AreEqual(false, state.IsHoney);
            Assert.AreEqual(false, state.IsHoneyFreeSpin);
            Assert.AreEqual(3, ((FreeSpin)state).Item.Id);
            Assert.AreEqual(15, ((FreeSpin)state).Item.Prize.Count);
            Assert.AreEqual(15, ((FreeSpin)state).Item.Prize.TotalSpin);
        }

        [TestCase(TestName = "Test Create Honey Bonus")]
        public void TestCreateHoneyBonus()
        {
            // Arrange
            var result = new BearResult("s");
            var reels = new List<int[]>
            {
                 new[] { 1, 6, 10 },
                new[] { 6, 9, 12 },
                new[] { 6, 10, 13 },
                new[] { 9, 13, 12, },
                new[] { 12, 10, 13 }
            };

            result.Bonus = new Stake(Guid.NewGuid(), 4);
            result.Bonus.Count = HoneyPot.getHoneyPots(HoneyPot.Mode.Primary, 0.75);
            result.Wheel = new BearWheel(1, ArrayModule.OfSeq(reels));
            result.BonusPositions = new List<BonusPosition> { new BonusPosition { Line = 1, Multiplier = 1, RowPositions = new List<int> { 3, 0, 2, 0, 2 } } };

            // action
            var bonus = module.CreateBonus(result);
            var state = ((BearBonus)bonus.Value).State;

            // Assert
            Assert.AreEqual(result.Bonus.Guid, bonus.Value.Guid);
            Assert.AreEqual(true, state.IsHoney);
            Assert.AreEqual(false, state.IsFreeSpin);
            Assert.AreEqual(false, state.IsHoneyFreeSpin);
            Assert.AreEqual(4, ((Honey)state).Item.Id);
            Assert.AreEqual(3, ((Honey)state).Item.Pot.Count);
            Assert.AreEqual(3, ((Honey)state).Item.Pot.TotalSpin);

            Assert.AreEqual(1, ((Honey)state).Item.BeeHive.Count);
            Assert.AreEqual(1, ((Honey)state).Item.BeeHive.TotalSpin);
        }

        [TestCase(0.25, TestName = "Test Main Game Honey 1 Pot", ExpectedResult = 1)]
        [TestCase(0.55, TestName = "Test Main Game Honey 2 Pots", ExpectedResult = 2)]
        [TestCase(0.75, TestName = "Test Main Game Honey 3 Pots", ExpectedResult = 3)]
        [TestCase(0.90, TestName = "Test Main Game Honey 4 Pots", ExpectedResult = 4)]
        [TestCase(1.0, TestName = "Test Main Game Honey 5 Pots", ExpectedResult = 5)]
        public int TestMainGameHoneyPotSelection(double ratio)
        {
            var mode = HoneyPot.Mode.Primary;
            return HoneyPot.getHoneyPots(mode, ratio);
        }

        [TestCase(0.20, TestName = "Test Free Game Honey 1 Pot", ExpectedResult = 1)]
        [TestCase(0.35, TestName = "Test Free Game Honey 2 Pots", ExpectedResult = 2)]
        [TestCase(0.60, TestName = "Test Free Game Honey 3 Pots", ExpectedResult = 3)]
        [TestCase(0.70, TestName = "Test Free Game Honey 4 Pots", ExpectedResult = 4)]
        [TestCase(1.00, TestName = "Test Free Game Honey 5 Pots", ExpectedResult = 5)]
        public int TestFreeGameHoneyPotSelection(double ratio)
        {
            var mode = HoneyPot.Mode.Additional;
            return HoneyPot.getHoneyPots(mode, ratio);
        }

        [TestCase(0.25, TestName = "Test Honey Pot Prize - 1", ExpectedResult = 1)]
        [TestCase(0.40, TestName = "Test Honey Pot Prize - 2", ExpectedResult = 2)]
        [TestCase(0.53, TestName = "Test Honey Pot Prize - 3", ExpectedResult = 3)]
        [TestCase(0.68, TestName = "Test Honey Pot Prize - 5", ExpectedResult = 5)]
        [TestCase(0.76, TestName = "Test Honey Pot Prize - 6", ExpectedResult = 6)]
        [TestCase(0.84, TestName = "Test Honey Pot Prize - 8", ExpectedResult = 8)]
        [TestCase(0.89, TestName = "Test Honey Pot Prize - 10", ExpectedResult = 10)]
        [TestCase(0.94, TestName = "Test Honey Pot Prize - 12", ExpectedResult = 12)]
        [TestCase(0.97, TestName = "Test Honey Pot Prize - 13", ExpectedResult = 13)]
        [TestCase(0.99, TestName = "Test Honey Pot Prize - 14", ExpectedResult = 14)]
        [TestCase(1.0, TestName = "Test Honey Pot Prize - 15", ExpectedResult = 15)]
        public int TestHoneyPotPrize(double ratio)
        {
            var mode = HoneyPot.Mode.Primary;
            return HoneyPot.getPrize(mode, ratio);
        }
    }
}