namespace Slot.UnitTests.FuDaoLe
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Core.Modules.Infrastructure.Models;
    using Core.Modules.Infrastructure;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Model.Entity;
    using NUnit.Framework;
    using Slot.Games.FuDaoLe;
    using Slot.Model;

    [TestFixture]
    internal class EngineTests
    {
        private static readonly UserGameKey user = new UserGameKey(-1, 99);
        private static IGameModule module;

        [SetUp]
        public void Settup()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()               
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<Slot.Games.FuDaoLe.Module>();
            module = new Slot.Games.FuDaoLe.Module(logger);
        }

        private static RequestContext<T> CreateRequestContext<T>(UserGameKey userGameKey)
        {
            var requestContext = new RequestContext<T>("uinttest", "fudaole", PlatformType.None)
            {
                GameSetting = new GameSetting { GameSettingGroupId = 1 },
                Query = new QueryCollection { },
            };

            var userSession = new UserSession
            {
                SessionKey = "simulation"
            };
            requestContext.UserSession = userSession;
            return requestContext;
        }

        private static FuDaoLeWheel CreateWheel(IReadOnlyList<int[]> reelStrips, int replaceBy)
        {
            var wheel = new FuDaoLeWheel();
            foreach (var item in reelStrips)
            {
                wheel.Reels.Add(item);
                wheel.ActualReels.Add(item.Select(ele => ele == 13 ? replaceBy : ele).ToArray());
            }
            return wheel;
        }

        [TestCase("0,18,18, 3,4,5, 5,8,6, 7,8,9, 4,2,11", 8, TestName = "Spin with non winning", ExpectedResult = 0)]
        [TestCase("0,18,18, 3,4,15, 5,8,14, 7,8,9, 4,2,11", 8, TestName = "Spin with Simple winning", ExpectedResult = 5)]
        [TestCase("0,18,18, 3,4,15, 5,8,14, 7,8,14, 4,2,14", 8, TestName = "Spin with 5 kind of Nine & Scatter winning", ExpectedResult = 25 + 2 * 38 * 1)]
        [TestCase("0,18,10, 3,4,15, 5,8,14, 7,8,14, 11,2,14", 8, TestName = "Spin with 5 kind of Nine & Scatter winning & Envelope Jackpot", ExpectedResult = 25 + 25 + 2 * 38 * 1 + (38 * 20 * 1))]
        public decimal TestSpin(string reelStripsString, int replaceBy)
        {
            // arrange
            var request = CreateRequestContext<SpinArgs>(user);
            request.Parameters = new SpinArgs
            {
                LineBet = 1.0m,
                Multiplier = 1
            }; ;
            var reelStrips = ParsheetTests.Encoding(reelStripsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var wheel = CreateWheel(reelStrips, replaceBy);
            wheel.Replace = replaceBy;

            // action
            var result = Engine.DoSpin(1, request, wheel);

            // assert
            return result.Win;
        }

        [TestCase("0,18,18, 3,4,15, 5,8,14, 7,8,14, 4,2,14", 8, TestName = "Spin and trigger Free Spin", ExpectedResult = 25 + 2 * 38 * 1)]
        public decimal TestSpinAndTriggerFreeSpinBonus(string reelStripsString, int replaceBy)
        {
            // arrange
            var request = CreateRequestContext<SpinArgs>(user);
            request.Parameters = new SpinArgs
            {
                LineBet = 1.0m,
                Multiplier = 1
            };
            var reelStrips = ParsheetTests.Encoding(reelStripsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var wheel = CreateWheel(reelStrips, replaceBy);
            wheel.Replace = replaceBy;

            // action
            var result = Engine.DoSpin(1, request, wheel);

            // assert
            Assert.AreEqual(result.Bonus.Count, 8);
            Assert.AreEqual(string.Join(',', result.BonusPositions[0].RowPositions), "0,0,3,3,3");
            return result.Win;
        }

        [TestCase("1,18,18, 3,4,5, 5,8,15, 7,8,15, 4,2,9", 8, TestName = "Free Spin without winning", ExpectedResult = 0)]
        [TestCase("1,18,18, 3,4,15, 5,8,15, 7,8,15, 4,2,9", 8, TestName = "Free Spin with winning & Non additional Free Spin", ExpectedResult = 10)]
        public decimal TestFreeSpin(string freeSpinStripsString, int replaceBy)
        {
            // arrange
            var request = CreateRequestContext<SpinArgs>(user);
            request.Parameters = new SpinArgs
            {
                LineBet = 1.0m,
                Multiplier = 1
            };
            var bonusRequest = CreateRequestContext<BonusArgs>(user);
            var reelStrips = ParsheetTests.Encoding("0,18,18,3,4,15,5,8,14,7,8,14,4,2,14".Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var wheel = CreateWheel(reelStrips, replaceBy);
            wheel.Replace = replaceBy;

            // action
            var result = Engine.DoSpin(1, request, wheel);
            var bonus = module.CreateBonus(result);
            var freeSpinBonus = bonus.Value as FreeSpinBonus;
            reelStrips = ParsheetTests.Encoding(freeSpinStripsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            wheel = CreateWheel(reelStrips, replaceBy);
            wheel.Replace = replaceBy;
            var freeSpinResult = Engine.ClaimBonus(1, freeSpinBonus, bonusRequest, wheel) as FuDaoLeFreeSpinResult;

            // assert
            Assert.AreEqual(freeSpinResult.Counter, 7);
            Assert.AreEqual(freeSpinResult.CumulativeWin, freeSpinResult.Win);
            Assert.AreEqual(freeSpinResult.IsCompleted, false);

            Assert.AreEqual(freeSpinBonus.CurrentStep, 2);
            Assert.AreEqual(freeSpinBonus.Counter, 7);
            Assert.AreEqual(freeSpinBonus.IsCompleted, false);
            Assert.AreEqual(string.Join(',', result.BonusPositions[0].RowPositions), "0,0,3,3,3");
            return freeSpinResult.Win;
        }

        [TestCase("1,18,18, 3,4,15, 5,8,15, 7,8,15, 4,2,14", 8, TestName = "Free Spin with winning & 1 additional Free Spin", ExpectedResult = 25)]
        [TestCase("1,18,18, 3,4,15, 5,8,15, 7,8,15, 14,2,14", 8, TestName = "Free Spin with winning & 2 additional Free Spin", ExpectedResult = 25 + 25)]
        [TestCase("1,18,18, 3,4,15, 5,8,15, 7,8,15, 14,14,14", 8, TestName = "Free Spin with winning & 3 additional Free Spin", ExpectedResult = 25 + 25 + 25)]
        public decimal TestFreeSpinAndHasAdditionalFreeSpin(string freeSpinStripsString, int replaceBy)
        {
            // arrange
            var request = CreateRequestContext<SpinArgs>(user);
            request.Parameters = new SpinArgs
            {
                LineBet = 1.0m,
                Multiplier = 1
            };
            var bonusRequest = CreateRequestContext<BonusArgs>(user);
            var reelStrips = ParsheetTests.Encoding("0,18,18,3,4,15,5,8,14,7,8,14,4,2,14".Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            var wheel = CreateWheel(reelStrips, replaceBy);
            wheel.Replace = replaceBy;

            // action
            var result = Engine.DoSpin(1, request, wheel);
            var bonus = module.CreateBonus(result);
            var freeSpinBonus = bonus.Value as FreeSpinBonus;
            reelStrips = ParsheetTests.Encoding(freeSpinStripsString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
            wheel = CreateWheel(reelStrips, replaceBy);
            wheel.Replace = replaceBy;
            var freeSpinResult = Engine.ClaimBonus(1, freeSpinBonus, bonusRequest, wheel) as FuDaoLeFreeSpinResult;
            var result1 = freeSpinResult.SpinResult;

            // assert
            Assert.AreEqual(freeSpinResult.Counter, 7 + result1.Bonus.Count);
            Assert.AreEqual(freeSpinResult.CumulativeWin, freeSpinResult.Win);
            Assert.AreEqual(freeSpinResult.IsCompleted, false);

            Assert.AreEqual(freeSpinBonus.CurrentStep, 2);
            Assert.AreEqual(freeSpinBonus.Counter, 7 + result1.Bonus.Count);
            Assert.AreEqual(freeSpinBonus.IsCompleted, false);
            Assert.AreEqual(string.Join(',', result.BonusPositions[0].RowPositions), "0,0,3,3,3");
            return freeSpinResult.Win;
        }
    }
}