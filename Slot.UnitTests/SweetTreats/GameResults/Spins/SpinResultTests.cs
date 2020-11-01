using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.SweetTreats.Configuration;
using Slot.Games.SweetTreats.Models.Engines;
using Slot.Model;
using static Slot.UnitTests.SweetTreats.SimulationHelper;
using SpinResult = Slot.Games.SweetTreats.Models.GameResults.Spins.SpinResult;
using SpinXml = Slot.Games.SweetTreats.Models.Xml.SpinXml;

namespace Slot.UnitTests.SweetTreats.GameResults.Spins
{
    [TestFixture]
    public class SpinResultTests
    {
        [TestCase("9,9,9|9,9,9|9,9,9", Levels.One, 1, TestName = "SweetTreats-PayoutTest-LevelOne-5600", ExpectedResult = 5600)]
        [TestCase("9,9,9|9,8,9|9,9,9", Levels.One, 1, TestName = "SweetTreats-PayoutTest-LevelOne-3600", ExpectedResult = 3600)]
        [TestCase("1,3,4|0,9,5|9,2,9", Levels.One, 1, TestName = "SweetTreats-PayoutTest-LevelOne-49", ExpectedResult = 49)]
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
                                                config.PayTable,
                                                wheel,
                                                spinBet.LineBet,
                                                spinBet.Lines,
                                                1);

            var spinResult = new SpinResult(spinBet, wheel, winPositions);

            return spinResult.Win;
        }

        [TestCase(Levels.One, TestName = "SweetTreats-CreateBonusXElementOfSpinResult")]
        public void EngineShouldCreateBonusXElementOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var xElement = spinResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "SweetTreats-CreateBonusResponseXmlOfSpinResult")]
        public void EngineShouldCreateBonusResponseXmlOfSpinResult(int level)
        {
            var spinResult = GenerateWinningSpinResult(level);
            var responseXml = spinResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "SweetTreats-ReadResponseXmlOfSpinResult")]
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
