using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Configuration.Bonuses;
using Slot.Games.XuanWuBlessing.Engines;
using Slot.Model;
using System.Linq;
using static Slot.UnitTests.XuanWuBlessing.SpinsHelper;
using SpinResult = Slot.Games.XuanWuBlessing.Models.GameResults.Spins.SpinResult;
using SpinXml = Slot.Games.XuanWuBlessing.Models.Xml.SpinXml;

namespace Slot.UnitTests.XuanWuBlessing.GameResults.Spins
{
    [TestFixture]
    public class SpinResultTests
    {
        [TestCase("6,0,8|12,12,5|6,8,11|13,13,13|13,13,13", 6, Levels.One, 1, TestName = "XuanWuBlessing-PayoutTest-LevelOne-Bet1-310", ExpectedResult = 310)]
        [TestCase("2,7,5|2,2,8|2,5,6|2,1,11|2,6,1", 2, Levels.One, 1, TestName = "XuanWuBlessing-PayoutTest-LevelOne-Bet1-55", ExpectedResult = 55)]
        [TestCase("5,3,9|12,12,3|3,5,12|8,10,3|6,2,10", 2, Levels.One, 1, TestName = "XuanWuBlessing-PayoutTest-LevelOne-Bet1-170", ExpectedResult = 170)]
        [TestCase("5,3,9|12,1,3|3,5,12|8,10,3|6,2,10", 2, Levels.One, 1, TestName = "XuanWuBlessing-PayoutTest-LevelOne-Bet1-40", ExpectedResult = 40)]
        public decimal SpinResultShouldCreateCorrectPayout(string wheelString, int replacementSymbol, int level, decimal bet)
        {
            SpinResult spinResult = null;
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

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var stackedReels = MainGameEngine.GetStackedSymbols(wheel);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var hasWildStackedReels = stackedReels.Any(sr => sr.Symbol == Symbols.Wild);
            var multiplier = hasWildStackedReels ? config.StackedWildMultiplier : spinBet.Multiplier;

            if (wheel.CountDistinct(Symbols.Mystery) > 0)
            {
                var replacedMysterySymbolWheel = MainGameEngine.GenerateReplacedMysterySymbolWheel(config, wheel, replacementSymbol);
                var winPositions = MainGameEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.PayTable,
                                                    replacedMysterySymbolWheel,
                                                    spinBet.LineBet,
                                                    spinBet.Lines,
                                                    multiplier);

                spinResult = new SpinResult(level, spinBet, wheel, winPositions, bonusPositions, replacementSymbol, multiplier);
            }
            else /// Calculate wins with initial wheel
            {
                var winPositions = MainGameEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.PayTable,
                                                    wheel,
                                                    spinBet.LineBet,
                                                    spinBet.Lines,
                                                    multiplier);

                spinResult = new SpinResult(level, spinBet, wheel, winPositions, bonusPositions, multiplier);
            }

            return spinResult.Win;
        }

        [TestCase("2,7,5|2,2,8|5,11,9|2,1,11|7,11,4", Levels.One, 1, TestName = "XuanWuBlessing-ShouldFlagSpinResultWithBonusOnScatter-WithBonus", ExpectedResult = true)]
        [TestCase("2,7,5|2,2,8|2,5,6|2,1,11|2,6,1", Levels.One, 1, TestName = "XuanWuBlessing-ShouldFlagSpinResultWithBonusOnScatter-NoBonus", ExpectedResult = false)]
        public bool SpinResultShouldFlagSpinResultWithBonusOnScatter(string wheelString, int level, decimal bet)
        {
            SpinResult spinResult = null;
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

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var stackedReels = MainGameEngine.GetStackedSymbols(wheel);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);
            var hasWildStackedReels = stackedReels.Any(sr => sr.Symbol == Symbols.Wild);
            var multiplier = hasWildStackedReels ? config.StackedWildMultiplier : spinBet.Multiplier;

            if (wheel.CountDistinct(Symbols.Mystery) > 0)
            {
                var replacementSymbol = MainGameEngine.GetMysterySymbolReplacement(config.MysterySymbolReplacementWeights);
                var replacedMysterySymbolWheel = MainGameEngine.GenerateReplacedMysterySymbolWheel(config, wheel, replacementSymbol);
                var winPositions = MainGameEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.PayTable,
                                                    replacedMysterySymbolWheel,
                                                    spinBet.LineBet,
                                                    spinBet.Lines,
                                                    multiplier);

                spinResult = new SpinResult(level, spinBet, wheel, winPositions, bonusPositions, replacementSymbol, multiplier);
            }
            else /// Calculate wins with initial wheel
            {
                var winPositions = MainGameEngine.GenerateWinPositions(
                                                    config.Payline,
                                                    config.PayTable,
                                                    wheel,
                                                    spinBet.LineBet,
                                                    spinBet.Lines,
                                                    multiplier);

                spinResult = new SpinResult(level, spinBet, wheel, winPositions, bonusPositions, multiplier);
            }

            return spinResult.HasBonus;
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void SpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var spinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var selection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(selection);
            freeSpinSelectionBonus.UpdateBonus(selection, freeSpinMode);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);

            freeSpinBonus.UpdateBonus(spinResult);
            spinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = spinResult.BonusElement.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = spinResult.BonusElement.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var responseXml = spinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ReadResponseXmlOfSpinResult")]
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
