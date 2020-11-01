using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FourGuardians.Configuration;
using Slot.Games.FourGuardians.Engines;
using Slot.Model;
using static Slot.UnitTests.FourGuardians.SpinsHelper;
using SpinResult = Slot.Games.FourGuardians.Models.GameResults.Spins.SpinResult;
using SpinXml = Slot.Games.FourGuardians.Models.Xml.SpinXml;

namespace Slot.UnitTests.FourGuardians.GameResults.Spins
{
    [TestFixture]
    public class SpinResultTests
    {
        [TestCase("7,8,5|7,8,10|0,11,0|7,8,10|7,8,6", Levels.One, 1, TestName = "FourGuardians-PayoutTest-LevelOne-2415", ExpectedResult = 2415)]
        [TestCase("0,0,3|0,4,0|0,1,3|0,7,3|0,1,0", Levels.One, 1, TestName = "FourGuardians-PayoutTest-LevelOne-31", ExpectedResult = 31)]
        [TestCase("5,7,6|5,6,3|6,4,2|6,4,3|6,6,6", Levels.One, 1, TestName = "FourGuardians-PayoutTest-LevelOne-110", ExpectedResult = 110)]
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
            var expandedWheel = ExpandingWildsEngine.GenerateWheelWithExpandedWilds(wheel, config);
            var winPositions = MainGameEngine.GenerateWinPositions(
                                                config.Payline,
                                                config.PayTable,
                                                expandedWheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                1);

            var spinResult = new SpinResult(spinBet, wheel, winPositions);

            return spinResult.Win;
        }

        [TestCase(Levels.One, TestName = "FourGuardians-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "FourGuardians-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var responseXml = spinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "FourGuardians-ReadResponseXmlOfSpinResult")]
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
