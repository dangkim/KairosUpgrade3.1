using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.AlchemyReels;
using Slot.Games.AlchemyReels.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slot.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Slot.Simulations
{
    [TestFixture]
    public class AlchemySimulation
    {
        [TestCase(AlchemyReelsConfiguration.GameId, 1, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL1] 100K Alchemy Reels")]
        [TestCase(AlchemyReelsConfiguration.GameId, 1, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL1] 1M Alchemy Reels")]
        [TestCase(AlchemyReelsConfiguration.GameId, 1, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL1] 10M Alchemy Reels")]
        [TestCase(AlchemyReelsConfiguration.GameId, 1, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL1] 100M Alchemy Reels")]
        public void TestAlchemyReels(int gameId, int level, string currencyCode, int numusers, int numItrPerUser, decimal bet, int lines, int mp)
        {
            var sdt = DateTime.Now;

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<AlchemyReelsModule>();

            var alchemyReelsModule = new AlchemyReelsModule(logger);
            var maxWin = 0m;
            AlchemyReelsCommon.CreateWheels(new List<int>() { 3, 3, 3 });

            var summData = new SummaryData();
            var users = GenerateUsers(gameId, numusers, level);
            var sbs = GenerateUserBets(users, bet, lines, mp);

            var requestContext = new RequestContext<SpinArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);

            var requestBonusContext = new RequestContext<BonusArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);

            Parallel.ForEach(users,
                () => new SummaryData(),
                (key, state, sdata) =>
                {
                    var sb = sbs[key.UserId];
                    requestContext.Currency = new Currency() { Id = sb.CurrencyId };
                    requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
                    requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

                    for (var i = 0; i < numItrPerUser; ++i)
                    {
                        var sr = alchemyReelsModule.ExecuteSpin(level, new UserGameSpinData(), requestContext).Value as AlchemyReelsCollapsingSpinResult;

                        if (sr.Win > maxWin)
                            maxWin = sr.Win;

                        UpdateSummaryData(sdata, sr);
                        UpdateAlchemyReelsSummaryData(sdata, sr);

                        if (sr.HasBonus)
                        {
                            var bonusCreated = alchemyReelsModule.CreateBonus(sr);

                            var bonus = bonusCreated.Value;

                            bonus.SpinTransactionId = sr.TransactionId;
                            bonus.GameResult = sr;

                            requestBonusContext.Currency = new Currency() { Id = sb.CurrencyId };
                            requestBonusContext.Parameters = new BonusArgs() { Bonus = "CollapsingSpin" };
                            requestBonusContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

                            BonusResult bonusResult;
                            var step = bonus.CurrentStep;

                            do
                            {
                                var entity = new BonusEntity
                                {
                                    UserId = key.UserId,
                                    GameId = AlchemyReelsConfiguration.GameId,
                                    Guid = bonus.Guid.ToString("N"),
                                    Data = bonus.ToByteArray(),
                                    BonusType = bonus.GetType().Name,
                                    Version = 2,
                                    IsOptional = bonus.IsOptional,
                                    IsStarted = bonus.IsStarted,
                                    RoundId = sr.RoundId
                                };

                                bonusResult = alchemyReelsModule.ExecuteBonus(level, entity, requestBonusContext).Value;
                                var alchemyFreeCollapsingSpinResult = bonusResult as AlchemyFreeCollapsingSpinResult;

                                UpdateSummaryDataCollapsing(sdata, alchemyFreeCollapsingSpinResult.SpinResult);
                                UpdateAlchemyReelsSummaryData(sdata, alchemyFreeCollapsingSpinResult.SpinResult);

                                bonus = bonusResult.Bonus;
                            }
                            while (!bonusResult.IsCompleted && bonusResult.Bonus != null);
                        }
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

            DisplayAlchemyReelsSummaryData(level, bet, lines, summData, sdt, maxWin, null);
        }

        [TestCase(1, TestName = "[LVL1] Full Cycle")]
        public void TestAlchemyReelsFullCycle(int level, List<int> rows)
        {
            var sdt = DateTime.Now;
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<AlchemyReelsModule>();

            var alchemyReelsModule = new AlchemyReelsModule(logger);
            var maxWin = 0m;
            var profile = new Dictionary<decimal, int>();
            var maxIndexPosition = new List<int>();
            AlchemyReelsCommon.CreateWheels(rows);

            var summData = new SummaryData();

            for (var reel0 = 0; reel0 < AlchemyReelsConfiguration.Wheels[level].Reels[0].Count; reel0++)
            {
                for (var reel1 = 0; reel1 < AlchemyReelsConfiguration.Wheels[level].Reels[1].Count; reel1++)
                {
                    for (var reel2 = 0; reel2 < AlchemyReelsConfiguration.Wheels[level].Reels[2].Count; reel2++)
                    {
                        var totalWin = (decimal)0;

                        var ugk = new UserGameKey()
                        {
                            UserId = -1,
                            GameId = AlchemyReelsConfiguration.GameId,
                            Level = 1
                        };

                        var sb = new SpinBet(ugk, PlatformType.None)
                        {
                            LineBet = 1,
                            Credits = 0,
                            Lines = AlchemyReelsConfiguration.BettingLines,
                            Multiplier = 1
                        };

                        var requestContext = new RequestContext<SpinArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);
                        var requestBonusContext = new RequestContext<BonusArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);
                        requestContext.Currency = new Currency() { Id = sb.CurrencyId };
                        requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
                        requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

                        var wheel = new Wheel(rows);
                        wheel.Reels[0] = GetRange(AlchemyReelsConfiguration.Wheels[level].Reels[0], reel0, wheel.Rows[0]);
                        wheel.Reels[1] = GetRange(AlchemyReelsConfiguration.Wheels[level].Reels[1], reel1, wheel.Rows[1]);
                        wheel.Reels[2] = GetRange(AlchemyReelsConfiguration.Wheels[level].Reels[2], reel2, wheel.Rows[2]);

                        var sr = new AlchemyReelsCollapsingSpinResult()
                        {
                            SpinBet = new SpinBet(ugk, PlatformType.None)
                            {
                                Lines = AlchemyReelsConfiguration.BettingLines,
                                Multiplier = 1,
                                LineBet = 1m
                            },

                            Wheel = wheel
                        };

                        sr.Wheel = wheel;
                        sr.TopIndices = new List<int>() { reel0, reel1, reel2 };

                        AlchemyReelsCommon.CalculateWin(sr);

                        totalWin = sr.Win;

                        UpdateSummaryData(summData, sr);
                        UpdateAlchemyReelsSummaryData(summData, sr);

                        if (sr.HasBonus)
                        {
                            var bonusCreated = alchemyReelsModule.CreateBonus(sr);

                            var bonus = bonusCreated.Value;

                            bonus.SpinTransactionId = sr.TransactionId;
                            bonus.GameResult = sr;

                            requestBonusContext.Currency = new Currency() { Id = sb.CurrencyId };
                            requestBonusContext.Parameters = new BonusArgs() { Bonus = "CollapsingSpin" };
                            requestBonusContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

                            BonusResult bonusResult;
                            var step = bonus.CurrentStep;

                            do
                            {
                                var entity = new BonusEntity
                                {
                                    UserId = ugk.UserId,
                                    GameId = AlchemyReelsConfiguration.GameId,
                                    Guid = bonus.Guid.ToString("N"),
                                    Data = bonus.ToByteArray(),
                                    BonusType = bonus.GetType().Name,
                                    Version = 2,
                                    IsOptional = bonus.IsOptional,
                                    IsStarted = bonus.IsStarted,
                                    RoundId = sr.RoundId
                                };

                                bonusResult = alchemyReelsModule.ExecuteBonus(level, entity, requestBonusContext).Value;
                                var alchemyFreeCollapsingSpinResult = bonusResult as AlchemyFreeCollapsingSpinResult;

                                var win = alchemyFreeCollapsingSpinResult.Win;

                                if (win > 0)
                                {
                                    totalWin += win;
                                }

                                var maxTopIndices = alchemyFreeCollapsingSpinResult.SpinResult.TopIndices.ToList();

                                if (totalWin > maxWin)
                                {
                                    maxWin = totalWin;

                                    maxIndexPosition = maxTopIndices;
                                }

                                UpdateSummaryDataCollapsing(summData, alchemyFreeCollapsingSpinResult.SpinResult);
                                UpdateAlchemyReelsSummaryData(summData, alchemyFreeCollapsingSpinResult.SpinResult);

                                bonus = bonusResult.Bonus;
                            }
                            while (!bonusResult.IsCompleted && bonusResult.Bonus != null);
                        }

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

            DisplayAlchemyReelsSummaryData(level, 1, 10, summData, sdt, maxWin, maxIndexPosition);
            if (profile != null)
            {
                foreach (var item in profile.OrderByDescending(p => p.Key))
                {
                    Console.WriteLine($"Win            : {item.Key}; {item.Value}");
                }

            }
        }

        [TestCase(1, TestName = "game333")]
        public void Test333AlchemyReelsFullCycle(int level)
        {
            TestAlchemyReelsFullCycle(level, new List<int>() { 3, 3, 3 });
        }

        [TestCase(1, TestName = "game533")]
        public void Test533AlchemyReelsFullCycle(int level)
        {
            TestAlchemyReelsFullCycle(level, new List<int>() { 5, 3, 3 });
        }

        [TestCase(1, TestName = "game353")]
        public void Test353AlchemyReelsFullCycle(int level)
        {
            TestAlchemyReelsFullCycle(level, new List<int>() { 3, 5, 3 });
        }

        [TestCase(1, TestName = "game335")]
        public void Test335AlchemyReelsFullCycle(int level)
        {
            TestAlchemyReelsFullCycle(level, new List<int>() { 3, 3, 5 });
        }

        [TestCase(1, TestName = "game553")]
        public void Test553AlchemyReelsFullCycle(int level)
        {
            TestAlchemyReelsFullCycle(level, new List<int>() { 5, 5, 3 });
        }

        [TestCase(1, TestName = "game535")]
        public void Test535AlchemyReelsFullCycle(int level)
        {
            TestAlchemyReelsFullCycle(level, new List<int>() { 5, 3, 5 });
        }

        [TestCase(1, TestName = "game355")]
        public void Test355AlchemyReelsFullCycle(int level)
        {
            TestAlchemyReelsFullCycle(level, new List<int>() { 3, 5, 5 });
        }

        [TestCase(1, TestName = "game555")]
        public void Test555AlchemyReelsFullCycle(int level)
        {
            TestAlchemyReelsFullCycle(level, new List<int>() { 5, 5, 5 });
        }

        private static List<UserGameKey> GenerateUsers(int gameId, int numusers, int level)
        {
            var ugk = new List<UserGameKey>();
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
            var sbs = new Dictionary<int, SpinBet>();

            foreach (var k in keys)
            {
                var sb = new SpinBet(k, PlatformType.None)
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
            var alchemySpinResult = sr as AlchemyReelsCollapsingSpinResult;
            sdata.SpinCounter++;
            sdata.TotalBet += alchemySpinResult.SpinBet.TotalBet;
            sdata.TotalWin += alchemySpinResult.Win;
        }

        private void UpdateAlchemyReelsSummaryData(SummaryData sd, SpinResult sr)
        {
            var alchemySpinResult = sr as AlchemyReelsCollapsingSpinResult;
            foreach (var wp in alchemySpinResult.WinPositions)
            {
                var scatter = wp.Line == 0;
                var vcounter = scatter ? sd.ScatterCounter : sd.WinCounter;
                vcounter[wp.Win] = vcounter.ContainsKey(wp.Win) ? vcounter[wp.Win] + 1 : 1;
            }            
        }

        private void DisplayAlchemyReelsSummaryData(int level, decimal bet, int lines, SummaryData summData, DateTime timeStart, decimal maxWin1, List<int> maxIndexPosition)
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
            Console.WriteLine($"MG Collapsing Spin TotalWin         : {summData.CollapsingSpinTotalWin}");
            Console.WriteLine($"Max Win             : {maxWin1}");

            var payoutHitrate = new SortedDictionary<int, TupleRW<int, decimal, decimal, decimal, decimal>>();

            foreach (var w in summData.WinCounter.OrderByDescending(x => x.Key))
            {
                var hits = summData.WinCounter.ContainsKey(w.Key) ? summData.WinCounter[w.Key] : 0;
                var probability = summData.WinCounter.ContainsKey(w.Key) ? (decimal)summData.WinCounter[w.Key] / (decimal)summData.SpinCounter : 0;
                var hitrate = summData.WinCounter.ContainsKey(w.Key) ? (decimal)summData.SpinCounter / (decimal)summData.WinCounter[w.Key] : 0;
                var win = w.Key * summData.SpinCounter * probability;
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
            Console.WriteLine($"Total CollapsingSpin               : {summData.RTPCollapsingSpin,0:P}");
            Console.WriteLine($"Total RTP               : {summData.RTPOverAll,0:P}");
        }

        private void UpdateSummaryDataCollapsing(SummaryData sdata, AlchemyReelsCollapsingSpinResult csr)
        {
            if (csr.IsCollapsingSpin)
            {
                sdata.CollapsingSpinCounter++;
                sdata.CollapsingSpinTotalWin += csr.Win;

                if (csr.CollapsingSpinCount == 1)
                    sdata.CollapsingSpinHitCounter++;
            }
        }

        private decimal TestPayout(string strwheel, decimal betperline, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var alchemyReelsModule = new AlchemyReelsModule(null);
            var requestContext = new RequestContext<SpinArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);

            AlchemyReelsCommon.CreateWheels(new List<int>() { 5, 5, 5 });

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            var arrstr = strwheel.Split(',');
            var arr = Array.ConvertAll(arrstr, int.Parse);

            var ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = AlchemyReelsConfiguration.GameId,
                Level = 1
            };

            var sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = AlchemyReelsConfiguration.BettingLines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            var sr = new AlchemyReelsCollapsingSpinResult()
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = AlchemyReelsConfiguration.BettingLines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(AlchemyReelsConfiguration.Width, AlchemyReelsConfiguration.Height, arr)
            };

            var win = AlchemyReelsCommon.CalculateWin(sr);

            Console.WriteLine("--- WIN POSITION ---");
            foreach (AlcheryReelsWinPosition wp in sr.WinPositions)
                Console.WriteLine(String.Format("[LINE:{0} MUL:{1} WIN:{2}]", wp.Line, wp.Multiplier, wp.Win));

            Console.WriteLine();
            Console.WriteLine("--- WIN TABLE ---");
            foreach (AlchemyReelTableWin tw in sr.TableWins)
                Console.WriteLine(String.Format("[CARD:{0} COUNT:{1} WILD:{2}]", tw.Card, tw.Count, tw.Wild));

            return win;
        }

        [TestCase("8,8,8,8,8,8,4,3,5", 1, ExpectedResult = 1593)]
        public decimal TestAlchemyReelsPayout(string strwheel, decimal betperline)
        {
            return TestCollapsingPayout(strwheel, betperline, MapWheelEncoding[WheelEncoding.Local]);
        }

        private decimal TestCollapsingPayout(string strwheel, decimal betperline, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var alchemyReelsModule = new AlchemyReelsModule(null);
            var maxWin = 0m;
            var totalWin = 0m;
            var maxIndexPosition = new List<int>();
            var requestContext = new RequestContext<SpinArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);
            var summData = new SummaryData();
            AlchemyReelsCommon.CreateWheels(new List<int>() { 3, 3, 3 });

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            string[] arrstr = strwheel.Split(',');
            int[] arr = Array.ConvertAll(arrstr, int.Parse);

            var ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = AlchemyReelsConfiguration.GameId,
                Level = 1
            };

            var sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = AlchemyReelsConfiguration.BettingLines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            var sr = new AlchemyReelsCollapsingSpinResult()
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = AlchemyReelsConfiguration.BettingLines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(AlchemyReelsConfiguration.Width, AlchemyReelsConfiguration.Height, arr)
            };

            sr.TopIndices = new List<int>() { 6, 6, 49 };

            totalWin = AlchemyReelsCommon.CalculateWin(sr);

            Console.WriteLine();
            Console.WriteLine("--- POSITION TABLE ---");
            foreach (AlchemyReelTableWin tw in sr.TableWins)
                Console.WriteLine(String.Format("[WIN:{0} SYM:{1} COUNT:{2}]", tw.Win, tw.Card, tw.Count));

            if (sr.HasBonus)
            {
                var bonusCreated = alchemyReelsModule.CreateBonus(sr);

                var bonus = bonusCreated.Value;

                bonus.SpinTransactionId = sr.TransactionId;
                bonus.GameResult = sr;
                                
                var requestBonusContext = new RequestContext<BonusArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);

                requestBonusContext.Currency = new Currency() { Id = sb.CurrencyId };
                requestBonusContext.Parameters = new BonusArgs() { Bonus = "CollapsingSpin" };
                requestBonusContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

                BonusResult bonusResult;
                int step = bonus.CurrentStep;

                do
                {
                    var entity = new BonusEntity
                    {
                        UserId = ugk.UserId,
                        GameId = AlchemyReelsConfiguration.GameId,
                        Guid = bonus.Guid.ToString("N"),
                        Data = bonus.ToByteArray(),
                        BonusType = bonus.GetType().Name,
                        Version = 2,
                        IsOptional = bonus.IsOptional,
                        IsStarted = bonus.IsStarted,
                        RoundId = sr.RoundId
                    };

                    bonusResult = alchemyReelsModule.ExecuteBonus(AlchemyReelsConfiguration.LevelOne, entity, requestBonusContext).Value;
                    var alchemyFreeCollapsingSpinResult = bonusResult as AlchemyFreeCollapsingSpinResult;

                    var win = alchemyFreeCollapsingSpinResult.Win;

                    if (win > 0)
                    {
                        totalWin += win;
                    }

                    var maxTopIndices = alchemyFreeCollapsingSpinResult.SpinResult.TopIndices.ToList();

                    if (totalWin > maxWin)
                    {
                        maxWin = totalWin;

                        maxIndexPosition = maxTopIndices;
                    }

                    Console.WriteLine("--- POSITION TABLE ---");
                    foreach (AlchemyReelTableWin tw in alchemyFreeCollapsingSpinResult.SpinResult.TableWins)
                        Console.WriteLine(String.Format("[WIN:{0} SYM:{1} COUNT:{2}]", tw.Win, tw.Card, tw.Count));                    

                    bonus = bonusResult.Bonus;
                }
                while (!bonusResult.IsCompleted && bonusResult.Bonus != null);
            }
            
            Console.WriteLine($"Win            : {totalWin}");
            return totalWin;
        }

        private static readonly Dictionary<WheelEncoding, Func<int, int, int[], Wheel>> MapWheelEncoding = new Dictionary<WheelEncoding, Func<int, int, int[], Wheel>>()
        {
            { WheelEncoding.Local, WheelEncodingLocal },
        };

        public static Wheel WheelEncodingLocal(int width, int height, int[] arr)
        {
            int currentIndex = 0;
            var w = new Wheel(new List<int>() { 3, 3, 3 });
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

        protected static List<int> GetRange(List<int> list, int startIndex, int count)
        {
            if (startIndex + count <= list.Count)
            {
                return list.GetRange(startIndex, count);
            }

            var result = list.GetRange(startIndex, list.Count - startIndex);

            result.AddRange(list.GetRange(0, count - (list.Count - startIndex)));

            return result;
        }
    }
}
