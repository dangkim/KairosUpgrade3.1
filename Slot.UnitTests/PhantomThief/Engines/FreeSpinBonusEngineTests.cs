using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.PhantomThief.Configuration;
using Slot.Games.PhantomThief.Engines;
using Slot.Model;
using static Slot.UnitTests.PhantomThief.SpinsHelper;

namespace Slot.UnitTests.PhantomThief.Engines
{
    [TestFixture]
    public class FreeSpinBonusEngineTests
    {
        [TestCase(Levels.One, TestName = "PhantomThief-ShouldCreateFreeSpinBonus")]
        public void EngineShouldCreateFreeSpinBonus(int level)
        {
            var spinResult = GenerateSpinResult(level);
            Assert.DoesNotThrow(() => FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult));
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldCreateFreeSpinResult")]
        public void EngineShouldCreateFreeSpinResult(int level)
        {
            var config = new Configuration();
            var requestContext = new RequestContext<BonusArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new BonusArgs(),
                Platform = PlatformType.All
            };

            Assert.DoesNotThrow(() => FreeSpinBonusEngine.CreateFreeSpinResult(level, requestContext, config));
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldCreateFreeSpinCollapsingResult")]
        public void EngineShouldCreateFreeSpinCollapsingResult(int level)
        {
            var config = new Configuration();
            var requestContext = new RequestContext<BonusArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new BonusArgs(),
                Platform = PlatformType.All
            };

            Assert.DoesNotThrow(() =>
            {
                var freeSpinResult = FreeSpinBonusEngine.CreateFreeSpinResult(level, requestContext, config);
                var targetWheel = MainGameEngine.GetTargetWheel(level, config, freeSpinResult.Wheel.ReelStripsId);
                var freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(freeSpinResult, targetWheel, config.Payline, config.FreeGamePayTable, config.FreeGameScatterSymbols);
            });
        }

        [TestCase(Levels.One, TestName = "PhantomThief-ShouldCreateFreeSpinBonusResultFromFreeSpinResult")]
        public void EngineShouldCreateFreeSpinBonusResultFromFreeSpinResult(int level)
        {
            var config = new Configuration();
            var requestContext = new RequestContext<BonusArgs>("", "", PlatformType.Web)
            {
                GameSetting = new Model.Entity.GameSetting { GameSettingGroupId = 0 },
                Currency = new Model.Entity.Currency { Id = 0 },
                Parameters = new BonusArgs(),
                Platform = PlatformType.All
            };

            Assert.DoesNotThrow(() =>
            {
                var freeSpinResult = FreeSpinBonusEngine.CreateFreeSpinResult(level, requestContext, config);
                var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinResult);

                var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult);
            });
        }
    }
}
