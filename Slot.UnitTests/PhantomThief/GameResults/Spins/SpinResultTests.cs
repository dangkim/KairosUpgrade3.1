using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using Slot.Model;
using static Slot.UnitTests.PhantomThief.SpinsHelper;
using SpinResult = Slot.Games.PhantomThief.Models.GameResults.Spins.SpinResult;
using SpinXml = Slot.Games.PhantomThief.Models.Xml.SpinXml;

namespace Slot.UnitTests.PhantomThief.GameResults.Spins
{
    [TestFixture]
    public class SpinResultTests
    {
        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", Levels.One, 1, TestName = "PhantomThief-PayoutTest-LevelOne-Bet1-900", ExpectedResult = 900)]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", Levels.One, 1, TestName = "PhantomThief-PayoutTest-LevelOne-Bet1-450", ExpectedResult = 450)]
        [TestCase("2,0,6|4,5,1|8,7,2|0,7,4|5,7,2", Levels.One, 1, TestName = "PhantomThief-PayoutTest-LevelOne-Bet1-0", ExpectedResult = 0)]
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

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.MainGamePayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                1);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var spinResult = new SpinResult(spinBet, wheel, null, winPositions, bonusPositions);

            return spinResult.Win;
        }

        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", Levels.One, 1, TestName = "PhantomThief-ShouldFlagSpinResultWithBonusOnScatterOrCollapse-900", ExpectedResult = true)]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", Levels.One, 1, TestName = "PhantomThief-ShouldFlagSpinResultWithBonusOnScatterOrCollapse-450", ExpectedResult = true)]
        [TestCase("2,0,6|4,5,1|8,7,2|0,7,4|5,7,2", Levels.One, 1, TestName = "PhantomThief-ShouldFlagSpinResultWithBonusOnScatterOrCollapse-0", ExpectedResult = false)]
        [TestCase("8,3,7|6,7,8|8,7,2|0,7,4|5,7,2", Levels.One, 1, TestName = "PhantomThief-ShouldFlagSpinResultWithBonusOnScatterOrCollapse-0-Scatter", ExpectedResult = true)]
        public bool SpinResultShouldFlagSpinResultWithBonusOnScatterOrCollapse(string wheelString, int level, decimal bet)
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

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.MainGamePayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                1);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var spinResult = new SpinResult(spinBet, wheel, null, winPositions, bonusPositions);

            return spinResult.HasBonus;
        }

        [TestCase("2,7,3|0,7,1|8,7,2|0,7,4|4,7,5", Levels.One, 1, TestName = "PhantomThief-ShouldFlagSpinResultWithFeatureOnScatter-900", ExpectedResult = false)]
        [TestCase("2,7,3|2,9,5|1,9,0|2,9,0|2,1,4", Levels.One, 1, TestName = "PhantomThief-ShouldFlagSpinResultWithFeatureOnScatter-450", ExpectedResult = false)]
        [TestCase("2,0,6|4,5,1|8,7,2|0,7,4|5,7,2", Levels.One, 1, TestName = "PhantomThief-ShouldFlagSpinResultWithFeatureOnScatter-0", ExpectedResult = false)]
        [TestCase("8,3,7|6,7,8|8,7,2|0,7,4|5,7,2", Levels.One, 1, TestName = "PhantomThief-ShouldFlagSpinResultWithFeatureOnScatter-0-Scatter", ExpectedResult = true)]
        public bool SpinResultShouldFlagSpinResultWithFeatureOnScatter(string wheelString, int level, decimal bet)
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

            var wheel = new Wheel(Game.WheelWidth, Game.WheelHeight, wheelString.ToFormattedWheelString());
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.MainGamePayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                1);
            var bonusPositions = MainGameEngine.GenerateBonusPositions(wheel);

            var spinResult = new SpinResult(spinBet, wheel, null, winPositions, bonusPositions);

            return spinResult.HasFeatureBonus;
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldHaveSameBonusDetailsOnResultUpdate")]
        public void SpinResultShouldHaveSameBonusDetailsOnResultUpdate(int level)
        {
            var spinResult = GenerateWinningFreeSpinResult(level);
            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult);

            freeSpinBonus.UpdateBonus(spinResult, 1);
            spinResult.UpdateBonus(freeSpinBonus);

            var isEqualBonusId = spinResult.BonusElement.Id == freeSpinBonus.Id;
            var isEqualBonusGuid = spinResult.BonusElement.Value == freeSpinBonus.Guid.ToString("N");

            Assert.IsTrue(isEqualBonusId && isEqualBonusGuid);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var responseXml = spinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ReadResponseXmlOfSpinResult")]
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
