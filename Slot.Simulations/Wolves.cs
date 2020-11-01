namespace Slot.Simulations
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System;
    using Core.Modules.Infrastructure.Models;
    using Core.Modules.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.FSharp.Collections;
    using Model.Entity;
    using NUnit.Framework;
    using Slot.Games.Wolves;
    using Slot.Model;
    using static Slot.Games.Wolves.Domain;

    [TestFixture]
    public class Wolves
    {
        private const int GameId = 93;
        private static IGameModule module;

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
            var logger = logFactory.CreateLogger<WolvesModule.Engine>();
            module = new WolvesModule.Engine(logger);
        }

        [TestCase(1, 1000, 1000, 1.0, TestName = "[LVL1][1M] 1st Wolves ")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[LVL1][1M] 2nd Wolves ")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[LVL1][1M] 3rd Wolves ")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 1st Wolves ")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 2nd Wolves ")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[LVL1][10M] 3rd Wolves ")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 1st Wolves ")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 2nd Wolves ")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[LVL1][100M] 3rd Wolves ")]
        [TestCase(1, 30000, 10000, 1.0, TestName = "[LVL1][300M] Wolves ")]
        [TestCase(1, 10000, 100000, 1.0, TestName = "[LVL1][1B] Wolves ")]

        [TestCase(2, 1000, 1000, 1.0, TestName = "[LVL2][1M] 1st Wolves ")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[LVL2][1M] 2nd Wolves ")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[LVL2][1M] 3rd Wolves ")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 1st Wolves ")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 2nd Wolves ")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[LVL2][10M] 3rd Wolves ")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 1st Wolves ")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 2nd Wolves ")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[LVL2][100M] 3rd Wolves ")]
        [TestCase(2, 30000, 10000, 1.0, TestName = "[LVL2][300M] Wolves ")]
        [TestCase(2, 10000, 100000, 1.0, TestName = "[LVL2][1B] Wolves ")]

        [TestCase(3, 1000, 1000, 1.0, TestName = "[LVL3][1M] 1st Wolves ")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[LVL3][1M] 2nd Wolves ")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[LVL3][1M] 3rd Wolves ")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 1st Wolves ")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 2nd Wolves ")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[LVL3][10M] 3rd Wolves ")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 1st Wolves ")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 2nd Wolves ")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[LVL3][100M] 3rd Wolves ")]
        [TestCase(3, 30000, 10000, 1.0, TestName = "[LVL3][300M] Wolves ")]
        [TestCase(3, 10000, 100000, 1.0, TestName = "[LVL3][1B] Wolves ")]

        [TestCase(4, 1000, 1000, 1.0, TestName = "[LVL4][1M] 1st Wolves ")]
        [TestCase(4, 1000, 1000, 1.0, TestName = "[LVL4][1M] 2nd Wolves ")]
        [TestCase(4, 1000, 1000, 1.0, TestName = "[LVL4][1M] 3rd Wolves ")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[LVL4][10M] 1st Wolves ")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[LVL4][10M] 2nd Wolves ")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[LVL4][10M] 3rd Wolves ")]
        [TestCase(4, 10000, 10000, 1.0, TestName = "[LVL4][100M] 1st Wolves ")]
        [TestCase(4, 10000, 10000, 1.0, TestName = "[LVL4][100M] 2nd Wolves ")]
        [TestCase(4, 10000, 10000, 1.0, TestName = "[LVL4][100M] 3rd Wolves ")]
        [TestCase(4, 30000, 10000, 1.0, TestName = "[LVL4][300M] Wolves ")]
        [TestCase(4, 10000, 100000, 1.0, TestName = "[LVL4][1B] Wolves ")]

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
                    var request = user.CreateRequestContext<SpinArgs>("Wolves-hunter");
                    request.Parameters = spinArgs;
                    foreach (var iter in new byte[numItrPerUser])
                    {
                        data.SpinCounter++;
                        data.TotalBet += 25 * spinBet;
                        var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                        var result = executeResult.Value as WolvesResult;
                        data.TotalWin += result.Win;
                        switch (result.Wheel.Type)
                        {
                            case 1:
                                data.MainGameWin += result.Win;
                                break;
                            case 2: // Guaranteed
                                Assert.True(result.Win > 0);
                                data.SwHit++;
                                data.Guaranteed += result.Win;
                                break;

                            case 3: // WolvesHowling
                                data.WhHit++;
                                if (result.HasBonus)
                                    data.WhTriggerFGHit++;
                                else
                                    data.WhEpicHit++;
                                data.WhEpicWin += result.Win;
                                break;

                            default:
                                throw new Exception();
                        }

                        if (result.HasBonus)
                        {
                            var bonus = result.Bonus;
                            var bonusRequest = user.CreateRequestContext<BonusArgs>("Wolves-hunter");
                            var typeOfBonus = bonus.ClientId;
                            switch (typeOfBonus)
                            {
                                case 1:
                                    data.MgRollingHit++;
                                    data += ExecuteMainGameCollapsing(level, bonusRequest, result);
                                    break;

                                case 4:
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
            var fileStream = new FileStream($@"..\..\..\Results\Wolves\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                  : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart              : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd                : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("SpinCount                   : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                    : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("Rolling Hit Rate       (MG) : {0,12:0.00}", summData.MgRollingHitRate));
            Console.WriteLine(String.Format("Smashing Wild Hit Rate (MG) : {0,12:0.00}", summData.SwHitRate));
            Console.WriteLine(String.Format("Wolves Mode Hit Rate   (MG) : {0,12:0.00}", summData.WhHitRate));
            Console.WriteLine(String.Format("Epic Win Hit Rate (MG)      : {0,12:0.00}", summData.WhEpicHitRate));
            Console.WriteLine(String.Format("Feature Hit Rate       (MG) : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("Rolling Hit Rate       (FG) : {0,12:0.00}", summData.FgRollingHitRate));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("MG Win                      : {0,12:0.00}", summData.MainGameWin));
            Console.WriteLine(String.Format("MG Rolling Win              : {0,12:0.00}", summData.MgRollingWin));
            Console.WriteLine(String.Format("MG Smashing Wild Win        : {0,12:0.00}", summData.Guaranteed));
            Console.WriteLine(String.Format("MG Wolves Mode Epic Win     : {0,12:0.00}", summData.WhEpicWin));
            Console.WriteLine(String.Format("FG Win                      : {0,12:0.00}", summData.FSTotalWin));
            Console.WriteLine(String.Format("FG Rolling Win              : {0,12:0.00}", summData.FgRollingWin));
            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("MG RTP                     : {0,11:0.00}%", 100 * summData.MainGameWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG Rolling  RTP            : {0,11:0.00}%", 100 * summData.MgRollingWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG Smash Wild RTP          : {0,11:0.00}%", 100 * summData.Guaranteed / summData.TotalBet));
                Console.WriteLine(String.Format("MG Wolves Epic RTP         : {0,11:0.00}%", 100 * summData.WhEpicWin / summData.TotalBet));
                Console.WriteLine(String.Format("Total MG RTP               : {0,11:0.00}%", 100 * summData.RTPMainGame));
                Console.WriteLine(String.Format("FG RTP                     : {0,11:0.00}%", 100 * summData.FSTotalWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG Rolling RTP             : {0,11:0.00}%", 100 * summData.FgRollingWin / summData.TotalBet));
            }
            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,11:0.0000}%", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        [TestCase(0, TestName = "Full cycle Main Game A1", ExpectedResult = 4104.6334)]
        [TestCase(1, TestName = "Full cycle Main Game A2", ExpectedResult = 4320.0)]
        [TestCase(2, TestName = "Full cycle Main Game A3", ExpectedResult = 104.5161)]
        [TestCase(3, TestName = "Full cycle Main Game A4", ExpectedResult = 51.1071)]
        [TestCase(4, TestName = "Full cycle Main Game A5", ExpectedResult = 894.5996)]
        [TestCase(5, TestName = "Full cycle Main Game A6", ExpectedResult = 836.0526)]
        [TestCase(6, TestName = "Full cycle Main Game A7", ExpectedResult = 43.7498)]
        [TestCase(7, TestName = "Full cycle Main Game A8", ExpectedResult = 0)]
        [TestCase(8, TestName = "Full cycle Main Game A9", ExpectedResult = 65.4171)]
        [TestCase(9, TestName = "Full cycle Main Game A10", ExpectedResult = 79.5536)]
        [TestCase(10, TestName = "Full cycle Main Game A11", ExpectedResult = 52.8000)]
        public decimal TestFullcycleMainParsheetRtp(int stripId)
        {
            var strips = MainGame.strips(stripId).ToList();
            var totalBet = 0.0m;
            var totalWin = 0.0m;
            var reel1 = strips[0];
            var reel2 = strips[1];
            var reel3 = strips[2];
            var reel4 = strips[3];
            var reel5 = strips[4];
            for (int i1 = 0; i1 < reel1.Length; i1++)
                for (int i2 = 0; i2 < reel2.Length; i2++)
                    for (int i3 = 0; i3 < reel3.Length; i3++)
                        for (int i4 = 0; i4 < reel4.Length; i4++)
                            for (int i5 = 0; i5 < reel5.Length; i5++)
                            {
                                totalBet += 25m;
                                var wheel = new List<int[]>();
                                wheel.Add(Global.takeRolling(i1, 3, reel1));
                                wheel.Add(Global.takeRolling(i2, 3, reel2));
                                wheel.Add(Global.takeRolling(i3, 3, reel3));
                                wheel.Add(Global.takeRolling(i4, 3, reel4));
                                wheel.Add(Global.takeRolling(i5, 3, reel5));
                                var result = Payout.calculate(1, 1, ListModule.OfSeq(wheel));
                                totalWin += result.Payable;
                            }
            Console.WriteLine(String.Format("RTP.Total            : {0,12:0.0000000000}", 100 * totalWin / totalBet));
            return Math.Round(100 * totalWin / totalBet, 4);
        }

        [TestCase(0, TestName = "Full cycle rolling Main Game A1", ExpectedResult = 684.1056)]
        [TestCase(1, TestName = "Full cycle rolling Main Game A2", ExpectedResult = 2630.0000)]
        [TestCase(2, TestName = "Full cycle rolling Main Game A3", ExpectedResult = 17.4194)]
        [TestCase(3, TestName = "Full cycle rolling Main Game A4", ExpectedResult = 30.6429)]
        [TestCase(4, TestName = "Full cycle rolling Main Game A5", ExpectedResult = 152.5107)]
        [TestCase(5, TestName = "Full cycle rolling Main Game A6", ExpectedResult = 497.3246)]
        [TestCase(6, TestName = "Full cycle rolling Main Game A7", ExpectedResult = 23.0633)]
        [TestCase(7, TestName = "Full cycle rolling Main Game A8", ExpectedResult = 0)]
        [TestCase(8, TestName = "Full cycle rolling Main Game A9", ExpectedResult = 48)]
        [TestCase(9, TestName = "Full cycle rolling Main Game A10", ExpectedResult = 3.1607)]
        [TestCase(10, TestName = "Full cycle rolling Main Game A11", ExpectedResult = 17.4667)]
        public decimal TestFullcycleRollingMainParsheetRtp(int stripId)
        {
            var strips = MainGame.strips(stripId).ToList();
            var totalBet = 0.0m;
            var totalWin = 0.0m;
            var totalRollingWin = 0.0m;
            var rollingHits = 0;
            var totalSpin = 0m;
            var reel1 = strips[0];
            var reel2 = strips[1];
            var reel3 = strips[2];
            var reel4 = strips[3];
            var reel5 = strips[4];
            var profile = new Dictionary<int, int>();
            var totalRolling = 0;
            for (int i1 = 0; i1 < reel1.Length; i1++)
                for (int i2 = 0; i2 < reel2.Length; i2++)
                    for (int i3 = 0; i3 < reel3.Length; i3++)
                        for (int i4 = 0; i4 < reel4.Length; i4++)
                            for (int i5 = 0; i5 < reel5.Length; i5++)
                            {
                                totalSpin++;
                                totalBet += 25m;
                                var reels = new List<int[]>();
                                reels.Add(Global.takeRolling(i1, 3, reel1));
                                reels.Add(Global.takeRolling(i2, 3, reel2));
                                reels.Add(Global.takeRolling(i3, 3, reel3));
                                reels.Add(Global.takeRolling(i4, 3, reel4));
                                reels.Add(Global.takeRolling(i5, 3, reel5));
                                var result = Payout.calculate(1, 1, ListModule.OfSeq(reels));
                                totalWin += result.Payable;
                                if (reels.Sum(r => r.Count(ele => ele == 9)) > 2) continue;
                                if (result.Payable > 0)
                                {
                                    rollingHits++;
                                    totalRolling = 1;
                                    var reelRollings = new []
                                    {
                                        new Domain.Rolling(i1, 3),
                                        new Domain.Rolling(i2, 3),
                                        new Domain.Rolling(i3, 3),
                                        new Domain.Rolling(i4, 3),
                                        new Domain.Rolling(i5, 3)
                                    };
                                    var wheel = new Games.Wolves.Domain.WolvesWheel(1, ListModule.OfSeq(reels), ListModule.OfSeq(new List<int[]>()), ArrayModule.OfSeq(reelRollings));
                                    var regions = result.WinPositions.Select(item => new Domain.Region(1, item.RowPositions.ToArray())).ToArray();
                                    var wheelRolling = Games.Wolves.Rolling.breakWinning(regions.ToArray(), wheel);
                                    var wheelFilled = Games.Wolves.ParSheet.fillUpForMainGame(stripId, wheelRolling);
                                    var rollingResult = Payout.calculate(1, 1, wheelFilled.Reels);
                                    do
                                    {
                                        totalRollingWin += rollingResult.Payable;
                                        if (rollingResult.Payable == 0) break;
                                        if (wheelFilled.Reels.Sum(r => r.Count(ele => ele == 9)) > 2) break;
                                        totalRolling++;
                                        regions = rollingResult.WinPositions.Select(item => new Domain.Region(1, item.RowPositions.ToArray())).ToArray();
                                        wheel = Games.Wolves.Rolling.breakWinning(regions.ToArray(), wheelFilled);
                                        wheelFilled = Games.Wolves.ParSheet.fillUpForMainGame(stripId, wheel);
                                        rollingResult = Payout.calculate(1, 1, wheelFilled.Reels);
                                    } while (true);
                                    if (!profile.ContainsKey(totalRolling))
                                        profile[totalRolling] = 0;
                                    profile[totalRolling]++;
                                }
                            }
            foreach (var item in profile.OrderBy(item => item.Key))
                Console.WriteLine(String.Format("Tumble Hit  {0}: {1}", item.Key, item.Value));
            Console.WriteLine(String.Format("Rolling Hit Rate    : {0,12:0.0000000000}", rollingHits == 0 ? 0 : totalSpin / rollingHits));
            Console.WriteLine(String.Format("RTP.Main            : {0,12:0.0000000000}", 100 * totalWin / totalBet));
            Console.WriteLine(String.Format("RTP.Rolling         : {0,12:0.0000000000}", 100 * totalRollingWin / totalBet));
            return Math.Round(100 * totalRollingWin / totalBet, 4);
        }

        [TestCase(0, TestName = "Full cycle rolling Free Game FA1", ExpectedResult = 11434.2015)]
        [TestCase(1, TestName = "Full cycle rolling Free Game FA2", ExpectedResult = 229.0309)]
        [TestCase(2, TestName = "Full cycle rolling Free Game FA3", ExpectedResult = 193.2571)]
        [TestCase(3, TestName = "Full cycle rolling Free Game FA4", ExpectedResult = 67.5242)]
        [TestCase(4, TestName = "Full cycle rolling Free Game FA5", ExpectedResult = 852.75)]

        public decimal TestFullcycleRollingFreeGameParsheetRtp(int stripId)
        {
            var strips = FreeGame.Strips(stripId).ToList();
            var totalBet = 0.0m;
            var totalWin = 0.0m;
            var totalRollingWin = 0.0m;
            var rollingHits = 0;
            var totalSpin = 0m;
            var reel1 = strips[0];
            var reel2 = strips[1];
            var reel3 = strips[2];
            var reel4 = strips[3];
            var reel5 = strips[4];
            var profile = new Dictionary<int, int>();
            var totalRolling = 0;
            for (int i1 = 0; i1 < reel1.Length; i1++)
                for (int i2 = 0; i2 < reel2.Length; i2++)
                    for (int i3 = 0; i3 < reel3.Length; i3++)
                        for (int i4 = 0; i4 < reel4.Length; i4++)
                            for (int i5 = 0; i5 < reel5.Length; i5++)
                            {
                                totalSpin++;
                                totalBet += 25m;
                                var reels = new List<int[]>();
                                reels.Add(Global.takeRolling(i1, 3, reel1));
                                reels.Add(Global.takeRolling(i2, 3, reel2));
                                reels.Add(Global.takeRolling(i3, 3, reel3));
                                reels.Add(Global.takeRolling(i4, 3, reel4));
                                reels.Add(Global.takeRolling(i5, 3, reel5));
                                var result = Payout.calculate(1, 1, ListModule.OfSeq(reels));
                                totalWin += result.Payable;

                                if (result.Payable > 0)
                                {
                                    rollingHits++;
                                    totalRolling = 1;
                                    var multiplier = 1;
                                    var reelRollings = new []
                                    {
                                        new Domain.Rolling(i1, 3),
                                        new Domain.Rolling(i2, 3),
                                        new Domain.Rolling(i3, 3),
                                        new Domain.Rolling(i4, 3),
                                        new Domain.Rolling(i5, 3)
                                    };
                                    var wheel = new Games.Wolves.Domain.WolvesWheel(1, ListModule.OfSeq(reels), ListModule.OfSeq(new List<int[]>()), ArrayModule.OfSeq(reelRollings));
                                    var regions = result.WinPositions.Select(item => new Domain.Region(1, item.RowPositions.ToArray())).ToArray();
                                    var wheelRolling = Games.Wolves.Rolling.breakWinning(regions.ToArray(), wheel);
                                    var wheelFilled = Games.Wolves.ParSheet.fillUpForFreeGame(stripId, wheelRolling);
                                    do
                                    {
                                        if (multiplier < 5)
                                            multiplier++;
                                        else multiplier = 10;
                                        var rollingResult = Payout.calculate(1, multiplier, wheelFilled.Reels);
                                        if (rollingResult.Payable == 0) break;
                                        totalRolling++;
                                        totalRollingWin += rollingResult.Payable;
                                        regions = rollingResult.WinPositions.Select(item => new Domain.Region(1, item.RowPositions.ToArray())).ToArray();
                                        wheel = Games.Wolves.Rolling.breakWinning(regions.ToArray(), wheelFilled);
                                        wheelFilled = Games.Wolves.ParSheet.fillUpForFreeGame(stripId, wheel);
                                    } while (true);
                                    if (!profile.ContainsKey(totalRolling))
                                        profile[totalRolling] = 0;
                                    profile[totalRolling]++;
                                }
                            }
            foreach (var item in profile.OrderBy(item => item.Key))
                Console.WriteLine(String.Format("Tumble Hit  {0}: {1}", item.Key, item.Value));
            Console.WriteLine(String.Format("Rolling Hit Rate    : {0,12:0.0000000000}", rollingHits == 0 ? 0 : totalSpin / rollingHits));
            Console.WriteLine(String.Format("RTP.Main            : {0,12:0.0000000000}", 100 * totalWin / totalBet));
            Console.WriteLine(String.Format("RTP.Rolling         : {0,12:0.0000000000}", 100 * totalRollingWin / totalBet));
            return Math.Round(100 * totalRollingWin / totalBet, 4);
        }

        [TestCase(0.35, TestName = "Full cycle B1", ExpectedResult = 303.20)]
        [TestCase(0.66, TestName = "Full cycle B2", ExpectedResult = 315.19)]
        [TestCase(0.97, TestName = "Full cycle B3", ExpectedResult = 235.63)]
        [TestCase(0.98, TestName = "Full cycle B4", ExpectedResult = 2421.40)]
        [TestCase(0.99, TestName = "Full cycle B5", ExpectedResult = 1528.49)]
        [TestCase(1.00, TestName = "Full cycle B6", ExpectedResult = 1595.20)]
        public decimal TestFullcycleSmashingParsheetRtp(double ratio)
        {
            var strips = SmashingWild.getStripsSet(ratio).ToList();
            var totalBet = 0.0m;
            var totalWin = 0.0m;
            var reel1 = strips[0];
            var reel2 = strips[1];
            var reel3 = strips[2];
            var reel4 = strips[3];
            var reel5 = strips[4];
            for (int i1 = 0; i1 < reel1.Length; i1++)
                for (int i2 = 0; i2 < reel2.Length; i2++)
                    for (int i3 = 0; i3 < reel3.Length; i3++)
                        for (int i4 = 0; i4 < reel4.Length; i4++)
                            for (int i5 = 0; i5 < reel5.Length; i5++)
                            {
                                totalBet += 25m;
                                var wheel = new List<int[]>();
                                wheel.Add(Global.takeRolling(i1, 3, reel1));
                                wheel.Add(Global.takeRolling(i2, 3, reel2));
                                wheel.Add(Global.takeRolling(i3, 3, reel3));
                                wheel.Add(Global.takeRolling(i4, 3, reel4));
                                wheel.Add(Global.takeRolling(i5, 3, reel5));
                                var result = Payout.calculate(1, 1, ListModule.OfSeq(wheel));
                                totalWin += result.Payable;
                            }
            Console.WriteLine(String.Format("RTP.Total            : {0,12:0.0000000000}", 100 * totalWin / totalBet));
            return Math.Round(100 * totalWin / totalBet, 2);
        }

        [TestCase(0.35, TestName = "SMASHING WILD MODE B1")]
        [TestCase(0.66, TestName = "SMASHING WILD MODE B2")]
        [TestCase(0.97, TestName = "SMASHING WILD MODE B3")]
        [TestCase(0.98, TestName = "SMASHING WILD MODE B4")]
        [TestCase(0.99, TestName = "SMASHING WILD MODE B5")]
        [TestCase(1.00, TestName = "SMASHING WILD MODE B6")]
        public void TestSmashingWildRtp(double ratio)
        {
            var spinBet = new SpinBet(new UserGameKey(-1, 93), PlatformType.None) { LineBet = 1, Multiplier = 1, Lines = 25 };
            var totalBet = 0.0m;
            var totalWin = 0.0m;
            var profile = new Dictionary<int, decimal>();
            for (var iter = 0; iter < 1000000; iter++)
            {
                totalBet += 25m;
                var result = GameReduce.wolvesSmashing(1, ratio, spinBet);
                Assert.IsTrue(result.Win > 0);
                totalWin += result.Win;
            }
            Console.WriteLine(String.Format("RTP.Total            : {0,12:0.0000000000}", 100 * totalWin / totalBet));
        }

        private static SummaryData ExecuteFreeSpin(int level, RequestContext<BonusArgs> bonusRequestcontext, WolvesResult result)
        {
            var bonus = module.CreateBonus(result);
            var summData = new SummaryData();
            var freeSpinBonus = bonus.Value as WolvesBonus;
            var totalFreeSpin = 0;
            var fscouter = 0;
            switch (result.Wheel.Reels.Sum(ele => ele.Count(item => item == 9)))
            {
                case 3:
                    totalFreeSpin = 10;
                    break;
                case 4:
                    totalFreeSpin = 15;
                    break;
                case 5:
                    totalFreeSpin = 20;
                    break;
            }
            do
            {
                var freeSpinResult = BonusReducer.dispatch(level, freeSpinBonus, bonusRequestcontext) as WolvesSeqFreeSpinResult;
                var spinResult = freeSpinResult.SpinResult;
                if (spinResult.Wheel.Type == 4) summData.FgRollingWin += spinResult.Win;
                else
                {
                    summData.FSpinCounter++;
                    fscouter++;
                    summData.FSTotalWin += spinResult.Win;
                }

                if (spinResult.HasBonus)
                {
                    summData.FgRollingHit++;
                    Assert.True(spinResult.Win > 0);
                }

                if (freeSpinResult.IsCompleted) break;
                freeSpinBonus = freeSpinResult.Bonus as WolvesBonus;
            } while (true);

            Assert.AreEqual(totalFreeSpin, fscouter);
            return summData;
        }

        private static SummaryData ExecuteMainGameCollapsing(int level, RequestContext<BonusArgs> bonusRequestcontext, WolvesResult result)
        {
            var bonus = module.CreateBonus(result);
            var summData = new SummaryData();
            var freeSpinBonus = bonus.Value as WolvesBonus;
            do
            {
                var freeSpinResult = BonusReducer.dispatch(level, freeSpinBonus, bonusRequestcontext) as WolvesSeqFreeSpinResult;
                summData.MgRollingWin += freeSpinResult.Win;
                if (freeSpinResult.SpinResult.HasBonus)
                {
                    if (freeSpinResult.SpinResult.Bonus.ClientId == 4)
                    {
                        summData.MgFHit++;
                        summData += ExecuteFreeSpin(level, bonusRequestcontext, freeSpinResult.SpinResult);
                        break;
                    }
                }

                if (freeSpinResult.IsCompleted)
                    break;
                freeSpinBonus = freeSpinResult.Bonus as WolvesBonus;
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
            public int FgRollingHit { get; set; }

            public decimal FgRollingHitRate
            {
                get { return (decimal) FSpinCounter / (FgRollingHit > 0 ? FgRollingHit : 1); }
            }

            public decimal FgRollingWin { get; set; }
            public long FSpinCounter { get; set; }
            public decimal FSTotalWin { get; set; }
            public decimal Guaranteed { get; set; }
            public decimal MainGameWin { get; set; }
            public int MgFHit { get; set; }

            public decimal MgFHitRate
            {
                get { return (decimal) SpinCounter / (MgFHit > 0 ? MgFHit : 1); }
            }

            public int MgRollingHit { get; set; }

            public decimal MgRollingHitRate
            {
                get { return (decimal) SpinCounter / (MgRollingHit > 0 ? MgRollingHit : 1); }
            }

            public decimal MgRollingWin { get; set; }

            public decimal RTPMainGame
            {
                get { return TotalBet == 0 ? 1 : (WhEpicWin + MainGameWin + Guaranteed + MgRollingWin) / (TotalBet); }
            }

            public decimal RTPOverAll
            {
                get { return TotalBet == 0 ? 1 : (WhEpicWin + MainGameWin + Guaranteed + MgRollingWin + FSTotalWin + FgRollingWin) / (TotalBet); }
            }

            public long SpinCounter { get; set; }
            public int SwHit { get; set; }

            public decimal SwHitRate
            {
                get { return (decimal) SpinCounter / (SwHit > 0 ? SwHit : 1); }
            }

            public decimal TotalBet { get; set; }
            public decimal TotalWin { get; set; }
            public int WhEpicHit { get; set; }

            public decimal WhEpicHitRate
            {
                get { return (decimal) SpinCounter / (WhEpicHit > 0 ? WhEpicHit : 1); }
            }

            public decimal WhEpicWin { get; set; }
            public int WhHit { get; set; }

            public decimal WhHitRate
            {
                get { return (decimal) SpinCounter / (WhHit > 0 ? WhHit : 1); }
            }

            public int WhTriggerFGHit { get; set; }

            public decimal WhTriggerFGHitRate
            {
                get { return (decimal) SpinCounter / (WhTriggerFGHit > 0 ? WhTriggerFGHit : 1); }
            }

            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.FSpinCounter += target.FSpinCounter;
                source.TotalBet += target.TotalBet;
                source.MainGameWin += target.MainGameWin;
                source.MgRollingWin += target.MgRollingWin;
                source.Guaranteed += target.Guaranteed;
                source.WhEpicWin += target.WhEpicWin;
                source.FSTotalWin += target.FSTotalWin;
                source.FgRollingWin += target.FgRollingWin;

                source.MgRollingHit += target.MgRollingHit;
                source.SwHit += target.SwHit;
                source.WhHit += target.WhHit;
                source.WhTriggerFGHit += target.WhTriggerFGHit;
                source.WhEpicHit += target.WhEpicHit;
                source.MgFHit += target.MgFHit;
                source.FgRollingHit += target.FgRollingHit;
                return source;
            }
        }
    }
}