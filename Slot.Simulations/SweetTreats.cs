using NUnit.Framework;
using Slot.Games.SweetTreats.Configuration;
using Slot.Games.SweetTreats.Models.Test;
using System;
using System.Linq;
using System.Threading.Tasks;
using Game = Slot.Games.SweetTreats.Configuration.Game;
using SpinResult = Slot.Games.SweetTreats.Models.GameResults.Spins.SpinResult;

namespace Slot.Simulations
{
    [TestFixture]
    public class SweetTreats
    {
        [TestCase(Configuration.Id, Levels.One, "CNY", 20, 100000, 1, TestName = "[LVL1][2M] SweetTreats")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 50, 100000, 1, TestName = "[LVL1][5M] SweetTreats")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 100, 100000, 1, TestName = "[LVL1][10M] SweetTreats")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 20, 100000, 1, TestName = "[LVL2][2M] SweetTreats")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 50, 100000, 1, TestName = "[LVL2][5M] SweetTreats")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 100, 100000, 1, TestName = "[LVL2][10M] SweetTreats")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 20, 100000, 1, TestName = "[LVL3][2M] SweetTreats")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 50, 100000, 1, TestName = "[LVL3][5M] SweetTreats")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 100, 100000, 1, TestName = "[LVL3][10M] SweetTreats")]
        public void RandomSpin(int gameId, int level, string currencyCode, int numOfUsers, int numItrPerUser, decimal bet)
        {
            const int lines = Game.Lines;

            var timeStart = DateTime.Now;
            var module = SimulationHelper.GetModule();
            var configuration = new Configuration();
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
    }
}
