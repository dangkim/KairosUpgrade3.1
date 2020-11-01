using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Model;
using System;
using System.Linq;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;
using SpinXml = Slot.Games.NuwaAndTheFiveElements.Models.Xml.SpinXml;

namespace Slot.UnitTests.NuwaAndTheFiveElements.GameResults
{
    [TestFixture]
    public class SpinResultTests
    {
        [TestCase(Levels.One, 1, 5, TestName = "NuwaAndTheFiveElements-PayoutTest-5")]
        [TestCase(Levels.One, 6, 16, TestName = "NuwaAndTheFiveElements-PayoutTest-16")]
        [TestCase(Levels.One, 17, 25, TestName = "NuwaAndTheFiveElements-PayoutTest-25")]
        [TestCase(Levels.One, 26, 37, TestName = "NuwaAndTheFiveElements-PayoutTest-37")]
        [TestCase(Levels.One, 38, int.MaxValue, TestName = "NuwaAndTheFiveElements-PayoutTest-38")]
        public void SpinResultShouldCreateCorrectPayout(int level, int minimumBetMultiplier, int maximumBetMultiplier)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var minimumLimit = spinResult.SpinBet.TotalBet * minimumBetMultiplier;
            var maximumLimit = spinResult.SpinBet.TotalBet * maximumBetMultiplier;

            while (spinResult.Win < minimumLimit || spinResult.Win > maximumLimit)
                spinResult = GenerateWinningSpinResult(level);

            Console.WriteLine($"Win: {spinResult.Win}");
            Console.WriteLine($"Top Indices: {string.Join(",", spinResult.TopIndices.Select(index => index))}");
            Console.WriteLine($"Wheel: {string.Join("|", spinResult.Wheel.Reels.Select(reel => string.Join(",", reel)))}");
            Assert.Pass();
        }

