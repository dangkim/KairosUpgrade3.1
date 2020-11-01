using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.JewelLand.Configuration;
using static Slot.Games.JewelLand.Models.Test.SimulationHelper;
using static Slot.UnitTests.JewelLand.SpinsHelper;

namespace Slot.UnitTests.JewelLand
{
    [TestFixture]
    public class ModuleTests
    {
        [TestCase(Configuration.Id, TestName = "JewelLand-ModuleShouldReturnValidGameId")]
        public void ModuleShouldReturnValidGameId(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsTrue(module.GameId > 0);
        }

        [TestCase(Configuration.Id, TestName = "JewelLand-ModuleShouldReturnValidConfiguration")]
        public void ModuleShouldReturnValidConfiguration(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsNotNull(module.Configuration);
        }

        [TestCase(Configuration.Id, 1, TestName = "JewelLand-ModuleShouldReturnCorrectTotalBet-5", ExpectedResult = 5)]
        [TestCase(Configuration.Id, 2, TestName = "JewelLand-ModuleShouldReturnCorrectTotalBet-10", ExpectedResult = 10)]
        [TestCase(Configuration.Id, 4, TestName = "JewelLand-ModuleShouldReturnCorrectTotalBet-20", ExpectedResult = 20)]
        public decimal ModuleShouldReturnCorrectTotalBet(int gameId, decimal lineBet)
        {
            var module = GetModule(gameId);

            var actualTotalBet = module.CalculateTotalBet(null, new RequestContext<SpinArgs>("", "", Model.PlatformType.Web)
            {
                Parameters = new SpinArgs { LineBet = lineBet }
            });

            return actualTotalBet;
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ModuleShouldConvertBonus")]
        public void ModuleShouldConvertBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWithBonusSpinResult(level);
            var bonus = module.CreateBonus(spinWithBonus).Value;
            var bonusEntity = GetBonusEntity(new Model.UserGameKey(-1, gameId), bonus);

            Assert.DoesNotThrow(() => module.ConvertToBonus(bonusEntity));
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ModuleShouldCreateBonus")]
        public void ModuleShouldCreateBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWithBonusSpinResult(level);

            Assert.DoesNotThrow(() =>
            {
                var bonus = module.CreateBonus(spinWithBonus);

                Assert.IsTrue(!bonus.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ModuleShouldExecuteBonus")]
        public void ModuleShouldExecuteBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWithBonusSpinResult(level);
            var bonus = module.CreateBonus(spinWithBonus).Value;
            var bonusEntity = GetBonusEntity(new Model.UserGameKey(-1, gameId), bonus);
            var bonusContext = GetMockBonusRequestContext(gameId, 0);

            Assert.DoesNotThrow(() =>
            {
                var bonusResult = module.ExecuteBonus(level, bonusEntity, bonusContext);

                Assert.IsTrue(!bonusResult.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ModuleShouldExecuteSpin")]
        public void ModuleShouldExecuteSpin(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() =>
            {
                var spin = module.ExecuteSpin(level, null, GetMockSpinRequestContext(0));

                Assert.IsTrue(!spin.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "JewelLand-ModuleShouldReturnExtraGameSettings")]
        public void ModuleShouldReturnExtraGameSettings(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() => module.GetExtraSettings(level));
        }

        [TestCase(Configuration.Id, TestName = "JewelLand-ModuleShouldReturnValidInitialRandomWheel")]
        public void ModuleShouldReturnValidInitialRandomWheel(int gameId)
        {
            var module = GetModule(gameId);

            Assert.IsNotNull(module.InitialRandomWheel());
        }
    }
}
