namespace Slot.Simulations
{
    using Core.Modules.Infrastructure;
    using Core.Modules.Infrastructure.Models;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Model.Entity;
    using NUnit.Framework;
    using Slot.Games.FortunePack;
    using System;
    using System.IO;
    using System.Linq;

    [TestFixture]
    public class FortunePack
    {
        private const int GameId = 14;
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
            var logger = logFactory.CreateLogger<FortunePackModule>();
            module = new FortunePackModule(logger);
        }

        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 1st Fortune Pack")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 2nd Fortune Pack")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 3rd Fortune Pack")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 1st Fortune Pack")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 2nd Fortune Pack")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 3rd Fortune Pack")]

        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 1st Fortune Pack")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 2nd Fortune Pack")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 3rd Fortune Pack")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 1st Fortune Pack")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 2nd Fortune Pack")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 3rd Fortune Pack")]

        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 1st Fortune Pack")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 2nd Fortune Pack")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 3rd Fortune Pack")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 1st Fortune Pack")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 2nd Fortune Pack")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 3rd Fortune Pack")]

        [TestCase(4, 1000, 10000, 1.0, TestName = "[LVL4][10M] 1st Fortune Pack")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[LVL4][10M] 2nd Fortune Pack")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[LVL4][10M] 3rd Fortune Pack")]
        [TestCase(4, 10000, 10000, 1.0, TestName = "[LVL4][100M] 1st Fortune Pack")]
        [TestCase(4, 10000, 10000, 1.0, TestName = "[LVL4][100M] 2nd Fortune Pack")]
        [TestCase(4, 10000, 10000, 1.0, TestName = "[LVL4][100M] 3rd Fortune Pack")]

        [TestCase(5, 1000, 10000, 1.0, TestName = "[LVL5][10M] 1st Fortune Pack")]
        [TestCase(5, 1000, 10000, 1.0, TestName = "[LVL5][10M] 2nd Fortune Pack")]
        [TestCase(5, 1000, 10000, 1.0, TestName = "[LVL5][10M] 3rd Fortune Pack")]
        [TestCase(5, 10000, 10000, 1.0, TestName = "[LVL5][100M] 1st Fortune Pack")]
        [TestCase(5, 10000, 10000, 1.0, TestName = "[LVL5][100M] 2nd Fortune Pack")]
        [TestCase(5, 10000, 10000, 1.0, TestName = "[LVL5][100M] 3rd Fortune Pack")]

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
                    var request = user.CreateRequestContext<SpinArgs>("FortunePack");
                    request.Parameters = spinArgs;
                    foreach (var iter in new byte[numItrPerUser])
                    {
                        data.SpinCounter++;
                        data.TotalBet += 8 * spinBet;

                        var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                        var result = executeResult.Value as FortunePackResult;

                        data.TotalWin += result.Win;
                    }
                    return data;
                }).AsEnumerable()
                .Aggregate((s1, s2) => s1 + s2);

            var edt = DateTime.Now;
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\FortunePack\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
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
            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,11:0.00}%", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        /// <summary>
        /// Defines the <see cref="SummaryData"/>
        /// </summary>
        private class SummaryData
        {
            public SummaryData()
            {
                SpinCounter = 0;
            }

            public decimal RTPOverAll
            {
                get { return TotalBet == 0 ? 1 : TotalWin / TotalBet; }
            }

            public long SpinCounter { get; set; }

            public decimal TotalBet { get; set; }
            public decimal TotalWin { get; set; }

            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.TotalBet += target.TotalBet;
                source.TotalWin += target.TotalWin;
                return source;
            }
        }
    }
}