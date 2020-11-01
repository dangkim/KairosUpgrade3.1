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
using Slot.Games.FountainOfFortune;
using Slot.Games.FountainOfFortune.Models;

namespace Slot.Simulations
{
    [TestFixture]
    public class FountainOfFortuneSimulation
    {
        [TestCase(FountainOfFortuneConfiguration.GameId, 1, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL1] 100K FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 1, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL1] 1M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 1, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL1] 10M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 1, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL1] 100M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 1, "CNY", 1, 1000000000, 1, 10, 1, TestName = "[LVL1] 1000M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 2, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL2] 100K FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 2, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL2] 1M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 2, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL2] 10M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 2, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL2] 100M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 2, "CNY", 1, 1000000000, 1, 10, 1, TestName = "[LVL2] 1000M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 3, "CNY", 1, 100000, 1, 10, 1, TestName = "[LVL3] 100K FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 3, "CNY", 1, 1000000, 1, 10, 1, TestName = "[LVL3] 1M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 3, "CNY", 1, 10000000, 1, 10, 1, TestName = "[LVL3] 10M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 3, "CNY", 1, 100000000, 1, 10, 1, TestName = "[LVL3] 100M FountainOfFortune Reels")]
        [TestCase(FountainOfFortuneConfiguration.GameId, 3, "CNY", 1, 1000000000, 1, 10, 1, TestName = "[LVL3] 1000M FountainOfFortune Reels")]
        public void TestFountainOfFortune(int gameId, int level, string currencyCode, int numusers, int numItrPerUser, decimal bet, int lines, int mp)
        {
            var sdt = DateTime.Now;

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<FountainOfFortuneModule>();

            var FountainOfFortuneModule = new FountainOfFortuneModule(logger);
            var maxWin = 0m;
            var summData = new SummaryData();
            var users = GenerateUsers(gameId, numusers, level);
            var sbs = GenerateUserBets(users, bet, lines, mp);

            RequestContext<SpinArgs> requestContext = new RequestContext<SpinArgs>("", FountainOfFortuneConfiguration.GameName, PlatformType.Web);

            RequestContext<BonusArgs> requestBonusContext = new RequestContext<BonusArgs>("", FountainOfFortuneConfiguration.GameName, PlatformType.Web);

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
                        var sr = FountainOfFortuneModule.ExecuteSpin(level, new UserGameSpinData(), requestContext).Value as FountainOfFortuneSpinResult;

                        if (sr.Win > maxWin)
                            maxWin = sr.Win;

                        UpdateSummaryData(sdata, sr);
                        UpdateFountainOfFortuneSummaryData(sdata, sr);
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

            DisplayFountainOfFortuneSummaryData(level, bet, lines, summData, sdt, maxWin, null);
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
            var alchemySpinResult = sr as FountainOfFortuneSpinResult;
            sdata.SpinCounter++;
            sdata.TotalBet += alchemySpinResult.SpinBet.TotalBet;
            sdata.TotalWin += alchemySpinResult.Win;
        }

        private void UpdateFountainOfFortuneSummaryData(SummaryData sd, SpinResult sr)
        {
            var alchemySpinResult = sr as FountainOfFortuneSpinResult;
            foreach (var wp in alchemySpinResult.WinPositions)
            {
                var scatter = wp.Line == 0;
                var vcounter = scatter ? sd.ScatterCounter : sd.WinCounter;
                vcounter[wp.Win] = vcounter.ContainsKey(wp.Win) ? vcounter[wp.Win] + 1 : 1;
                //var counter80 = sd.WinCounter.Where(x => x.Key == 80).FirstOrDefault().Value;
            }

            //Console.WriteLine($"Reesl: {AlchemyReelsCommon.Rows[0]}; {AlchemyReelsCommon.Rows[1]}; {AlchemyReelsCommon.Rows[2]}");
        }

        private void DisplayFountainOfFortuneSummaryData(int level, decimal bet, int lines, SummaryData summData, DateTime timeStart, decimal maxWin1, List<int> maxIndexPosition)
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
    }
}
