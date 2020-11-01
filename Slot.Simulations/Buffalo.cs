namespace Slot.Simulations
{
    using System.IO;
    using System.Linq;
    using System;
    using Core.Modules.Infrastructure.Models;
    using Core.Modules.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Model.Entity;
    using Model;
    using NUnit.Framework;
    using Slot.Games.Buffalo;

    [TestFixture]
    public class Buffalo
    {
        private const int GameId = 91;
        private static IGameModule module;
        /// <summary>
        /// Defines the <see cref="SummaryData"/>
        /// </summary>
        private class SummaryData
        {
            public int MgFHit { get; set; }
            public int FgFHit { get; set; }
            public decimal FSTotalWin { get; set; }
            public long SpinCounter { get; set; }
            public long FSpinCounter { get; set; }
            public decimal TotalBet { get; set; }
            public decimal TotalWin { get; set; }

            public decimal MgFHitRate
            {
                get { return (decimal) SpinCounter / (MgFHit > 0 ? MgFHit : 1); }
            }

            public decimal FgFHitRate
            {
                get { return (decimal) FSpinCounter / (FgFHit > 0 ? FgFHit : 1); }
            }
            public decimal RTPOverAll
            {
                get { return TotalBet == 0 ? 1 : (TotalWin + FSTotalWin) / (TotalBet); }
            }
            public SummaryData()
            {
                SpinCounter = 0;
                FSpinCounter = 0;
            }
            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.FSpinCounter += target.FSpinCounter;
                source.TotalBet += target.TotalBet;
                source.TotalWin += target.TotalWin;
                source.FSTotalWin += target.FSTotalWin;
                source.MgFHit += target.MgFHit;
                source.FgFHit += target.FgFHit;
                return source;
            }
        }

        /// <summary>
        /// Dependency resolve and simulation setting stuff
        /// </summary>
        [SetUp]
        public void Settup()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddDistributedMemoryCache()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<BuffaloModule>();
            module = new BuffaloModule(logger);
        }

        [TestCase(1, 1000, 1000, 1.0, TestName = "[LVL1][1M] 1st Buffalo ")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[LVL1][1M] 2nd Buffalo ")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[LVL1][1M] 3rd Buffalo ")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 1st Buffalo ")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 2nd Buffalo ")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 3rd Buffalo ")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 1st Buffalo ")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 2nd Buffalo ")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 3rd Buffalo ")]

        [TestCase(2, 1000, 1000, 1.0, TestName = "[LVL2][1M] 1st Buffalo ")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[LVL2][1M] 2nd Buffalo ")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[LVL2][1M] 3rd Buffalo ")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 1st Buffalo ")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 2nd Buffalo ")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 3rd Buffalo ")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 1st Buffalo ")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2[100M] 2nd Buffalo ")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 3rd Buffalo ")]
        public void Spin(int level, int numusers, int numItrPerUser, decimal spinBet)
        {
            var sdt = DateTime.Now;
            var spinArgs = new SpinArgs
            {
                LineBet = spinBet,
                Multiplier = 1
            };

            var users = Utilities.GenerateUsers(GameId, numusers, level);
            var summData = users.AsParallel().Select(user =>
                {
                    var data = new SummaryData();
                    var request = user.CreateRequestContext<SpinArgs>("buffalo");
                    request.Parameters = spinArgs;
                    foreach (var iter in new byte[numItrPerUser])
                    {
                        data.SpinCounter++;
                        data.TotalBet += 30 * spinBet;

                        var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                        var result = executeResult.Value as BuffaloResult;

                        data.TotalWin += result.Win;
                        if (result.HasBonus)
                        {
                            var bonusRequest = user.CreateRequestContext<BonusArgs>("buffalo");
                            var freeSpinResult = ExecuteFreeSpin(level, bonusRequest, result);
                            data += freeSpinResult;
                            data.MgFHit++;
                        }
                    }
                    return data;
                }).AsEnumerable()
                .Aggregate((s1, s2) => s1 + s2);

            var edt = DateTime.Now;
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\Buffalo\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart            : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd              : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.SpinMode             : {0}", "Random"));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("SpinCount                 : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                  : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine(String.Format("Game Win                  : {0,12:0.00}", summData.TotalWin));
            Console.WriteLine(String.Format("Free Spin Win             : {0,12:0.00}", summData.FSTotalWin));
            Console.WriteLine(String.Format("Free SpinCount            : {0,12:0.00}", summData.FSpinCounter));
            Console.WriteLine(String.Format("MG Feature Hit Rate       : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("FG Feature Hit Rate       : {0,12:0.00}", summData.FgFHitRate));
            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("Main Game RTP             : {0,11:0.00}%", 100 * summData.TotalWin / summData.TotalBet));
                Console.WriteLine(String.Format("Free Spin RTP             : {0,11:0.00}%", 100 * summData.FSTotalWin / summData.TotalBet));
            }

            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,11:0.00}%", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        private static SummaryData ExecuteFreeSpin(int level, RequestContext<BonusArgs> bonusRequestcontext, BuffaloResult result)
        {
            var bonus = module.CreateBonus(result);
            var summData = new SummaryData();
            var freeSpinBonus = bonus.Value as FreeSpinBonus;
            do
            {
                summData.FSpinCounter++;
                var freeSpinResult = BuffaloEngine.ClaimBonus(level, freeSpinBonus, bonusRequestcontext) as BuffaloFreeSpinResult;
                summData.FSTotalWin += freeSpinResult.Win;
                if (freeSpinResult.SpinResult.HasBonus)
                    summData.FgFHit++;

                if (freeSpinResult.IsCompleted)
                    break;
            } while (true);
            return summData;
        }
    }
}