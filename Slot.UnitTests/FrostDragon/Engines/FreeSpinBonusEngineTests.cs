using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Games.FrostDragon.Configuration;
using Slot.Games.FrostDragon.Engines;
using Slot.Model;
using static Slot.UnitTests.FrostDragon.SpinsHelper;

namespace Slot.UnitTests.FrostDragon.Engines
{
    [TestFixture]
    public class FreeSpinBonusEngineTests
    {
        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCreateFreeSpinBonus")]
        public void EngineShouldCreateFreeSpinBonus(int level)
        {
            var spinResult = GenerateSpinResult(level);
            Assert.DoesNotThrow(() => FreeSpinBonusEngine.CreateFreeSpinBonus(spinResult));
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCreateFreeSpinResult")]
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

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCreateFreeSpinCollapsingResult")]
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
                var freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(freeSpinResult, targetWheel, config.BonusConfig.FreeSpin.Multipliers, config.Payline, config.PayTable);
            });
        }

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCreateFreeSpinBonusResultFromFreeSpinResult")]
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

        [TestCase(Levels.One, TestName = "FrostDragon-ShouldCreateFreeSpinBonusResultFromFreeSpinCollapsingResult")]
        public void EngineShouldCreateFreeSpinBonusResultFromFreeSpinCollapsingResult(int level)
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
                var targetWheel = MainGameEngine.GetTargetWheel(level, config, freeSpinResult.Wheel.ReelStripsId);
                var freeSpinCollapsingResult = FreeSpinBonusEngine.CreateFreeSpinCollapsingResult(freeSpinResult, targetWheel, config.BonusConfig.FreeSpin.Multipliers, config.Payline, config.PayTable);

                var freeSpinBonusResult = FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinCollapsingResult);
            });
        }
    }
}
