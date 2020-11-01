namespace Slot.Simulations
{
    using Core.Modules.Infrastructure;
    using Core.Modules.Infrastructure.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Model.Entity;
    using NUnit.Framework;
    using Slot.Games.GeniesLuck;
    using System;
    using System.IO;
    using System.Linq;

    [TestFixture]
    public class GeniesLuck
    {
        private const int GameId = 30;
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
            var logger = logFactory.CreateLogger<GeniesLuckModule>();
            module = new GeniesLuckModule(logger);
        }

        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][96.46][10M] 1st Genies Luck")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][96.46][10M] 2nd Genies Luck")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][96.46][10M] 3rd Genies Luck")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][96.46][100M] 1st Genies Luck")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][96.46][100M] 2nd Genies Luck")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][96.46][100M] 3rd Genies Luck")]

        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2]94.84][10M] 1st Genies Luck")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2]94.84][10M] 2nd Genies Luck")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2]94.84][10M] 3rd Genies Luck")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2]94.84][100M] 1st Genies Luck")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2]94.84][100M] 2nd Genies Luck")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2]94.84][100M] 3rd Genies Luck")]
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
                    var request = user.CreateRequestContext<SpinArgs>("geniesluck");
                    request.Parameters = spinArgs;
                    foreach (var iter in new byte[numItrPerUser])
                    {
                        data.SpinCounter++;
                        data.TotalBet += 40 * spinBet;

                        var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                        var result = executeResult.Value as GeniesLuckResult;

                        data.MainGameWin += result.Win;
                        if (result.HasBonus)
                        {
                            if (result.Bonus.ClientId == 3)
                                data.MgFHit++;
                            else
                                data.MgReHit++;
                            var bonusRequest = user.CreateRequestContext<BonusArgs>("geniesluck");
                            var freeSpinResult = ExecuteBonus(level, bonusRequest, result);
                            data += freeSpinResult;
                        }
                    }
                    return data;
                }).AsEnumerable()
                .Aggregate((s1, s2) => s1 + s2);

            var edt = DateTime.Now;
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\GeniesLuck\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart            : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd              : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.SpinMode             : {0}", "Random"));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("SpinCount                 : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                  : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine(String.Format("Game Win                  : {0,12:0.00}", summData.MainGameWin));
            Console.WriteLine(String.Format("Free Spin Win             : {0,12:0.00}", summData.FSWin));
            Console.WriteLine(String.Format("Free SpinCount            : {0,12:0.00}", summData.FSpinCounter));
            Console.WriteLine(String.Format("MG ReSpin Hit Rate        : {0,12:0.00}", summData.MgReHitRate));
            Console.WriteLine(String.Format("MG Feature Hit Rate       : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("FG ReSpin Hit Rate        : {0,12:0.00}", summData.FgReHitRate));
            Console.WriteLine(String.Format("FG Feature Hit Rate       : {0,12:0.00}", summData.FgFHitRate));

            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("Main Game RTP             : {0,11:0.00}%", 100 * summData.MainGameWin / summData.TotalBet));
                Console.WriteLine(String.Format("ReSpin RTP                : {0,11:0.00}%", 100 * summData.ReSpinWin / summData.TotalBet));
                Console.WriteLine(String.Format("Main Game RTP             : {0,11:0.00}%", 100 * summData.FSWin / summData.TotalBet));
                Console.WriteLine(String.Format("ReSpin Free Game RTP      : {0,11:0.00}%", 100 * summData.ReSpinFSWin / summData.TotalBet));
            }

            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,11:0.00}%", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        private static SummaryData ExecuteBonus(int level, RequestContext<BonusArgs> requestContext, GeniesLuckResult result)
        {
            var bonus = module.CreateBonus(result);
            var geniesLuckBonus = bonus.Value as GeniesLuckBonus;
            var entity = new BonusEntity
            {
                UserId = -1,
                GameId = GameId,
                Guid = geniesLuckBonus.Guid.ToString("N"),
                Data = Model.Utility.Extension.ToByteArray(geniesLuckBonus),
                BonusType = bonus.GetType().Name,
                Version = 3,
                RoundId = 1,
                BetReference = ""
            };

            var summData = new SummaryData();
            do
            {
                var freeSpinResult = module.ExecuteBonus(level, entity, requestContext).Value as GeniesLuckBonusSpinResult;
                var spinResult = freeSpinResult.SpinResult;
                switch (freeSpinResult.GameResultType)
                {
                    case Model.GameResultType.FreeSpinResult:
                        summData.FSWin += freeSpinResult.Win;
                        summData.FSpinCounter++;
                        break;

                    case Model.GameResultType.CollapsingSpinResult:
                        summData.ReSpinWin += freeSpinResult.Win;
                        break;

                    case Model.GameResultType.FreeSpinCollapsingSpinResult:
                        summData.ReSpinFSWin += freeSpinResult.Win;
                        break;
                }

                if (spinResult.HasBonus)
                {
                    if (spinResult.Bonus.ClientId == 3)
                        summData.FgFHit++;
                    else
                        summData.FgReHit++;
                }

                if (freeSpinResult.IsCompleted)
                    break;
                entity.Data = Model.Utility.Extension.ToByteArray(freeSpinResult.Bonus);
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

            public int MgFHit { get; set; }

            public decimal MgFHitRate
            {
                get { return (decimal)SpinCounter / (MgFHit > 0 ? MgFHit : 1); }
            }

            public int MgReHit { get; set; }

            public decimal MgReHitRate
            {
                get { return (decimal)SpinCounter / (MgReHit > 0 ? MgReHit : 1); }
            }

            public int FgFHit { get; set; }

            public decimal FgFHitRate
            {
                get { return (decimal)FSpinCounter / (FgFHit > 0 ? FgFHit : 1); }
            }

            public int FgReHit { get; set; }

            public decimal FgReHitRate
            {
                get { return (decimal)FSpinCounter / (FgReHit > 0 ? FgReHit : 1); }
            }

            public decimal MainGameWin { get; set; }
            public decimal ReSpinWin { get; set; }
            public decimal FSWin { get; set; }
            public decimal ReSpinFSWin { get; set; }

            public decimal RTPOverAll
            {
                get { return TotalBet == 0 ? 1 : (MainGameWin + ReSpinWin + FSWin + ReSpinFSWin) / (TotalBet); }
            }

            public long SpinCounter { get; set; }
            public long FSpinCounter { get; set; }
            public decimal TotalBet { get; set; }

            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.FSpinCounter += target.FSpinCounter;
                source.TotalBet += target.TotalBet;
                source.MainGameWin += target.MainGameWin;
                source.ReSpinWin += target.ReSpinWin;
                source.FSWin += target.FSWin;
                source.ReSpinFSWin += target.ReSpinFSWin;

                source.MgFHit += target.MgFHit;
                source.MgReHit += target.MgReHit;
                source.FgReHit += target.FgReHit;
                source.FgFHit += target.FgFHit;
                return source;
            }
        }
    }
}