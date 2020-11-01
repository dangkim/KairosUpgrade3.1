using NUnit.Framework;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using Slot.Games.FrostDragon.Models.Bonuses;
using Slot.Games.FrostDragon.Models.GameResults.Spins;
using Slot.Games.FrostDragon.Models.Test;
using Slot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSpinBonus = Slot.Games.FrostDragon.Models.Bonuses.FreeSpinBonus;

namespace Slot.Simulations
{
    [TestFixture]
    public class FrostDragon
    {
        [TestCase(Configuration.Id, Levels.One, "CNY", 20, 500000, 1, TestName = "[LVL1][10M] FrostDragon")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 50, 2000000, 1, TestName = "[LVL1][100M] FrostDragon")]
        [TestCase(Configuration.Id, Levels.One, "CNY", 50, 5000000, 1, TestName = "[LVL1][250M] FrostDragon")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 20, 500000, 1, TestName = "[LVL2][10M] FrostDragon")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 50, 2000000, 1, TestName = "[LVL2][100M] FrostDragon")]
        [TestCase(Configuration.Id, Levels.Two, "CNY", 50, 5000000, 1, TestName = "[LVL2][250M] FrostDragon")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 20, 500000, 1, TestName = "[LVL3][10M] FrostDragon")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 50, 2000000, 1, TestName = "[LVL3][100M] FrostDragon")]
        [TestCase(Configuration.Id, Levels.Three, "CNY", 50, 5000000, 1, TestName = "[LVL3][250M] FrostDragon")]
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

        [TestCase(Configuration.Id, Levels.One, "MainGameA", TestName = "[LVL1][MainGameA] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.One, "MainGameB", TestName = "[LVL1][MainGameB] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.One, "MainGameC", TestName = "[LVL1][MainGameC] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.One, "MainGameD", TestName = "[LVL1][MainGameD] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.One, "MainGameE", TestName = "[LVL1][MainGameE] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Two, "MainGameA", TestName = "[LVL2][MainGameA] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Two, "MainGameB", TestName = "[LVL2][MainGameB] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Two, "MainGameC", TestName = "[LVL2][MainGameC] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Two, "MainGameD", TestName = "[LVL2][MainGameD] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Two, "MainGameE", TestName = "[LVL2][MainGameE] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Three, "MainGameA", TestName = "[LVL3][MainGameA] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Three, "MainGameB", TestName = "[LVL3][MainGameB] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Three, "MainGameC", TestName = "[LVL3][MainGameC] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Three, "MainGameD", TestName = "[LVL3][MainGameD] Main Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Three, "MainGameE", TestName = "[LVL3][MainGameE] Main Game Full Cycle FrostDragon")]
        public void TestMainGameFullCycle(int gameId, int level, string reelStripId)
        {
            var timeStart = DateTime.Now;
            var module = SimulationHelper.GetModule(gameId);
            var configuration = new Configuration();
            var targetRtpLevel = Math.Round(configuration.RtpLevels.FirstOrDefault(rl => rl.Level == level).Rtp, 2);
            var totalSummaryData = new SummaryData();
            var spinRequestContext = SimulationHelper.GetMockSpinRequestContext(gameId);
            var bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(0, gameId);
            var targetWheel = MainGameEngine.GetTargetWheel(level, configuration, reelStripId);
            var userGameKey = new UserGameKey()
            {
                UserId = -1,
                GameId = gameId,
                Level = level
            };

            var spinBet = MainGameEngine.GenerateSpinBet(spinRequestContext);
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight) { ReelStripsId = reelStripId };

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
                                var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
                                var avalancheMultiplier = configuration.BonusConfig.Collapse.Multipliers.First();

                                var spinResult = new SpinResult(spinBet, wheel, topIndices, winPositions, bonusPositions, avalancheMultiplier)
                                {
                                    PlatformType = spinRequestContext.Platform,
                                    Level = level
                                };

                                totalSummaryData.Update(spinResult);

                                if (spinResult.HasBonus)
                                {
                                    var bonus = module.CreateBonus(spinResult).Value;

                                    if (bonus is FreeSpinBonus)
                                        bonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(spinResult);

                                    var collapsingSpinBonus = bonus as CollapsingSpinBonus;

                                    while (!bonus.IsCompleted)
                                    {
                                        var previousSpinResult = collapsingSpinBonus.PreviousGameResult == null ? collapsingSpinBonus.SpinResult : collapsingSpinBonus.PreviousSpinResult;

                                        var referenceWheel = MainGameEngine.GetTargetWheel(level, configuration, previousSpinResult.Wheel.ReelStripsId);
                                        var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(
                                                                                            previousSpinResult,
                                                                                            referenceWheel,
                                                                                            configuration.BonusConfig.Collapse.Multipliers,
                                                                                            configuration.Payline,
                                                                                            configuration.PayTable);

                                        collapsingSpinBonus.UpdateBonus(collapsingSpinResult);
                                        var bonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingSpinBonus, collapsingSpinResult);

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

        [TestCase(Configuration.Id, Levels.One, "FreeGameA", TestName = "[LVL1][FreeGameA] Free Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.One, "FreeGameB", TestName = "[LVL1][FreeGameB] Free Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Two, "FreeGameA", TestName = "[LVL2][FreeGameA] Free Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Two, "FreeGameB", TestName = "[LVL2][FreeGameB] Free Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Three, "FreeGameA", TestName = "[LVL3][FreeGameA] Free Game Full Cycle FrostDragon")]
        [TestCase(Configuration.Id, Levels.Three, "FreeGameB", TestName = "[LVL3][FreeGameB] Free Game Full Cycle FrostDragon")]
        public void TestFreeGameFullCycle(int gameId, int level, string reelStripId)
        {
            var timeStart = DateTime.Now;
            var module = SimulationHelper.GetModule(gameId);
            var configuration = new Configuration();
            var targetRtpLevel = Math.Round(configuration.RtpLevels.FirstOrDefault(rl => rl.Level == level).Rtp, 2);
            var totalSummaryData = new SummaryData();
            var spinRequestContext = SimulationHelper.GetMockSpinRequestContext(gameId);
            var bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(0, gameId);
            var targetWheel = MainGameEngine.GetTargetWheel(level, configuration, reelStripId);
            var userGameKey = new UserGameKey()
            {
                UserId = -1,
                GameId = gameId,
                Level = level
            };

            var spinBet = MainGameEngine.GenerateSpinBet(spinRequestContext);
            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight) { ReelStripsId = reelStripId };

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
                                var avalancheMultiplier = configuration.BonusConfig.FreeSpin.Multipliers.First();
                                var winPositions = MainGameEngine.GenerateWinPositions(configuration.Payline, configuration.PayTable, wheel, spinBet.LineBet, spinBet.Lines, avalancheMultiplier);
                                var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

                                var spinResult = new SpinResult(spinBet, wheel, topIndices, winPositions, bonusPositions, avalancheMultiplier)
                                {
                                    PlatformType = spinRequestContext.Platform,
                                    Level = level
                                };

                                totalSummaryData.Update(spinResult);

                                if (spinResult.HasBonus)
                                {
                                    var bonus = module.CreateBonus(spinResult).Value;

                                    if (bonus is FreeSpinBonus)
                                        bonus = CollapsingBonusEngine.CreateCollapsingSpinBonus(spinResult);

                                    var collapsingSpinBonus = bonus as CollapsingSpinBonus;

                                    while (!bonus.IsCompleted)
                                    {
                                        var previousSpinResult = collapsingSpinBonus.PreviousGameResult == null ? collapsingSpinBonus.SpinResult : collapsingSpinBonus.PreviousSpinResult;

                                        var referenceWheel = MainGameEngine.GetTargetWheel(level, configuration, previousSpinResult.Wheel.ReelStripsId);
                                        var collapsingSpinResult = CollapsingBonusEngine.CreateCollapsingSpinResult(
                                                                                            previousSpinResult,
                                                                                            referenceWheel,
                                                                                            configuration.BonusConfig.FreeSpin.Multipliers,
                                                                                            configuration.Payline,
                                                                                            configuration.PayTable);

                                        collapsingSpinBonus.UpdateBonus(collapsingSpinResult);
                                        var bonusResult = CollapsingBonusEngine.CreateCollapsingBonusResult(collapsingSpinBonus, collapsingSpinResult);

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
