using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.PhantomThief.Configuration;
using static Slot.Games.PhantomThief.Models.Test.SimulationHelper;
using static Slot.UnitTests.PhantomThief.SpinsHelper;

namespace Slot.UnitTests.PhantomThief
{
    [TestFixture]
    public class ModuleTests
    {
        [TestCase(Configuration.Id, TestName = "PhantomThief-ModuleShouldReturnValidGameId")]
        public void ModuleShouldReturnValidGameId(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsTrue(module.GameId > 0);
        }

        [TestCase(Configuration.Id, TestName = "PhantomThief-ModuleShouldReturnValidConfiguration")]
        public void ModuleShouldReturnValidConfiguration(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsNotNull(module.Configuration);
        }

        [TestCase(Configuration.Id, 1, TestName = "PhantomThief-ModuleShouldReturnCorrectTotalBet-30", ExpectedResult = 30)]
        [TestCase(Configuration.Id, 2, TestName = "PhantomThief-ModuleShouldReturnCorrectTotalBet-60", ExpectedResult = 60)]
        [TestCase(Configuration.Id, 4, TestName = "PhantomThief-ModuleShouldReturnCorrectTotalBet-120", ExpectedResult = 120)]
        public decimal ModuleShouldReturnCorrectTotalBet(int gameId, decimal lineBet)
        {
            var module = GetModule(gameId);

            var actualTotalBet = module.CalculateTotalBet(null, new RequestContext<SpinArgs>("", "", Model.PlatformType.Web)
            {
                Parameters = new SpinArgs { LineBet = lineBet }
            });

            return actualTotalBet;
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "PhantomThief-ModuleShouldConvertBonus")]
        public void ModuleShouldConvertBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWithBonusSpinResult(level);
            var bonus = module.CreateBonus(spinWithBonus).Value;
            var bonusEntity = GetBonusEntity(new Model.UserGameKey(-1, gameId), bonus);

            Assert.DoesNotThrow(() => module.ConvertToBonus(bonusEntity));
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "PhantomThief-ModuleShouldCreateBonus")]
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

        [TestCase(Configuration.Id, Levels.One, TestName = "PhantomThief-ModuleShouldExecuteBonus")]
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

        [TestCase(Configuration.Id, Levels.One, TestName = "PhantomThief-ModuleShouldExecuteSpin")]
        public void ModuleShouldExecuteSpin(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() =>
            {
                var spin = module.ExecuteSpin(level, null, GetMockSpinRequestContext(0));

                Assert.IsTrue(!spin.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "PhantomThief-ModuleShouldReturnExtraGameSettings")]
        public void ModuleShouldReturnExtraGameSettings(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() => module.GetExtraSettings(level));
        }

        [TestCase(Configuration.Id, TestName = "PhantomThief-ModuleShouldReturnValidInitialRandomWheel")]
        public void ModuleShouldReturnValidInitialRandomWheel(int gameId)
        {
            var module = GetModule(gameId);

            Assert.IsNotNull(module.InitialRandomWheel());
        }
    }
}
