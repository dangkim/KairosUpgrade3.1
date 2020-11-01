using Slot.Core.RandomNumberGenerators;

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
    using NUnit.Framework;
    using Slot.Games.FuDaoLe;
    using Slot.Model;

    [TestFixture]
    public class FuDaoLe
    {
        private const int GameId = 99;
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
            var logger = logFactory.CreateLogger<Slot.Games.FuDaoLe.Module>();
            module = new Slot.Games.FuDaoLe.Module(logger);
        }

        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 1st FuDaoLe ")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 2nd FuDaoLe ")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 3rd FuDaoLe ")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 1st FuDaoLe ")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 2nd FuDaoLe ")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 3rd FuDaoLe ")]
        [TestCase(1, 10000, 100000, 1.0, TestName = "[LVL1][1B] 1st FuDaoLe ")]

        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 1st FuDaoLe ")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 2nd FuDaoLe ")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 3rd FuDaoLe ")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 1st FuDaoLe ")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 2nd FuDaoLe ")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 3rd FuDaoLe ")]
        [TestCase(2, 10000, 100000, 1.0, TestName = "[LVL2][1B] FuDaoLe ")]

        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 1st FuDaoLe ")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 2nd FuDaoLe ")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 3rd FuDaoLe ")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 1st FuDaoLe ")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 2nd FuDaoLe ")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 3rd FuDaoLe ")]
        [TestCase(3, 10000, 100000, 1.0, TestName = "[LVL3][1B] FuDaoLe ")]


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
                var request = user.CreateRequestContext<SpinArgs>("fudaole");
                request.Parameters = spinArgs;
                foreach (var iter in new byte[numItrPerUser])
                {
                    data.SpinCounter++;
                    data.TotalBet += 38 * spinBet;
                    var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                    var result = executeResult.Value as FuDaoLeResult;
                    if (result.Wheel.Reels[2].All(ele => ele == 15))
                        data.ExpandingHit++;
                    if (result.Win <= 0)
                        continue;
                    var normalWinning = result.WinPositions.Where(item => item.Line > 0).ToList();
                    var nonNormalWinning = result.WinPositions.Where(item => item.Line == 0).ToList();

                    data.MainGameWin += normalWinning.Sum(ele => ele.Win);
                    data.EnvelopeJackpotWin += nonNormalWinning.Where(ele => ele.Symbol < 14).Sum(item => item.Win);
                    data.ScatterWin += nonNormalWinning.Where(ele => ele.Symbol == 14).Sum(item => item.Win);

                    if (result.HasBonus)
                    {
                        var bonusRequest = user.CreateRequestContext<BonusArgs>("fudaole");
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
            var fileStream = new FileStream($@"..\..\..\Results\FuDaoLe\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                 : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart             : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd               : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.SpinMode              : {0}", "Random"));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("SpinCount                  : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("Expanding Hit              : {0}", summData.ExpandingHit));
            Console.WriteLine(String.Format("Scatter Hit                : {0}", summData.MgFHit));
            Console.WriteLine(String.Format("TotalBet                   : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine(String.Format("Game Win                   : {0,12:0.00}", summData.MainGameWin));
            Console.WriteLine(String.Format("Scatter Win                : {0,12:0.00}", summData.ScatterWin));
            Console.WriteLine(String.Format("Envelope Jackpot Win       : {0,12:0.00}", summData.EnvelopeJackpotWin));
            Console.WriteLine(String.Format("Free Spin Win              : {0,12:0.00}", summData.FSTotalWin));
            Console.WriteLine(String.Format("Free SpinCount             : {0,12:0.00}", summData.FSpinCounter));
            Console.WriteLine(String.Format("Av Num of Free Games       : {0,12:0.00}", (decimal)summData.FSpinCounter / summData.MgFHit));
            Console.WriteLine(String.Format("Expanding Hit Rate         : {0,12:0.00}", summData.ExpandingHitHitRate));
            Console.WriteLine(String.Format("MG Feature Hit Rate        : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("FG Feature Hit Rate        : {0,12:0.00}", summData.FgFHitRate));
            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("Main Game Non Scatter RTP  : {0,11:0.00}%", 100 * summData.MainGameWin / summData.TotalBet));
                Console.WriteLine(String.Format("Scatter RTP                : {0,11:0.00}%", 100.0m * summData.ScatterWin / summData.TotalBet));
                Console.WriteLine(String.Format("Envelope Jackpot RTP       : {0,11:0.00}%", 100 * summData.EnvelopeJackpotWin / summData.TotalBet));
                Console.WriteLine(String.Format("Free Spin RTP              : {0,11:0.00}%", 100 * summData.FSTotalWin / summData.TotalBet));
            }

            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)       : {0,11:0.00}%", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        private static SummaryData ExecuteFreeSpin(int level, RequestContext<BonusArgs> bonusRequestcontext, FuDaoLeResult result)
        {
            var bonus = module.CreateBonus(result);
            var summData = new SummaryData();
            var freeSpinBonus = bonus.Value as FreeSpinBonus;
            do
            {
                summData.FSpinCounter++;
                var mystery = ParSheet.GetFreeGameMysteryIndex(RandomNumberEngine.NextDouble());
                var wheel = ParSheet.CreateWheelForFreeSpin(level, mystery);
                var freeSpinResult = Engine.ClaimBonus(level, freeSpinBonus, bonusRequestcontext, wheel) as FuDaoLeFreeSpinResult;
                summData.FSTotalWin += freeSpinResult.Win;
                if (freeSpinResult.SpinResult.HasBonus)
                    summData.FgFHit++;

                if (freeSpinResult.IsCompleted)
                    break;
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

            public int FgFHit { get; set; }
            public long FSpinCounter { get; set; }
            public decimal FSTotalWin { get; set; }
            public decimal MainGameWin { get; set; }
            public int MgFHit { get; set; }
            public int ExpandingHit { get; set; }
            public long SpinCounter { get; set; }
            public decimal TotalBet { get; set; }
            public decimal EnvelopeJackpotWin { get; set; }
            public decimal ScatterWin { get; set; }

            public decimal ExpandingHitHitRate
            {
                get { return (decimal)SpinCounter / (ExpandingHit > 0 ? ExpandingHit : 1); }
            }

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
                get { return TotalBet == 0 ? 1 : (EnvelopeJackpotWin + MainGameWin + ScatterWin + FSTotalWin) / (TotalBet); }
            }

            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.FSpinCounter += target.FSpinCounter;
                source.ExpandingHit += target.ExpandingHit;
                source.TotalBet += target.TotalBet;
                source.MgFHit += target.MgFHit;
                source.FgFHit += target.FgFHit;

                source.MainGameWin += target.MainGameWin;
                source.EnvelopeJackpotWin += target.EnvelopeJackpotWin;
                source.ScatterWin += target.ScatterWin;
                source.FSTotalWin += target.FSTotalWin;
                return source;
            }
        }
    }
}