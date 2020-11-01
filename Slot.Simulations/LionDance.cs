using NUnit.Framework;
using Slot.Games.LionDance.Configuration;
using Slot.Games.LionDance.Engines;
using Slot.Games.LionDance.Models.Test;
using Slot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game = Slot.Games.LionDance.Configuration.Game;
using SpinResult = Slot.Games.LionDance.Models.GameResults.Spins.SpinResult;

namespace Slot.Simulations
{
    public class LionDance
    {
        [TestCase(Configuration.Id, Levels.One, "CNY", 10, 100000, 1, TestName = "[LVL1][1M] LionDance")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 30, 100000, 1, TestName = "[LVL1][3M] LionDance")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 50, 100000, 1, TestName = "[LVL1][5M] LionDance")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 20, 500000, 1, TestName = "[LVL1][10M] LionDance")]
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

        [TestCase(Configuration.Id, Levels.One, TestName = "[LVL1] Full Cycle LionDance")]
        public void TestMainGameFullCycle(int gameId, int level)
        {
            var timeStart = DateTime.Now;
            var module = SimulationHelper.GetModule(gameId);
            var configuration = new Configuration();
            var targetRtpLevel = Math.Round(configuration.RtpLevels.FirstOrDefault(rl => rl.Level == level).Rtp, 2);
            var totalSummaryData = new SummaryData();
            var spinRequestContext = SimulationHelper.GetMockSpinRequestContext(gameId);
            var bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(0, gameId);
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
                        var winPositions = MainGameEngine.GenerateWinPositions(configuration.Payline, configuration.PayTable, wheel, spinBet.LineBet, spinBet.Multiplier);

                        var spinResult = new SpinResult(spinBet, wheel, topIndices, winPositions)
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
