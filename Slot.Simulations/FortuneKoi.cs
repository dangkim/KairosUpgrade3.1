namespace Slot.Simulations
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.FortuneKoi;
    using Slot.Model.Entity;
    using System;
    using System.IO;
    using System.Linq;

    [TestFixture]
    internal class FortuneKoi
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
            var logger = logFactory.CreateLogger<FortuneKoiModule>();
            module = new FortuneKoiModule(logger);
            maxCollapse = 0;
        }

        [TestCase(1, 1000, 10000, 1.0, TestName = "[96.00][LVL1][10M] 1st FortuneKoi")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[96.00][LVL1][10M] 2nd FortuneKoi")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[96.00][LVL1][10M] 3rd FortuneKoi")]

        [TestCase(2, 1000, 10000, 1.0, TestName = "[94.00][LVL2][10M] 1st FortuneKoi")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[94.00][LVL2][10M] 2nd FortuneKoi")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[94.00][LVL2][10M] 3rd FortuneKoi")]

        [TestCase(3, 1000, 10000, 1.0, TestName = "[92.00][LVL3][10M] 1st FortuneKoi")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[92.00][LVL3][10M] 2nd FortuneKoi")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[92.00][LVL3][10M] 3rd FortuneKoi")]

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
                var request = user.CreateRequestContext<SpinArgs>("fortunekoi");
                request.Parameters = spinArgs;
                foreach (var iter in new byte[numItrPerUser])
                {
                    data.SpinCounter++;
                    data.TotalBet += 10 * spinBet;
                    var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                    var result = executeResult.Value as FortuneKoiSpinResult;
                    data.MgTotalWin += result.Win;
                    if (result.HasBonus)
                    {
                        var bonus = result.Bonus;
                        var bonusRequest = user.CreateRequestContext<BonusArgs>("fortunekoi");
                        data.MgFHit++;
                        data += ExecuteBonus(level, bonusRequest, result);
                    }
                }
                return data;
            }).AsEnumerable()
                .Aggregate((s1, s2) => s1 + s2);

            var edt = DateTime.Now;
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\FortuneKoi\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                  : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart              : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd                : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("SpinCount                   : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                    : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("MG Avg Feature Hit Rate     : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("Fg Feature Hit Rate         : {0,12:0.00}", summData.FgFHitRate));
            Console.WriteLine(String.Format("MG Win                      : {0,12:0.00}", summData.MgTotalWin));
            Console.WriteLine(String.Format("FG ReSpin Win               : {0,12:0.00}", summData.FgTotalWin));
            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("MG RTP                      : {0,11:0.00}%", 100 * summData.MgTotalWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG ReSpin RTP               : {0,11:0.00}%", 100 * summData.FgTotalWin / summData.TotalBet));
            }
            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,11:0.0000}%", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        private static SummaryData ExecuteBonus(int level, RequestContext<BonusArgs> requestContext, FortuneKoiSpinResult result)
        {
            var bonus = module.CreateBonus(result);
            var fortunekoiBonus = bonus.Value as FortuneKoiBonus;
            var summData = new SummaryData();
            var bonusContext = new BonusStateContext(level, fortunekoiBonus);
            do
            {
                summData.FSpinCounter++;
                var bonusSpinResult = bonusContext.SetState(fortunekoiBonus.State).Transform(requestContext);
                var spinResult = bonusSpinResult.SpinResult;
                if (spinResult.HasBonus)
                {
                    summData.FgFHit++;
                }

                summData.FgTotalWin += spinResult.Win;
                if (bonusContext.IsCompleted)
                    break;
                fortunekoiBonus.State = bonusContext.GetState();
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
            }

            public long FSpinCounter { get; set; }

            public decimal MgTotalWin { get; set; }
            public decimal FgTotalWin { get; set; }
            public int MgFHit { get; set; }
            public int FgFHit { get; set; }

            public decimal MgFHitRate
            {
                get { return (decimal)SpinCounter / (MgFHit > 0 ? MgFHit : 1); }
            }

            public decimal FgFHitRate
            {
                get { return (decimal)FSpinCounter / (FgFHit > 0 ? FgFHit : 1); }
            }

            public decimal RTPOverAll
            {
                get { return TotalBet == 0 ? 1 : (MgTotalWin + FgTotalWin) / (TotalBet); }
            }

            public long SpinCounter { get; set; }
            public decimal TotalBet { get; set; }

            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.FSpinCounter += target.FSpinCounter;
                source.TotalBet += target.TotalBet;
                source.MgTotalWin += target.MgTotalWin;
                source.FgTotalWin += target.FgTotalWin;
                source.MgFHit += target.MgFHit;
                source.FgFHit += target.FgFHit;
                return source;
            }
        }
    }
}