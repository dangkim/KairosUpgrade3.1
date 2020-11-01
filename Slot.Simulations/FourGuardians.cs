using NUnit.Framework;
using Slot.Games.FourGuardians;
using Slot.Games.FourGuardians.Configuration;
using Slot.Games.FourGuardians.Engines;
using Slot.Games.FourGuardians.Models.GameResults.Spins;
using Slot.Games.FourGuardians.Models.Test;
using Slot.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.Simulations
{
    [TestFixture]
    public class FourGuardians
    {
        [TestCase(Module.Id, Levels.One, "CNY", 10, 100000, 1, TestName = "[LVL1][1M] FourGuardians")]
        [TestCase(Module.Id, Levels.One, "CNY", 20, 500000, 1, TestName = "[LVL1][10M] FourGuardians")]
        [TestCase(Module.Id, Levels.One, "CNY", 50, 2000000, 1, TestName = "[LVL1][100M] FourGuardians")]
        [TestCase(Module.Id, Levels.One, "CNY", 50, 5000000, 1, TestName = "[LVL1][250M] FourGuardians")]
        [TestCase(Module.Id, Levels.One, "CNY", 100, 5000000, 1, TestName = "[LVL1][500M] FourGuardians")]
        [TestCase(Module.Id, Levels.One, "CNY", 500, 2000000, 1, TestName = "[LVL1][1B] FourGuardians")]
        [TestCase(Module.Id, Levels.Two, "CNY", 10, 100000, 1, TestName = "[LVL2][1M] FourGuardians")]
        [TestCase(Module.Id, Levels.Two, "CNY", 20, 500000, 1, TestName = "[LVL2][10M] FourGuardians")]
        [TestCase(Module.Id, Levels.Two, "CNY", 50, 2000000, 1, TestName = "[LVL2][100M] FourGuardians")]
        [TestCase(Module.Id, Levels.Two, "CNY", 50, 5000000, 1, TestName = "[LVL2][250M] FourGuardians")]
        [TestCase(Module.Id, Levels.Two, "CNY", 100, 5000000, 1, TestName = "[LVL2][500M] FourGuardians")]
        [TestCase(Module.Id, Levels.Two, "CNY", 500, 2000000, 1, TestName = "[LVL2][1B] FourGuardians")]
        [TestCase(Module.Id, Levels.Three, "CNY", 10, 100000, 1, TestName = "[LVL3][1M] FourGuardians")]
        [TestCase(Module.Id, Levels.Three, "CNY", 20, 500000, 1, TestName = "[LVL3][10M] FourGuardians")]
        [TestCase(Module.Id, Levels.Three, "CNY", 50, 2000000, 1, TestName = "[LVL3][100M] FourGuardians")]
        [TestCase(Module.Id, Levels.Three, "CNY", 50, 5000000, 1, TestName = "[LVL3][250M] FourGuardians")]
        [TestCase(Module.Id, Levels.Three, "CNY", 100, 5000000, 1, TestName = "[LVL3][500M] FourGuardians")]
        [TestCase(Module.Id, Levels.Three, "CNY", 500, 2000000, 1, TestName = "[LVL3][1B] FourGuardians")]
        public void RandomSpin(int gameId, int level, string currencyCode, int numOfUsers, int numItrPerUser, decimal bet)
        {
            const int lines = Game.Lines;

            var timeStart = DateTime.Now;
            var module = SimulationHelper.GetModule(gameId);
            var configuration = module.Configuration;
            var targetRtpLevel = Math.Round(configuration.RtpLevels.FirstOrDefault(rl => rl.Level == level).Rtp, 2);
            var totalSummaryData = new SummaryData();

            var users = SimulationHelper.GetUsers(gameId, numOfUsers, level);
            var spinBets = SimulationHelper.GetUserBets(users, bet, lines);
            var spinRequestContext = SimulationHelper.GetMockSpinRequestContext(gameId);

            Parallel.ForEach(users,
                () => new SummaryData(),
                (key, state, summaryData) =>
                {
                    var spinBet = spinBets[key.UserId];

                    for (var ctr = 0; ctr < numItrPerUser; ctr++)
                    {
                        var spinResult = module.ExecuteSpin(level, null, spinRequestContext).Value as SpinResult;

                        summaryData.Update(spinResult);
                    }

                    return summaryData;
                },
                summaryData =>
                {
                    lock (totalSummaryData)
                    {
                        totalSummaryData.Sum(summaryData);
                    }
                });

            totalSummaryData.DisplayData(level, timeStart, targetRtpLevel);
            totalSummaryData.DisplayPayoutsData(bet, lines);

            var isWithinRtp = totalSummaryData.RtpData.OverallRtp >= targetRtpLevel - 1 && totalSummaryData.RtpData.OverallRtp <= targetRtpLevel + 1;

            Assert.True(isWithinRtp, $"RTP not matching. The result is {totalSummaryData.RtpData.OverallRtp}.");
        }

        [TestCase(Module.Id, Levels.One, TestName = "[LVL1] Full Cycle FourGuardians")]
        [TestCase(Module.Id, Levels.Two, TestName = "[LVL2] Full Cycle FourGuardians")]
        [TestCase(Module.Id, Levels.Three, TestName = "[LVL3] Full Cycle FourGuardians")]
        public void FullCycle(int gameId, int level)
        {
            var timeStart = DateTime.Now;
            var module = SimulationHelper.GetModule(gameId);
            var configuration = module.Configuration;
            var targetRtpLevel = Math.Round(configuration.RtpLevels.FirstOrDefault(rl => rl.Level == level).Rtp, 2);
            var totalSummaryData = new SummaryData();
            var spinRequestContext = SimulationHelper.GetMockSpinRequestContext(gameId);
            var targetWheel = MainGameEngine.GetTargetWheel(level, configuration);
            var userGameKey = new UserGameKey()
            {
                UserId = -1,
                GameId = gameId,
                Level = level
            };

            var spinBet = MainGameEngine.GenerateSpinBet(spinRequestContext);
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight);

            for (var reel1 = 0; reel1 < targetWheel[0].Count; reel1++)
            {
                for (var reel2 = 0; reel2 < targetWheel[1].Count; reel2++)
                {
                    for (var reel3 = 0; reel3 < targetWheel[2].Count; reel3++)
                    {
                        for (var reel4 = 0; reel4 < targetWheel[3].Count; reel4++)
                        {
                            for (var reel5 = 0; reel5 < targetWheel[4].Count; reel5++)
                            {
                                wheel.Reels[0] = SimulationHelper.GetReelRange(targetWheel[0], reel1);
                                wheel.Reels[1] = SimulationHelper.GetReelRange(targetWheel[1], reel2);
                                wheel.Reels[2] = SimulationHelper.GetReelRange(targetWheel[2], reel3);
                                wheel.Reels[3] = SimulationHelper.GetReelRange(targetWheel[3], reel4);
                                wheel.Reels[4] = SimulationHelper.GetReelRange(targetWheel[4], reel5);

                                var expandedWheel = ExpandingWildsEngine.GenerateWheelWithExpandedWilds(wheel, configuration);
                                var winPositions = MainGameEngine.GenerateWinPositions(
                                                                    configuration.Payline,
                                                                    configuration.PayTable,
                                                                    expandedWheel,
                                                                    spinBet.LineBet,
                                                                    spinBet.Lines,
                                                                    spinBet.Multiplier);

                                var spinResult = new SpinResult(spinBet, wheel, winPositions)
                                {
                                    PlatformType = spinRequestContext.Platform,
                                    Level = level
                                };

                                totalSummaryData.Update(spinResult);
                            }
                        }
                    }
                }
            }

            totalSummaryData.DisplayData(level, timeStart, targetRtpLevel);
            var resultOverallRtp = Math.Round(totalSummaryData.RtpData.OverallRtp, 2);

            var isWithinRtp = totalSummaryData.RtpData.OverallRtp >= targetRtpLevel - 0.5m && totalSummaryData.RtpData.OverallRtp <= targetRtpLevel + 0.5m;

            Assert.True(isWithinRtp, $"RTP not matching. The result is {resultOverallRtp}. Expected is {targetRtpLevel}");
        }
    }
}
