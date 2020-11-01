using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.PandaWarrior;
using Slot.Games.PandaWarrior.Models;
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
using Slot.Games.PandaWarrior.BonusFeatures;

namespace Slot.Simulations
{
    [TestFixture]
    public class PandaWarriorSimulation
    {
        [TestCase(PandaWarriorConfiguration.GameId, 1, "CNY", 1, 100000, 1, 30, 1, TestName = "[LVL1] 100K PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 1, "CNY", 1, 1000000, 1, 30, 1, TestName = "[LVL1] 1M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 1, "CNY", 1, 10000000, 1, 30, 1, TestName = "[LVL1] 10M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 1, "CNY", 1, 100000000, 1, 30, 1, TestName = "[LVL1] 100M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 1, "CNY", 1, 1000, 1, 30, 1, TestName = "[LVL1] 1K PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 1, "CNY", 1, 10, 1, 30, 1, TestName = "[LVL1] 10 PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 2, "CNY", 1, 100000, 1, 30, 1, TestName = "[LVL2] 100K PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 2, "CNY", 1, 1000000, 1, 30, 1, TestName = "[LVL2] 1M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 2, "CNY", 1, 10000000, 1, 30, 1, TestName = "[LVL2] 10M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 2, "CNY", 1, 100000000, 1, 30, 1, TestName = "[LVL2] 100M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 2, "CNY", 1, 1000, 1, 30, 1, TestName = "[LVL2] 1K PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 2, "CNY", 1, 10, 1, 30, 1, TestName = "[LVL2] 10 PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 3, "CNY", 1, 100000, 1, 30, 1, TestName = "[LVL3] 100K PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 3, "CNY", 1, 1000000, 1, 30, 1, TestName = "[LVL3] 1M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 3, "CNY", 1, 10000000, 1, 30, 1, TestName = "[LVL3] 10M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 3, "CNY", 1, 100000000, 1, 30, 1, TestName = "[LVL3] 100M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 3, "CNY", 1, 1000, 1, 30, 1, TestName = "[LVL3] 1K PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 3, "CNY", 1, 10, 1, 30, 1, TestName = "[LVL3] 10 PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 4, "CNY", 1, 100000, 1, 30, 1, TestName = "[LVL4] 100K PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 4, "CNY", 1, 1000000, 1, 30, 1, TestName = "[LVL4] 1M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 4, "CNY", 1, 10000000, 1, 30, 1, TestName = "[LVL4] 10M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 4, "CNY", 1, 100000000, 1, 30, 1, TestName = "[LVL4] 100M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 4, "CNY", 1, 1000, 1, 30, 1, TestName = "[LVL4] 1K PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 4, "CNY", 1, 10, 1, 30, 1, TestName = "[LVL4] 10 PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 5, "CNY", 1, 100000, 1, 30, 1, TestName = "[LVL5] 100K PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 5, "CNY", 1, 1000000, 1, 30, 1, TestName = "[LVL5] 1M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 5, "CNY", 1, 10000000, 1, 30, 1, TestName = "[LVL5] 10M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 5, "CNY", 1, 100000000, 1, 30, 1, TestName = "[LVL5] 100M PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 5, "CNY", 1, 1000, 1, 30, 1, TestName = "[LVL5] 1K PandaWarrior")]
        [TestCase(PandaWarriorConfiguration.GameId, 5, "CNY", 1, 10, 1, 30, 1, TestName = "[LVL5] 10 PandaWarrior")]
        public void TestPandaWarrior(int gameId, int level, string currencyCode, int numusers, int numItrPerUser, decimal bet, int lines, int mp)
        {
            var sdt = DateTime.Now;

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<PandaWarriorModule>();

            var PandaWarriorModule = new PandaWarriorModule(logger);
            var maxWin = 0m;
            var profile = new Dictionary<int, int>();
            var profileFreeSpin = new Dictionary<decimal, int>();
            var summData = new SummaryData();
            var users = GenerateUsers(gameId, numusers, level);
            var sbs = GenerateUserBets(users, bet, lines, mp);

            var requestContext = new RequestContext<SpinArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web);

            var requestBonusContext = new RequestContext<BonusArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web);

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
                        var sr = PandaWarriorModule.ExecuteSpin(level, new UserGameSpinData(), requestContext).Value as PandaWarriorCollapsingSpinResult;

