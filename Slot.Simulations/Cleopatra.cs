namespace Slot.Simulations
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.Cleopatra;
    using Slot.Model.Entity;
    using System;
    using System.IO;
    using System.Linq;

    [TestFixture]
    internal class Cleopatra
    {
        private static IGameModule module;
        private const int GameId = 51;

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
            var logger = logFactory.CreateLogger<CleopatraModule>();
            module = new CleopatraModule(logger);
        }

        [TestCase(1, 1000, 1000, 1.0, TestName = "[95.36][LVL1][1M] 1st Cleopatra")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[95.36][LVL1][1M] 2nd Cleopatra")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[95.36][LVL1][1M] 3rd Cleopatra")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[95.36][LVL1][10M] 1st Cleopatra")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[95.36][LVL1][10M] 2nd Cleopatra")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[95.36][LVL1][10M] 3rd Cleopatra")]

        [TestCase(2, 1000, 1000, 1.0, TestName = "[92.72][LVL2][1M] 1st Cleopatra")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[92.72][LVL2][1M] 2nd Cleopatra")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[92.72][LVL2][1M] 3rd Cleopatra")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[92.72][LVL2][10M] 1st Cleopatra")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[92.72][LVL2][10M] 2nd Cleopatra")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[92.72][LVL2][10M] 3rd Cleopatra")]

        [TestCase(3, 1000, 1000, 1.0, TestName = "[90.34][LVL3][1M] 1st Cleopatra")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[90.34][LVL3][1M] 2nd Cleopatra")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[90.34][LVL3][1M] 3rd Cleopatra")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[90.34][LVL3][10M] 1st Cleopatra")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[90.34][LVL3][10M] 2nd Cleopatra")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[90.34][LVL3][10M] 3rd Cleopatra")]

        [TestCase(4, 1000, 1000, 1.0, TestName = "[97.00][LVL4][1M] 1st Cleopatra")]
        [TestCase(4, 1000, 1000, 1.0, TestName = "[97.00][LVL4][1M] 2nd Cleopatra")]
        [TestCase(4, 1000, 1000, 1.0, TestName = "[97.00][LVL4][1M] 3rd Cleopatra")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[97.00][LVL4][10M] 1st Cleopatra")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[97.00][LVL4][10M] 2nd Cleopatra")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[97.00][LVL4][10M] 3rd Cleopatra")]

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
                    var request = user.CreateRequestContext<SpinArgs>("cleopatra");
                    request.Parameters = spinArgs;
                    foreach (var iter in new byte[numItrPerUser])
                    {
                        data.SpinCounter++;
                        data.TotalBet += 50 * spinBet;
                        var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                        var result = executeResult.Value as CleopatraSpinResult;
                        data.MainGameWin += result.Win;
                        if (result.HasBonus)
                        {
                            var bonus = result.Bonus;
                            var typeOfBonus = bonus.ClientId;
                            var bonusRequest = user.CreateRequestContext<BonusArgs>("cleopatra");
                            data.MgFHit++;
                            data += ExecuteBonus(level, bonusRequest, result);
                        }
                    }
                    return data;
                }).AsEnumerable()
                .Aggregate((s1, s2) => s1 + s2);

            var edt = DateTime.Now;
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\Cleopatra\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                  : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart              : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd                : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("SpinCount                   : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                    : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine("----------------------------------------");

            Console.WriteLine(String.Format("Feature Hit Rate       (MG) : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("Sarchophagus Hit Rate  (FG) : {0,12:0.00}", summData.FgSarchophagusRate));
            Console.WriteLine(String.Format("MG Win                      : {0,12:0.00}", summData.MainGameWin));
            Console.WriteLine(String.Format("FG Win                      : {0,12:0.00}", summData.FSTotalWin));
            Console.WriteLine(String.Format("FG Sarchophagus Win         : {0,12:0.00}", summData.FgSarchophagusWin));

            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("MG RTP                      : {0,11:0.00}%", 100 * summData.MainGameWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG RTP                      : {0,11:0.00}%", 100 * summData.FSTotalWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG Sarchophagus RTP         : {0,11:0.00}%", 100 * summData.FgSarchophagusWin / summData.TotalBet));
            }
            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)        : {0,11:0.0000}%", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        private static SummaryData ExecuteBonus(int level, RequestContext<BonusArgs> requestContext, CleopatraSpinResult result)
        {
            var bonus = module.CreateBonus(result);
            var cleopatraBonus = bonus.Value as CleopatraBonus;
            var summData = new SummaryData();
            var bonusContext = new BonusStateContext(level, cleopatraBonus);
            do
            {
                var bonusSpinResult = bonusContext.SetState(cleopatraBonus.State).Transform(requestContext);
                if (bonusSpinResult.SpinResult.HasBonus)
                    summData.FgSarchophagusHit++;
                switch (bonusSpinResult.GameResultType)
                {
                    case Model.GameResultType.FreeSpinResult:
                        summData.FSpinCounter++;
                        summData.FSTotalWin += bonusSpinResult.Win;
                        break;

                    case Model.GameResultType.RevealResult:
                        summData.FgSarchophagusWin += bonusSpinResult.Win;
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                if (bonusSpinResult.IsCompleted)
                    break;
                cleopatraBonus.State = bonusContext.GetState();
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
            public decimal FSTotalWin { get; set; }
            public decimal MainGameWin { get; set; }
            public int MgFHit { get; set; }

            public int FgSarchophagusHit { get; set; }

            public decimal FgSarchophagusRate
            {
                get { return (decimal)FSpinCounter / (FgSarchophagusHit > 0 ? FgSarchophagusHit : 1); }
            }

            public decimal MgFHitRate
            {
                get { return (decimal)SpinCounter / (MgFHit > 0 ? MgFHit : 1); }
            }

            public decimal FgSarchophagusWin { get; set; }

            public decimal RTPOverAll
            {
                get { return TotalBet == 0 ? 1 : (MainGameWin + FgSarchophagusWin + FSTotalWin) / TotalBet; }
            }

            public long SpinCounter { get; set; }
            public decimal TotalBet { get; set; }

            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.FSpinCounter += target.FSpinCounter;
                source.TotalBet += target.TotalBet;
                source.MainGameWin += target.MainGameWin;
                source.FgSarchophagusWin += target.FgSarchophagusWin;
                source.FSTotalWin += target.FSTotalWin;

                source.FgSarchophagusHit += target.FgSarchophagusHit;
                source.MgFHit += target.MgFHit;
                return source;
            }
        }
    }
}