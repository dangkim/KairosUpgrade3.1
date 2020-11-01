using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Slot.Games.MagicPaper;
using Slot.Games.MagicPaper.Models.GameResults;
using Slot.Games.MagicPaper.Models.Test;
using Slot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Config = Slot.Games.MagicPaper.Configuration.Configuration;

namespace Slot.Simulations
{
    [TestFixture]
    public class MagicPaper
    {
        private Module GetModule()
        {
            var serviceProvider = new ServiceCollection()
                                        .AddLogging()
                                        .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<Module>();
            var module = new Module(logger);

            return module;
        }

        [TestCase(Config.GameId, Config.LevelOne, "CNY", 1, 1000000, 1, Config.Lines, 1, TestName = "[LVL1-BET1] 1M MagicPaper")]
        [TestCase(Config.GameId, Config.LevelOne, "CNY", 1, 1000000, 1, Config.Lines, 2, TestName = "[LVL1-BET2] 1M MagicPaper")]
        [TestCase(Config.GameId, Config.LevelOne, "CNY", 1, 200000, 1, Config.Lines, 3, TestName = "[LVL1-BET3] 1M MagicPaper")]
        [TestCase(Config.GameId, Config.LevelTwo, "CNY", 1, 1000000, 1, Config.Lines, 1, TestName = "[LVL2-BET1] 1M MagicPaper")]
        [TestCase(Config.GameId, Config.LevelTwo, "CNY", 1, 1000000, 1, Config.Lines, 2, TestName = "[LVL2-BET2] 1M MagicPaper")]
        [TestCase(Config.GameId, Config.LevelTwo, "CNY", 1, 1000000, 1, Config.Lines, 3, TestName = "[LVL2-BET3] 1M MagicPaper")]
        [TestCase(Config.GameId, Config.LevelThree, "CNY", 1, 1000000, 1, Config.Lines, 1, TestName = "[LVL3-BET1] 1M MagicPaper")]
        [TestCase(Config.GameId, Config.LevelThree, "CNY", 1, 1000000, 1, Config.Lines, 2, TestName = "[LVL3-BET2] 1M MagicPaper")]
        [TestCase(Config.GameId, Config.LevelThree, "CNY", 1, 1000000, 1, Config.Lines, 3, TestName = "[LVL3-BET3] 1M MagicPaper")]
        public void TestBets(int gameId, int level, string currencyCode, int numOfUsers, int numItrPerUser, decimal bet, int lines, int mp)
        {
            var timeStart = DateTime.Now;
            var module = GetModule();
            var targetRtpLevel = Config.RtpLevels.FirstOrDefault(rl => rl.Level == level && rl.Multiplier == mp).Rtp;
            var totalSummaryData = new SummaryData();

            var users = GenerateUsers(gameId, numOfUsers, level);
            var spinBets = GenerateUserBets(users, bet, 0, lines, mp);

            Parallel.ForEach(users,
                () => new SummaryData(),
                (key, state, summaryData) =>
                {
                    var spinBet = spinBets[key.UserId];
                    for (var i = 0; i < numItrPerUser; i++)
                    {
                        var spinResult = new MagicPaperSpinResult() { SpinBet = spinBet }
                                                .CreateWheel(Config.Wheel.Width, Config.Wheel.Height)
                                                .GenerateRandomWheel(Config.Wheels, level, mp)
                                               
                                                .CalculateWin(bet, mp);

                        summaryData.Update(spinResult);
                    }

                    return summaryData;
                },
                sdata => { lock (totalSummaryData) { totalSummaryData.Sum(sdata); } });

            totalSummaryData.DisplayData(level, timeStart, targetRtpLevel);
            totalSummaryData.DisplayPayoutsData(bet, lines);

            var isWithinRtp = totalSummaryData.RtpData.OverallRtp >= targetRtpLevel - 1 && totalSummaryData.RtpData.OverallRtp <= targetRtpLevel + 1;

            Assert.True(isWithinRtp, $"RTP not matching. The result is {totalSummaryData.RtpData.OverallRtp}.");
        }

