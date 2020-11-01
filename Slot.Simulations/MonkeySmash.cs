namespace Slot.Simulations {
    using System.IO;
    using System.Linq;
    using System;
    using Core.Modules.Infrastructure.Models;
    using Core.Modules.Infrastructure;
    using Games.ReelGems;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Model.Entity;
    using Model;
    using NUnit.Framework;
    [TestFixture]
    public class MonkeySmash {
        private const int GameId = 85;
        private static IGameModule module;
        /// <summary>
        /// Defines the <see cref="SummaryData" />
        /// </summary>
        private class SummaryData {
            public SummaryData() {
                BuyReel = new BuyReelSummaryData();
            }
            public decimal FSTotalWin { get; set; }
            public decimal RTPOverAll {
                get { return TotalBet == 0 ? 1 : (TotalWin + FSTotalWin + BuyReel.TotalWin + BuyReel.FSTotalWin) / (TotalBet + BuyReel.TotalBet); }
            }
            public long SpinCounter { get; set; }
            public decimal TotalBet { get; set; }
            public decimal TotalWin { get; set; }
            public BuyReelSummaryData BuyReel { get; set; }
            public static SummaryData operator +(SummaryData source, SummaryData target) {
                source.SpinCounter += target.SpinCounter;
                source.TotalBet += target.TotalBet;
                source.TotalWin += target.TotalWin;
                source.FSTotalWin += target.FSTotalWin;
                source.BuyReel += target.BuyReel;
                return source;
            }
        }
        private class BuyReelSummaryData {
            public decimal FSTotalWin { get; set; }
            public long SpinCounter { get; set; }
            public decimal TotalBet { get; set; }
            public decimal TotalWin { get; set; }
            public bool IsContinuosBuyReel { get; set; }
            public static BuyReelSummaryData operator +(BuyReelSummaryData source, BuyReelSummaryData target) {
                source.SpinCounter += target.SpinCounter;
                source.TotalBet += target.TotalBet;
                source.TotalWin += target.TotalWin;
                source.FSTotalWin += target.FSTotalWin;
                return source;
            }
        }
        /// <summary>
        /// Dependency resolve and simulation setting stuff
        /// </summary>
        [SetUp]
        public void Settup() {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddDistributedMemoryCache()
                .BuildServiceProvider();
            var logFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = logFactory.CreateLogger<ReelGemsModule>();
            module = new ReelGemsModule(logger);            
        }

        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] ReelGems")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] ReelGems")]
        public void Spin(int level, int numusers, int numItrPerUser, decimal spinBet) {
            var sdt = DateTime.Now;
            var spinArgs = new SpinArgs {
                LineBet = spinBet,
                Multiplier = 1
            };
            
            var users = Utilities.GenerateUsers(GameId, numusers, level);
            var summData = users
                .AsParallel()
                .Select(user => {
                    var data = new SummaryData();
                    var request = user.CreateRequestContext<SpinArgs>("monkeysmash");
                    request.Parameters = spinArgs;
                    foreach (var iter in new byte[numItrPerUser]) {
                        data.SpinCounter++;
                        data.TotalBet += spinBet * 25;
                        var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                        var result = executeResult.Value as ReelGemResult;
                        data.TotalWin += result.Win;
                        if (result.HasBonus) {
                            var bonusRequest = user.CreateRequestContext<BonusArgs>("monkeysmash");
                            var freeSpinResult = ExecuteFreeSpin(level, bonusRequest, result);
                            data.FSTotalWin += freeSpinResult.TotalWin;
                        } else {
                            var priorSpinResult = result.Copy();
                            for (int pr = 0; pr < 5; pr++) {
                                var buyReelResult = BuyReel(level, pr, priorSpinResult);
                                data.BuyReel += buyReelResult;
                            }
                        }
                    }
                    return data;
                })
                .AsEnumerable()
                .Aggregate((s1, s2) => s1 + s2);           

            var edt = DateTime.Now;
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\MonkeySmash\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart            : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd              : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.SpinMode             : {0}", "Random"));
            Console.WriteLine("---NORMAL -------------------------------");
            Console.WriteLine(String.Format("Test.SpinCount            : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                  : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine(String.Format("Game Win                  : {0,12:0.00}", summData.TotalWin));
            Console.WriteLine(String.Format("Free Spin Win             : {0,12:0.00}", summData.FSTotalWin));
            if (summData.TotalBet > 0) {
            Console.WriteLine(String.Format("Main Game RTP             : {0,12:0.00}", 100 * summData.TotalWin / summData.TotalBet));
            Console.WriteLine(String.Format("Free Spin RTP             : {0,12:0.00}", 100 * summData.FSTotalWin / summData.TotalBet));
            Console.WriteLine(String.Format("Total RTP                 : {0,12:0.00}", 100 * (summData.TotalWin + summData.FSTotalWin) / summData.TotalBet));
            }
            var buyReelSum = summData.BuyReel;
            Console.WriteLine("--- BUY REEL----------------------------");
            Console.WriteLine(String.Format("Test.SpinCount            : {0}", buyReelSum.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                  : {0,12:0.00}", buyReelSum.TotalBet));
            Console.WriteLine(String.Format("Game Win                  : {0,12:0.00}", buyReelSum.TotalWin));
            Console.WriteLine(String.Format("Free Spin Win             : {0,12:0.00}", buyReelSum.FSTotalWin));
            if (summData.BuyReel.TotalBet > 0) {
            Console.WriteLine(String.Format("Main Game RTP             : {0,12:0.00}", 100 * buyReelSum.TotalWin / buyReelSum.TotalBet));
            Console.WriteLine(String.Format("Free Spin RTP             : {0,12:0.00}", 100 * buyReelSum.FSTotalWin / buyReelSum.TotalBet));
            Console.WriteLine(String.Format("Total RTP                 : {0,12:0.00}", 100 * (buyReelSum.TotalWin + buyReelSum.FSTotalWin) / buyReelSum.TotalBet));
            }
            Console.WriteLine("--- RTP.OverAll -------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,12:0.00}", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        [TestCase(1, 0, 1000000, "3,10,5,5,10,1,10,1,11,10,1,8,3,10,0", TestName = "[LVL1][1M] RESPIN 1st REEL")]
        [TestCase(1, 1, 1000000, "3,10,5,5,10,1,10,1,11,10,1,8,3,10,0", TestName = "[LVL1][1M] RESPIN 2nd REEL")]
        [TestCase(1, 2, 1000000, "3,10,5,5,10,1,10,1,11,10,1,8,3,10,0", TestName = "[LVL1][1M] RESPIN 3rd REEL")]
        [TestCase(1, 3, 1000000, "3,10,5,5,10,1,10,1,11,10,1,8,3,10,0", TestName = "[LVL1][1M] RESPIN 4th REEL")]
        [TestCase(1, 4, 1000000, "3,10,5,5,10,1,10,1,11,10,1,8,3,10,0", TestName = "[LVL1][1M] RESPIN 5th REEL")]
        public void ReSpinMode(int level, int purchaseReel, int numItrPerUser, string stringWheel) {
            var sdt = DateTime.Now;
            var summData = new SummaryData {
                SpinCounter = numItrPerUser,
                TotalBet = 0
            };
            var spin = new SpinArgs {
                LineBet = 1,
                Multiplier = 1
            };
            var user = new UserGameKey(-1, GameId) { Level = level };
            var requestContext = user.CreateRequestContext<SpinArgs>("monkeysmash");
            requestContext.Parameters = spin;
            var bonusRequestcontext = user.CreateRequestContext<BonusArgs>("monkeysmash");
            var reels = Utilities.Encoding(stringWheel);
            var priorSpinResult = new ReelGemResult() {
                Wheel = Utilities.Encoding(stringWheel, 5, 3),
                ReelRespinCredits = ReelGemsEngine.CalcWagerCost(level, reels)
            };
            foreach (var item in new sbyte[numItrPerUser]) {
                summData.TotalBet += priorSpinResult.ReelRespinCredits[purchaseReel];
                var result = ReelGemsEngine.BuyReel(level, purchaseReel, priorSpinResult, requestContext);
                Assert.NotNull(result);
                summData.TotalWin += result.Win;
                if (result.HasBonus) {
                    var freeSpinResult = ExecuteFreeSpin(level, bonusRequestcontext, result);
                    summData.FSTotalWin += freeSpinResult.TotalWin;
                }
            }
            PrintResult(level, sdt, DateTime.Now, summData, TestContext.CurrentContext.Test.Name);
        }
        private static BuyReelSummaryData BuyReel(int level, int purchaseReel, ReelGemResult priorSpinResult) {
            var summData = new BuyReelSummaryData {
                TotalBet = 0,
                IsContinuosBuyReel = true,
                SpinCounter = 1
            };
            var user = new UserGameKey(-1, GameId) { Level = level };
            var requestContext = user.CreateRequestContext<SpinArgs>("monkeysmash");
            var bonusRequestcontext = user.CreateRequestContext<BonusArgs>("monkeysmash");
            requestContext.Parameters = new SpinArgs {
                LineBet = 1,
                Multiplier = 1
            };
            summData.TotalBet = priorSpinResult.ReelRespinCredits[purchaseReel];
            var result = ReelGemsEngine.BuyReel(level, purchaseReel, priorSpinResult, requestContext);
            summData.TotalWin = result.Win;
            if (result.HasBonus) {
                var freeSpinResult = ExecuteFreeSpin(level, bonusRequestcontext, result);
                summData.FSTotalWin += freeSpinResult.TotalWin;
                summData.IsContinuosBuyReel = false;
            }
            return summData;
        }
        private static SummaryData ExecuteFreeSpin(int level, RequestContext<BonusArgs> bonusRequestcontext, ReelGemResult result) {
            var bonus = module.CreateBonus(result);
            var summData = new SummaryData();
            var freeSpinBonus = bonus.Value as FreeSpinBonus;
            do {
                var freeSpinResult = ReelGemsEngine.ClaimBonus(level, freeSpinBonus, bonusRequestcontext);
                summData.TotalWin += freeSpinResult.Win;
                if (freeSpinResult.IsCompleted)
                    break;
            } while (true);
            return summData;
        }
        private static void PrintResult(int level, DateTime sdt, DateTime edt, SummaryData summData, string output) {
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\MonkeySmash\{output}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.SpinCount             : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("Test.Level                 : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart             : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd               : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.SpinMode              : {0}", "Random"));
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine(String.Format("Main Game Win              : {0,12:0.00}", summData.TotalWin));
            Console.WriteLine(String.Format("Free Spin Win              : {0,12:0.00}", summData.FSTotalWin));
            Console.WriteLine(String.Format("TotalBet                   : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine("--- RTP ---------------------------------------------------");
            Console.WriteLine(String.Format("Main Game RTP              : {0,12:0.0000000}", 100 * summData.TotalWin / summData.TotalBet));
            Console.WriteLine(String.Format("Free Spin RTP              : {0,12:0.0000000}", 100 * summData.FSTotalWin / summData.TotalBet));
            Console.WriteLine(String.Format("RTP.Total                  : {0,12:0.0000000}", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }
    }
}