                        if (sr.Win > maxWin)
                            maxWin = sr.Win;

                        UpdateSummaryData(sdata, sr);

                        if (sr.HasBonus)
                        {
                            var bonusCreated = PandaWarriorModule.CreateBonus(sr);

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
                                    GameId = PandaWarriorConfiguration.GameId,
                                    Guid = bonus.Guid.ToString("N"),
                                    Data = bonus.ToByteArray(),
                                    BonusType = bonus.GetType().Name,
                                    Version = 2,
                                    IsOptional = bonus.IsOptional,
                                    IsStarted = bonus.IsStarted,
                                    RoundId = sr.RoundId
                                };

                                bonusResult = PandaWarriorModule.ExecuteBonus(level, entity, requestBonusContext).Value;
                                if (bonusResult is PandaWarriorFreeSpinResult)
                                {
                                    var PandaWarriorFreeSpinResult = bonusResult as PandaWarriorFreeSpinResult;
                                    UpdateSummaryDataFS(sdata, PandaWarriorFreeSpinResult);
                                }
                                else
                                {
                                    var PandaWarriorCollapsingBonusResult = bonusResult as PandaWarriorCollapsingBonusResult;

                                    if (!PandaWarriorCollapsingBonusResult.SpinResult.IsFreeSpin && PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount > 0)
                                    {
                                        UpdateSummaryMainGameDataCollapsing(sdata, PandaWarriorCollapsingBonusResult.SpinResult);

                                        var csr = PandaWarriorCollapsingBonusResult.SpinResult;

                                        if (bonusResult.IsCompleted && csr.MainGameCollapsingSpinCount > 0)
                                        {
                                            if (!profile.ContainsKey(PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount))
                                            {
                                                profile.Add(PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount, 1);
                                            }
                                            else
                                            {
                                                profile.TryGetValue(PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount, out int count);
                                                profile.Remove(PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount);
                                                profile.Add(PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount, count + 1);
                                            }
                                        }
                                    }

                                    if (PandaWarriorCollapsingBonusResult.SpinResult.IsFreeSpin && !PandaWarriorCollapsingBonusResult.SpinResult.IsBackToInitial)
                                    {
                                        UpdateSummaryFreeSpinDataCollapsing(sdata, PandaWarriorCollapsingBonusResult.SpinResult);

                                        if (!PandaWarriorCollapsingBonusResult.SpinResult.Collapse
                                            && PandaWarriorCollapsingBonusResult.SpinResult.SimulationDatas.Count == 0
                                            && PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount > 0)
                                        {
                                            if (!profileFreeSpin.ContainsKey(PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount))
                                            {
                                                profileFreeSpin.Add(PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount, 1);
                                            }
                                            else
                                            {
                                                profileFreeSpin.TryGetValue(PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount, out int count);
                                                profileFreeSpin.Remove(PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount);
                                                profileFreeSpin.Add(PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount, count + 1);
                                            }
                                        }
                                    }
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

            if (profile != null)
            {
                foreach (var item in profile.OrderByDescending(p => p.Key))
                {
                    Console.WriteLine($"Collapsing            : {item.Key}; {item.Value}");
                }

            }

            if (profileFreeSpin != null)
            {
                foreach (var item in profileFreeSpin.OrderByDescending(p => p.Key))
                {
                    Console.WriteLine($"Free Spin Collapsing            : {item.Key}; {item.Value}");
                }

            }

            //DisplayPandaWarriorSummaryData(level, bet, lines, summData, sdt, maxWin, null);
            decimal nonScatterRtp = 0;
            foreach (var w in summData.WinCounter.OrderByDescending(x => x.Key))
            {
                var hitrate = summData.GetWinRate(w.Key, summData.SpinCounter);
                var hreturn = w.Key * summData.SpinCounter * hitrate;
                var hrtp = hreturn / summData.TotalBet;
                //var payout = (int)(w.Key / bet);

                nonScatterRtp += hrtp;
            }

            decimal scatterRtp = 0;
            foreach (var s in summData.ScatterCounter.OrderByDescending(x => x.Key))
            {
                var hitrate = summData.GetScatterRate(s.Key, summData.SpinCounter);
                var hreturn = s.Key * summData.SpinCounter * hitrate;
                var hrtp = hreturn / summData.TotalBet;
                //var payout = (int)(((s.Key / bet)) / lines);

                scatterRtp += hrtp;
            }

            decimal nonCollapsingScatterRtp = 0;
            foreach (var w in summData.CollapsingWinCounter.OrderByDescending(x => x.Key))
            {
                var hitrate = summData.GetCollapsingWinRate(w.Key, summData.CollapsingSpinCounter);
                var hreturn = w.Key * summData.CollapsingSpinCounter * hitrate;
                var hrtp = hreturn / summData.TotalBet;
                //var payout = (int)(w.Key / bet);

                nonCollapsingScatterRtp += hrtp;
            }

            decimal collapsingScatterRtp = 0;
            foreach (var s in summData.CollapsingScatterCounter.OrderByDescending(x => x.Key))
            {
                var hitrate = summData.GetCollapsingScatterRate(s.Key, summData.CollapsingSpinCounter);
                var hreturn = s.Key * summData.CollapsingSpinCounter * hitrate;
                var hrtp = hreturn / summData.TotalBet;
                //var payout = (int)(((s.Key / bet)) / lines);

                collapsingScatterRtp += hrtp;
            }

            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine(String.Format("Test.SpinCount                     : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("Test.FreeSpinHitCount              : {0}", summData.FSHitCounter));
            Console.WriteLine(String.Format("Test.FreeSpinRetriggerHitCount     : {0}", summData.FSRetriggerHitCounter));
            Console.WriteLine(String.Format("Test.FreeSpinCount                 : {0}", summData.FSCounter));
            Console.WriteLine(String.Format("Test.FreeSpinCountTumble           : {0}", summData.FSCollapsingSpinCounter));
            Console.WriteLine(String.Format("Test.Level             : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart         : {0} {1}", sdt.ToShortDateString(), sdt.ToShortTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd           : {0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()));
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine(String.Format("TotalBet               : {0,8}", summData.TotalBet));
            Console.WriteLine(String.Format("TotalWin.Spin          : {0,8}", summData.TotalWin));
            Console.WriteLine(String.Format("TotalWin.Tumble        : {0,8}", summData.CollapsingSpinTotalWin));
            Console.WriteLine(String.Format("TotalWin.FreeSpin      : {0,8}", summData.FSTotalWin));
            Console.WriteLine(String.Format("TotalWin.FreeSpinTumble: {0,8}", summData.FSCollapsingSpinTotalWin));
            Console.WriteLine("--- HIT RATE ----------------------------------------------");
            Console.WriteLine(String.Format("HitRate.FreeSpin           : {0,8:0.00}", summData.FSHitRate));
            Console.WriteLine(String.Format("HitRate.RetriggerFreeSpin  : {0,8:0.00}", summData.FSRetriggerHitRate));
            Console.WriteLine("--- FEATURE RTP -------------------------------------------");
            Console.WriteLine(String.Format("Non-Scatter            : {0,8:P}", nonScatterRtp));
            Console.WriteLine(String.Format("Scatter                : {0,8:P}", scatterRtp));
            Console.WriteLine(String.Format("Tumble Non-Scatter     : {0,8:P}", nonCollapsingScatterRtp));
            Console.WriteLine(String.Format("Tumble Scatter         : {0,8:P}", collapsingScatterRtp));
            Console.WriteLine("--- RTP ---------------------------------------------------");
            Console.WriteLine(String.Format("RTP.Spin               : {0,8:P}", summData.RTPSpin));
            Console.WriteLine(String.Format("RTP.Tumble             : {0,8:P}", summData.RTPCollapsingSpin));
            Console.WriteLine(String.Format("RTP.FreeSpin           : {0,8:P}", summData.RTPFreeSpin));
            Console.WriteLine(String.Format("RTP.FreeSpinTumble     : {0,8:P}", summData.RTPFSCollapsingSpin));
            Console.WriteLine(String.Format("RTP.Total              : {0,8:P}", summData.RTPOverAll));
        }

        //[TestCase(1, TestName = "[LVL1] Full Cycle")]
        public void TestPandaWarriorFullCycle(int level, List<int> rows)
        {
            var sdt = DateTime.Now;
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<PandaWarriorModule>();

            var PandaWarriorModule = new PandaWarriorModule(logger);

            var profile = new Dictionary<decimal, int>();
            var profileFreeSpin = new Dictionary<decimal, int>();

            var maxIndexPosition = new List<int>();

            var summData = new SummaryData();

            for (var reel0 = 0; reel0 < PandaWarriorConfiguration.Wheels[level].Reels[0].Count; reel0++)
            {
                for (var reel1 = 0; reel1 < PandaWarriorConfiguration.Wheels[level].Reels[1].Count; reel1++)
                {
                    for (var reel2 = 0; reel2 < PandaWarriorConfiguration.Wheels[level].Reels[2].Count; reel2++)
                    {
                        for (var reel3 = 0; reel3 < PandaWarriorConfiguration.Wheels[level].Reels[3].Count; reel3++)
                        {
                            for (var reel4 = 0; reel4 < PandaWarriorConfiguration.Wheels[level].Reels[4].Count; reel4++)
                            {
                                var totalWin = (decimal)0;

                                var ugk = new UserGameKey()
                                {
                                    UserId = -1,
                                    GameId = PandaWarriorConfiguration.GameId,
                                    Level = level
                                };

                                var sb = new SpinBet(ugk, PlatformType.None)
                                {
                                    LineBet = 1,
                                    Credits = 0,
                                    Lines = PandaWarriorConfiguration.Lines,
                                    Multiplier = 1
                                };

                                var requestContext = new RequestContext<SpinArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web);
                                var requestBonusContext = new RequestContext<BonusArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web);
                                requestContext.Currency = new Currency() { Id = sb.CurrencyId };
                                requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
                                requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

                                var wheel = new Wheel(rows);
                                wheel.Reels[0] = GetRange(PandaWarriorConfiguration.Wheels[level].Reels[0], reel0, wheel.Rows[0]);
                                wheel.Reels[1] = GetRange(PandaWarriorConfiguration.Wheels[level].Reels[1], reel1, wheel.Rows[1]);
                                wheel.Reels[2] = GetRange(PandaWarriorConfiguration.Wheels[level].Reels[2], reel2, wheel.Rows[2]);
                                wheel.Reels[3] = GetRange(PandaWarriorConfiguration.Wheels[level].Reels[3], reel3, wheel.Rows[3]);
                                wheel.Reels[4] = GetRange(PandaWarriorConfiguration.Wheels[level].Reels[4], reel4, wheel.Rows[4]);

                                var sr = new PandaWarriorCollapsingSpinResult()
                                {
                                    SpinBet = new SpinBet(ugk, PlatformType.None)
                                    {
                                        Lines = PandaWarriorConfiguration.Lines,
                                        Multiplier = 1,
                                        LineBet = 1m
                                    },
                                    IsFreeSpin = false,
                                    Level = level,
                                    Wheel = wheel
                                };

                                sr.Wheel = wheel;
                                sr.TopIndices = new List<int>() { reel0, reel1, reel2, reel3, reel4 };

                                PandaWarriorCommon.CalculateWin(sr);

                                totalWin = sr.Win;

                                sr.InitialWheel = sr.Wheel.Copy();
                                sr.InitialTopIndices = sr.TopIndices.ToList();
                                sr.InitialWinPositions = sr.WinPositions.ToList();

                                if (PandaWarriorCommon.CheckFreeSpin(sr.Wheel))
                                {
                                    sr.IsBonus = true;
                                    sr.IsFreeSpin = true;
                                    sr.InitialBonusPositions = PandaWarriorFreeSpinFeature.CreatePosition(sr);
                                    sr.CurrentStep = 1;
                                    sr.CurrentFreeSpinCounter = PandaWarriorConfiguration.FreeSpinCount;
                                    sr.NumOfFreeSpin = PandaWarriorConfiguration.FreeSpinCount;
                                }
                                else if (sr.Collapse)
                                {
                                    sr.IsBonus = true;
                                }

                                UpdateSummaryData(summData, sr);

                                if (sr.HasBonus)
                                {
                                    var bonusCreated = PandaWarriorModule.CreateBonus(sr);

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
                                            GameId = PandaWarriorConfiguration.GameId,
                                            Guid = bonus.Guid.ToString("N"),
                                            Data = bonus.ToByteArray(),
                                            BonusType = bonus.GetType().Name,
                                            Version = 2,
                                            IsOptional = bonus.IsOptional,
                                            IsStarted = bonus.IsStarted,
                                            RoundId = sr.RoundId
                                        };

                                        bonusResult = PandaWarriorModule.ExecuteBonus(level, entity, requestBonusContext).Value;
                                        if (bonusResult is PandaWarriorFreeSpinResult)
                                        {
                                            var PandaWarriorFreeSpinResult = bonusResult as PandaWarriorFreeSpinResult;
                                            UpdateSummaryDataFS(summData, PandaWarriorFreeSpinResult);
                                        }
                                        else
                                        {
                                            var PandaWarriorCollapsingBonusResult = bonusResult as PandaWarriorCollapsingBonusResult;

                                            if (!PandaWarriorCollapsingBonusResult.SpinResult.IsFreeSpin && PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount > 0)
                                            {
                                                UpdateSummaryMainGameDataCollapsing(summData, PandaWarriorCollapsingBonusResult.SpinResult);

                                                var csr = PandaWarriorCollapsingBonusResult.SpinResult;

                                                if (bonusResult.IsCompleted && csr.MainGameCollapsingSpinCount > 0)
                                                {
                                                    if (!profile.ContainsKey(PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount))
                                                    {
                                                        profile.Add(PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount, 1);
                                                    }
                                                    else
                                                    {
                                                        profile.TryGetValue(PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount, out int count);
                                                        profile.Remove(PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount);
                                                        profile.Add(PandaWarriorCollapsingBonusResult.SpinResult.MainGameCollapsingSpinCount, count + 1);
                                                    }
                                                }
                                            }

                                            if (PandaWarriorCollapsingBonusResult.SpinResult.IsFreeSpin && !PandaWarriorCollapsingBonusResult.SpinResult.IsBackToInitial)
                                            {
                                                UpdateSummaryFreeSpinDataCollapsing(summData, PandaWarriorCollapsingBonusResult.SpinResult);

                                                if (!PandaWarriorCollapsingBonusResult.SpinResult.Collapse
                                                    && PandaWarriorCollapsingBonusResult.SpinResult.SimulationDatas.Count == 0
                                                    && PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount > 0)
                                                {
                                                    if (!profileFreeSpin.ContainsKey(PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount))
                                                    {
                                                        profileFreeSpin.Add(PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount, 1);
                                                    }
                                                    else
                                                    {
                                                        profileFreeSpin.TryGetValue(PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount, out int count);
                                                        profileFreeSpin.Remove(PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount);
                                                        profileFreeSpin.Add(PandaWarriorCollapsingBonusResult.SpinResult.CollapsingSpinCount, count + 1);
                                                    }
                                                }
                                            }
                                        }

                                        bonus = bonusResult.Bonus;
                                    }
                                    while (!bonusResult.IsCompleted && bonusResult.Bonus != null);
                                }
                            }
                        }
                    }
                }
            }

