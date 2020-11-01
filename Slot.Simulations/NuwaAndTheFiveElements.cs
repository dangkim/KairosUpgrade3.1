using NUnit.Framework;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Configuration.Bonuses;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Games.NuwaAndTheFiveElements.Models.Bonuses;
using Slot.Games.NuwaAndTheFiveElements.Models.Test;
using Slot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpinResult = Slot.Games.NuwaAndTheFiveElements.Models.GameResults.Spins.SpinResult;

namespace Slot.Simulations
{
    [TestFixture]
    public class NuwaAndTheFiveElements
    {
        [TestCase(Configuration.Id, Levels.One, "CNY", 50, 200000, 1, TestName = "[LVL1][10M] NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 50, 1000000, 1, TestName = "[LVL1][50M] NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 100, 1000000, 1, TestName = "[LVL1][100M] NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 1, 100000, 1, TestName = "[LVL2][100K] NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 50, 200000, 1, TestName = "[LVL2][10M] NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 50, 1000000, 1, TestName = "[LVL2][50M] NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 100, 1000000, 1, TestName = "[LVL2][100M] NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 50, 200000, 1, TestName = "[LVL3][10M] NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 50, 1000000, 1, TestName = "[LVL3][50M] NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 100, 1000000, 1, TestName = "[LVL3][100M] NuwaAndTheFiveElements")]
        public void RandomSpin(int gameId, int level, string currencyCode, int numOfUsers, int numItrPerUser, decimal bet)
        {
            var timeStart = DateTime.Now;
            var module = SimulationHelper.GetModule(gameId);
            var configuration = new Configuration();
            var targetRtpLevel = Math.Round(configuration.RtpLevels.FirstOrDefault(rl => rl.Level == level).Rtp, 2);
            var totalSummaryData = new SummaryData();

            var users = SimulationHelper.GetUsers(gameId, numOfUsers, level);
            var spinBets = SimulationHelper.GetUserBets(users, bet, Game.Lines);
            var spinRequestContext = SimulationHelper.GetMockSpinRequestContext(gameId);
            var bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(gameId, 0);

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
                                if (bonus is RevealBonus)
                                {
                                    bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(gameId, RandomNumberEngine.Next(Reveal.RandomWeightMinRange, Reveal.RandomWeightMaxRange));
                                }

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
            totalSummaryData.DisplayPayoutsData(bet, Game.Lines);

            var isWithinRtp = totalSummaryData.RtpData.OverallRtp >= targetRtpLevel - 1 && totalSummaryData.RtpData.OverallRtp <= targetRtpLevel + 1;

            Assert.True(isWithinRtp, $"RTP not matching. The result is {totalSummaryData.RtpData.OverallRtp}. Expected is {targetRtpLevel}.");
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "[LVL1] Full Cycle NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.Two, TestName = "[LVL2] Full Cycle NuwaAndTheFiveElements")]
        [TestCase(Configuration.Id, Levels.Three, TestName = "[LVL3] Full Cycle NuwaAndTheFiveElements")]
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
                        for (var reel4 = 0; reel4 < targetWheel[3].Count; reel4++)
                        {
                            for (var reel5 = 0; reel5 < targetWheel[4].Count; reel5++)
                            {
                                wheel.Reels[0] = SimulationHelper.GetReelRange(targetWheel[0], reel1);
                                wheel.Reels[1] = SimulationHelper.GetReelRange(targetWheel[1], reel2);
                                wheel.Reels[2] = SimulationHelper.GetReelRange(targetWheel[2], reel3);
                                wheel.Reels[3] = SimulationHelper.GetReelRange(targetWheel[3], reel4);
                                wheel.Reels[4] = SimulationHelper.GetReelRange(targetWheel[4], reel5);

                                var topIndices = new List<int> { reel1, reel2, reel3, reel4, reel5 };
                                var winPositions = MainGameEngine.GenerateWinPositions(configuration.Payline, configuration.PayTable, wheel, spinBet.LineBet, spinBet.Lines, spinBet.Multiplier);
                                var matchingSymbolPositions = MainGameEngine.GenerateMatchingSymbolPositions(configuration.SymbolCollapsePairs, winPositions.Select(wp => wp.Symbol).ToList(), wheel);
                                var bombAndStopperPositions = MainGameEngine.GenerateBombAndStopperPositions(wheel, winPositions);

                                var spinResult = new SpinResult(level, spinBet, wheel, topIndices, winPositions, matchingSymbolPositions, bombAndStopperPositions)
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
                                        if (bonus is RevealBonus)
                                        {
                                            bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(gameId, RandomNumberEngine.Next(Reveal.RandomWeightMinRange, Reveal.RandomWeightMaxRange));
                                        }

                                        var bonusResult = SimulationHelper.ExecuteBonus(level, bonus, bonusRequestContext, configuration).Value;

                                        totalSummaryData.UpdateBonus(bonusResult);

                                        bonus = bonusResult.Bonus;
                                    }
                                }
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

        [TestCase(Configuration.Id, Levels.One, TestName = "[LVL1] FG Full Cycle NuwaAndTheFiveElements")]
        public void TestFreeGameFullCycle(int gameId, int level)
        {
            var timeStart = DateTime.Now;
            var module = SimulationHelper.GetModule(gameId);
            var configuration = new Configuration();
            var targetRtpLevel = Math.Round(configuration.RtpLevels.FirstOrDefault(rl => rl.Level == level).Rtp, 2);
            var totalSummaryData = new SummaryData();
            var requestContext = SimulationHelper.GetMockSpinRequestContext(gameId);
            var bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(gameId, 0);
            var targetWheel = MainGameEngine.GetTargetWheel(level, configuration);
            var userGameKey = new UserGameKey()
            {
                UserId = -1,
                GameId = gameId,
                Level = level
            };

            var mockSpinResult = MainGameEngine.CreateSpinResult(level, requestContext, configuration);
            var spinBet = MainGameEngine.GenerateSpinBet(bonusRequestContext, mockSpinResult, FreeSpin.Multiplier);
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

                                var topIndices = new List<int> { reel1, reel2, reel3, reel4, reel5 };
                                var winPositions = MainGameEngine.GenerateWinPositions(configuration.Payline, configuration.PayTable, wheel, spinBet.LineBet, spinBet.Lines, spinBet.Multiplier);
                                var matchingSymbolPositions = MainGameEngine.GenerateMatchingSymbolPositions(configuration.SymbolCollapsePairs, winPositions.Select(wp => wp.Symbol).ToList(), wheel);
                                var bombAndStopperPositions = MainGameEngine.GenerateBombAndStopperPositions(wheel, winPositions);

                                var spinResult = new SpinResult(level, spinBet, wheel, topIndices, winPositions, matchingSymbolPositions, bombAndStopperPositions)
                                {
                                    PlatformType = bonusRequestContext.Platform,
                                    Level = level
                                };

                                totalSummaryData.Update(spinResult);

                                if (spinResult.HasBonus)
                                {
                                    var bonus = module.CreateBonus(spinResult).Value;

                                    while (!bonus.IsCompleted)
                                    {
                                        if (bonus is RevealBonus)
                                        {
                                            bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(gameId, RandomNumberEngine.Next(Reveal.RandomWeightMinRange, Reveal.RandomWeightMaxRange));
                                        }

                                        var bonusResult = SimulationHelper.ExecuteBonus(level, bonus, bonusRequestContext, configuration).Value;

                                        totalSummaryData.UpdateBonus(bonusResult);

                                        bonus = bonusResult.Bonus;
                                    }
                                }
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
