using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Slot.Games.FaFaZhu;
using Slot.Games.FaFaZhu.Models;

namespace Slot.Simulations
{
    [TestFixture]
    public class FaFaZhuSimulation
    {
        [TestCase(FaFaZhuConfiguration.GameId, 1, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL1] 100K Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 1, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL1] 1M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 1, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL1] 10M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 1, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL1] 100M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 1, "CNY", 1, 100000000, 1, 10, 2, TestName = "[LVL1] 100M Little Piggy mul2")]
        [TestCase(FaFaZhuConfiguration.GameId, 1, "CNY", 1, 100000000, 1, 10, 3, TestName = "[LVL1] 100M Little Piggy mul3")]
        [TestCase(FaFaZhuConfiguration.GameId, 2, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL2] 100K Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 2, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL2] 1M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 2, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL2] 10M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 2, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL2] 100M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 2, "CNY", 1, 100000000, 1, 10, 2, TestName = "[LVL2] 100M Little Piggy mul2")]
        [TestCase(FaFaZhuConfiguration.GameId, 2, "CNY", 1, 100000000, 1, 10, 3, TestName = "[LVL2] 100M Little Piggy mul3")]
        [TestCase(FaFaZhuConfiguration.GameId, 3, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL3] 100K Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 3, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL3] 1M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 3, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL3] 10M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 3, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL3] 100M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 3, "CNY", 1, 100000000, 1, 10, 2, TestName = "[LVL3] 100M Little Piggy mul2")]
        [TestCase(FaFaZhuConfiguration.GameId, 3, "CNY", 1, 100000000, 1, 10, 3, TestName = "[LVL3] 100M Little Piggy mul3")]
        [TestCase(FaFaZhuConfiguration.GameId, 4, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL4] 100K Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 4, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL4] 1M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 4, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL4] 10M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 4, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL4] 100M Little Piggy")]
        [TestCase(FaFaZhuConfiguration.GameId, 4, "CNY", 1, 100000000, 1, 10, 2, TestName = "[LVL4] 100M Little Piggy mul2")]
        [TestCase(FaFaZhuConfiguration.GameId, 4, "CNY", 1, 100000000, 1, 10, 3, TestName = "[LVL4] 100M Little Piggy mul3")]
        public void TestFaFaZhu(int gameId, int level, string currencyCode, int numusers, int numItrPerUser, decimal bet, int lines, int mp)
        {
            var sdt = DateTime.Now;

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<FaFaZhuModule>();

            var FaFaZhuModule = new FaFaZhuModule(logger);
            var maxWin = 0m;

            var summData = new SummaryData();
            var users = GenerateUsers(gameId, numusers, level);
            var sbs = GenerateUserBets(users, bet, lines, mp);

            RequestContext<SpinArgs> requestContext = new RequestContext<SpinArgs>("", FaFaZhuConfiguration.GameName, PlatformType.Web);

            RequestContext<BonusArgs> requestBonusContext = new RequestContext<BonusArgs>("", FaFaZhuConfiguration.GameName, PlatformType.Web);

            Parallel.ForEach(users,
                () => new SummaryData(),
                (key, state, sdata) =>
                {
                    var sb = sbs[key.UserId];
                    requestContext.Currency = new Currency() { Id = sb.CurrencyId };
                    requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines, Multiplier = mp };
                    requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

                    for (var i = 0; i < numItrPerUser; ++i)
                    {
                        var sr = FaFaZhuModule.ExecuteSpin(level, new UserGameSpinData(), requestContext).Value as FaFaZhuSpinResult;

                        if (sr.Win > maxWin)
                            maxWin = sr.Win;

                        UpdateSummaryData(sdata, sr);
                        UpdateFaFaZhuSummaryData(sdata, sr);
                    }

                    return sdata;
                },
                sdata =>
                {
                    lock (summData)
                    {
                        summData.Sum(sdata);
                    }
                });

            DisplayFaFaZhuSummaryData(level, bet, lines, summData, sdt, maxWin, null);
        }

        public void TestFaFaZhuFullCycle(int level, List<int> rows)
        {
            var sdt = DateTime.Now;
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<FaFaZhuModule>();

            var FaFaZhuModule = new FaFaZhuModule(logger);
            var maxWin = 0m;
            Dictionary<decimal, int> profile = new Dictionary<decimal, int>();
            List<int> maxIndexPosition = new List<int>();
            var summData = new SummaryData();
            var reel1Pointer = 0;
            var reel2Pointer = 0;
            var reel3Pointer = 0;

            for (var reel0 = 0; reel0 < FaFaZhuConfiguration.VirtualReels[level][0]; reel0++)
            {
                for (var reel1 = 0; reel1 < FaFaZhuConfiguration.VirtualReels[level][1]; reel1++)
                {
                    for (var reel2 = 0; reel2 < FaFaZhuConfiguration.VirtualReels[level][2]; reel2++)
                    {
                        var totalWin = (decimal)0;

                        UserGameKey ugk = new UserGameKey()
                        {
                            UserId = -1,
                            GameId = FaFaZhuConfiguration.GameId,
                            Level = 1
                        };

                        SpinBet sb = new SpinBet(ugk, PlatformType.None)
                        {
                            LineBet = 1,
                            Credits = 0,
                            Lines = FaFaZhuConfiguration.BettingLines,
                            Multiplier = 1
                        };

                        RequestContext<SpinArgs> requestContext = new RequestContext<SpinArgs>("", FaFaZhuConfiguration.GameName, PlatformType.Web);
                        RequestContext<BonusArgs> requestBonusContext = new RequestContext<BonusArgs>("", FaFaZhuConfiguration.GameName, PlatformType.Web);
                        requestContext.Currency = new Currency() { Id = sb.CurrencyId };
                        requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
                        requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

                        var wheel = new Wheel(rows);

                        for (int i = 1; i < FaFaZhuConfiguration.Wheels[level].Reels[0].Count - 1; i++)
                        {
                            if (reel0 < FaFaZhuConfiguration.CumulativeWeightReels[level][0][i - 1])
                            {
                                reel1Pointer = i;

                                wheel.Reels[0] = FaFaZhuCommon.GetRange(FaFaZhuConfiguration.Wheels[level].Reels[0], reel1Pointer, wheel.Rows[0]);

                                break;
                            }
                        }

                        for (int j = 1; j < FaFaZhuConfiguration.Wheels[level].Reels[1].Count - 1; j++)
                        {
                            if (reel1 < FaFaZhuConfiguration.CumulativeWeightReels[level][1][j - 1])
                            {
                                reel2Pointer = j;

                                wheel.Reels[1] = FaFaZhuCommon.GetRange(FaFaZhuConfiguration.Wheels[level].Reels[1], reel2Pointer, wheel.Rows[1]);

                                break;
                            }
                        }

                        for (int k = 1; k < FaFaZhuConfiguration.Wheels[level].Reels[2].Count - 1; k++)
                        {
                            if (reel2 < FaFaZhuConfiguration.CumulativeWeightReels[level][2][k - 1])
                            {
                                reel3Pointer = k;

                                wheel.Reels[2] = FaFaZhuCommon.GetRange(FaFaZhuConfiguration.Wheels[level].Reels[2], reel3Pointer, wheel.Rows[2]);

                                break;
                            }
                        }

                        FaFaZhuSpinResult sr = new FaFaZhuSpinResult(ugk)
                        {
                            SpinBet = new SpinBet(ugk, PlatformType.None)
                            {
                                Lines = FaFaZhuConfiguration.BettingLines,
                                Multiplier = 1,
                                LineBet = 1m
                            },

                            WheelActual = wheel
                        };

                        sr.WheelActual = wheel;

                        FaFaZhuCommon.CalculateWin(sr);

                        totalWin = sr.Win;

                        UpdateSummaryData(summData, sr);
                        UpdateFaFaZhuSummaryData(summData, sr);

                        if (!profile.ContainsKey(totalWin))
                        {
                            profile.Add(totalWin, 1);
                        }
                        else
                        {
                            profile.TryGetValue(totalWin, out int count);
                            profile.Remove(totalWin);
                            profile.Add(totalWin, count + 1);
                        }
                    }
                }
            }

            DisplayFaFaZhuSummaryData(level, 1, 10, summData, sdt, maxWin, maxIndexPosition);
            if (profile != null)
            {
                foreach (var item in profile.OrderByDescending(p => p.Key))
                {
                    Console.WriteLine($"Win            : {item.Key}; {item.Value}");
                }
            }
        }

        [TestCase(1, TestName = "[LVL1] Full Cycle")]
        [TestCase(2, TestName = "[LVL2] Full Cycle")]
        [TestCase(3, TestName = "[LVL3] Full Cycle")]
        [TestCase(4, TestName = "[LVL4] Full Cycle")]
        public void Test333FaFaZhuFullCycle(int level)
        {
            TestFaFaZhuFullCycle(level, new List<int>() { 3, 3, 3 });
        }

        private static List<UserGameKey> GenerateUsers(int gameId, int numusers, int level)
        {
            List<UserGameKey> ugk = new List<UserGameKey>();
            for (int i = 1; i < numusers + 1; ++i)
                ugk.Add(new UserGameKey(-i, gameId) { Level = level });
            return ugk;
        }

        private Dictionary<int, SpinBet> GenerateUserBets(List<UserGameKey> keys, decimal bet, int lines, int mp)
        {
            return GenerateUserBets(keys, bet, 0, lines, mp);
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

        private void UpdateSummaryData(SummaryData sdata, SpinResult sr)
        {
            var FaFaZhuSpinResult = sr as FaFaZhuSpinResult;
            sdata.SpinCounter++;
            sdata.TotalBet += FaFaZhuSpinResult.SpinBet.TotalBet;
            sdata.TotalWin += FaFaZhuSpinResult.Win;
        }

        private void UpdateFaFaZhuSummaryData(SummaryData sd, SpinResult sr)
        {
            var FaFaZhuSpinResult = sr as FaFaZhuSpinResult;
            foreach (var wp in FaFaZhuSpinResult.WinPositions)
            {
                var vcounter = sd.WinCounter;
                vcounter[wp.Win] = vcounter.ContainsKey(wp.Win) ? vcounter[wp.Win] + 1 : 1;
            }
        }

        private void DisplayFaFaZhuSummaryData(int level, decimal bet, int lines, SummaryData summData, DateTime timeStart, decimal maxWin1, List<int> maxIndexPosition)
        {
            var timeEnd = DateTime.Now;
            Console.WriteLine($"-----------------------------------------------------------");
            Console.WriteLine($"Test.SpinCount      : {summData.SpinCounter}");
            Console.WriteLine($"Test.Level          : {level}");
            Console.WriteLine($"Test.TimeStart      : {timeStart.ToShortDateString()} {timeStart.ToShortTimeString()}");
            Console.WriteLine($"Test.TimeEnd        : {timeEnd.ToShortDateString()} {timeEnd.ToShortTimeString()}");
            Console.WriteLine($"-----------------------------------------------------------");
            Console.WriteLine($"TotalBet            : {summData.TotalBet}");
            Console.WriteLine($"MG TotalWin         : {summData.TotalWin}");
            Console.WriteLine($"Max Win             : {maxWin1}");

            var payoutHitrate = new SortedDictionary<int, TupleRW<int, decimal, decimal, decimal, decimal>>();

            foreach (var w in summData.WinCounter.OrderByDescending(x => x.Key))
            {
                var hits = summData.WinCounter.ContainsKey(w.Key) ? summData.WinCounter[w.Key] : 0;
                var probability = summData.WinCounter.ContainsKey(w.Key) ? (decimal)summData.WinCounter[w.Key] / (decimal)summData.SpinCounter : 0;
                var hitrate = summData.WinCounter.ContainsKey(w.Key) ? (decimal)summData.SpinCounter / (decimal)summData.WinCounter[w.Key] : 0;
                var win = w.Key * summData.SpinCounter * probability; //Win
                var rtp = win / summData.TotalBet;
                var payout = (int)(w.Key / bet);

                if (!payoutHitrate.ContainsKey(payout))
                    payoutHitrate.Add(payout, new TupleRW<int, decimal, decimal, decimal, decimal>(0, 0, 0, 0, 0));

                payoutHitrate[payout].Item1 += hits;
                payoutHitrate[payout].Item2 += win;
                payoutHitrate[payout].Item3 += (rtp * 100);
                payoutHitrate[payout].Item4 += probability;
                payoutHitrate[payout].Item5 += hitrate;
            }

            Console.WriteLine($"-----------------------------------------------------------");
            Console.WriteLine($"MG Spin Count           : {summData.SpinCounter}");
            Console.WriteLine($"-----------------------------------------------------------");

            Console.WriteLine($"--- HIT RATE ----------------------------------------------");
            foreach (var payout in payoutHitrate.OrderByDescending(p => p.Key))
            {
                Console.WriteLine($"{payout.Key,-10} \t{payout.Value.Item2,10:0.00} \t{payout.Value.Item1,10} \t{payout.Value.Item3,10:0.000} \t{payout.Value.Item4,10:0.000} \t{payout.Value.Item5,10:0.000}");
            }

            Console.WriteLine($"-----------------------------------------------------------");
            Console.WriteLine($"MG RTP                  : {summData.RTPSpin,0:P}");
            Console.WriteLine($"-----------------------------------------------------------");
            Console.WriteLine($"Total RTP               : {summData.RTPOverAll,0:P}");
        }

        private static readonly Dictionary<WheelEncoding, Func<int, int, int[], Wheel>> MapWheelEncoding = new Dictionary<WheelEncoding, Func<int, int, int[], Wheel>>()
        {
            { WheelEncoding.Local, WheelEncodingLocal },
        };

        public static Wheel WheelEncodingLocal(int width, int height, int[] arr)
        {
            int currentIndex = 0;
            Wheel w = new Wheel(width, height);
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < w.Rows[i]; ++j)
                {
                    if (i > 0)
                    {
                        w[i].Add(arr[currentIndex + j]);
                    }
                    else
                    {
                        w[i].Add(arr[j]);
                    }
                }

                currentIndex = currentIndex + w.Rows[i];
            }
            return w;
        }
    }
}