            if (profile != null)
            {
                foreach (var item in profile.OrderByDescending(p => p.Key))
                {
                    Console.WriteLine($"Collapsing            : {item.Key}; {item.Value}");
                }

            }

            if (profileFreeSpin != null)
            {
                foreach (var item in profileFreeSpin.OrderByDescending(p => p.Key))
                {
                    Console.WriteLine($"FreeSpin Collapsing            : {item.Key}; {item.Value}");
                }

            }

            decimal nonScatterRtp = 0;
            foreach (var w in summData.WinCounter.OrderByDescending(x => x.Key))
            {
                var hitrate = summData.GetWinRate(w.Key, summData.SpinCounter);
                var hreturn = w.Key * summData.SpinCounter * hitrate;
                var hrtp = hreturn / summData.TotalBet;
                //var payout = (int)(w.Key / bet);

                nonScatterRtp += hrtp;
            }

            decimal scatterRtp = 0;
            foreach (var s in summData.ScatterCounter.OrderByDescending(x => x.Key))
            {
                var hitrate = summData.GetScatterRate(s.Key, summData.SpinCounter);
                var hreturn = s.Key * summData.SpinCounter * hitrate;
                var hrtp = hreturn / summData.TotalBet;
                //var payout = (int)(((s.Key / bet)) / lines);

                scatterRtp += hrtp;
            }

