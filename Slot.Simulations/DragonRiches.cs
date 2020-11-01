namespace Slot.Simulations
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.DragonRiches;
    using Slot.Games.DragonRiches.Configuration;
    using Slot.Model;
    using Slot.Model.Entity;
    using System;
    using System.IO;
    using System.Linq;

    [TestFixture]
    public class DragonRiches
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
            var logger = logFactory.CreateLogger<DragonRichesModule>();
            module = new DragonRichesModule(logger);
        }

        [TestCase(1, 1000, 10000, 1.0, TestName = "[96.98][LVL1][10M] 1st Dragon Riches")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[96.98][LVL1][10M] 2nd Dragon Riches")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[96.98][LVL1][10M] 3rd Dragon Riches")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[96.98][LVL1][100M] 1st Dragon Riches")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[96.98][LVL1][100M] 2nd Dragon Riches")]
        [TestCase(1, 10000, 10000, 1.0, TestName = "[96.98][LVL1][100M] 3rd Dragon Riches")]
        [TestCase(1, 10000, 100000, 1.0, TestName = "[96.98][LVL1][1B] 3rd Dragon Riches")]
        [TestCase(1, 10000, 1000000, 1.0, TestName = "[96.98][LVL1][10B] 3rd Dragon Riches")]

        [TestCase(2, 1000, 10000, 1.0, TestName = "[96.51][LVL2][10M] 1st Dragon Riches")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[96.51][LVL2][10M] 2nd Dragon Riches")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[96.51][LVL2][10M] 3rd Dragon Riches")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[96.51][LVL2][100M] 1st Dragon Riches")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[96.51][LVL2][100M] 2nd Dragon Riches")]
        [TestCase(2, 10000, 10000, 1.0, TestName = "[96.51][LVL2][100M] 3rd Dragon Riches")]

        [TestCase(3, 1000, 10000, 1.0, TestName = "[96.03][LVL3][10M] 1st Dragon Riches")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[96.03][LVL3][10M] 2nd Dragon Riches")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[96.03][LVL3][10M] 3rd Dragon Riches")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[96.03][LVL3][100M] 1st Dragon Riches")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[96.03][LVL3][100M] 2nd Dragon Riches")]
        [TestCase(3, 10000, 10000, 1.0, TestName = "[96.03][LVL3][100M] 3rd Dragon Riches")]

        [TestCase(4, 1000, 10000, 1.0, TestName = "[95.02][LVL4][10M] 1st Dragon Riches")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[95.02][LVL4][10M] 2nd Dragon Riches")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[95.02][LVL4][10M] 3rd Dragon Riches")]
        [TestCase(4, 10000, 10000, 1.0, TestName = "[95.02][LVL4][100M] 1st Dragon Riches")]
        [TestCase(4, 10000, 10000, 1.0, TestName = "[95.02][LVL4][100M] 2nd Dragon Riches")]
        [TestCase(4, 10000, 10000, 1.0, TestName = "[95.02][LVL4][100M] 3rd Dragon Riches")]

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
                    var request = user.CreateRequestContext<SpinArgs>("dragon-riches");
                    request.Parameters = spinArgs;
                    foreach (var iter in new byte[numItrPerUser])
                    {
                        data.SpinCounter++;
                        data.TotalBet += 30 * spinBet;
                        var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                        var result = executeResult.Value as DragonRichesResult;
                        data.MainGameWin += result.Win;

                        if (result.HasBonus)
                        {
                            var bonus = result.Bonus;
                            var typeOfBonus = bonus.ClientId;
                            switch (typeOfBonus)
                            {
                                case 4:
                                    data.MgHoldSpinHit++;

                                    break;

                                case 3:
                                    data.MgFHit++;
                                    break;

                                default:
                                    throw new Exception();
                            }

                            var bonusRequest = user.CreateRequestContext<BonusArgs>("dragonriches");
                            var bonusResult = ExecuteBonus(level, bonusRequest, result);
                            data += bonusResult;
                        }
                    }
                    return data;
                }).AsEnumerable()
                .Aggregate((s1, s2) => s1 + s2);

            var edt = DateTime.Now;
            var oldOut = Console.Out;
            var fileStream = new FileStream($@"..\..\..\Results\DragonRiches\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                  : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart              : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd                : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("SpinCount                   : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                    : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("Hold Spin Hit Rate     (MG) : {0,12:0.00}", summData.MgHoldSpinHitRate));
            Console.WriteLine(String.Format("Feature Hit Rate       (MG) : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("Feature Hit Rate       (FG) : {0,12:0.00}", summData.FgFHitRate));
            Console.WriteLine(String.Format("Hold Spin Hit Rate     (FG) : {0,12:0.00}", summData.FgHoldSpinHitRate));
            Console.WriteLine(String.Format("MG Win                      : {0,12:0.00}", summData.MainGameWin));
            Console.WriteLine(String.Format("MG Hold Spin Win            : {0,12:0.00}", summData.MgHoldSpinWin));
            Console.WriteLine(String.Format("FG Win                      : {0,12:0.00}", summData.FSTotalWin));
            Console.WriteLine(String.Format("FG Hold Spin Win            : {0,12:0.00}", summData.FgHoldSpinWin));
            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("MG RTP                     : {0,11:0.00}%", 100 * summData.MainGameWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG Hold Spin  RTP          : {0,11:0.00}%", 100 * summData.MgHoldSpinWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG RTP                     : {0,11:0.00}%", 100 * summData.FSTotalWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG Hold Spin RTP           : {0,11:0.00}%", 100 * summData.FgHoldSpinWin / summData.TotalBet));
            }
            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,11:0.0000}%", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        private static SummaryData ExecuteBonus(int level, RequestContext<BonusArgs> requestContext, DragonRichesResult result)
        {
            var bonus = module.CreateBonus(result);
            var dragonBonus = bonus.Value as DragonRichesBonus;
            var summData = new SummaryData();
            IDragonAction action(BonusState state)
            {
                if (state is HoldSpin)
                    return new HoldSpinAction(level, requestContext.Platform, dragonBonus.SpinBet);

                if (state is FreeSpin)
                    return new FreeSpinAction(level, requestContext.Platform, dragonBonus.SpinBet, () => Config.FreeGameReelStrip[1]);

                if (state is HoldFreeSpin)
                    return new HoldFreeSpinAction(level, requestContext.Platform, dragonBonus.SpinBet);

                return new NonAction();
            }
            do
            {
                var bonusSpinResult = BonusReducer.Dispatch(dragonBonus, action);
                var spinResult = bonusSpinResult.SpinResult;
                if (spinResult.HasBonus)
                {
                    var additionBonus = spinResult.Bonus;
                    if (additionBonus.ClientId == 4)
                        summData.FgHoldSpinHit++;
                    else if (additionBonus.ClientId == 3)
                        summData.FgFHit++;
                }

                switch (bonusSpinResult.Type)
                {
                    case "fs":
                        summData.FSpinCounter++;
                        summData.FSTotalWin += spinResult.Win;
                        break;

                    case "cs":
                        if (spinResult.Win > 0)
                        {
                            var wheel = result.Wheel.Reels;
                            if (result.Wheel.Coins.SelectMany(c => c).Select((v, i) => new { v, i }).Any(item => item.v > 0 && wheel[item.i / 3][item.i % 3] != 11))
                                throw new Exception();
                        }
                        summData.MgHoldSpinWin += spinResult.Win;
                        break;

                    case "fscs":
                        if (spinResult.Win > 0)
                        {
                            var wheel = result.Wheel.Reels;
                            if (result.Wheel.Coins.SelectMany(c => c).Select((v, i) => new { v, i }).Any(item => item.v > 0 && wheel[item.i / 3][item.i % 3] != 11))
                                throw new Exception();
                        }

                        summData.FgHoldSpinWin += spinResult.Win;
                        break;
                }

                if (bonusSpinResult.IsCompleted)
                    break;
                dragonBonus = bonusSpinResult.Bonus as DragonRichesBonus;
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

            public decimal FgFHitRate
            {
                get { return (decimal)FSpinCounter / (FgFHit > 0 ? FgFHit : 1); }
            }

            public int FgHoldSpinHit { get; set; }

            public decimal FgHoldSpinHitRate
            {
                get { return (decimal)FSpinCounter / (FgHoldSpinHit > 0 ? FgHoldSpinHit : 1); }
            }

            public decimal FgHoldSpinWin { get; set; }
            public long FSpinCounter { get; set; }
            public decimal FSTotalWin { get; set; }
            public decimal MainGameWin { get; set; }
            public int MgFHit { get; set; }

            public decimal MgFHitRate
            {
                get { return (decimal)SpinCounter / (MgFHit > 0 ? MgFHit : 1); }
            }

            public int MgHoldSpinHit { get; set; }

            public decimal MgHoldSpinHitRate
            {
                get { return (decimal)SpinCounter / (MgHoldSpinHit > 0 ? MgHoldSpinHit : 1); }
            }

            public decimal MgHoldSpinWin { get; set; }

            public decimal RTPOverAll
            {
                get { return TotalBet == 0 ? 1 : (MainGameWin + MgHoldSpinWin + FSTotalWin + FgHoldSpinWin) / (TotalBet); }
            }

            public long SpinCounter { get; set; }
            public decimal TotalBet { get; set; }

            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.FSpinCounter += target.FSpinCounter;
                source.TotalBet += target.TotalBet;
                source.MainGameWin += target.MainGameWin;
                source.MgHoldSpinWin += target.MgHoldSpinWin;
                source.FSTotalWin += target.FSTotalWin;
                source.FgHoldSpinWin += target.FgHoldSpinWin;

                source.MgHoldSpinHit += target.MgHoldSpinHit;
                source.MgFHit += target.MgFHit;
                source.FgFHit += target.FgFHit;
                source.FgHoldSpinHit += target.FgHoldSpinHit;
                return source;
            }
        }
    }
}