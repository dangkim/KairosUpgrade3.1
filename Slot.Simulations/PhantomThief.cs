using NUnit.Framework;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Models.Test;
using System;
using System.Linq;
using System.Threading.Tasks;
using Game = Slot.Games.PhantomThief.Configuration.Game;
using SpinResult = Slot.Games.PhantomThief.Models.GameResults.Spins.SpinResult;

namespace Slot.Simulations
{
    [TestFixture]
    public class PhantomThief
    {
        [TestCase(Configuration.Id, Levels.One, "CNY", 20, 500000, 1, TestName = "[LVL1][10M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 50, 2000000, 1, TestName = "[LVL1][100M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 50, 5000000, 1, TestName = "[LVL1][250M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 20, 500000, 1, TestName = "[LVL2][10M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 50, 2000000, 1, TestName = "[LVL2][100M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 50, 5000000, 1, TestName = "[LVL2][250M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 20, 500000, 1, TestName = "[LVL3][10M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 50, 2000000, 1, TestName = "[LVL3][100M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 50, 5000000, 1, TestName = "[LVL3][250M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Four, "CNY", 20, 500000, 1, TestName = "[LVL4][10M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Four, "CNY", 50, 2000000, 1, TestName = "[LVL4][100M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Four, "CNY", 50, 5000000, 1, TestName = "[LVL4][250M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Five, "CNY", 20, 500000, 1, TestName = "[LVL5][10M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Five, "CNY", 50, 2000000, 1, TestName = "[LVL5][100M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Five, "CNY", 50, 5000000, 1, TestName = "[LVL5][250M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Six, "CNY", 20, 500000, 1, TestName = "[LVL6][10M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Six, "CNY", 50, 2000000, 1, TestName = "[LVL6][100M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Six, "CNY", 50, 5000000, 1, TestName = "[LVL6][250M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Seven, "CNY", 20, 500000, 1, TestName = "[LVL7][10M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Seven, "CNY", 50, 2000000, 1, TestName = "[LVL7][100M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Seven, "CNY", 50, 5000000, 1, TestName = "[LVL7][250M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Eight, "CNY", 20, 500000, 1, TestName = "[LVL8][10M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Eight, "CNY", 50, 2000000, 1, TestName = "[LVL8][100M] PhantomThief")]
        [TestCase(Configuration.Id, Levels.Eight, "CNY", 50, 5000000, 1, TestName = "[LVL8][250M] PhantomThief")]
        public void RandomSpin(int gameId, int level, string currencyCode, int numOfUsers, int numItrPerUser, decimal bet)
        {
            const int lines = Game.Lines;

            var timeStart = DateTime.Now;
            var module = SimulationHelper.GetModule(gameId);
            var configuration = new Configuration();
            var targetRtpLevel = Math.Round(configuration.RtpLevels.FirstOrDefault(rl => rl.Level == level).Rtp, 2);
            var totalSummaryData = new SummaryData();

            var users = SimulationHelper.GetUsers(gameId, numOfUsers, level);
            var spinBets = SimulationHelper.GetUserBets(users, bet, lines);
            var spinRequestContext = SimulationHelper.GetMockSpinRequestContext(gameId);
            var bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(0, gameId);

            Parallel.ForEach(users,
                () => new SummaryData(),
                (key, state, summaryData) =>
                {
                    var spinBet = spinBets[key.UserId];

                    for (var ctr = 0; ctr < numItrPerUser; ctr++)
                    {
                        var spinResult = module.ExecuteSpin(level, null, spinRequestContext).Value as SpinResult;

                        summaryData.Update(spinResult);

                        if (spinResult.HasBonus)
                        {
                            var bonus = module.CreateBonus(spinResult).Value;

                            while (!bonus.IsCompleted)
                            {
                                var bonusResult = SimulationHelper.ExecuteBonus(level, bonus, bonusRequestContext, configuration).Value;

                                summaryData.UpdateBonus(bonusResult);

                                bonus = bonusResult.Bonus;
                            }
                        }
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