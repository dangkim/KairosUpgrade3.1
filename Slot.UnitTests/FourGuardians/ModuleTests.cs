using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FourGuardians;
using Slot.Games.FourGuardians.Configuration;
using System;
using static Slot.Games.FourGuardians.Models.Test.SimulationHelper;

namespace Slot.UnitTests.FourGuardians
{
    [TestFixture]
    public class ModuleTests
    {
        [TestCase(Module.Id, TestName = "FourGuardians-ModuleShouldReturnValidGameId")]
        public void ModuleShouldReturnValidGameId(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsTrue(module.GameId > 0);
        }

        [TestCase(Module.Id, TestName = "FourGuardians-ModuleShouldReturnValidConfiguration")]
        public void ModuleShouldReturnValidConfiguration(int gameId)
        {
            var module = GetModule(gameId);
            Assert.IsNotNull(module.Configuration);
        }

        [TestCase(Module.Id, 1, TestName = "FourGuardians-ModuleShouldReturnCorrectTotalBet-20", ExpectedResult = 20)]
        [TestCase(Module.Id, 2, TestName = "FourGuardians-ModuleShouldReturnCorrectTotalBet-40", ExpectedResult = 40)]
        [TestCase(Module.Id, 4, TestName = "FourGuardians-ModuleShouldReturnCorrectTotalBet-80", ExpectedResult = 80)]
        public decimal ModuleShouldReturnCorrectTotalBet(int gameId, decimal lineBet)
        {
            var module = GetModule(gameId);

            var actualTotalBet = module.CalculateTotalBet(null, new RequestContext<SpinArgs>("", "", Model.PlatformType.Web)
            {
                Parameters = new SpinArgs { LineBet = lineBet }
            });

            return actualTotalBet;
        }

        [TestCase(Module.Id, TestName = "FourGuardians-ModuleShouldThrowNotImplementedOnConvertBonus")]
        public void ModuleShouldThrowNotImplementedOnConvertBonus(int gameId)
        {
            var module = GetModule(gameId);
            Assert.Throws<NotImplementedException>(() => module.ConvertToBonus(null));
        }

        [TestCase(Module.Id, Levels.One, TestName = "FourGuardians-ModuleShouldThrowNotImplementedOnCreateBonus")]
        public void ModuleShouldThrowNotImplementedOnCreateBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            Assert.Throws<NotImplementedException>(() => module.CreateBonus(null));
        }

        [TestCase(Module.Id, Levels.One, TestName = "FourGuardians-ModuleShouldThrowNotImplementedOnExecuteBonus")]
        public void ModuleShouldThrowNotImplementedOnExecuteBonus(int gameId, int level)
        {
            var module = GetModule(gameId);
            Assert.Throws<NotImplementedException>(() => module.ExecuteBonus(level, null, null));
        }

        [TestCase(Module.Id, Levels.One, TestName = "FourGuardians-ModuleShouldExecuteSpin")]
        public void ModuleShouldExecuteSpin(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() => module.ExecuteSpin(level, null, GetMockSpinRequestContext(0)));
        }

        [TestCase(Module.Id, Levels.One, TestName = "FourGuardians-ModuleShouldReturnExtraGameSettings")]
        public void ModuleShouldReturnExtraGameSettings(int gameId, int level)
        {
            var module = GetModule(gameId);

            Assert.DoesNotThrow(() => module.GetExtraSettings(level));
        }

        [TestCase(Module.Id, TestName = "FourGuardians-ModuleShouldReturnValidInitialRandomWheel")]
        public void ModuleShouldReturnValidInitialRandomWheel(int gameId)
        {
            var module = GetModule(gameId);

            Assert.IsNotNull(module.InitialRandomWheel());
        }
    }
}
