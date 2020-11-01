using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slot.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Slot.Games.ChessRoyale;
using Slot.Games.ChessRoyale.Models;

namespace Slot.Simulations
{
    [TestFixture]
    public class ChessRoyaleSimulation
    {
        [TestCase(ChessRoyaleConfiguration.GameId, 1, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL1] 100K ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 1, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL1] 1M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 1, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL1] 10M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 1, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL1] 100M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 1, "CNY", 1, 1000000000, 1, 10, 1, TestName = "[LVL1] 1000M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 2, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL2] 100K ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 2, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL2] 1M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 2, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL2] 10M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 2, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL2] 100M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 2, "CNY", 1, 1000000000, 1, 10, 1, TestName = "[LVL2] 1000M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 3, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL3] 100K ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 3, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL3] 1M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 3, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL3] 10M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 3, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL3] 100M ChessRoyale Reels")]
        [TestCase(ChessRoyaleConfiguration.GameId, 3, "CNY", 1, 1000000000, 1, 10, 1, TestName = "[LVL3] 1000M ChessRoyale Reels")]
        public void TestChessRoyale(int gameId, int level, string currencyCode, int numusers, int numItrPerUser, decimal bet, int lines, int mp)
        {
            var sdt = DateTime.Now;

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<ChessRoyaleModule>();

            var chessRoyaleModule = new ChessRoyaleModule(logger);
            var maxWin = 0m;
            //ChessRoyaleCommon.CreateWheels(new List<int>() { 3, 3, 3 });
            //ChessRoyaleCommon.CreatePayLine();
            var summData = new SummaryData();
            var users = GenerateUsers(gameId, numusers, level);
            var sbs = GenerateUserBets(users, bet, lines, mp);

            RequestContext<SpinArgs> requestContext = new RequestContext<SpinArgs>("", ChessRoyaleConfiguration.GameName, PlatformType.Web);

            RequestContext<BonusArgs> requestBonusContext = new RequestContext<BonusArgs>("", ChessRoyaleConfiguration.GameName, PlatformType.Web);

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
                        var sr = chessRoyaleModule.ExecuteSpin(level, new UserGameSpinData(), requestContext).Value as ChessRoyaleSpinResult;

                        if (sr.Win > maxWin)
                            maxWin = sr.Win;

                        UpdateSummaryData(sdata, sr);
                        UpdateChessRoyaleSummaryData(sdata, sr);

                        if (sr.HasBonus)
                        {
                            var bonusCreated = chessRoyaleModule.CreateBonus(sr);

                            var bonus = bonusCreated.Value;

                            bonus.SpinTransactionId = sr.TransactionId;
                            bonus.GameResult = sr;

                            requestBonusContext.Currency = new Currency() { Id = sb.CurrencyId };
                            requestBonusContext.Parameters = new BonusArgs() { Bonus = "FreeSpin" };
                            requestBonusContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

                            BonusResult bonusResult;
                            int step = bonus.CurrentStep;
                            do
                            {
                                var entity = new BonusEntity
                                {
                                    UserId = key.UserId,
                                    GameId = ChessRoyaleConfiguration.GameId,
                                    Guid = bonus.Guid.ToString("N"),
                                    Data = bonus.ToByteArray(),
                                    BonusType = bonus.GetType().Name,
                                    Version = 2,
                                    IsOptional = bonus.IsOptional,
                                    IsStarted = bonus.IsStarted,
                                    RoundId = sr.RoundId
                                };

                                //bonusEntity = bonusEntityCached.FromByteArray<BonusEntity>();

                                bonusResult = chessRoyaleModule.ExecuteBonus(level, entity, requestBonusContext).Value;
                                var chessRoyaleFreeSpinResult = bonusResult.Bonus.GameResult as ChessRoyaleFreeSpinResult;

                                UpdateSummaryDataFS(sdata, chessRoyaleFreeSpinResult);

                                step = bonusResult.Step + 1;

                                if (bonusResult.ErrorCode != 0)
                                {
                                    Console.WriteLine(String.Format("BonusError Code: {0}", bonusResult.ErrorCode));
                                }

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

            DisplayChessRoyaleSummaryData(level, bet, lines, summData, sdt, maxWin, null);
        }

        [TestCase("7,1,5,9,2,4,3,4,5,1,5,0,9,1,3", 1, ExpectedResult = 5)]
        public decimal TestChessRoyalePayout(string strwheel, decimal betperline)
        {
            //return TestPayout(strwheel, betperline, MapWheelEncoding[WheelEncoding.Local]);
            return TestPayout(strwheel, betperline, MapWheelEncoding[WheelEncoding.Local]);
        }

        private decimal TestPayout(string strwheel, decimal betperline, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var sdt = DateTime.Now;

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<ChessRoyaleModule>();

            var chessRoyaleModule = new ChessRoyaleModule(logger);

            //var maxWin = 0m;
            var totalWin = 0m;
            List<int> maxIndexPosition = new List<int>();
            RequestContext<SpinArgs> requestContext = new RequestContext<SpinArgs>("", ChessRoyaleConfiguration.GameName, PlatformType.Web);
            var summData = new SummaryData();
            //ChessRoyaleCommon.CreateWheels(new List<int>() { 3, 3, 3, 3, 3 });

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            string[] arrstr = strwheel.Split(',');
            int[] arr = Array.ConvertAll(arrstr, int.Parse);

            UserGameKey ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = ChessRoyaleConfiguration.GameId,
                Level = 1
            };

            SpinBet sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = ChessRoyaleConfiguration.Lines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            ChessRoyaleSpinResult sr = new ChessRoyaleSpinResult(ugk)
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = ChessRoyaleConfiguration.Lines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(ChessRoyaleConfiguration.Width, ChessRoyaleConfiguration.Height, arr)
            };


            totalWin = ChessRoyaleCommon.CalculateWin(sr, 1);

            if (sr.HasBonus)
            {
                var bonusCreated = chessRoyaleModule.CreateBonus(sr);

                var bonus = bonusCreated.Value;

                bonus.SpinTransactionId = sr.TransactionId;
                bonus.GameResult = sr;

                //RequestContext<SpinArgs> requestContext = new RequestContext<SpinArgs>("", AlchemyReelsConfiguration.GameName, PlatformType.Web);
                RequestContext<BonusArgs> requestBonusContext = new RequestContext<BonusArgs>("", ChessRoyaleConfiguration.GameName, PlatformType.Web);

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
                        GameId = ChessRoyaleConfiguration.GameId,
                        Guid = bonus.Guid.ToString("N"),
                        Data = bonus.ToByteArray(),
                        BonusType = bonus.GetType().Name,
                        Version = 2,
                        IsOptional = bonus.IsOptional,
                        IsStarted = bonus.IsStarted,
                        RoundId = sr.RoundId
                    };

                    bonusResult = chessRoyaleModule.ExecuteBonus(ChessRoyaleConfiguration.LevelOne, entity, requestBonusContext).Value;
                    var chessRoyaleFreeSpinResult = bonusResult as ChessRoyaleFreeSpinResult;

                    var win = chessRoyaleFreeSpinResult.Win;

                    if (win > 0)
                    {
                        totalWin += win;
                    }

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
            Wheel w = new Wheel(new List<int>() { 3, 3, 3, 3, 3 });
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

        private void UpdateSummaryData(SummaryData sdata, SpinResult sr)
        {
            var alchemySpinResult = sr as ChessRoyaleSpinResult;
            sdata.SpinCounter++;
            sdata.TotalBet += alchemySpinResult.SpinBet.TotalBet;
            sdata.TotalWin += alchemySpinResult.Win;
        }

        private void UpdateChessRoyaleSummaryData(SummaryData sd, SpinResult sr)
        {
            var alchemySpinResult = sr as ChessRoyaleSpinResult;
            foreach (var wp in alchemySpinResult.WinPositions)
            {
                var scatter = wp.Line == 0;
                var vcounter = scatter ? sd.ScatterCounter : sd.WinCounter;
                vcounter[wp.Win] = vcounter.ContainsKey(wp.Win) ? vcounter[wp.Win] + 1 : 1;
                //var counter80 = sd.WinCounter.Where(x => x.Key == 80).FirstOrDefault().Value;
            }

            //Console.WriteLine($"Reesl: {AlchemyReelsCommon.Rows[0]}; {AlchemyReelsCommon.Rows[1]}; {AlchemyReelsCommon.Rows[2]}");
        }

        private void DisplayChessRoyaleSummaryData(int level, decimal bet, int lines, SummaryData summData, DateTime timeStart, decimal maxWin1, List<int> maxIndexPosition)
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
            Console.WriteLine($"FG TotalWin         : {summData.FSTotalWin}");
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
            Console.WriteLine($"FG Spin Count           : {summData.FSCounter}");
            Console.WriteLine($"-----------------------------------------------------------");
            Console.WriteLine($"MG Feature Hit Counter     : {summData.FSHitCounter,0:0.000}");
            Console.WriteLine($"MG Feature Hit Rate     : {summData.FSHitRate,0:0.000}");

            Console.WriteLine($"--- HIT RATE ----------------------------------------------");
            foreach (var payout in payoutHitrate.OrderByDescending(p => p.Key))
            {
                Console.WriteLine($"{payout.Key,-10} \t{payout.Value.Item2,10:0.00} \t{payout.Value.Item1,10} \t{payout.Value.Item3,10:0.000} \t{payout.Value.Item4,10:0.000} \t{payout.Value.Item5,10:0.000}");
            }

            Console.WriteLine($"-----------------------------------------------------------");
            Console.WriteLine($"MG RTP                  : {summData.RTPSpin,0:P}");
            Console.WriteLine($"FG RTP                  : {summData.RTPFreeSpin,0:P}");
            Console.WriteLine($"-----------------------------------------------------------");
            Console.WriteLine($"Total RTP               : {summData.RTPOverAll,0:P}");
        }

        protected static void UpdateSummaryDataFS(SummaryData sdata, ChessRoyaleFreeSpinResult fsr)
        {
            sdata.FSCounter++;
            sdata.FSTotalWin += fsr.Win;
            if (fsr.Step == 1) sdata.FSHitCounter++;

            if (fsr.SpinResult.SimulationDatas != null && fsr.SpinResult.SimulationDatas.ContainsKey("additionalFreeSpin"))
            {
                sdata.FSRetriggerHitCounter++;
                sdata.FSRetriggerCounter += Convert.ToInt32(fsr.SpinResult.SimulationDatas["additionalFreeSpin"]);
            }
        }
    }
}
