using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.LionDance.Configuration;
using Slot.Games.LionDance.Engines;
using Slot.Model;
using static Slot.UnitTests.LionDance.SpinsHelper;
using SpinResult = Slot.Games.LionDance.Models.GameResults.Spins.SpinResult;
using SpinXml = Slot.Games.LionDance.Models.Xml.SpinXml;

namespace Slot.UnitTests.LionDance.GameResults.Spins
{
    [TestFixture]
    public class SpinResultTests
    {
        [TestCase("1,8,5|0,1,3|2,0,8", Levels.One, 0.25, TestName = "LionDance-PayoutTest-LevelOne-0p25-7p25", ExpectedResult = 7.25)]
        [TestCase("1,4,5|8,2,3|0,8,6", Levels.One, 0.25, TestName = "LionDance-PayoutTest-LevelOne-0p25-7p5", ExpectedResult = 7.5)]
        [TestCase("1,0,1|0,2,6|0,7,4", Levels.One, 0.25, TestName = "LionDance-PayoutTest-LevelOne-0p25-1", ExpectedResult = 1)]
        [TestCase("6,1,4|4,5,1|7,4,1", Levels.One, 0.25, TestName = "LionDance-PayoutTest-LevelOne-0p25-4", ExpectedResult = 4)]
        [TestCase("2,1,8|6,0,4|7,4,1", Levels.One, 0.25, TestName = "LionDance-PayoutTest-LevelOne-0p25-4p5", ExpectedResult = 4.5)]
        [TestCase("2,6,3|1,3,0|0,2,1", Levels.One, 0.25, TestName = "LionDance-PayoutTest-LevelOne-0p25-0", ExpectedResult = 0)]
        [TestCase("7,6,3|1,3,0|0,2,1", Levels.One, 0.25, TestName = "LionDance-PayoutTest-LevelOne-0p25-0p5", ExpectedResult = 0.5)]
        [TestCase("7,6,3|8,3,0|0,2,1", Levels.One, 0.25, TestName = "LionDance-PayoutTest-LevelOne-0p25-1p25", ExpectedResult = 1.25)]
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
            var winPositions = MainGameEngine.GenerateWinPositions(config.Payline, config.PayTable, wheel, spinBet.LineBet, 1);

            var spinResult = new SpinResult(spinBet, wheel, null, winPositions);

            return spinResult.Win;
        }

        [TestCase(Levels.One, TestName = "LionDance-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "LionDance-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var responseXml = spinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "LionDance-ReadResponseXmlOfSpinResult")]
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
