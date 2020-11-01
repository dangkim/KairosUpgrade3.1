using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.LionDance.Configuration;
using static Slot.Games.LionDance.Models.Test.SimulationHelper;
using static Slot.UnitTests.LionDance.SpinsHelper;

namespace Slot.UnitTests.LionDance
{
    [TestFixture]
    public class ModuleTests
    {
        [TestCase(Configuration.Id, TestName = "LionDance-ModuleShouldReturnValidGameId")]
        public void ModuleShouldReturnValidGameId(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsTrue(module.GameId > 0);
        }

        [TestCase(Configuration.Id, TestName = "LionDance-ModuleShouldReturnValidConfiguration")]
        public void ModuleShouldReturnValidConfiguration(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsNotNull(module.Configuration);
        }

        [TestCase(Configuration.Id, 1, TestName = "LionDance-ModuleShouldReturnCorrectTotalBet-10", ExpectedResult = 10)]
        [TestCase(Configuration.Id, 2, TestName = "LionDance-ModuleShouldReturnCorrectTotalBet-20", ExpectedResult = 20)]
        [TestCase(Configuration.Id, 4, TestName = "LionDance-ModuleShouldReturnCorrectTotalBet-40", ExpectedResult = 40)]
        public decimal ModuleShouldReturnCorrectTotalBet(int gameId, decimal lineBet)
        {
            var module = GetModule(gameId);

            var actualTotalBet = module.CalculateTotalBet(null, new RequestContext<SpinArgs>("", "", Model.PlatformType.Web)
            {
                Parameters = new SpinArgs { LineBet = lineBet }
            });

            return actualTotalBet;
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "LionDance-ModuleShouldConvertBonus")]
        public void ModuleShouldConvertBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWinningSpinResult(level);
            var bonus = module.CreateBonus(spinWithBonus).Value;
            var bonusEntity = GetBonusEntity(new Model.UserGameKey(-1, gameId), bonus);

            Assert.DoesNotThrow(() => module.ConvertToBonus(bonusEntity));
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "LionDance-ModuleShouldCreateBonus")]
        public void ModuleShouldCreateBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWinningSpinResult(level);

            Assert.DoesNotThrow(() =>
            {
                var bonus = module.CreateBonus(spinWithBonus);

                Assert.IsTrue(!bonus.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "LionDance-ModuleShouldExecuteBonus")]
        public void ModuleShouldExecuteBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWinningSpinResult(level);
            var bonus = module.CreateBonus(spinWithBonus).Value;
            var bonusEntity = GetBonusEntity(new Model.UserGameKey(-1, gameId), bonus);
            var bonusContext = GetMockBonusRequestContext(gameId, 0);

            Assert.DoesNotThrow(() =>
            {
                var bonusResult = module.ExecuteBonus(level, bonusEntity, bonusContext);

                Assert.IsTrue(!bonusResult.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "LionDance-ModuleShouldExecuteSpin")]
        public void ModuleShouldExecuteSpin(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() =>
            {
                var spin = module.ExecuteSpin(level, null, GetMockSpinRequestContext(0));

                Assert.IsTrue(!spin.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "LionDance-ModuleShouldReturnExtraGameSettings")]
        public void ModuleShouldReturnExtraGameSettings(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() => module.GetExtraSettings(level));
        }

        [TestCase(Configuration.Id, TestName = "LionDance-ModuleShouldReturnValidInitialRandomWheel")]
        public void ModuleShouldReturnValidInitialRandomWheel(int gameId)
        {
            var module = GetModule(gameId);

            Assert.IsNotNull(module.InitialRandomWheel());
        }
    }
}