        [TestCase("10,12,12,12,12,9,9,10,12,12,12,12,12,12,10,12,12,12,11,7,7,10,12,12,12,5,5,0,10,12,13,12,9,9,9", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-1100-ValidReels(14,14,19,13,12)", ExpectedResult = 1100)]
        [TestCase("10,12,12,12,12,9,9,10,12,12,12,12,12,12,10,12,12,12,11,7,7,6,1,1,8,8,8,8,5,5,0,0,7,7,7", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-300-ValidReels(14,14,19,23,23)", ExpectedResult = 300)]
        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-1935", ExpectedResult = 1935)]
        [TestCase("0,1,2,2,4,5,6,0,6,5,4,3,2,1,0,2,5,6,7,3,1,0,1,4,6,2,2,1,0,4,2,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-850", ExpectedResult = 850)]
        [TestCase("6,1,2,3,4,5,6,4,6,5,4,8,2,1,0,2,6,6,7,3,1,3,6,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-80", ExpectedResult = 80)]
        [TestCase("1,3,2,2,4,5,6,1,6,5,4,3,2,1,1,2,5,6,7,3,1,3,0,4,6,2,2,1,7,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-60", ExpectedResult = 60)]
        [TestCase("0,1,2,3,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-20", ExpectedResult = 20)]
        [TestCase("0,1,2,1,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-0", ExpectedResult = 0)]
        [TestCase("0,1,2,11,4,5,6,4,1,5,4,11,2,1,0,2,0,6,7,11,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-1", ExpectedResult = 75)]
        [TestCase("12,12,12,12,9,9,9,12,12,12,12,12,12,12,12,12,12,11,7,7,2,9,9,9,9,4,4,11,11,8,8,8,8,3,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-PayoutTest-LevelOne-Bet1-1-ValidReels(15,15,20,6,5)", ExpectedResult = 75)]
        public decimal SpinResultShouldCreateCorrectPayout(string wheelString, int level, decimal bet)
        {
            var config = new Configuration();
            var spinBet = MainGameEngine.GenerateSpinBet(new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = bet,
                    Multiplier = 1
                }
            });

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                spinBet.Multiplier);

            var spinResult = new Games.NuwaAndTheFiveElements.Models.GameResults.Spins.SpinResult(level, spinBet, wheel, null, winPositions, null, null);

            return spinResult.Win;
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldFlagSpinResultWithBonusOnScatter")]
        public void SpinResultShouldFlagSpinResultWithBonusOnScatter(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);

            Assert.IsTrue(spinResult.HasFeatureBonus);
        }

        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,13", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldHaveBombCollapseWhenReelsHaveBombAndWin-True", ExpectedResult = true)]
        [TestCase("0,1,2,4,4,5,6,4,1,5,4,7,2,1,0,2,0,6,7,5,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldHaveBombCollapseWhenReelsHaveBombAndWin-False", ExpectedResult = false)]
        [TestCase("0,1,2,11,4,5,6,4,1,5,4,11,2,1,0,2,0,6,7,11,1,3,0,4,6,2,2,1,0,4,0,6,1,2,13", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldHaveBombCollapseWhenReelsHaveBombAndWin-False-HasFeature", ExpectedResult = false)]
        public bool SpinResultShouldHaveBombCollapseWhenReelsHaveBombAndWin(string wheelString, int level, decimal bet)
        {
            var config = new Configuration();
            var spinBet = MainGameEngine.GenerateSpinBet(new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = bet,
                    Multiplier = 1
                }
            });

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                spinBet.Multiplier);
            var bombAndStopperPositions = MainGameEngine.GenerateBombAndStopperPositions(wheel, winPositions);

            var spinResult = new Games.NuwaAndTheFiveElements.Models.GameResults.Spins.SpinResult(level, spinBet, wheel, null, winPositions, null, bombAndStopperPositions);

            return spinResult.HasBomb && spinResult.Collapse;
        }

        [TestCase("10,1,2,2,4,5,6,10,6,5,4,3,2,1,10,2,5,6,7,3,1,10,1,4,6,2,2,1,10,4,2,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldCreateValidBonusElementOnUpdateWhenWin-LevelOne-Bet1-1935")]
        [TestCase("0,1,2,2,4,5,6,0,6,5,4,3,2,1,0,2,5,6,7,3,1,0,1,4,6,2,2,1,0,4,2,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldCreateValidBonusElementOnUpdateWhenWin-LevelOne-Bet1-850")]
        [TestCase("6,1,2,3,4,5,6,4,6,5,4,8,2,1,0,2,6,6,7,3,1,3,6,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldCreateValidBonusElementOnUpdateWhenWin-LevelOne-Bet1-80")]
        [TestCase("1,3,2,2,4,5,6,1,6,5,4,3,2,1,1,2,5,6,7,3,1,3,0,4,6,2,2,1,7,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldCreateValidBonusElementOnUpdateWhenWin-LevelOne-Bet1-60")]
        [TestCase("0,1,2,3,4,5,6,4,1,5,4,3,2,1,0,2,0,6,7,3,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldCreateValidBonusElementOnUpdateWhenWin-LevelOne-Bet1-20")]
        [TestCase("0,1,2,11,4,5,6,4,1,5,4,11,2,1,0,2,0,6,7,11,1,3,0,4,6,2,2,1,0,4,0,6,1,2,3", Levels.One, 1, TestName = "NuwaAndTheFiveElements-ShouldCreateValidBonusElementOnUpdateWhenWin-LevelOne-Bet1-1")]
        public void SpinResultShouldCreateValidBonusElementOnUpdateWhenWin(string wheelString, int level, decimal bet)
        {
            var config = new Configuration();
            var spinBet = MainGameEngine.GenerateSpinBet(new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = bet,
                    Multiplier = 1,
                },
            });

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                spinBet.Multiplier);

            var spinResult = new Games.NuwaAndTheFiveElements.Models.GameResults.Spins.SpinResult(level, spinBet, wheel, null, winPositions, null, null) { Level = level };
            var featureType = FeatureBonusEngine.GetFeatureType(spinResult, config.BonusConfig.TriggerWeights[spinResult.Level]);
            var bonus = FeatureBonusEngine.GetBonus(featureType, spinResult, config);

            spinResult.UpdateBonus(bonus);

            Assert.IsTrue(spinResult.Bonus.Id > 0 && !string.IsNullOrWhiteSpace(spinResult.Bonus.Value));
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void SpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var spinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            freeSpinBonus.UpdateBonus(spinResult);
            spinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = spinResult.Bonus.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = spinResult.Bonus.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var responseXml = spinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ReadResponseXmlOfSpinResult")]
        public void EngineShouldReadResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                using (var xmlReader = xElement.CreateReader())
                {
                    var responseXml = new SpinXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }
    }
}