            decimal nonCollapsingScatterRtp = 0;
            foreach (var w in summData.CollapsingWinCounter.OrderByDescending(x => x.Key))
            {
                var hitrate = summData.GetCollapsingWinRate(w.Key, summData.CollapsingSpinCounter);
                var hreturn = w.Key * summData.CollapsingSpinCounter * hitrate;
                var hrtp = hreturn / summData.TotalBet;
                //var payout = (int)(w.Key / bet);

                nonCollapsingScatterRtp += hrtp;
            }

            decimal collapsingScatterRtp = 0;
            foreach (var s in summData.CollapsingScatterCounter.OrderByDescending(x => x.Key))
            {
                var hitrate = summData.GetCollapsingScatterRate(s.Key, summData.CollapsingSpinCounter);
                var hreturn = s.Key * summData.CollapsingSpinCounter * hitrate;
                var hrtp = hreturn / summData.TotalBet;
                //var payout = (int)(((s.Key / bet)) / lines);

                collapsingScatterRtp += hrtp;
            }

            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine(String.Format("Test.SpinCount                     : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("Test.FreeSpinHitCount              : {0}", summData.FSHitCounter));
            Console.WriteLine(String.Format("Test.FreeSpinRetriggerHitCount     : {0}", summData.FSRetriggerHitCounter));
            Console.WriteLine(String.Format("Test.FreeSpinCount                 : {0}", summData.FSCounter));
            Console.WriteLine(String.Format("Test.FreeSpinCountTumble           : {0}", summData.FSCollapsingSpinCounter));
            Console.WriteLine(String.Format("Test.Level             : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart         : {0} {1}", sdt.ToShortDateString(), sdt.ToShortTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd           : {0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()));
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine(String.Format("TotalBet               : {0,8}", summData.TotalBet));
            Console.WriteLine(String.Format("TotalWin.Spin          : {0,8}", summData.TotalWin));
            Console.WriteLine(String.Format("TotalWin.Tumble        : {0,8}", summData.CollapsingSpinTotalWin));
            Console.WriteLine(String.Format("TotalWin.FreeSpin      : {0,8}", summData.FSTotalWin));
            Console.WriteLine(String.Format("TotalWin.FreeSpinTumble: {0,8}", summData.FSCollapsingSpinTotalWin));
            Console.WriteLine("--- HIT RATE ----------------------------------------------");
            Console.WriteLine(String.Format("HitRate.FreeSpin           : {0,8:0.00}", summData.FSHitRate));
            Console.WriteLine(String.Format("HitRate.RetriggerFreeSpin  : {0,8:0.00}", summData.FSRetriggerHitRate));
            Console.WriteLine("--- FEATURE RTP -------------------------------------------");
            Console.WriteLine(String.Format("Non-Scatter            : {0,8:P}", nonScatterRtp));
            Console.WriteLine(String.Format("Scatter                : {0,8:P}", scatterRtp));
            Console.WriteLine(String.Format("Tumble Non-Scatter     : {0,8:P}", nonCollapsingScatterRtp));
            Console.WriteLine(String.Format("Tumble Scatter         : {0,8:P}", collapsingScatterRtp));
            Console.WriteLine("--- RTP ---------------------------------------------------");
            Console.WriteLine(String.Format("RTP.Spin               : {0,8:P}", summData.RTPSpin));
            Console.WriteLine(String.Format("RTP.Tumble             : {0,8:P}", summData.RTPCollapsingSpin));
            Console.WriteLine(String.Format("RTP.FreeSpin           : {0,8:P}", summData.RTPFreeSpin));
            Console.WriteLine(String.Format("RTP.FreeSpinTumble     : {0,8:P}", summData.RTPFSCollapsingSpin));
            Console.WriteLine(String.Format("RTP.Total              : {0,8:P}", summData.RTPOverAll));
        }

        [TestCase(1, TestName = "[LVL1] Full Cycle")]
        [TestCase(2, TestName = "[LVL2] Full Cycle")]
        [TestCase(3, TestName = "[LVL3] Full Cycle")]
        [TestCase(4, TestName = "[LVL4] Full Cycle")]
        [TestCase(5, TestName = "[LVL5] Full Cycle")]
        public void Test333PandaWarriorFullCycle(int level)
        {
            TestPandaWarriorFullCycle(level, new List<int>() { 3, 3, 3, 3, 3 });
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
            var PandaWarriorCollapsingSpinResult = sr as PandaWarriorCollapsingSpinResult;
            sdata.SpinCounter++;
            sdata.TotalBet += PandaWarriorCollapsingSpinResult.SpinBet.TotalBet;
            sdata.TotalWin += PandaWarriorCollapsingSpinResult.Win;
        }

        private void UpdatePandaWarriorSummaryData(SummaryData sd, PandaWarriorCollapsingSpinResult csr)
        {
            //var PandaWarriorCollapsingSpinResult = csr as PandaWarriorCollapsingSpinResult;
            foreach (var wp in csr.WinPositions)
            {
                var scatter = wp.Line == 0;
                var vcounter = scatter ? (csr.Collapse ? sd.CollapsingScatterCounter : sd.ScatterCounter) : (csr.Collapse ? sd.CollapsingWinCounter : sd.WinCounter);
                vcounter[wp.Win] = vcounter.ContainsKey(wp.Win) ? vcounter[wp.Win] + 1 : 1;
            }
        }

        private void DisplayPandaWarriorSummaryData(int level, decimal bet, int lines, SummaryData summData, DateTime timeStart, decimal maxWin1, List<int> maxIndexPosition)
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

        private void UpdateSummaryMainGameDataCollapsing(SummaryData sdata, PandaWarriorCollapsingSpinResult csr)
        {
            //if (csr.Collapse || csr.MainGameCollapsingSpinCount > 0)
            //{
            sdata.CollapsingSpinCounter++;
            sdata.CollapsingSpinTotalWin += csr.Win;

            if (csr.MainGameCollapsingSpinCount == 1)
                sdata.CollapsingSpinHitCounter++;
            //}
        }

        protected static void UpdateSummaryDataFS(SummaryData sdata, PandaWarriorFreeSpinResult fsr)
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

        protected static void UpdateSummaryFreeSpinDataCollapsing(SummaryData sdata, PandaWarriorCollapsingSpinResult csr)
        {
            if (csr == null)
                return;
            sdata.FSCollapsingSpinCounter++;
            sdata.FSCollapsingSpinTotalWin += csr.Win;

            if (csr.SimulationDatas.ContainsKey("additionalFreeSpin"))
            {
                sdata.FSRetriggerHitCounter++;
                sdata.FSRetriggerCounter += Convert.ToInt32(csr.SimulationDatas["additionalFreeSpin"]);
            }
        }

        private decimal TestPayout(string strwheel, decimal betperline, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var PandaWarriorModule = new PandaWarriorModule(null);
            var requestContext = new RequestContext<SpinArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web);

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            var arrstr = strwheel.Split(',');
            var arr = Array.ConvertAll(arrstr, int.Parse);

            var ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = PandaWarriorConfiguration.GameId,
                Level = 1
            };

            var sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = PandaWarriorConfiguration.Lines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            var sr = new PandaWarriorCollapsingSpinResult()
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = PandaWarriorConfiguration.Lines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(PandaWarriorConfiguration.Width, PandaWarriorConfiguration.Height, arr)
            };

