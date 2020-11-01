using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Configuration.Bonuses;
using static Slot.Games.XuanWuBlessing.Models.Test.SimulationHelper;
using static Slot.UnitTests.XuanWuBlessing.SpinsHelper;

namespace Slot.UnitTests.XuanWuBlessing
{
    [TestFixture]
    public class ModuleTests
    {
        [TestCase(Configuration.Id, TestName = "XuanWuBlessing-ModuleShouldReturnValidGameId")]
        public void ModuleShouldReturnValidGameId(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsTrue(module.GameId > 0);
        }

        [TestCase(Configuration.Id, TestName = "XuanWuBlessing-ModuleShouldReturnValidConfiguration")]
        public void ModuleShouldReturnValidConfiguration(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsNotNull(module.Configuration);
        }

        [TestCase(Configuration.Id, 1, TestName = "XuanWuBlessing-ModuleShouldReturnCorrectTotalBet-30", ExpectedResult = 20)]
        [TestCase(Configuration.Id, 2, TestName = "XuanWuBlessing-ModuleShouldReturnCorrectTotalBet-60", ExpectedResult = 40)]
        [TestCase(Configuration.Id, 4, TestName = "XuanWuBlessing-ModuleShouldReturnCorrectTotalBet-120", ExpectedResult = 80)]
        public decimal ModuleShouldReturnCorrectTotalBet(int gameId, decimal lineBet)
        {
            var module = GetModule(gameId);

            var actualTotalBet = module.CalculateTotalBet(null, new RequestContext<SpinArgs>("", "", Model.PlatformType.Web)
            {
                Parameters = new SpinArgs { LineBet = lineBet }
            });

            return actualTotalBet;
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "XuanWuBlessing-ModuleShouldConvertBonus")]
        public void ModuleShouldConvertBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWithBonusSpinResult(level);
            var bonus = module.CreateBonus(spinWithBonus).Value;
            var bonusEntity = GetBonusEntity(new Model.UserGameKey(-1, gameId), bonus);

            Assert.DoesNotThrow(() => module.ConvertToBonus(bonusEntity));
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "XuanWuBlessing-ModuleShouldCreateBonus")]
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

        [TestCase(Configuration.Id, Levels.One, TestName = "XuanWuBlessing-ModuleShouldExecuteBonus")]
        public void ModuleShouldExecuteBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWithBonusSpinResult(level);
            var bonus = module.CreateBonus(spinWithBonus).Value;
            var bonusEntity = GetBonusEntity(new Model.UserGameKey(-1, gameId), bonus);
            var bonusContext = GetMockBonusRequestContext(RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection), gameId);

            Assert.DoesNotThrow(() =>
            {
                var bonusResult = module.ExecuteBonus(level, bonusEntity, bonusContext);

                Assert.IsTrue(!bonusResult.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "XuanWuBlessing-ModuleShouldExecuteSpin")]
        public void ModuleShouldExecuteSpin(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() =>
            {
                var spin = module.ExecuteSpin(level, null, GetMockSpinRequestContext(0));

                Assert.IsTrue(!spin.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "XuanWuBlessing-ModuleShouldReturnExtraGameSettings")]
        public void ModuleShouldReturnExtraGameSettings(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() => module.GetExtraSettings(level));
        }

        [TestCase(Configuration.Id, TestName = "XuanWuBlessing-ModuleShouldReturnValidInitialRandomWheel")]
        public void ModuleShouldReturnValidInitialRandomWheel(int gameId)
        {
            var module = GetModule(gameId);

            Assert.IsNotNull(module.InitialRandomWheel());
        }
    }
}
