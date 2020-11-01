namespace Slot.Simulations
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Slot.Core.Modules.Infrastructure;
    using Slot.Core.Modules.Infrastructure.Models;
    using Slot.Games.BikiniBeach;
    using Slot.Games.BikiniBeach.Configuration;
    using Slot.Model.Entity;
    using System;
    using System.IO;
    using System.Linq;

    [TestFixture]
    internal class BikiniBeach
    {
        private static IGameModule module;
        private const int GameId = 3;

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
            var logger = logFactory.CreateLogger<BikiniBeachModule>();
            module = new BikiniBeachModule(logger);
        }

        [TestCase(1, 1000, 1000, 1.0, TestName = "[95.36][LVL1][1M] 1st Bikini Beach")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[95.36][LVL1][1M] 2nd Bikini Beach")]
        [TestCase(1, 1000, 1000, 1.0, TestName = "[95.36][LVL1][1M] 3rd Bikini Beach")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[95.36][LVL1][10M] 1st Bikini Beach")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[95.36][LVL1][10M] 2nd Bikini Beach")]
        [TestCase(1, 1000, 10000, 1.0, TestName = "[95.36][LVL1][10M] 3rd Bikini Beach")]

        [TestCase(2, 1000, 1000, 1.0, TestName = "[92.72][LVL2][1M] 1st Bikini Beach")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[92.72][LVL2][1M] 2nd Bikini Beach")]
        [TestCase(2, 1000, 1000, 1.0, TestName = "[92.72][LVL2][1M] 3rd Bikini Beach")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[92.72][LVL2][10M] 1st Bikini Beach")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[92.72][LVL2][10M] 2nd Bikini Beach")]
        [TestCase(2, 1000, 10000, 1.0, TestName = "[92.72][LVL2][10M] 3rd Bikini Beach")]

        [TestCase(3, 1000, 1000, 1.0, TestName = "[90.34][LVL3][1M] 1st Bikini Beach")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[90.34][LVL3][1M] 2nd Bikini Beach")]
        [TestCase(3, 1000, 1000, 1.0, TestName = "[90.34][LVL3][1M] 3rd Bikini Beach")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[90.34][LVL3][10M] 1st Bikini Beach")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[90.34][LVL3][10M] 2nd Bikini Beach")]
        [TestCase(3, 1000, 10000, 1.0, TestName = "[90.34][LVL3][10M] 3rd Bikini Beach")]

        [TestCase(4, 1000, 1000, 1.0, TestName = "[97.00][LVL4][1M] 1st Bikini Beach")]
        [TestCase(4, 1000, 1000, 1.0, TestName = "[97.00][LVL4][1M] 2nd Bikini Beach")]
        [TestCase(4, 1000, 1000, 1.0, TestName = "[97.00][LVL4][1M] 3rd Bikini Beach")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[97.00][LVL4][10M] 1st Bikini Beach")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[97.00][LVL4][10M] 2nd Bikini Beach")]
        [TestCase(4, 1000, 10000, 1.0, TestName = "[97.00][LVL4][10M] 3rd Bikini Beach")]

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
                    var request = user.CreateRequestContext<SpinArgs>("bikini-beach");
                    request.Parameters = spinArgs;
                    foreach (var iter in new byte[numItrPerUser])
                    {
                        data.SpinCounter++;
                        data.TotalBet += 30 * spinBet;
                        var executeResult = module.ExecuteSpin(level, new UserGameSpinData(), request);
                        var result = executeResult.Value as BikiniBeachResult;
                        data.MainGameWin += result.Win;

                        if (result.HasBonus)
                        {
                            var bonus = result.Bonus;
                            var typeOfBonus = bonus.ClientId;
                            var bonusRequest = user.CreateRequestContext<BonusArgs>("bikinibeach");
                            switch (typeOfBonus)
                            {
                                case 2:
                                    data.MgFHit++;
                                    var bonusResult = ExecuteFreeSpinBonus(level, bonusRequest, result);
                                    data += bonusResult;
                                    break;

                                case 3:
                                    data.MgSwimWearHit++;
                                    data.MgSwimWearWin += ExecuteBonus(level, bonusRequest, result);
                                    break;

                                case 4:
                                    data.MgBodyPartHit++;
                                    data.MgBodyPartWin += ExecuteBonus(level, bonusRequest, result);
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
            var fileStream = new FileStream($@"..\..\..\Results\BikiniBeach\{TestContext.CurrentContext.Test.Name}.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var writer = new StreamWriter(fileStream);
            Console.SetOut(writer);
            Console.WriteLine(String.Format("Test.Level                  : {0}", level));
            Console.WriteLine(String.Format("Test.TimeStart              : {0} {1}", sdt.ToShortDateString(), sdt.ToLongTimeString()));
            Console.WriteLine(String.Format("Test.TimeEnd                : {0} {1}", edt.ToShortDateString(), edt.ToLongTimeString()));
            Console.WriteLine(String.Format("SpinCount                   : {0}", summData.SpinCounter));
            Console.WriteLine(String.Format("TotalBet                    : {0,12:0.00}", summData.TotalBet));
            Console.WriteLine("----------------------------------------");
            Console.WriteLine(String.Format("Swim Wear Hit Rate     (MG) : {0,12:0.00}", summData.MgHoldSpinHitRate));
            Console.WriteLine(String.Format("Body Part Hit Rate     (MG) : {0,12:0.00}", summData.MgBodyPartHitRate));
            Console.WriteLine(String.Format("Feature Hit Rate       (MG) : {0,12:0.00}", summData.MgFHitRate));
            Console.WriteLine(String.Format("MG Win                      : {0,12:0.00}", summData.MainGameWin));
            Console.WriteLine(String.Format("MG Swim Wear Win            : {0,12:0.00}", summData.MgSwimWearWin));
            Console.WriteLine(String.Format("MG Body Part Win            : {0,12:0.00}", summData.MgBodyPartWin));
            Console.WriteLine(String.Format("FG Win                      : {0,12:0.00}", summData.FSTotalWin));
            Console.WriteLine("----------------------------------------");
            if (summData.TotalBet > 0)
            {
                Console.WriteLine(String.Format("MG RTP                     : {0,11:0.00}%", 100 * summData.MainGameWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG Swim Wear RTP           : {0,11:0.00}%", 100 * summData.MgSwimWearWin / summData.TotalBet));
                Console.WriteLine(String.Format("MG Body Part RTP           : {0,11:0.00}%", 100 * summData.MgBodyPartWin / summData.TotalBet));
                Console.WriteLine(String.Format("FG RTP                     : {0,11:0.00}%", 100 * summData.FSTotalWin / summData.TotalBet));
            }
            Console.WriteLine("--- RTP.OverAll ------------------------");
            Console.WriteLine(String.Format("RTP.Total (Over All)      : {0,11:0.0000}%", 100 * summData.RTPOverAll));
            Console.SetOut(oldOut);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Done");
        }

        private static decimal ExecuteBonus(int level, RequestContext<BonusArgs> requestContext, BikiniBeachResult result)
        {
            var bonus = module.CreateBonus(result);
            var bikiniBeachBonus = bonus.Value as BikiniBeachBonus;
            var accumulate = 0m;
            IBikiniBeachAction action(BonusState state)
            {
                switch (state)
                {
                    case SwimWear _:
                        return new SwimWearAction(level, requestContext.Platform, bikiniBeachBonus.SpinBet);

                    default:
                        return new BodyPartChooseAction(level, requestContext.Platform, bikiniBeachBonus.SpinBet, requestContext.Parameters.Param);
                }
            }
            do
            {
                var bonusSpinResult = BonusReducer.Dispatch(bikiniBeachBonus, action);
                accumulate += bonusSpinResult.Win;
                if (bonusSpinResult.IsCompleted)
                    break;
                bikiniBeachBonus = bonusSpinResult.Bonus as BikiniBeachBonus;
            } while (true);

            return accumulate;
        }

        private static SummaryData ExecuteFreeSpinBonus(int level, RequestContext<BonusArgs> requestContext, BikiniBeachResult result)
        {
            var bonus = module.CreateBonus(result);
            var bikiniBeachBonus = bonus.Value as BikiniBeachBonus;
            var summData = new SummaryData();
            IBikiniBeachAction action(BonusState state)
            {
                switch (state)
                {
                    default: return new FreeSpinAction(level, requestContext.Platform, bikiniBeachBonus.SpinBet, () => Config.FreeGameReelStrip);
                }
            }
            do
            {
                var bonusSpinResult = BonusReducer.Dispatch(bikiniBeachBonus, action);
                summData.FSpinCounter++;
                summData.FSTotalWin += bonusSpinResult.Win;

                if (bonusSpinResult.IsCompleted)
                    break;
                bikiniBeachBonus = bonusSpinResult.Bonus as BikiniBeachBonus;
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

            public decimal MgFHitRate
            {
                get { return (decimal)SpinCounter / (MgFHit > 0 ? MgFHit : 1); }
            }

            public int MgSwimWearHit { get; set; }

            public decimal MgHoldSpinHitRate
            {
                get { return (decimal)SpinCounter / (MgSwimWearHit > 0 ? MgSwimWearHit : 1); }
            }

            public int MgBodyPartHit { get; set; }

            public decimal MgBodyPartHitRate
            {
                get { return (decimal)SpinCounter / (MgBodyPartHit > 0 ? MgBodyPartHit : 1); }
            }

            public decimal MgSwimWearWin { get; set; }

            public decimal MgBodyPartWin { get; set; }

            public decimal RTPOverAll
            {
                get { return TotalBet == 0 ? 1 : (MainGameWin + MgSwimWearWin + MgBodyPartWin + FSTotalWin) / TotalBet; }
            }

            public long SpinCounter { get; set; }
            public decimal TotalBet { get; set; }

            public static SummaryData operator +(SummaryData source, SummaryData target)
            {
                source.SpinCounter += target.SpinCounter;
                source.FSpinCounter += target.FSpinCounter;
                source.TotalBet += target.TotalBet;
                source.MainGameWin += target.MainGameWin;
                source.MgSwimWearWin += target.MgSwimWearWin;
                source.MgBodyPartWin += target.MgBodyPartWin;
                source.FSTotalWin += target.FSTotalWin;
                source.MgBodyPartHit += target.MgBodyPartHit;

                source.MgSwimWearHit += target.MgSwimWearHit;
                source.MgFHit += target.MgFHit;
                return source;
            }
        }
    }
}