        [TestCase(Config.LevelOne, 1, TestName = "[LVL1-BET1] Full Cycle MagicPaper")]
        [TestCase(Config.LevelOne, 2, TestName = "[LVL1-BET2] Full Cycle MagicPaper")]
        [TestCase(Config.LevelOne, 3, TestName = "[LVL1-BET3] Full Cycle MagicPaper")]
        [TestCase(Config.LevelTwo, 1, TestName = "[LVL2-BET1] Full Cycle MagicPaper")]
        [TestCase(Config.LevelTwo, 2, TestName = "[LVL2-BET2] Full Cycle MagicPaper")]
        [TestCase(Config.LevelTwo, 3, TestName = "[LVL2-BET3] Full Cycle MagicPaper")]
        [TestCase(Config.LevelThree, 1, TestName = "[LVL3-BET1] Full Cycle MagicPaper")]
        [TestCase(Config.LevelThree, 2, TestName = "[LVL3-BET2] Full Cycle MagicPaper")]
        [TestCase(Config.LevelThree, 3, TestName = "[LVL3-BET3] Full Cycle MagicPaper")]
        public void TestFullCycle(int level, int betLines)
        {
            var timeStart = DateTime.Now;
            var module = GetModule();
            var targetRtpLevel = Math.Round(Config.RtpLevels.FirstOrDefault(rl => rl.Level == level && rl.Multiplier == betLines).Rtp, 2);
            var totalSummaryData = new SummaryData();

            var ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = module.GameId,
                Level = 888
            };

            var reelStrip = Config.LevelReels
                                    .FirstOrDefault(lr => lr.Level == level)
                                    .ReelStrips
                                        .FirstOrDefault(rs => rs.BetLines == betLines);

            reelStrip.Symbols[0].ForEach(sym1 =>
            {
                reelStrip.Symbols[1].ForEach(sym2 =>
                {
                    reelStrip.Symbols[2].ForEach(sym3 =>
                    {
                        var spinResult = new MagicPaperSpinResult()
                        {
                            SpinBet = new SpinBet(ugk, PlatformType.None)
                            {
                                Lines = Config.Lines,
                                Multiplier = betLines,
                                LineBet = 1
                            },
                            Wheel = GetFullCycleWheel(Config.Lines, new List<int> { sym1, sym2, sym3 })
                        }
                        .CalculateWin(1, betLines);

                        totalSummaryData.Update(spinResult);
                    });
                });
            });

            totalSummaryData.DisplayData(level, timeStart, targetRtpLevel);
            var resultOverallRtp = Math.Round(totalSummaryData.RtpData.OverallRtp, 2);

            var isRtpEquivalent = resultOverallRtp == targetRtpLevel;
            Assert.True(isRtpEquivalent, $"RTP not matching. The result is {resultOverallRtp}.");
        }

        private static Wheel GetFullCycleWheel(int linebet, List<int> symbols)
        {
            var width = Config.Wheel.Width;
            var height = Config.Wheel.Height;
            var wheel = new Wheel(width, height);

            for (var widthIndex = 0; widthIndex < width; widthIndex++)
            {
                for (var heightIndex = 0; heightIndex < height; heightIndex++)
                {
                    wheel[widthIndex].Add(linebet == heightIndex ? symbols[widthIndex] : 0);
                }
            }

            return wheel;
        }

        private List<UserGameKey> GenerateUsers(int gameId, int numusers, int level)
        {
            List<UserGameKey> ugk = new List<UserGameKey>();
            for (int i = 1; i < numusers + 1; ++i)
                ugk.Add(new UserGameKey(-i, gameId) { Level = level });
            return ugk;
        }

        private Dictionary<int, SpinBet> GenerateUserBets(List<UserGameKey> keys, decimal bet, int credits, int lines, int mp)
        {
            Dictionary<int, SpinBet> sbs = new Dictionary<int, SpinBet>();

            foreach (var k in keys)
            {
                SpinBet sb = new SpinBet(k, PlatformType.None)
                {
                    LineBet = bet,
                    Credits = credits,
                    Lines = lines,
                    Multiplier = mp
                };
                sbs[k.UserId] = sb;
            }

            return sbs;
        }
    }
}
