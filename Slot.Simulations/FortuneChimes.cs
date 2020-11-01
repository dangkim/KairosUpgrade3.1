namespace Slot.Simulations
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.FortuneChimes;
    using Slot.Model;
    using Slot.Model.Entity;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    [TestFixture]
    internal class FortuneChimes
    {
        private static IGameModule module;
        private const int GameId = 109;

        private static int maxCollapse;

        /// <summary>
        /// Dependency resolve and simulation setting stuff
        /// </summary>
        [SetUp]
        public void Settup()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<FortuneChimesModule>();
            module = new FortuneChimesModule(logger);
            maxCollapse = 0;
        }

        [TestCase(1, 1000, 1000, 1.0, TestName = "[97.01][LVL1][1M] 1st FortuneChimes")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[97.01][LVL1][1M] 2nd FortuneChimes")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[97.01][LVL1][1M] 3rd FortuneChimes")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[97.01][LVL1][1M] 4th FortuneChimes")]

        [TestCase(1, 1000, 10000, 1.0, TestName = "[97.01][LVL1][10M] 1st FortuneChimes")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[97.01][LVL1][10M] 2nd FortuneChimes")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[97.01][LVL1][10M] 3rd FortuneChimes")]
        [TestCase(1, 1000, 100000, 1.0, TestName = "[97.01][LVL1][100M] 1st FortuneChimes")]

        [TestCase(2, 1000, 1000, 1.0, TestName = "[96.50][LVL2][1M] 1st FortuneChimes")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[96.50][LVL2][1M] 2nd FortuneChimes")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[96.50][LVL2][1M] 3rd FortuneChimes")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[96.50][LVL2][1M] 4th FortuneChimes")]

        [TestCase(2, 1000, 10000, 1.0, TestName = "[96.50][LVL2][10M] 1st FortuneChimes")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[96.50][LVL2][10M] 2nd FortuneChimes")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[96.50][LVL2][10M] 3rd FortuneChimes")]
        [TestCase(2, 1000, 100000, 1.0, TestName = "[96.50][LVL2][100M] 1st FortuneChimes")]

        [TestCase(3, 1000, 1000, 1.0, TestName = "[94.27][LVL3][1M] 1st FortuneChimes")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[94.27][LVL3][1M] 2nd FortuneChimes")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[94.27][LVL3][1M] 3rd FortuneChimes")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[94.27][LVL3][1M] 4th FortuneChimes")]

        [TestCase(3, 1000, 10000, 1.0, TestName = "[94.27][LVL3][10M] 1st FortuneChimes")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[94.27][LVL3][10M] 2nd FortuneChimes")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[94.27][LVL3][10M] 3rd FortuneChimes")]
        [TestCase(3, 1000, 100000, 1.0, TestName = "[94.27][LVL3][100M] 1st FortuneChimes")]

        public void Spin(int level, int numusers, int numItrPerUser, decimal spinBet)
        {
            var sdt = DateTime.Now;
            var spinArgs = new SpinArgs
            {
                LineBet = spinBet,
                Multiplier = 1
            };

            var users = Utilities.GenerateUsers(GameId, numusers, level);
            var summData = users.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount).Select(user =>
                {
                    var data = new SummaryData();
                    var request = user.CreateRequestContext<SpinArgs>("fortunechimes");
                    request.Parameters = spinArgs;
                    foreach (var iter in new byte[numItrPerUser])
                    {
                        data.SpinCounter++;
                        data.TotalBet += 30 * spinBet;
                        var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                        var result = executeResult.Value as FortuneChimesSpinResult;
                        var scatterPayout = result.WinPositions.Where(ele => ele.Line == 0 && ele.Symbol == 10).Sum(item => item.Win);
                        data.MainGameScatterWin += scatterPayout;
                        data.MainGameNoneScatterWin += result.Win - scatterPayout;
                        if (result.HasBonus)
                        {
                            var bonus = result.Bonus;
                            var bonusRequest = user.CreateRequestContext<BonusArgs>("fortunechimes");
                            var typeOfBonus = bonus.ClientId;
                            switch (typeOfBonus)
                            {
                                case 4:
                                case 5:
                                    data.MgExplodingHit++;
                                    break;

                                case 3:
                                    data.MgFHit++;
                                    break;

                                default:
                                    throw new Exception();
                            }
                            data += ExecuteBonus(level, bonusRequest, result);
                        }
                    }
                    return data;
                }).AsEnumerable()
                .Aggregate((s1, s2) => s1 + s2);

            var edt = DateTime.Now;
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\FortuneChimes\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                  : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart              : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd                : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("SpinCount                   : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                    : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("Exploding Hit Rate     (MG) : {0,12:0.00}", summData.MgExplodingHitRate));
            Console.WriteLine(String.Format("MG Avg Feature Hit Rate     : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("Exploding Feature Hit Rate  : {0,12:0.00}", summData.MgExplodingFHitRate));
            Console.WriteLine(String.Format("Exploding Hit Rate     (FG) : {0,12:0.00}", summData.FgExplodingHitRate));
            Console.WriteLine(String.Format("Reveal Hit Rate        (FG) : {0,12:0.00}", summData.FgRevealHitRate));
            Console.WriteLine(String.Format("MG Scatter Win              : {0,12:0.00}", summData.MainGameScatterWin));
            Console.WriteLine(String.Format("MG None Scatter Win         : {0,12:0.00}", summData.MainGameNoneScatterWin));
            Console.WriteLine(String.Format("MG ReSpin Scatter Win       : {0,12:0.00}", summData.MgScatterExplodingWin));
            Console.WriteLine(String.Format("MG ReSpin None Scatter Win  : {0,12:0.00}", summData.MgNonScatterExplodingWin));
            Console.WriteLine(String.Format("MG Reveal Win               : {0,12:0.00}", summData.MgRevealWin));
            Console.WriteLine(String.Format("FG Win                      : {0,12:0.00}", summData.FSTotalWin));
            Console.WriteLine(String.Format("FG Exploding Win            : {0,12:0.00}", summData.FgExplodingWin));
            Console.WriteLine(String.Format("FG Reveal Win               : {0,12:0.00}", summData.FgRevealWin));
            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("MG Scatter RTP             : {0,11:0.00}%", 100 * summData.MainGameScatterWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG None Scatter RTP        : {0,11:0.00}%", 100 * summData.MainGameNoneScatterWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG ReSpin Scatter RTP      : {0,11:0.00}%", 100 * summData.MgScatterExplodingWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG ReSpin NOne Scatter RTP : {0,11:0.00}%", 100 * summData.MgNonScatterExplodingWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG Reveal  RTP             : {0,11:0.00}%", 100 * summData.MgRevealWin / summData.TotalBet));

                Console.WriteLine(String.Format("FG RTP                     : {0,11:0.00}%", 100 * summData.FSTotalWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG Exploding RTP           : {0,11:0.00}%", 100 * summData.FgExplodingWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG Reveal RTP              : {0,11:0.00}%", 100 * summData.FgRevealWin / summData.TotalBet));
            }
            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,11:0.0000}%", 100 * summData.RTPOverAll));

            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        [TestCase(1, 1.0, TestName = "[MG][FullCycle] FortuneChimes")]
        public void FullCycle(int level, decimal spinBet)
        {
            var strips = Slot.Games.FortuneChimes.Configuration.Config.MainGameReelStrip[0];
            var reel1 = strips[0];
            var reel2 = strips[1];
            var reel3 = strips[2];
            var reel4 = strips[3];
            var reel5 = strips[4];
            var user = new UserGameKey(-1, 109) { Level = level };
            var summData = new SummaryData();
            var sdt = DateTime.Now;
            var spinArgs = new SpinArgs
            {
                LineBet = spinBet,
                Multiplier = 1
            };
            var request = user.CreateRequestContext<SpinArgs>("fortunechimes");
            for (int i1 = 0; i1 < reel1.Count; i1++)
                for (int i2 = 0; i2 < reel2.Count; i2++)
                    for (int i3 = 0; i3 < reel3.Count; i3++)
                        for (int i4 = 0; i4 < reel4.Count; i4++)
                            for (int i5 = 0; i5 < reel5.Count; i5++)
                            {
                                request.Parameters = spinArgs;
                                var r1 = reel1.TakeCircular(i1, 3).ToArray();
                                var r2 = reel2.TakeCircular(i2, 3).ToArray();
                                var r3 = reel3.TakeCircular(i3, 3).ToArray();
                                var r4 = reel4.TakeCircular(i4, 3).ToArray();
                                var r5 = reel5.TakeCircular(i5, 3).ToArray();
                                if (i1 == 2 && i2 == i3 && i3 == i4 && i4 == 9)
                                {
                                }
                                var wheel = new FortuneChimesWheel
                                {
                                    Reels = new List<int[]> { r1, r2, r3, r4, r5 },
                                    Indices = new int[] { i1, i2, i3, i4, i5 }
                                };
                                summData.SpinCounter++;
                                summData.TotalBet += Slot.Games.FortuneChimes.Configuration.Config.Lines * spinBet;
                                var result = GameReduce.DoSpin(level, request, wheel);
                                var scatterPayout = result.WinPositions.Where(ele => ele.Line == 0 && ele.Symbol == 10).Sum(item => item.Win);
                                summData.MainGameScatterWin += scatterPayout;
                                summData.MainGameNoneScatterWin += result.Win - scatterPayout;
                                if (scatterPayout > 0)
                                    summData.MgFHit++;
                                if (result.HasBonus)
                                {
                                    var bonus = result.Bonus;
                                    var bonusRequest = user.CreateRequestContext<BonusArgs>("fortunechimes");
                                    var typeOfBonus = bonus.ClientId;
                                    switch (typeOfBonus)
                                    {
                                        case 4:
                                        case 5:
                                            summData.MgExplodingHit++;
                                            break;

                                        case 3:
                                            summData.MgFHit++;
                                            break;

                                        default:
                                            throw new Exception();
                                    }
                                    summData += ExecuteBonus(level, bonusRequest, result);
                                }
                            }

            var edt = DateTime.Now;
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\FortuneChimes\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                  : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart              : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd                : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("SpinCount                   : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                    : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("Exploding Hit Rate     (MG) : {0,12:0.00}", summData.MgExplodingHitRate));
            Console.WriteLine(String.Format("MG Feature Hit Rate         : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("MG Scatter ReSpin Hit       : {0,12:0.00}", summData.MgScatterReSpinFHit));
            Console.WriteLine(String.Format("MG None Scatter ReSpin Hit  : {0,12:0.00}", summData.MgNoneScatterReSpinFHit));
            Console.WriteLine(String.Format("MG ReSpin Feature Hit Rate  : {0,12:0.00}", summData.MgReSpinFHitRate));

            //Console.WriteLine(String.Format("MG Avg Feature Hit Rate     : {0,12:0.00}", summData));
            Console.WriteLine(String.Format("Exploding Feature Hit Rate  : {0,12:0.00}", summData.MgExplodingFHitRate));
            Console.WriteLine(String.Format("Exploding Hit Rate     (FG) : {0,12:0.00}", summData.FgExplodingHitRate));
            Console.WriteLine(String.Format("Reveal Hit Rate        (FG) : {0,12:0.00}", summData.FgRevealHitRate));
            Console.WriteLine(String.Format("MG Scatter Win              : {0,12:0.00}", summData.MainGameScatterWin));
            Console.WriteLine(String.Format("MG None Scatter Win         : {0,12:0.00}", summData.MainGameNoneScatterWin));
            Console.WriteLine(String.Format("MG ReSpin Scatter Win       : {0,12:0.00}", summData.MgScatterExplodingWin));
            Console.WriteLine(String.Format("MG ReSpin None Scatter Win  : {0,12:0.00}", summData.MgNonScatterExplodingWin));
            Console.WriteLine(String.Format("MG Reveal Win               : {0,12:0.00}", summData.MgRevealWin));
            Console.WriteLine(String.Format("FG Win                      : {0,12:0.00}", summData.FSTotalWin));
            Console.WriteLine(String.Format("FG Exploding Win            : {0,12:0.00}", summData.FgExplodingWin));
            Console.WriteLine(String.Format("FG Reveal Win               : {0,12:0.00}", summData.FgRevealWin));
            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("MG Scatter RTP             : {0,11:0.00000000000000000000000000}%", 100 * summData.MainGameScatterWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG None Scatter RTP        : {0,11:0.00000000000000000000000000}%", 100 * summData.MainGameNoneScatterWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG ReSpin Scatter RTP      : {0,11:0.00000000000000000000000000}%", 100 * summData.MgScatterExplodingWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG ReSpin None Scatter RTP : {0,11:0.00000000000000000000000000}%", 100 * summData.MgNonScatterExplodingWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG Reveal  RTP             : {0,11:0.00000000000000000000000000}%", 100 * summData.MgRevealWin / summData.TotalBet));

                Console.WriteLine(String.Format("FG RTP                     : {0,11:0.00}%", 100 * summData.FSTotalWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG Exploding RTP           : {0,11:0.00}%", 100 * summData.FgExplodingWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG Reveal RTP              : {0,11:0.00}%", 100 * summData.FgRevealWin / summData.TotalBet));
            }
            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,11:0.0000}%", 100 * summData.RTPOverAll));

            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();

            Console.WriteLine($"Max Collapse {maxCollapse}");
            Console.WriteLine("Done");
        }

        private static SummaryData ExecuteBonus(int level, RequestContext<BonusArgs> requestContext, FortuneChimesSpinResult result)
        {
            var bonus = module.CreateBonus(result);
            var fortunechimesBonus = bonus.Value as FortuneChimesBonus;
            var summData = new SummaryData();
            var bonusContext = new BonusStateContext(level, fortunechimesBonus);
            do
            {
                var bonusSpinResult = bonusContext.SetState(fortunechimesBonus.State).Transform(requestContext);
                var spinResult = bonusSpinResult.SpinResult;
                if (spinResult.HasBonus)
                {
                    var additionBonus = spinResult.Bonus;
                    if (additionBonus.ClientId == 3)
                        summData.MgFHit++;
                    else if (additionBonus.ClientId == 4)
                        summData.FgExplodingHit++;
                    else
                    if (additionBonus.ClientId == 5)
                        summData.FgRevealHit++;
                }

                switch (fortunechimesBonus.State)
                {
                    case ReSpinState _:
                        var nonScatterWin = spinResult.WinPositions.Where(winline => winline.Line > 0).Sum(item => item.Win);
                        var instanceWin = spinResult.InstanceWin != null ? spinResult.InstanceWin.Win : 0;
                        summData.MgNonScatterExplodingWin += nonScatterWin;
                        summData.MgScatterExplodingWin += (spinResult.Win - instanceWin - nonScatterWin);
                        summData.ExplodingCounter += spinResult.InstanceWin != null ? 1 : 0;
                        summData.MgRevealWin += instanceWin;
                        break;

                    case FreeSpinReSpinState _:

                        instanceWin = spinResult.InstanceWin != null ? spinResult.InstanceWin.Win : 0;
                        summData.FgExplodingWin += (spinResult.Win - instanceWin);
                        summData.FgRevealWin += instanceWin;
                        break;

                    case FreeSpinState _:
                        summData.FSTotalWin += spinResult.Win;
                        summData.FSpinCounter++;
                        break;

                    default:
                        throw new Exception();
                }
                if (bonusContext.IsCompleted)
                    break;
                fortunechimesBonus.State = bonusContext.GetState();
            } while (true);

            return summData;
        }

        /// <summary>
        /// Defines the <see cref="SummaryData"/>
        /// </summary>
        private class SummaryData
        {
            public SummaryData()
            {
                SpinCounter = 0;
                FSpinCounter = 0;
                ExplodingCounter = 0;
            }

            public int FgExplodingHit { get; set; }

            public decimal FgExplodingHitRate
            {
                get { return (decimal)FSpinCounter / (FgExplodingHit > 0 ? FgExplodingHit : 1); }
            }

            public int FgRevealHit { get; set; }

            public decimal FgRevealHitRate
            {
                get { return (decimal)FSpinCounter / (FgRevealHit > 0 ? FgRevealHit : 1); }
            }

            public decimal FgExplodingWin { get; set; }
            public decimal FgRevealWin { get; set; }
            public long FSpinCounter { get; set; }
            public decimal FSTotalWin { get; set; }
            public decimal MainGameScatterWin { get; set; }

            public decimal MainGameNoneScatterWin { get; set; }
            public int MgFHit { get; set; }
            public int MgScatterReSpinFHit { get; set; }
            public int MgNoneScatterReSpinFHit { get; set; }
            public int MgExplodingFHit { get; set; }

            public decimal MgFHitRate
            {
                get { return (decimal)SpinCounter / (MgFHit > 0 ? MgFHit : 1); }
            }

            public decimal MgReSpinFHitRate
            {
                get { return (decimal)SpinCounter / (MgScatterReSpinFHit + MgNoneScatterReSpinFHit > 0 ? MgScatterReSpinFHit + MgNoneScatterReSpinFHit : 1); }
            }

            public decimal MgExplodingFHitRate
            {
                get { return (decimal)SpinCounter / (MgExplodingFHit > 0 ? MgExplodingFHit : 1); }
            }

            public int MgExplodingHit { get; set; }

            public decimal MgExplodingHitRate
            {
                get { return (decimal)SpinCounter / (MgExplodingHit > 0 ? MgExplodingHit : 1); }
            }

            public decimal MgNonScatterExplodingWin { get; set; }
            public decimal MgScatterExplodingWin { get; set; }

            public decimal MgRevealWin { get; set; }

            public decimal RTPOverAll
            {
                get { return TotalBet == 0 ? 1 : (MainGameScatterWin + MainGameNoneScatterWin + MgNonScatterExplodingWin + MgScatterExplodingWin + MgRevealWin + FSTotalWin + FgExplodingWin + FgRevealWin) / (TotalBet); }
            }

            public long SpinCounter { get; set; }
            public long ExplodingCounter { get; set; }
            public decimal TotalBet { get; set; }

            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.ExplodingCounter += target.ExplodingCounter;
                source.FSpinCounter += target.FSpinCounter;
                source.TotalBet += target.TotalBet;
                source.MainGameScatterWin += target.MainGameScatterWin;
                source.MainGameNoneScatterWin += target.MainGameNoneScatterWin;
                source.MgNonScatterExplodingWin += target.MgNonScatterExplodingWin;
                source.MgScatterExplodingWin += target.MgScatterExplodingWin;
                source.MgRevealWin += target.MgRevealWin;
                source.FSTotalWin += target.FSTotalWin;
                source.FgExplodingWin += target.FgExplodingWin;
                source.FgRevealWin += target.FgRevealWin;

                source.MgExplodingHit += target.MgExplodingHit;
                source.MgFHit += target.MgFHit;
                source.MgNoneScatterReSpinFHit += target.MgNoneScatterReSpinFHit;
                source.MgScatterReSpinFHit += target.MgScatterReSpinFHit;
                source.MgExplodingFHit += target.MgExplodingFHit;

                source.FgRevealHit += target.FgRevealHit;
                source.FgExplodingHit += target.FgExplodingHit;
                return source;
            }
        }
    }
}