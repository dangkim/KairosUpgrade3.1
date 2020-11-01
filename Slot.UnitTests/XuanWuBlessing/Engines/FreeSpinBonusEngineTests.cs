using NUnit.Framework;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.RandomNumberGenerators;
using Slot.Games.XuanWuBlessing.Configuration;
using Slot.Games.XuanWuBlessing.Configuration.Bonuses;
using Slot.Games.XuanWuBlessing.Engines;
using Slot.Model;
using static Slot.UnitTests.XuanWuBlessing.SpinsHelper;

namespace Slot.UnitTests.XuanWuBlessing.Engines
{
    [TestFixture]
    public class FreeSpinBonusEngineTests
    {
        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldCreateFreeSpinSelectionBonus")]
        public void EngineShouldCreateFreeSpinSelectionBonus(int level)
        {
            var spinResult = GenerateSpinResult(level);
            Assert.DoesNotThrow(() => FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult));
        }

        [TestCase(Levels.One, 1, TestName = "XuanWuBlessing-ShouldCreateFreeSpinSelectionBonus-1", ExpectedResult = "FreeSpinModeOne")]
        [TestCase(Levels.One, 2, TestName = "XuanWuBlessing-ShouldCreateFreeSpinSelectionBonus-2", ExpectedResult = "FreeSpinModeTwo")]
        [TestCase(Levels.One, 3, TestName = "XuanWuBlessing-ShouldCreateFreeSpinSelectionBonus-3", ExpectedResult = "FreeSpinModeThree")]
        [TestCase(Levels.One, 4, TestName = "XuanWuBlessing-ShouldCreateFreeSpinSelectionBonus-4", ExpectedResult = "FreeSpinModeFour")]
        [TestCase(Levels.One, 5, TestName = "XuanWuBlessing-ShouldCreateFreeSpinSelectionBonus-5", ExpectedResult = "FreeSpinModeFive")]
        public string EngineShouldCreateFreeSpinMode(int level, int freeSpinSelection)
        {
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            return freeSpinMode.GetType().Name;
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldCreateFreeSpinSelectionBonusResult")]
        public void EngineShouldCreateFreeSpinSelectionBonusResult(int level)
        {
            var spinResult = GenerateSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);

            Assert.DoesNotThrow(() => FreeSpinBonusEngine.CreateFreeSpinSelectionBonusResult(freeSpinBonus));
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldCreateFreeSpinBonus")]
        public void EngineShouldCreateFreeSpinBonus(int level)
        {
            var spinResult = GenerateSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            Assert.DoesNotThrow(() => FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus));
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldCreateFreeSpinResult")]
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
            var spinResult = GenerateSpinResult(level);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);

            Assert.DoesNotThrow(() => FreeSpinBonusEngine.CreateFreeSpinResult(level, requestContext, freeSpinMode, spinResult, config));
        }

        [TestCase(Levels.One, TestName = "XuanWuBlessing-ShouldCreateFreeSpinBonusResultFromFreeSpinResult")]
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

            var spinResult = GenerateSpinResult(level);
            var freeSpinSelectionBonus = FreeSpinBonusEngine.CreateFreeSpinSelectionBonus(spinResult);
            var freeSpinSelection = RandomNumberEngine.Next(FreeSpinMode.MinimumFreeSpinSelection, FreeSpinMode.MaximumFreeSpinSelection);
            var freeSpinMode = FreeSpinBonusEngine.GetFreeSpinMode(freeSpinSelection);
            freeSpinSelectionBonus.UpdateBonus(freeSpinSelection, freeSpinMode);

            var freeSpinBonus = FreeSpinBonusEngine.CreateFreeSpinBonus(freeSpinSelectionBonus);
            var freeSpinResult = FreeSpinBonusEngine.CreateFreeSpinResult(level, requestContext, freeSpinMode, spinResult, config);
             
            Assert.DoesNotThrow(() => FreeSpinBonusEngine.CreateFreeSpinBonusResult(freeSpinBonus, freeSpinResult));
        }
    }
}
