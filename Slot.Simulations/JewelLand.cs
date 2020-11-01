using NUnit.Framework;
using Slot.Games.JewelLand.Configuration;
using Slot.Games.JewelLand.Engines;
using Slot.Games.JewelLand.Models.GameResults.Spins;
using Slot.Games.JewelLand.Models.Test;
using Slot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.Simulations
{
    [TestFixture]
    public class JewelLand
    {
        [TestCase(Configuration.Id, Levels.One, "CNY", 20, 50000, 1, TestName = "[LVL1][1M] JewelLand")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 10, 500000, 1, TestName = "[LVL1][5M] JewelLand")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 20, 500000, 1, TestName = "[LVL1][10M] JewelLand")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 20, 5000000, 1, TestName = "[LVL1][100M] JewelLand")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 50, 10000000, 1, TestName = "[LVL1][500M] JewelLand")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 50, 20000000, 1, TestName = "[LVL1][1B] JewelLand")]
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

            Assert.True(isWithinRtp, $"RTP not matching. The result is {totalSummaryData.RtpData.OverallRtp}. Expected is {targetRtpLevel}");
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "[LVL1] Full Cycle JewelLand")]
        public void TestFullCycle(int gameId, int level)
        {
            var timeStart = DateTime.Now;
            var module = SimulationHelper.GetModule(gameId);
            var configuration = new Configuration();
            var targetRtpLevel = Math.Round(configuration.RtpLevels.FirstOrDefault(rl => rl.Level == level).Rtp, 2);
            var totalSummaryData = new SummaryData();
            var spinRequestContext = SimulationHelper.GetMockSpinRequestContext(gameId);
            var bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(gameId, 0);
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
                        wheel.Reels[0] = SimulationHelper.GetReelRange(targetWheel[0], reel1);
                        wheel.Reels[1] = SimulationHelper.GetReelRange(targetWheel[1], reel2);
                        wheel.Reels[2] = SimulationHelper.GetReelRange(targetWheel[2], reel3);

                        var topIndices = new List<int> { reel1, reel2, reel3 };
                        var winPositions = MainGameEngine.GenerateWinPositions(configuration.Payline, configuration.PayTable, wheel, spinBet.LineBet, spinBet.Lines, spinBet.Multiplier);
                        var stackedReels = MainGameEngine.GetStackedReels(wheel, configuration.PayTable);
                        var bonusPositions = MainGameEngine.GenerateBonusPositions(stackedReels);

                        var spinResult = new SpinResult(spinBet, wheel, winPositions, bonusPositions)
                        {
                            PlatformType = spinRequestContext.Platform,
                            Level = level
                        };

                        totalSummaryData.Update(spinResult);

                        if (spinResult.HasBonus)
                        {
                            var bonus = module.CreateBonus(spinResult).Value;

                            while (!bonus.IsCompleted)
                            {
                                var bonusResult = SimulationHelper.ExecuteBonus(level, bonus, bonusRequestContext, configuration).Value;

                                totalSummaryData.UpdateBonus(bonusResult);
                                bonus = bonusResult.Bonus;
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