            var win = PandaWarriorCommon.CalculateWin(sr);

            Console.WriteLine("--- WIN POSITION ---");
            foreach (PandaWarriorWinPosition wp in sr.WinPositions)
                Console.WriteLine(String.Format("[LINE:{0} MUL:{1} WIN:{2}]", wp.Line, wp.Multiplier, wp.Win));

            Console.WriteLine();
            Console.WriteLine("--- WIN TABLE ---");
            foreach (PandaWarriorTableWin tw in sr.TableWins)
                Console.WriteLine(String.Format("[CARD:{0} COUNT:{1} WILD:{2}]", tw.Card, tw.Count, tw.Wild));

            return win;
        }

        [TestCase("8,3,7,7,1,0,7,3,1,0,7,4,0,7,2", 0.1, ExpectedResult = 2.5)]
        public decimal TestPandaWarriorPayout(string strwheel, decimal betperline)
        {
            return TestCollapsingPayout(strwheel, betperline, MapWheelEncoding[WheelEncoding.Local]);
        }

        private decimal TestCollapsingPayout(string strwheel, decimal betperline, Func<int, int, int[], Wheel> wheelEncoding)
        {
            var PandaWarriorModule = new PandaWarriorModule(null);
            var maxWin = 0m;
            var totalWin = 0m;
            var maxIndexPosition = new List<int>();
            var requestContext = new RequestContext<SpinArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web);
            var summData = new SummaryData();

            Assert.That(strwheel, Is.Not.Null.Or.Empty);

            string[] arrstr = strwheel.Split(',');
            int[] arr = Array.ConvertAll(arrstr, int.Parse);

            var ugk = new UserGameKey()
            {
                UserId = -1,
                GameId = PandaWarriorConfiguration.GameId,
                Level = 1
            };

            var sb = new SpinBet(ugk, PlatformType.None)
            {
                LineBet = 1,
                Credits = 0,
                Lines = PandaWarriorConfiguration.Lines,
                Multiplier = 1
            };

            requestContext.Currency = new Currency() { Id = sb.CurrencyId };
            requestContext.Parameters = new SpinArgs() { LineBet = sb.LineBet, BettingLines = sb.Lines };
            requestContext.GameSetting = new GameSetting() { GameSettingGroupId = sb.GameSettingGroupId };

            var sr = new PandaWarriorCollapsingSpinResult()
            {
                SpinBet = new SpinBet(ugk, PlatformType.None)
                {
                    Lines = PandaWarriorConfiguration.Lines,
                    Multiplier = 1,
                    LineBet = betperline
                },

                Wheel = wheelEncoding(PandaWarriorConfiguration.Width, PandaWarriorConfiguration.Height, arr)
            };

            sr.TopIndices = new List<int>() { 4, 39, 19, 10, 36 };

            totalWin = PandaWarriorCommon.CalculateWin(sr);

            Console.WriteLine();
            Console.WriteLine("--- POSITION TABLE ---");
            foreach (PandaWarriorTableWin tw in sr.TableWins)
                Console.WriteLine(String.Format("[WIN:{0} SYM:{1} COUNT:{2}]", tw.Win, tw.Card, tw.Count));

            if (sr.HasBonus)
            {
                var bonusCreated = PandaWarriorModule.CreateBonus(sr);

                var bonus = bonusCreated.Value;

                bonus.SpinTransactionId = sr.TransactionId;
                bonus.GameResult = sr;

                var requestBonusContext = new RequestContext<BonusArgs>("", PandaWarriorConfiguration.GameName, PlatformType.Web);

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
                        GameId = PandaWarriorConfiguration.GameId,
                        Guid = bonus.Guid.ToString("N"),
                        Data = bonus.ToByteArray(),
                        BonusType = bonus.GetType().Name,
                        Version = 2,
                        IsOptional = bonus.IsOptional,
                        IsStarted = bonus.IsStarted,
                        RoundId = sr.RoundId
                    };

                    bonusResult = PandaWarriorModule.ExecuteBonus(PandaWarriorConfiguration.LevelOne, entity, requestBonusContext).Value;
                    var pandaWarriorCollapsingSpinResult = bonusResult as PandaWarriorCollapsingBonusResult;

                    var win = pandaWarriorCollapsingSpinResult.Win;

                    if (win > 0)
                    {
                        totalWin += win;
                    }

                    var maxTopIndices = pandaWarriorCollapsingSpinResult.SpinResult.TopIndices.ToList();

                    if (totalWin > maxWin)
                    {
                        maxWin = totalWin;

                        maxIndexPosition = maxTopIndices;
                    }

                    Console.WriteLine("--- POSITION TABLE ---");
                    foreach (PandaWarriorTableWin tw in pandaWarriorCollapsingSpinResult.SpinResult.TableWins)
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
            var w = new Wheel(new List<int>() { 3, 3, 3, 3, 3 });
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
