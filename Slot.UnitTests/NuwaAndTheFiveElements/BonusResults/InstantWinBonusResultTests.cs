using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Engines;
using Slot.Model;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements.BonusResults
{
    [TestFixture]
    public class InstantWinBonusResultTests
    {
        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-WithSameGameResultAndBonus")]
        public void EngineShouldCreateInstantWinBonusResultWithSameGameResultAndBonus(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);

            Assert.AreSame(instantWinBonusResult.Bonus, instantWinBonus);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-InstantWinBonusResultWithSameSpinTransactionId")]
        public void EngineShouldCreateInstantWinBonusResultWithSameSpinTransactionId(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);

            Assert.IsTrue(instantWinBonusResult.SpinTransactionId == instantWinBonus.SpinTransactionId);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CompleteInstantWinBonusResultOnCreate")]
        public void EngineShouldCompleteInstantWinBonusResultOnCreate(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);

            Assert.IsTrue(instantWinBonusResult.IsCompleted);
        }

        [TestCase("3,1,2,2,11,5,6,4,6,5,4,3,2,1,5,2,5,6,7,11,1,6,1,4,6,2,2,1,8,4,2,6,1,2,11", Levels.One, 5, TestName = "NuwaAndTheFiveElements-PayoutTest-5", ExpectedResult = 375)]
        [TestCase("3,1,2,2,11,5,6,4,6,5,4,3,2,1,5,2,5,6,7,11,1,6,1,4,6,2,2,1,8,4,2,6,1,2,11", Levels.One, 10, TestName = "NuwaAndTheFiveElements-PayoutTest-10", ExpectedResult = 750)]
        public decimal EngineShouldCreateCorrectPayout(string wheelString, int level, int scatterMultiplier)
        {
            var config = new Configuration();
            var spinBet = MainGameEngine.GenerateSpinBet(new RequestContext<SpinArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new SpinArgs
                {
                    LineBet = 1,
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
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);

            instantWinBonus.UpdateBonus(scatterMultiplier);
            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);

            return instantWinBonusResult.Win;
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusXElementOfInstantWinBonusResult")]
        public void EngineShouldCreateBonusXElementOfInstantWinBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);

            var xElement = instantWinBonusResult.ToXElement();

            Assert.IsNotNull(xElement);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-CreateBonusResponseXmlOfInstantWinBonusResult")]
        public void EngineShouldCreateBonusResponseXmlOfInstantWinBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);

            var responseXml = instantWinBonusResult.ToResponseXml(ResponseXmlFormat.History);

            Assert.IsNotNull(responseXml);
        }

        [TestCase(Levels.One, TestName = "NuwaAndTheFiveElements-ReadResponseXmlOfInstantBonusResult")]
        public void EngineShouldReadResponseXmlOfInstantBonusResult(int level)
        {
            var config = new Configuration();
            var spinResult = GenerateWithBonusSpinResult(level);
            var instantWinBonus = InstantWinBonusEngine.CreateInstantWinBonus(spinResult);
            var instantWinBonusResult = InstantWinBonusEngine.CreateInstantWinBonusResult(instantWinBonus);

            var xElement = instantWinBonusResult.ToXElement();

            Assert.DoesNotThrow(() =>
            {
                using (var xmlReader = xElement.CreateReader())
                {
                    var responseXml = new BonusXml();
                    responseXml.ReadXml(xmlReader);
                }
            });
        }
    }
}
