namespace Slot.Simulations
{
    using Core.Modules.Infrastructure;
    using Core.Modules.Infrastructure.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Model.Entity;
    using NUnit.Framework;
    using Slot.Games.CaptainRabbit;
    using System;
    using System.IO;
    using System.Linq;
    using static Slot.Games.CaptainRabbit.Domain;

    [TestFixture]
    public class CaptainRabbit
    {
        private const int GameId = 103;
        private static IGameModule module;

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
            var logger = logFactory.CreateLogger<BonusBearModule.Engine>();
            module = new BonusBearModule.Engine(logger);
        }

        [TestCase(1, 1000, 1000, 1.0, TestName = "[LVL1][1M] 1st CaptainRabbit")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[LVL1][1M] 2nd CaptainRabbit")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[LVL1][1M] 3rd CaptainRabbit")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 1st CaptainRabbit")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 2nd CaptainRabbit")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 3rd CaptainRabbit")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 1st CaptainRabbit")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 2nd CaptainRabbit")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 3rd CaptainRabbit")]

        [TestCase(2, 1000, 1000, 1.0, TestName = "[LVL2][1M] 1st CaptainRabbit")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[LVL2][1M] 2nd CaptainRabbit")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[LVL2][1M] 3rd CaptainRabbit")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 1st CaptainRabbit")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 2nd CaptainRabbit")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 3rd CaptainRabbit")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 1st CaptainRabbit")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 2nd CaptainRabbit")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 3rd CaptainRabbit")]

        [TestCase(3, 1000, 1000, 1.0, TestName = "[LVL3][1M] 1st CaptainRabbit")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[LVL3][1M] 2nd CaptainRabbit")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[LVL3][1M] 3rd CaptainRabbit")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 1st CaptainRabbit")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 2nd CaptainRabbit")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 3rd CaptainRabbit")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 1st CaptainRabbit")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 2nd CaptainRabbit")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 3rd CaptainRabbit")]

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
                    var request = user.CreateRequestContext<SpinArgs>("bonus-bear");
                    request.Parameters = spinArgs;
                    foreach (var iter in new byte[numItrPerUser])
                    {
                        data.SpinCounter++;
                        data.TotalBet += 25 * spinBet;
                        var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                        var result = executeResult.Value as BearResult;
                        data.MainGameWin += result.Win;

                        if (result.HasBonus)
                        {
                            var bonus = result.Bonus;
                            var bonusRequest = user.CreateRequestContext<BonusArgs>("Bonus Bear-hunter");
                            var typeOfBonus = bonus.ClientId;
                            switch (typeOfBonus)
                            {
                                case 4:
                                    data.MgHoneyHit++;
                                    data += ExecuteMainGameHoney(level, bonusRequest, result);
                                    break;

                                case 3:
                                    data.MgFHit++;
                                    data += ExecuteFreeSpin(level, bonusRequest, result);
                                    break;

                                default:
                                    throw new Exception();
                            }
                        }
                    }
                    return data;
                }).AsEnumerable()
                .Aggregate((s1, s2) => s1 + s2);

            var edt = DateTime.Now;
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\CaptainRabbit\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                  : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart              : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd                : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("SpinCount                   : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                    : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("Honey Hit Rate         (MG) : {0,12:0.00}", summData.MgHoneyHitRate));
            Console.WriteLine(String.Format("Feature Hit Rate       (MG) : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("Honey Hit Rate         (FG) : {0,12:0.00}", summData.FgHoneyHitRate));
            Console.WriteLine(String.Format("Feature Hit Rate       (FG) : {0,12:0.00}", summData.FgFHitRate));
            Console.WriteLine(String.Format("MG Win                      : {0,12:0.00}", summData.MainGameWin));
            Console.WriteLine(String.Format("MG Honey Win                : {0,12:0.00}", summData.MgHoneyWin));
            Console.WriteLine(String.Format("FG Win                      : {0,12:0.00}", summData.FSTotalWin));
            Console.WriteLine(String.Format("FG Honey Win                : {0,12:0.00}", summData.FgHoneyWin));
            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("MG RTP                     : {0,11:0.00}%", 100 * summData.MainGameWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG Honey  RTP              : {0,11:0.00}%", 100 * summData.MgHoneyWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG RTP                     : {0,11:0.00}%", 100 * summData.FSTotalWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG Honey RTP               : {0,11:0.00}%", 100 * summData.FgHoneyWin / summData.TotalBet));
            }
            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,11:0.0000}%", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        private static SummaryData ExecuteFreeSpin(int level, RequestContext<BonusArgs> bonusRequestcontext, BearResult result)
        {
            var bonus = module.CreateBonus(result);
            var summData = new SummaryData();
            var freeSpinBonus = bonus.Value as BearBonus;
            do
            {
                summData.FSpinCounter++;
                var freeSpinResult = BonusReducer.dispatch(level, freeSpinBonus, bonusRequestcontext);
                var spinResult = freeSpinResult.SpinResult;
                if (spinResult.HasBonus)
                {
                    var additionBonus = spinResult.Bonus;
                    if (additionBonus.ClientId == 4)
                        summData.FgHoneyHit++;
                    else if (additionBonus.ClientId == 3) summData.FgFHit++;
                }

                if (spinResult.Type == "b")
                    summData.FgHoneyWin += spinResult.Win;
                else
                {
                    summData.FSTotalWin += spinResult.Win;
                }

                if (freeSpinResult.IsCompleted) break;
                freeSpinBonus = freeSpinResult.Bonus as BearBonus;
            } while (true);

            return summData;
        }

        private static SummaryData ExecuteMainGameHoney(int level, RequestContext<BonusArgs> bonusRequestcontext, BearResult result)
        {
            var bonus = module.CreateBonus(result);
            var summData = new SummaryData();
            var freeSpinBonus = bonus.Value as BearBonus;
            do
            {
                var freeSpinResult = BonusReducer.dispatch(level, freeSpinBonus, bonusRequestcontext);
                summData.MgHoneyWin += freeSpinResult.Win;
                if (freeSpinResult.IsCompleted)
                    break;
                freeSpinBonus = freeSpinResult.Bonus as BearBonus;
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

            public int FgHoneyHit { get; set; }

            public decimal FgHoneyHitRate
            {
                get { return (decimal)FSpinCounter / (FgHoneyHit > 0 ? FgHoneyHit : 1); }
            }

            public decimal FgHoneyWin { get; set; }
            public long FSpinCounter { get; set; }
            public decimal FSTotalWin { get; set; }
            public decimal MainGameWin { get; set; }
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

            public int MgHoneyHit { get; set; }

            public decimal MgHoneyHitRate
            {
                get { return (decimal)SpinCounter / (MgHoneyHit > 0 ? MgHoneyHit : 1); }
            }

            public decimal MgHoneyWin { get; set; }

            public decimal RTPOverAll
            {
                get { return TotalBet == 0 ? 1 : (MainGameWin + MgHoneyWin + FSTotalWin + FgHoneyWin) / (TotalBet); }
            }

            public long SpinCounter { get; set; }
            public decimal TotalBet { get; set; }

            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.FSpinCounter += target.FSpinCounter;
                source.TotalBet += target.TotalBet;
                source.MainGameWin += target.MainGameWin;
                source.MgHoneyWin += target.MgHoneyWin;
                source.FSTotalWin += target.FSTotalWin;
                source.FgHoneyWin += target.FgHoneyWin;

                source.MgHoneyHit += target.MgHoneyHit;
                source.MgFHit += target.MgFHit;
                source.FgFHit += target.FgFHit;
                source.FgHoneyHit += target.FgHoneyHit;
                return source;
            }
        }
    }
}