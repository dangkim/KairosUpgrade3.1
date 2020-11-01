using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.NuwaAndTheFiveElements.Configuration;
using Slot.Games.NuwaAndTheFiveElements.Configuration.Bonuses;
using Slot.Games.NuwaAndTheFiveElements.Models.Bonuses;
using static Slot.Games.NuwaAndTheFiveElements.Models.Test.SimulationHelper;
using static Slot.UnitTests.NuwaAndTheFiveElements.SpinsHelper;

namespace Slot.UnitTests.NuwaAndTheFiveElements
{
    [TestFixture]
    public class ModuleTests
    {
        [TestCase(Configuration.Id, TestName = "NuwaAndTheFiveElements-ModuleShouldReturnValidGameId")]
        public void ModuleShouldReturnValidGameId(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsTrue(module.GameId > 0);
        }

        [TestCase(Configuration.Id, TestName = "NuwaAndTheFiveElements-ModuleShouldReturnValidConfiguration")]
        public void ModuleShouldReturnValidConfiguration(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsNotNull(module.Configuration);
        }

        [TestCase(Configuration.Id, 1, TestName = "NuwaAndTheFiveElements-ModuleShouldReturnCorrectTotalBet-75", ExpectedResult = 75)]
        [TestCase(Configuration.Id, 2, TestName = "NuwaAndTheFiveElements-ModuleShouldReturnCorrectTotalBet-150", ExpectedResult = 150)]
        [TestCase(Configuration.Id, 4, TestName = "NuwaAndTheFiveElements-ModuleShouldReturnCorrectTotalBet-300", ExpectedResult = 300)]
        public decimal ModuleShouldReturnCorrectTotalBet(int gameId, decimal lineBet)
        {
            var module = GetModule(gameId);

            var actualTotalBet = module.CalculateTotalBet(null, new RequestContext<SpinArgs>("", "", Model.PlatformType.Web)
            {
                Parameters = new SpinArgs { LineBet = lineBet }
            });

            return actualTotalBet;
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-ModuleShouldConvertBonus")]
        public void ModuleShouldConvertBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWithBonusSpinResult(level);
            var bonus = module.CreateBonus(spinWithBonus).Value;
            var bonusEntity = GetBonusEntity(new Model.UserGameKey(-1, gameId), bonus);

            Assert.DoesNotThrow(() => module.ConvertToBonus(bonusEntity));
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-ModuleShouldCreateBonus")]
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

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-ModuleShouldExecuteBonus")]
        public void ModuleShouldExecuteBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            var spinWithBonus = GenerateWithBonusSpinResult(level);
            var bonus = module.CreateBonus(spinWithBonus).Value;
            var bonusEntity = GetBonusEntity(new Model.UserGameKey(-1, gameId), bonus);
            var bonusContext = GetMockBonusRequestContext(gameId, bonus is RevealBonus ? RandomNumberEngine.Next(Reveal.RandomWeightMinRange, Reveal.RandomWeightMaxRange) : 0);

            Assert.DoesNotThrow(() =>
            {
                var bonusResult = module.ExecuteBonus(level, bonusEntity, bonusContext);

                Assert.IsTrue(!bonusResult.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-ModuleShouldExecuteSpin")]
        public void ModuleShouldExecuteSpin(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() =>
            {
                var spin = module.ExecuteSpin(level, null, GetMockSpinRequestContext(0));

                Assert.IsTrue(!spin.IsError);
            });
        }

        [TestCase(Configuration.Id, Levels.One, TestName = "NuwaAndTheFiveElements-ModuleShouldReturnExtraGameSettings")]
        public void ModuleShouldReturnExtraGameSettings(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() => module.GetExtraSettings(level));
        }

        [TestCase(Configuration.Id, TestName = "NuwaAndTheFiveElements-ModuleShouldReturnValidInitialRandomWheel")]
        public void ModuleShouldReturnValidInitialRandomWheel(int gameId)
        {
            var module = GetModule(gameId);

            Assert.IsNotNull(module.InitialRandomWheel());
        }
    }
}
