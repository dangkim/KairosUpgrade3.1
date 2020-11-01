using NUnit.Framework;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Configuration.Bonuses;
using Slot.Games.XuanWuBlessing.Engines;
using Slot.Games.XuanWuBlessing.Models.Bonuses;
using Slot.Games.XuanWuBlessing.Models.GameResults.Spins;
using Slot.Games.XuanWuBlessing.Models.Test;
using Slot.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Slot.Simulations
{
    public class XuanWuBlessing
    {
        [TestCase(Configuration.Id, 0, Levels.One, "CNY", 100, 10000000, 1, TestName = "[LVL1][1B][R][FG-0] XuanWuBlessing")]
        [TestCase(Configuration.Id, 0, Levels.One, "CNY", 1000, 10000000, 1, TestName = "[LVL1][10B][R][FG-0] XuanWuBlessing")]
        [TestCase(Configuration.Id, 0, Levels.One, "CNY", 100, 10000, 1, TestName = "[LVL1][1M][R][FG-0] XuanWuBlessing")]
        [TestCase(Configuration.Id, 0, Levels.One, "CNY", 100, 1000000, 1, TestName = "[LVL1][100M][R][FG-0] XuanWuBlessing")]
        [TestCase(Configuration.Id, 1, Levels.One, "CNY", 100, 1000000, 1, TestName = "[LVL1][100M][R][FG-1] XuanWuBlessing")]
        [TestCase(Configuration.Id, 2, Levels.One, "CNY", 100, 1000000, 1, TestName = "[LVL1][100M][R][FG-2] XuanWuBlessing")]
        [TestCase(Configuration.Id, 3, Levels.One, "CNY", 100, 1000000, 1, TestName = "[LVL1][100M][R][FG-3] XuanWuBlessing")]
        [TestCase(Configuration.Id, 4, Levels.One, "CNY", 100, 1000000, 1, TestName = "[LVL1][100M][R][FG-4] XuanWuBlessing")]
        [TestCase(Configuration.Id, 5, Levels.One, "CNY", 100, 1000000, 1, TestName = "[LVL1][100M][R][FG-5] XuanWuBlessing")]
        [TestCase(Configuration.Id, 6, Levels.One, "CNY", 100, 1000000, 1, TestName = "[LVL1][100M][R][FG-6] XuanWuBlessing")]
        [TestCase(Configuration.Id, 7, Levels.One, "CNY", 100, 1000000, 1, TestName = "[LVL1][100M][R][FG-7] XuanWuBlessing")]
        [TestCase(Configuration.Id, 7, Levels.One, "CNY", 1000, 1000000, 1, TestName = "[LVL1][1B][R][FG-7] XuanWuBlessing")]

        [TestCase(Configuration.Id, 0, Levels.Two, "CNY", 100, 10000000, 1, TestName = "[LVL2][1B][R][FG-0] XuanWuBlessing")]
        [TestCase(Configuration.Id, 0, Levels.Two, "CNY", 1000, 10000000, 1, TestName = "[LVL2][10B][R][FG-0] XuanWuBlessing")]
        [TestCase(Configuration.Id, 0, Levels.Two, "CNY", 100, 1000000, 1, TestName = "[LVL2][100M][R][FG-0] XuanWuBlessing")]
        [TestCase(Configuration.Id, 1, Levels.Two, "CNY", 100, 1000000, 1, TestName = "[LVL2][100M][R][FG-1] XuanWuBlessing")]
        [TestCase(Configuration.Id, 2, Levels.Two, "CNY", 100, 1000000, 1, TestName = "[LVL2][100M][R][FG-2] XuanWuBlessing")]
        [TestCase(Configuration.Id, 3, Levels.Two, "CNY", 100, 1000000, 1, TestName = "[LVL2][100M][R][FG-3] XuanWuBlessing")]
        [TestCase(Configuration.Id, 4, Levels.Two, "CNY", 100, 1000000, 1, TestName = "[LVL2][100M][R][FG-4] XuanWuBlessing")]
        [TestCase(Configuration.Id, 5, Levels.Two, "CNY", 100, 1000000, 1, TestName = "[LVL2][100M][R][FG-5] XuanWuBlessing")]
        [TestCase(Configuration.Id, 6, Levels.Two, "CNY", 100, 1000000, 1, TestName = "[LVL2][100M][R][FG-6] XuanWuBlessing")]
        [TestCase(Configuration.Id, 7, Levels.Two, "CNY", 100, 1000000, 1, TestName = "[LVL2][100M][R][FG-7] XuanWuBlessing")]
        [TestCase(Configuration.Id, 7, Levels.Two, "CNY", 1000, 1000000, 1, TestName = "[LVL2][1B][R][FG-7] XuanWuBlessing")]


        [TestCase(Configuration.Id, 0, Levels.Three, "CNY", 100, 10000000, 1, TestName = "[LVL3][1B][R][FG-0] XuanWuBlessing")]
        [TestCase(Configuration.Id, 0, Levels.Three, "CNY", 1000, 10000000, 1, TestName = "[LVL3][10B][R][FG-0] XuanWuBlessing")]
        [TestCase(Configuration.Id, 0, Levels.Three, "CNY", 100, 1000000, 1, TestName = "[LVL3][100M][R][FG-0] XuanWuBlessing")]
        [TestCase(Configuration.Id, 1, Levels.Three, "CNY", 100, 1000000, 1, TestName = "[LVL3][100M][R][FG-1] XuanWuBlessing")]
        [TestCase(Configuration.Id, 2, Levels.Three, "CNY", 100, 1000000, 1, TestName = "[LVL3][100M][R][FG-2] XuanWuBlessing")]
        [TestCase(Configuration.Id, 3, Levels.Three, "CNY", 100, 1000000, 1, TestName = "[LVL3][100M][R][FG-3] XuanWuBlessing")]
        [TestCase(Configuration.Id, 4, Levels.Three, "CNY", 100, 1000000, 1, TestName = "[LVL3][100M][R][FG-4] XuanWuBlessing")]
        [TestCase(Configuration.Id, 5, Levels.Three, "CNY", 100, 1000000, 1, TestName = "[LVL3][100M][R][FG-5] XuanWuBlessing")]
        [TestCase(Configuration.Id, 6, Levels.Three, "CNY", 100, 1000000, 1, TestName = "[LVL3][100M][R][FG-6] XuanWuBlessing")]
        [TestCase(Configuration.Id, 7, Levels.Three, "CNY", 100, 1000000, 1, TestName = "[LVL3][100M][R][FG-7] XuanWuBlessing")]
        [TestCase(Configuration.Id, 7, Levels.Three, "CNY", 1000, 1000000, 1, TestName = "[LVL3][1B][R][FG-7] XuanWuBlessing")]
        public void RandomSpinRandomFreeGameSelection(int gameId, int freeGameSelection, int level, string currencyCode, int numOfUsers, int numItrPerUser, decimal bet)
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
                                if (bonus is FreeSpinSelectionBonus)
                                    bonusRequestContext = SimulationHelper.GetMockBonusRequestContext(freeGameSelection == 0 ? RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection - 1) : freeGameSelection, gameId);

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

        [TestCase(Configuration.Id, Levels.One, "MainGameA", TestName = "[LVL1][MainGameA] Main Game Full Cycle XuanWuBlessing")]
        [TestCase(Configuration.Id, Levels.One, "MainGameB", TestName = "[LVL1][MainGameB] Main Game Full Cycle XuanWuBlessing")]
        [TestCase(Configuration.Id, Levels.One, "MainGameC", TestName = "[LVL1][MainGameC] Main Game Full Cycle XuanWuBlessing")]
        [TestCase(Configuration.Id, Levels.One, "MainGameD", TestName = "[LVL1][MainGameD] Main Game Full Cycle XuanWuBlessing")]
        [TestCase(Configuration.Id, Levels.One, "MainGameE", TestName = "[LVL1][MainGameE] Main Game Full Cycle XuanWuBlessing")]
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

                                var topIndices = MainGameEngine.GenerateRandomWheelIndices(targetWheel);
                                var stackedReels = MainGameEngine.GetStackedSymbols(wheel);
                                var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
                                var hasWildStackedReels = stackedReels.Any(sr => sr.Symbol == Symbols.Wild);
                                var multiplier = hasWildStackedReels ? configuration.StackedWildMultiplier : spinBet.Multiplier;

                                if (stackedReels.Any(sr => sr.Symbol == Symbols.Mystery)) /// Calculate wins with replaced mystery symbols
                                {
                                    var replacementSymbol = MainGameEngine.GetMysterySymbolReplacement(configuration.MysterySymbolReplacementWeights);
                                    var replacedMysterySymbolWheel = MainGameEngine.GenerateReplacedMysterySymbolWheel(configuration, wheel, replacementSymbol);
                                    var winPositions = MainGameEngine.GenerateWinPositions(
                                                        configuration.Payline,
                                                        configuration.PayTable,
                                                        replacedMysterySymbolWheel,
                                                        spinBet.LineBet,
                                                        spinBet.Lines,
                                                        multiplier);

                                    var spinResult = new SpinResult(level, spinBet, wheel, winPositions, bonusPositions, multiplier, replacementSymbol);

                                    totalSummaryData.Update(spinResult);
                                }
                                else /// Calculate wins with initial wheel
                                {
                                    var winPositions = MainGameEngine.GenerateWinPositions(
                                                        configuration.Payline,
                                                        configuration.PayTable,
                                                        wheel,
                                                        spinBet.LineBet,
                                                        spinBet.Lines,
                                                        multiplier);

                                    var spinResult = new SpinResult(level, spinBet, wheel, winPositions, bonusPositions, multiplier);

                                    totalSummaryData.Update(spinResult);
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
